using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTQueueInDB
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting......");
            //ZTRabbitMQBase.ZTRabbitMQBase queue = new ZTRabbitMQBase.ZTRabbitMQBase();
            //Program p = new Program();

            //while (true)
            //{
            //    //string message = queue.Read();

            //    Console.WriteLine("{0}, {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
            //}
            ZTRabbitMQBase.ZTRabbitMQBase queue = new ZTRabbitMQBase.ZTRabbitMQBase(ZTRabbitMQBase.QueueType.Receive);
            queue.ReceiveProperty += (message) =>
            {
                Console.WriteLine("{0}, {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
            };
            Console.ReadLine();
        }

        //public override void Receive(string message)
        //{
        //    Console.WriteLine("{0}, {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
        //}
    }
}
