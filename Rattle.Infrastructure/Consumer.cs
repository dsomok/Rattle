﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rattle.Core.Bus;
using Rattle.Core.Messages;
using System;

namespace Rattle.Infrastructure
{
    public class Consumer : IConsumer<BasicDeliverEventArgs>
    {
        private readonly IModel m_channel;
        private readonly IMessageSerializer m_serializer;



        public Consumer(IModel channel, IMessageSerializer serializer)
        {
            m_channel = channel;
            m_serializer = serializer;
        }



        public void Consume(string queue, bool noAck, bool cancelOnReceive, Action<BasicDeliverEventArgs> receiveHandler)
        {
            var consumer = new EventingBasicConsumer(m_channel);
            consumer.Received += (sender, deliveryArgs) =>
            {
                receiveHandler(deliveryArgs);

                if (!noAck)
                {
                    m_channel.BasicAck(deliveryArgs.DeliveryTag, false);
                }

                if (cancelOnReceive)
                {
                    m_channel.BasicCancel(consumer.ConsumerTag);
                }
            };

            m_channel.BasicConsume(queue, noAck, consumer);
        }
    }
}
