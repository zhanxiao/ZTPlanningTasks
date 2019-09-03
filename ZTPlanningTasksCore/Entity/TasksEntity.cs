using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTPlanningTasksCore.Entity
{
    public class TasksEntity
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string GroupName { get; set; }
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string CronExpression { get; set; }
        public int Priority { get; set; }
        public bool IsNeedStart { get; set; }
        public int State { get; set; }
        public string Remark { get; set; }
        public DateTime UpdateTime { get; set; }
        public string UpdateMan { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateMan { get; set; }
    }
}
