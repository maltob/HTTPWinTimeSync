using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTPWinTimeSync
{
    public class HTTPTimeSyncConfiguration
    {
        private static readonly string urlFileName = "urls.txt";
        public static Uri[] GetSyncURL ()
        {
           var urls = new  List<Uri>();

           if(File.Exists(urlFileName))
            {

                //Read i nthe URLs line by line and add only the valid ones
                using (var fs = File.OpenRead(urlFileName))
                {
                    var br = new StreamReader(new BufferedStream(fs));
                    while (!br.EndOfStream)
                    {

                        string line = br.ReadLine();
                        Uri tURL;
                        if (Uri.TryCreate(line.Trim(), UriKind.Absolute, out tURL))
                        {
                            urls.Add(tURL);
                        }
                        else
                        {
                            
                        }
                    }
                    fs.Close();
                }
            }

           return urls.ToArray();
        }
    }
}
