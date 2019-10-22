using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTPlanningTasksCore;
using ZTUtils;

namespace JobMPos
{
    public class ProcessSettlementPayFileTask : JobBase 
    {
        public override Task Execute()
        {
            string url = "http://localhost:8322/MPOSPayment_server/ProcessSettlementPayFileTask";
            HttpUtils.HttpGet(url);
            return Task.CompletedTask;
        }
    }
}
