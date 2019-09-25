using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTUtils;

namespace JobMPos 
{
    class GetSettlementPayFileTask : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            string url = "http://localhost:8322/MPOSPayment_server/GetSettlementPayFileTask";
            string param = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            HttpUtils.HttpPost(url, param);
            return Task.CompletedTask;
        }
    }
}
