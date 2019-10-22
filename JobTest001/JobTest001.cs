using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTPlanningTasksCore;

namespace JobTest001
{
    public class JobTest001 : JobBase 
    {
        public override Task Execute()
        {
            return Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "：JobTest001");
        }
    }
}
