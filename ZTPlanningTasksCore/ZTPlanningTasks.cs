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

        public static ZTRabbitMQBase.ZTRabbitMQBase mq = new ZTRabbitMQBase.ZTRabbitMQBase(ZTRabbitMQBase.QueueType.Send);

        public static async void Run()
        {
            WriteLog("定时任务程序启动", ConsoleColor.Green);
            sched = await schedFact.GetScheduler(cts.Token);
            //启动 scheduler
            await sched.Start(cts.Token);

            TasksDataBase tdb = new TasksDataBase();
            List<TasksEntity> list = tdb.Get();
            foreach(TasksEntity task in list)
            {
                //StartJob("JobTest002", "JobTest002.dll", "JobTest002.JobTest002", "");
                dictCount.Add(task.JobName, 0);
                bool result = await StartJob(task);
                if (result)
                {
                    task.State = 1;
                    tdb.UpdateState(task);
                }
            }

            using (NamedPipeServerStream server = new NamedPipeServerStream("ZTPlanningTasksCore", PipeDirection.InOut, 1))
            {
                while (true)
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

        public static void End()
        {
            TasksDataBase tdb = new TasksDataBase();
            tdb.UpdateState();
        }

        static async Task<bool> StartJob(TasksEntity task)
        {
            bool result = false;
            if (string.IsNullOrEmpty(task.GroupName))
            {
                task.GroupName = "DEFAULT";
            }
            try
            {
                Assembly assembly = Assembly.LoadFile(Path.GetFullPath(task.AssemblyName));
                Type t = assembly.GetType(task.ClassName);

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

        static async void StopJob(string jobName)
        {
            await sched.DeleteJob(new JobKey(jobName));
        }

        static void Execute(string command)
        {

        }

        static void WriteLog(string log, ConsoleColor color = ConsoleColor.White)
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
            return Task.Delay(0);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            //return Console.Out.WriteLineAsync("JobToBeExecuted");
            return Task.Delay(0);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            ZTPlanningTasks.dictCount[context.JobDetail.Key.Name]++;
            var log = $"JobWasExecuted,{ZTPlanningTasks.dictCount[context.JobDetail.Key.Name]}次," + Environment.NewLine +
                $"上次执行时间：{(context.PreviousFireTimeUtc.HasValue ? context.PreviousFireTimeUtc.Value.ToString("yyyy-MM-dd HH:mm:ss,fff") : "")}" + Environment.NewLine +
                $"本次执行时间：{context.FireTimeUtc.ToString("yyyy-MM-dd HH:mm:ss,fff")}" + Environment.NewLine +
                $"下次执行时间：{context.NextFireTimeUtc.Value.ToString("yyyy-MM-dd HH:mm:ss,fff")}" + Environment.NewLine +
                $"异常信息：{(jobException == null ? "" : jobException.InnerException.InnerException.Message)}";
            ZTPlanningTasks.mq.Write(log); 
            return Console.Out.WriteLineAsync(log);
        }
    }

    class TriggerListener : ITriggerListener
    {
        public string Name => "TriggerListener";
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            return Task.Delay(0);
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.Delay(0);
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            return Task.Delay(0);
        }

        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return new Task<bool>(() => { return true; });
        }
    }
}
