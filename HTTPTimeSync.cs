using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace HTTPWinTimeSync
{
    public partial class HTTPTimeSync : ServiceBase
    {
        Thread syncThread;
        HTTPTimeSyncManager timeSyncManager = new HTTPTimeSyncManager();
        public HTTPTimeSync()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            syncThread = new Thread(new ThreadStart(timeSyncManager.Sync));
            syncThread.Start();
        }

        protected override void OnStop()
        {
            if (syncThread != null && syncThread.IsAlive)
            {
                syncThread.Abort();
            }
        }
    }
}
