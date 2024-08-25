using System;
using System.ServiceProcess;

namespace SystemMonitoDirectory
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SystemMonit()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
