using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTPlanningTasksCore.DataBase
{
    public abstract class DataBase
    {
        protected IDbConnection db = new SqlConnection(Config.Instance().ConnectionString);
    }
}
