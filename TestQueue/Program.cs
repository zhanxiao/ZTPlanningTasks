using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTRabbitMQBase;

namespace TestQueue
{
    class Program : ZTRabbitMQBase.ZTRabbitMQBase
    {
        Program() : base(QueueType.Send)
        {

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Starting......");
            //ZTRabbitMQBase.ZTRabbitMQBase queue = new ZTRabbitMQBase.ZTRabbitMQBase();
            Program p = new Program();
            while(true)
            {
                p.Write(Guid.NewGuid().ToString());
            }
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
