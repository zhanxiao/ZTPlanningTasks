using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTPlanningTasksCore.Entity;

namespace ZTPlanningTasksCore.DataBase
{
    public class JobLogDataBase : DataBase
    {
        public int Insert(JobLogEntity jobLog)
        {
            string sql = @"INSERT INTO T_JobLog (JobName, JobGroup, PreviousTime, CurrentTime, NextTime, StartTime, EndTime, Times, Exception) 
                VALUES (@JobName, @JobGroup, @PreviousTime, @CurrentTime, @NextTime, @StartTime, @EndTime, @Times, @Exception)";
            int result = db.Execute(sql, jobLog);
            return result;
        }

        public List<JobLogEntity> Get()
        {
            return (List<JobLogEntity>)db.Query<JobLogEntity>("SELECT * FROM T_JobLog");
        }
    }
}
