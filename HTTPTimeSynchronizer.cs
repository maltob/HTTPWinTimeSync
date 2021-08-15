using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HTTPWinTimeSync
{
    class HTTPTimeSynchronizer : IDisposable
    {
        Uri syncURL;
        HttpClient http = new HttpClient();
        public HTTPTimeSynchronizer(Uri uri)
        {
            syncURL = uri;

        }

        public Uri SyncURL { get {return syncURL; } }

        public async Task<double> Offset()
        {
            var startDT = DateTime.Now;

            var res = await http.SendAsync(new HttpRequestMessage(HttpMethod.Head, syncURL));

            var endDT = DateTime.Now;

            

            if (res.IsSuccessStatusCode)
            {



                //Record how long the call took 
                var callTime = endDT.Subtract(startDT).TotalMilliseconds;

                //If we took over 5 minutes...something is wrong just assume our time is right
                if (callTime > 1000 * 60 * 5)
                {
                    return 0;
                }
                else
                {
                    DateTime serverTime;
                    if (res.Headers.Contains("Date") && DateTime.TryParse(res.Headers.GetValues("Date").First(), out serverTime))
                        return (double)serverTime.Subtract(startDT).TotalMilliseconds;
                    else
                        throw new HTTPClientSyncException(syncURL, ((int)res.StatusCode));
                }

            }
            else
            {
                throw new HTTPClientSyncException(syncURL, ((int)res.StatusCode));
            }
            
        }


        public void Dispose()
        {
            http.Dispose();
        }
    }
}
