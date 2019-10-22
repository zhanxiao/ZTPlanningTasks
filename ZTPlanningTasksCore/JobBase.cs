using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTPlanningTasksCore
{
    public abstract class JobBase : MarshalByRefObject, IJob
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
