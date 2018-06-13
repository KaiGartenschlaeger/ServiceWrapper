using System;
using System.ServiceProcess;

namespace ServiceWrapper
{
    static class Program
    {
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                RunAsService();
            }
            else
            {
                RunAsConsole();
            }
        }

        private static void RunAsConsole()
        {
            Console.CursorVisible = false;

            Console.WriteLine("Console-Mode (Press enter to exit)");

            HostEngine engine = new HostEngine(true);
            engine.Start();

            Console.ReadLine();
        }

        private static void RunAsService()
        {
            ServiceBase.Run(new HostService());
        }
    }
}