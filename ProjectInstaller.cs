using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace HTTPWinTimeSync
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

           //Allow write access to the log file
            string logFIle = Path.Combine(Path.GetDirectoryName(Context.Parameters["assemblypath"]), "HTTPTimeSyncManager.log");
            if(!File.Exists(logFIle))
            {
                File.WriteAllText(logFIle, "");

            }

            var logFileInfo = new FileInfo(logFIle);
            var acl =  logFileInfo.GetAccessControl();
            acl.AddAccessRule(new FileSystemAccessRule("NT AUTHORITY\\LOCALSERVICE", FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.Modify, AccessControlType.Allow));
            logFileInfo.SetAccessControl(acl);

            //If there is a URLs.txt here, copy it over
            if(File.Exists("URLs.txt"))
            {
                string urlFileDest = Path.Combine(Path.GetDirectoryName(Context.Parameters["assemblypath"]), "URLs.txt");
                File.Copy("URLs.txt", urlFileDest, true);
            }

        }
    }
}
