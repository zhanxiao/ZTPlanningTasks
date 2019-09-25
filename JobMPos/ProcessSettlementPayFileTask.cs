using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTUtils;

namespace JobMPos
{
    public class ProcessSettlementPayFileTask : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            string url = "http://localhost:8322/MPOSPayment_server/ProcessSettlementPayFileTask";
            HttpUtils.HttpGet(url);
            return Task.CompletedTask;
        }
    }
}
