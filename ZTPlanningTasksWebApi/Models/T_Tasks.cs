//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZTPlanningTasksWebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class T_Tasks
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
        public System.DateTime UpdateTime { get; set; }
        public string UpdateMan { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateMan { get; set; }
    }
}
