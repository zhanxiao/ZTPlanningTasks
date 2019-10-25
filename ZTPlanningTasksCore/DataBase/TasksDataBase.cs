using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTPlanningTasksCore.Entity;
using Dapper;

namespace ZTPlanningTasksCore.DataBase
{
    public class TasksDataBase : DataBase
    {
        public int Insert(TasksEntity tasks)
        {
            string sql = @"INSERT INTO T_JobLog (JobName, JobGroup, AssemblyName, ClassName, CronExpression, Priority, IsNeedStart, State, Remark, UpdateTime, UpdateMan, CreateMan) 
                VALUES (@JobName, @JobGroup, @AssemblyName, @ClassName, @CronExpression, @Priority, @IsNeedStart, @State, @Remark, @UpdateTime, @UpdateMan, @CreateMan))";
            int result = db.Execute(sql, tasks);
            return result;
        }
        public List<TasksEntity> Get()
        {
            return (List<TasksEntity>)db.Query<TasksEntity>("SELECT * FROM T_Tasks WHERE IsNeedStart = 1");
        }

        public TasksEntity Get(string jobName)
        {
            return db.QueryFirst<TasksEntity>("SELECT * FROM T_Tasks WHERE JobName = @JobName", new { JobName = jobName });
        }

        public int UpdateState(TasksEntity task)
        {
            return db.Execute("UPDATE T_Tasks SET State = @State WHERE JobName = @JobName", new { State = task.State, JobName = task.JobName });
        }

        public int UpdateState()
        {
            return db.Execute("UPDATE T_Tasks SET State = 0");
        }
    }
}
