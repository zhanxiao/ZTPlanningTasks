using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTPlanningTasksCore.Entity
{
    public class JobLogEntity
    {
        public int Id { get; set; }
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public DateTime? PreviousTime { get; set; }
        public DateTime? CurrentTime { get; set; }
        public DateTime? NextTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Times { get; set; }
        public string Exception { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
