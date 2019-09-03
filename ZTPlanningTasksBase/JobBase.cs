using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTPlanningTasksBase
{
    public abstract class JobBase : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            return Execute();
        }

        public abstract Task Execute();

        public void WriteLog()
        {

        }
    }
}
