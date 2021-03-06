﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace ZTRabbitMQBase
{
    public class ZTRabbitMQBase
    {
        static ConnectionFactory factory = new ConnectionFactory
        {
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/",
            HostName = "127.0.0.1"
        };
        static int count = 0;
        static Stopwatch sw = new Stopwatch();

        IConnection connection = null;
        IModel channel = null;
        IBasicProperties properties = null;

        EventingBasicConsumer consumer = null;

        public Action<string> ReceiveProperty { set; get; }

        public ZTRabbitMQBase(QueueType queueType)
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare("log", true, false, false, null);
            channel.ConfirmSelect();
            channel.BasicAcks += channel_BasicAcks;
            channel.ExchangeDeclare("log", ExchangeType.Topic, true);
            channel.QueueBind("log", "log", "log");
            properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;
            properties.Expiration = "604800000"; //7天
            properties.Priority = 5;

            //consumer = new DefaultBasicConsumer(channel);
            //consumer.

            if (queueType == QueueType.Receive)
            {
                consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume("log", true, consumer);
                channel.ConfirmSelect();
                consumer.Received += Consumer_Received;
            }
        }        

        public bool Write(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("log", "log", properties, body);
            return true;
        }

        public bool Write(TJobLog log)
        {
            var json = JsonConvert.SerializeObject(log);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish("log", "log", properties, body);
            return true;
        }

        //public string Read()
        //{
        //    //consumer.Received += action;
        //    BasicGetResult result = channel.BasicGet("log", false);
        //    string message = "";
        //    if (result != null)
        //    {
        //        message = Encoding.UTF8.GetString(result.Body);
        //    }
        //    return message;
        //}

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);
            //Receive(message);
            this.ReceiveProperty(message);
        }

        public virtual void Receive(string message)
        {
        }

        //static void channel_BasicRecoverOk(object sender, EventArgs e)
        //{
        //    var a = e;
        //    throw new NotImplementedException();
        //}
        static void channel_BasicAcks(object sender, BasicAckEventArgs e)
        {
            ulong a = e.DeliveryTag;
            //throw new NotImplementedException();
        }
    }

    public enum QueueType
    {
        Send,
        Receive
    }

    public class TJobLog
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public DateTime PreviousTime { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime NextTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Times { get; set; }
        public string Exception { get; set; }
    }
}
