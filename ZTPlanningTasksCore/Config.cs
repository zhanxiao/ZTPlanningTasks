using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ZTPlanningTasksCore
{
    public class Config
    {
        private static Config instance;
        private static object objLock = new object();
        private Config()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["ZTPT"].ConnectionString;
        }
        public static Config Instance()
        {
            if(instance == null)
            {
                lock(objLock)
                {
                    if(instance == null)
                    {
                        instance = new Config();
                    }
                }
            }
            return instance;
        }

        public string ConnectionString { get; }
    }
}
