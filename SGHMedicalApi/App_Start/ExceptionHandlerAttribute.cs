using log4net;
using SGHMedicalApi.Controllers;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace SGHMedicalApi.Common
{
    public class MvcExceptionHandlerAttribute : HandleErrorAttribute
    {

        public override void OnException(System.Web.Mvc.ExceptionContext context)
        {
            if (context.ExceptionHandled) return;

            Exception ex = context.Exception;
            context.ExceptionHandled = true;
            var controllerName = context.RouteData.Values["controller"].ToString();
            var actionName = context.RouteData.Values["action"].ToString();

            var log = LogManager.GetLogger(typeof(MvcExceptionHandlerAttribute).Name);
            log.Error(string.Format("Error while executing {0}: {1}/{2} " + System.Environment.NewLine
                + "Parameter: {3}", context.RequestContext.HttpContext.Request.HttpMethod,
                controllerName, actionName, context.HttpContext.Request.Form));
            if (!context.Exception.Message.Contains("Ip address doesn't exist.")
                && !context.Exception.Message.Contains("Cannot connect to QMS. Device is not registered.")) //log only Stacktrace for known client 
            {
                log.Error(string.Format("Stacktrace:  {0}", context.Exception));
            }

            if (context.HttpContext.Request.IsAjaxRequest())
            {
                // if request was an Ajax request, respond with json with Error field
                var jsonResult = new AjaxErrorController { ControllerContext = context }.GetJsonError(context.Exception);
                jsonResult.ExecuteResult(context);
            }
            else
            {
                // if not an ajax request, continue with logic implemented by MVC -> html error page
                base.OnException(context);
            }
        }
    }

    public class ApiExceptionHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Exception ex = context.Exception;
            var url = context.Request.RequestUri.ToString();

            var log = LogManager.GetLogger(typeof(ApiExceptionHandlerAttribute).Name);
            log.Error(string.Format("Error while executing {0}: {1} \n{2}", context.Request.Method, url, ex.Message));

            if (!context.Exception.Message.Contains("Ip address doesn't exist.")
                && !context.Exception.Message.Contains("Cannot connect to QMS. Device is not registered.")) //log only Stacktrace for known client 
            {
                log.Error(string.Format("Stacktrace:  {0}", context.Exception));
            }

            HttpResponseMessage msg = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(context.Exception.Message),
                ReasonPhrase = context.Exception.Message
            };

            context.Response = msg;
        }

    }

    
}
