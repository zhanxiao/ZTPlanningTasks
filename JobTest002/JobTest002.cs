using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTPlanningTasksBase;

namespace JobTest002
{
    public class JobTest002 : JobBase
    {
        public override Task Execute()
        {
            //throw new Exception("ex");
            //var d = 0;
            //var a = 2 / d;
            return Console.Out.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "：JobTest002");
        }
    }
}
