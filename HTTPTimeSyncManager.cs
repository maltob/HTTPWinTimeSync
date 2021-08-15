using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTTPWinTimeSync
{
    class HTTPTimeSyncManager
    {
        List<HTTPTimeSynchronizer> timeSynchronizers = new List<HTTPTimeSynchronizer>();
        List<Uri> skipURL = new List<Uri>();
        int lastSkipURLClear = 0;
        int sleepTime = 120;

        private static NLog.Logger Logger ;

        public HTTPTimeSyncManager()
        {
            //Setup logging
            SetupLogging();

           // System.IO.File.WriteAllText("touch.txt", "");

            //Setup the syncnonizers
            timeSynchronizers.Clear();
            foreach (Uri u in HTTPTimeSyncConfiguration.GetSyncURL())
            {
                Logger.Info("Adding {0} to URLs to sync", u);
                timeSynchronizers.Add(new HTTPTimeSynchronizer(u));
            }
        }

        public void SetupLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "HTTPTimeSyncManager.log" };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
            Logger = NLog.LogManager.GetCurrentClassLogger();
            
            Logger.Info("Started HTTPTimeSyncManager");
        }

        public void Sync()
        {
            //Set the timer we will use to retry URLs
            lastSkipURLClear = Environment.TickCount & Int32.MaxValue;


            try
            {
                while (true)
                {
                    

                    List<double> offsets = new List<double>();
                    foreach (var sync in timeSynchronizers)
                    {

                        try
                        {

                            //Sync any URL's not excluded
                            if (!skipURL.Contains(sync.SyncURL))
                            {
                                var offsetRes = sync.Offset();
                                offsetRes.Wait();
                                offsets.Add(offsetRes.Result);

                            }



                        }
                        catch (HTTPClientSyncException httpSyncEx)
                        {

                            // We didn't get a successful status, add to list to skip
                            skipURL.Add(sync.SyncURL);
                            Logger.Debug(httpSyncEx.Message);
                        }
                        catch(Exception e)
                        {
                            //Something else went wrong
                        }
                    }

                    //Adjust the times
                    var adjustedMilli = AdjustTime(offsets.ToArray());
                    if(Math.Abs(adjustedMilli) < 15*1000)
                        Logger.Info("Adjusted time by {0} seconds", adjustedMilli/1000);
                    else
                        Logger.Info("Did not adjust the time - it was only off by {0} milliseconds", adjustedMilli);



                    //Wait for sleepTime minutes to resync
                    Logger.Debug("Pausing for {0} minutes before next sync", sleepTime);
                    Thread.Sleep(sleepTime * 60 * 1000);


                    // Clear the skip list if its been over 4 hours - Untested what happens at 49.8 days
                    if(skipURL.Count > 0 && Math.Abs((((long)(Environment.TickCount & Int32.MaxValue))) - lastSkipURLClear) > (1000 * 60 * 60 * 4)) 
                    {
                        lastSkipURLClear = Environment.TickCount & Int32.MaxValue;
                        skipURL.Clear();
                        Logger.Debug("Cleared skip URL list");
                    }

                    //Double how long till we sync
                    if (sleepTime < 60 * 30)
                        sleepTime *= 2;

                }

            }
            catch (ThreadAbortException tae)
            {
                // Service is being shutdown
                foreach (var ts in timeSynchronizers)
                {
                    ts.Dispose();
                }

            }
        }





        public double AdjustTime( double[] times )
        {

            var mean = times.Sum() / times.Length;

            var filteredTImes = new List<double>();

            //Remove any times over 15 seconds from the mean in case some server is very wonky
            foreach(var t in times)
            {
                if (Math.Abs(mean - t) < 15 * 1000)
                    filteredTImes.Add(t);
            }


            //Get the mean again
            var meanDiff = filteredTImes.Sum() / filteredTImes.Count;

            //Change if we are over 15 seconds off
            if(Math.Abs(meanDiff) > 15*1000 )
            {
                var AdjustedDate = DateTime.Now.AddMilliseconds(meanDiff);
                SYSTEMTIME adjustedST = new SYSTEMTIME();
                adjustedST.FromDateTime(AdjustedDate);
                SetLocalTime(ref adjustedST);

                return meanDiff;
            }
            else
            {
                return meanDiff;
            }

        }

        [DllImport("kernel32.dll")]
        static extern bool SetLocalTime([In] ref SYSTEMTIME lpLocalTime);

        public struct SYSTEMTIME
        {
            public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        /// <summary>
        /// Convert form System.DateTime
        /// </summary>
        /// <param name="time"></param>
        public void FromDateTime(DateTime time)
        {
            wYear = (ushort)time.Year;
            wMonth = (ushort)time.Month;
            wDayOfWeek = (ushort)time.DayOfWeek;
            wDay = (ushort)time.Day;
            wHour = (ushort)time.Hour;
            wMinute = (ushort)time.Minute;
            wSecond = (ushort)time.Second;
            wMilliseconds = (ushort)time.Millisecond;
        }

        /// <summary>
        /// Convert to System.DateTime
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }
        /// <summary>
        /// STATIC: Convert to System.DateTime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }
    }
}
}
