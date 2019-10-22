using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTPlanningTasksCore;
using ZTUtils;

namespace JobMPos 
{
    class GetSettlementPayFileTask : JobBase
    {
        public override Task Execute()
        {
            var b = 0;
            var a = 5 / b;
            string url = "http://localhost:8322/MPOSPayment_server/GetSettlementPayFileTask";
            string param = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
            HttpUtils.HttpPost(url, param);
            return Task.CompletedTask;
        }
    }
}
