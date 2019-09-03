using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTest002
{
    public class JobTest002 : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            //throw new Exception("ex");
            var d = 0;
            var a = 2 / d;
            return Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "：JobTest002");
           
        }
    }
}
