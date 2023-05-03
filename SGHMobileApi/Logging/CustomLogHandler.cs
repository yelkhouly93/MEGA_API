using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using DataLayer.Reception.Business;

namespace SGHMobileApi.Logging
{
    public class CustomLogHandler : DelegatingHandler
    {
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var logMetadata = BuildRequestMetadata(request);


            var requestedMethod = request.Method;
            var userHostAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";
            var useragent = request.Headers.UserAgent.ToString();
            var requestMessage = await request.Content.ReadAsByteArrayAsync();
            var uriAccessed = request.RequestUri.AbsoluteUri;

            var responseHeadersString = new StringBuilder();
            foreach (var header in request.Headers)
            {
                responseHeadersString.Append($"{header.Key}: {String.Join(", ", header.Value)}{Environment.NewLine}");
            }

            var messageLoggingHandler = new MessageLogging();

            var requestLog = new ApiLog()
            {
                Headers = responseHeadersString.ToString(),
                AbsoluteUri = uriAccessed,
                Host = userHostAddress,
                RequestBody = Encoding.UTF8.GetString(requestMessage),
                UserHostAddress = userHostAddress,
                Useragent = useragent,
                RequestedMethod = requestedMethod.ToString(),
                StatusCode = string.Empty
            };

            messageLoggingHandler.IncomingMessageAsync(requestLog);
            
            var response = await base.SendAsync(request, cancellationToken);
            
            //logMetadata = BuildResponseMetadata(logMetadata, response);
            //await SendToLog(logMetadata);
            
            //logMetadata = BuildResponseMetadata(logMetadata, response);

            //await SendToLog(logMetadata);
            
            byte[] responseMessage;
            if (response.IsSuccessStatusCode)
                responseMessage = await response.Content.ReadAsByteArrayAsync();
            else
                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);

            var responseLog = new ApiLog()
            {
                Headers = responseHeadersString.ToString(),
                AbsoluteUri = uriAccessed,
                Host = userHostAddress,
                RequestBody = Encoding.UTF8.GetString(responseMessage),
                UserHostAddress = userHostAddress,
                Useragent = useragent,
                RequestedMethod = requestedMethod.ToString(),
                StatusCode = string.Empty
            };

            messageLoggingHandler.OutgoingMessageAsync(responseLog);
            

            return response;
        }
        private LogMetadata BuildRequestMetadata(HttpRequestMessage request)
        {
            HttpContent requestContent = request.Content;
            LogMetadata log = new LogMetadata
            {
                //RequestContentType = request.Content.Headers.ContentType.MediaType,
                RequestMethod = request.Method.Method,
                RequestTimestamp = DateTime.Now,
                RequestUri = request.RequestUri.ToString(),
                RequestContent = requestContent != null ? requestContent.ReadAsStringAsync().Result : ""
            };
            
            return log;
        }
        private LogMetadata BuildResponseMetadata(LogMetadata logMetadata, HttpResponseMessage response)
        {
            HttpContent responseContent = response.Content;

            logMetadata.ResponseStatusCode = response.StatusCode;
            logMetadata.ResponseStatusMessage = response.ReasonPhrase;
            logMetadata.ResponseTimestamp = DateTime.Now;
            if (response != null && response.Content != null && response.Content.Headers != null &&
                response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType != null)
            {
                logMetadata.ResponseContentType = response.Content.Headers.ContentType.MediaType;
                logMetadata.RequestContentType = response.Content.Headers.ContentType.MediaType;
            }
            else
            {
                logMetadata.ResponseContentType = "";
                logMetadata.RequestContentType = "";
            }

            if (responseContent != null)
            {
                logMetadata.ResponseContent = responseContent.ReadAsStringAsync().Result;
            }
            else
            {
                logMetadata.ResponseContent = "";
            }

            return logMetadata;
        }
        private async Task<bool> SendToLog(LogMetadata logMetadata)
        {
            LoggerDB _loggerDB = new LoggerDB();
            if (!logMetadata.RequestUri.Contains("swagger"))
            {
                //_loggerDB.RequestAndResponseLogging(logMetadata);
            }
            return true;
        }
    }



    public class MessageLogging
    {
        public void IncomingMessageAsync(ApiLog apiLog)
        {
            apiLog.RequestType = "Request";
            var sqlErrorLogging = new ApiLogging();
            sqlErrorLogging.InsertLog(apiLog);
        }

        public void OutgoingMessageAsync(ApiLog apiLog)
        {
            apiLog.RequestType = "Response";
            var sqlErrorLogging = new ApiLogging();
            sqlErrorLogging.InsertLog(apiLog);
        }
    }


}