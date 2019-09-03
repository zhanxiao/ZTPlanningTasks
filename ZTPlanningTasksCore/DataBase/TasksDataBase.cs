using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTPlanningTasksCore.Entity;
using Dapper;

namespace ZTPlanningTasksCore.DataBase
{
    public class TasksDataBase
    {
        static IDbConnection db = new SqlConnection(Config.Instance().ConnectionString);
        public List<TasksEntity>Get()
        {
            return (List<TasksEntity>)db.Query<TasksEntity>("SELECT * FROM tTasks WHERE IsNeedStart = 1");
        }

        public int UpdateState(TasksEntity task)
        {
            return db.Execute("UPDATE tTasks SET State = @State WHERE Id = @Id", new { State = task.State, Id = task.Id });
        }

        public int UpdateState()
        {
            return db.Execute("UPDATE tTasks SET State = 0");
        }
    }
}
