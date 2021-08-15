﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTTPWinTimeSync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //
            using (var mut = new Mutex(false, "HTTPWinTimeSyncService"))
            {
                bool alreadyRunning = !mut.WaitOne(TimeSpan.Zero);
                if (!alreadyRunning)
                {
                    //CD to our install dir
                    System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                new HTTPTimeSync()
                    };
                    ServiceBase.Run(ServicesToRun);
                }
               
                mut.ReleaseMutex();
                
            }
        }
    }
}
