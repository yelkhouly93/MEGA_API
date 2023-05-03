using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SGHMobileApi.Extension
{
    //public class AuthenticationFailureResult : IHttpActionResult
    //{
    //    public string ReasonPhrase { get; private set; }
    //    public HttpRequestMessage Request { get; private set; }

    //    public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
    //    {
    //        ReasonPhrase = reasonPhrase;
    //        Request = request;
    //    }

    //    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    //    {
    //        return Task.FromResult(execute());
    //    }

    //    private HttpResponseMessage execute()
    //    {
    //        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
    //        response.RequestMessage = Request;
    //        response.ReasonPhrase = ReasonPhrase;
    //        return response;
    //    }
    //}
    public class AuthenticationFailureResult : IHttpActionResult
    {
        private object ResponseMessage;
        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request, object responseMessage)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
            ResponseMessage = responseMessage;
        }

        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public string ReasonPhrase { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            System.Net.Http.Formatting.MediaTypeFormatter jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            response.Content = new System.Net.Http.ObjectContent<object>(ResponseMessage, jsonFormatter);
            response.RequestMessage = Request;
            response.ReasonPhrase = ReasonPhrase;
            return response;
        }
    }
}