using System;
using System.Runtime.InteropServices;

namespace ZTPlanningTasksConsole
{
    class Program
    {
        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);
        private static ControlCtrlDelegate cancelHandler = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    ZTPlanningTasksCore.ZTPlanningTasks.End();
                    Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭
                    break;
                case 2:
                    ZTPlanningTasksCore.ZTPlanningTasks.End();
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭                    
                    break;
            }
            //Console.ReadLine();
            return false;
        }

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(cancelHandler, true);
            ZTPlanningTasksCore.ZTPlanningTasks.Run();
        }
    }
}
