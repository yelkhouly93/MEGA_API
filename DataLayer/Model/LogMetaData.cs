using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DataLayer.Model
{
    public class LogMetadata
    {
        public string RequestContentType { get; set; }
        public string RequestContent { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public DateTime? RequestTimestamp { get; set; }

        public string ResponseContentType { get; set; }
        public string ResponseContent { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseStatusMessage { get; set; }
        public DateTime? ResponseTimestamp { get; set; }
    }


    public class ApiLog
    {
        public string Host { get; set; }
        public string Headers { get; set; }
        public string StatusCode { get; set; }
        public string RequestBody { get; set; }
        public string RequestedMethod { get; set; }
        public string UserHostAddress { get; set; }
        public string Useragent { get; set; }
        public string AbsoluteUri { get; set; }
        public string RequestType { get; set; }
    }

}
