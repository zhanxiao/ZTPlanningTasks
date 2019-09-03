using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobTest001
{
    public class JobTest003 : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "：JobTest003");
        }
    }
}
