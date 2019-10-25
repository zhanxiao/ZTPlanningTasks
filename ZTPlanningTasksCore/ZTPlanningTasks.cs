using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZTPlanningTasksCore.DataBase;
using ZTPlanningTasksCore.Entity;

namespace ZTPlanningTasksCore
{
    public class ZTPlanningTasks 
    {
        static CancellationTokenSource cts = new CancellationTokenSource();
        static ISchedulerFactory schedFact = new StdSchedulerFactory();
        static IScheduler sched;

        public static Dictionary<string, int> dictCount = new Dictionary<string, int>();
        public static Dictionary<string, AppDomain> AppDomainDict = new System.Collections.Generic.Dictionary<string, AppDomain>();

        public static ZTRabbitMQBase.ZTRabbitMQBase mq = new ZTRabbitMQBase.ZTRabbitMQBase(ZTRabbitMQBase.QueueType.Send);

        static TasksDataBase tdb = new TasksDataBase();

        public async void Run()
        {
            WriteLog("定时任务程序启动", ConsoleColor.Green);
            sched = await schedFact.GetScheduler(cts.Token);
            //启动 scheduler
            await sched.Start(cts.Token);

            tdb.UpdateState();

            List<TasksEntity> list = tdb.Get();//从任务表读取需要启动的任务
            foreach(TasksEntity task in list)
            {
                //StartJob("JobTest002", "JobTest002.dll", "JobTest002.JobTest002", "");
                dictCount.Add(task.JobName, 0);//任务记数加入计数字典
                bool result = await StartupJob(task);//启动任务
                if (result)//任务启动成功修改状态
                {
                    task.State = 1;
                    tdb.UpdateState(task);
                }
            }

            //通过管道读取命令
            while (true)
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream("ZTPlanningTasksCore", PipeDirection.InOut, 1))
                {
                    server.WaitForConnection();
                    server.ReadMode = PipeTransmissionMode.Byte;
                    using (StreamReader sr = new StreamReader(server))
                    {
                        string command = sr.ReadToEnd();
                        Execute(command);
                    }
                }
            }
        }

        public void End()
        {
            tdb.UpdateState();
        }

        async Task<bool> StartupJob(TasksEntity task)
        {
            bool result = false;
            if (string.IsNullOrEmpty(task.GroupName))
            {
                task.GroupName = "DEFAULT";
            }
            try
            {
                if (!dictCount.ContainsKey(task.JobName))
                {
                    dictCount.Add(task.JobName, 0);
                }
                AppDomain appDomain = null;
                if (!AppDomainDict.ContainsKey(task.AssemblyName))
                {
                    appDomain = AppDomain.CreateDomain(task.AssemblyName);
                    AppDomainDict.Add(task.AssemblyName, appDomain);
                }
                else
                {
                    appDomain = AppDomainDict[task.AssemblyName];
                }
                var instance = appDomain.CreateInstanceFromAndUnwrap(Path.GetFullPath(task.AssemblyName), task.ClassName);

                Type t = instance.GetType();

                //Assembly assembly = Assembly.LoadFile(Path.GetFullPath(task.AssemblyName));
                //Type t = assembly.GetType(task.ClassName);

                IJobDetail job = JobBuilder.Create(t)
                           .WithIdentity(task.JobName)
                           .Build();

                ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("trigger_" + task.JobName, task.GroupName)
                        //.WithSimpleSchedule(x => x.WithIntervalInSeconds(1))
                        //.WithCronSchedule("*/5 * * * * ? *")
                        //.WithCronSchedule("*/5 25-26 14 * * ?")
                        //.WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever())
                        .WithCronSchedule(task.CronExpression)
                        .Build();

                await sched.ScheduleJob(job, trigger);
                sched.ListenerManager.AddJobListener(new JobListener());
                //sched.ListenerManager.AddTriggerListener(new TriggerListener());
                result = true;
            }
            catch (Exception ex)
            {
                WriteLog($"{task.JobName},{ex.Message}", ConsoleColor.Red);
            }
            return result;
        }

        async void StartJob(string jobName)
        {
            var task = tdb.Get(jobName);
            await StartupJob(task);
            tdb.UpdateState(new TasksEntity { JobName = jobName, State = 1 });
        }

        async void StopJob(string jobName)
        {
            var triggerKey = new TriggerKey("trigger_" + jobName);
            await sched.PauseTrigger(triggerKey);
            await sched.UnscheduleJob(triggerKey);
            await sched.DeleteJob(new JobKey(jobName));
            tdb.UpdateState(new TasksEntity { JobName = jobName, State = 0 });
        }

        void UnloadDll(string assemblyName)
        {
            if(AppDomainDict.ContainsKey(assemblyName))
            {
                var appDomain = AppDomainDict[assemblyName];
                AppDomain.Unload(appDomain);
                AppDomainDict.Remove(assemblyName);
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command"></param>
        void Execute(string command)
        {
            string[] cmd = command.Trim().Split(':');
            var key = cmd[0].ToLower();
            var jobName = cmd[1];
            switch(key)
            {
                case "start":
                    StartJob(jobName);
                    break;
                case "stop":
                    StopJob(jobName);
                    break;
                case "unload":
                    UnloadDll(jobName);
                    break;
            }
        }

        void WriteLog(string log, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}:{log}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    class JobListener : IJobListener
    {
        public string Name => "JobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            //return Console.Out.WriteLineAsync("JobExecutionVetoed");            
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            //return Console.Out.WriteLineAsync("JobToBeExecuted");
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {   
            ZTPlanningTasks.dictCount[context.JobDetail.Key.Name]++;
            var log = $"{context.JobDetail.Key.Name},{context.JobDetail.Key.Group},{ZTPlanningTasks.dictCount[context.JobDetail.Key.Name]}次" + //Environment.NewLine +
                $",上次执行时间：{(context.PreviousFireTimeUtc.HasValue ? context.PreviousFireTimeUtc.Value.ToString("yyyy-MM-dd HH:mm:ss,fff") : "")}" + //Environment.NewLine +
                $",本次执行时间：{context.FireTimeUtc.ToString("yyyy-MM-dd HH:mm:ss,fff")}" + //Environment.NewLine +
                $",下次执行时间：{context.NextFireTimeUtc.Value.ToString("yyyy-MM-dd HH:mm:ss,fff")}" + //Environment.NewLine +
                //$"开始执行时间：{context.Trigger.StartTimeUtc.ToString("yyyy-MM-dd HH:mm:ss,fff")}" + Environment.NewLine +
                //$"结束执行时间：{(context.Trigger.EndTimeUtc.HasValue ? context.Trigger.EndTimeUtc.Value.ToString("yyyy-MM-dd HH:mm:ss,fff") : "")}" + Environment.NewLine + 
                $",异常信息：{(jobException == null ? "无" : jobException.InnerException.InnerException.Message)}";// + Environment.NewLine;
            ZTPlanningTasks.mq.Write(log);

            #region 执行日志入库
            JobLogDataBase jdb = new JobLogDataBase();

            DateTime? previousTime = null, currentTime = null, nextTime = null, startTime = null, endTime = null;
            string exception = jobException == null ? "" : jobException.InnerException.InnerException.Message;
            if (context.PreviousFireTimeUtc.HasValue) previousTime = context.PreviousFireTimeUtc.Value.UtcDateTime;
            if (context.FireTimeUtc != null) currentTime = context.FireTimeUtc.UtcDateTime;
            if (context.NextFireTimeUtc.HasValue) nextTime = context.NextFireTimeUtc.Value.UtcDateTime;
            if (context.Trigger.StartTimeUtc != null) startTime = context.Trigger.StartTimeUtc.UtcDateTime;
            if (context.Trigger.EndTimeUtc.HasValue) endTime = context.Trigger.EndTimeUtc.Value.UtcDateTime;

            var jobLog = new JobLogEntity
            {
                JobName = context.JobDetail.Key.Name,
                JobGroup = context.JobDetail.Key.Group,
                PreviousTime = previousTime,
                CurrentTime = currentTime,
                NextTime = nextTime,
                StartTime = startTime,
                EndTime = endTime,
                Times = ZTPlanningTasks.dictCount[context.JobDetail.Key.Name],
                Exception = exception
            };
            jdb.Insert(jobLog);
            #endregion

            return Console.Out.WriteLineAsync(log);
        }
    }

    class TriggerListener : ITriggerListener
    {
        public string Name => "TriggerListener";
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }
}
