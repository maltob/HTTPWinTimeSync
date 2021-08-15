using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTPWinTimeSync
{
    class HTTPClientSyncException : Exception
    {
        Uri serverUri;
        int status;
        public HTTPClientSyncException(Uri serverUri, int statusCode)
        {
            this.serverUri = serverUri;
            this.status = statusCode;
        }

        public override string Message
        {
            get { return String.Format("Failed to successfully sync with server at {0}, received code {1}.", serverUri, status); }
        }
    }
}
