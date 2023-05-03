using log4net;
using Microsoft.Owin;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;


namespace SGHMedicalApi.Common
{
    public class MvcActionLoggerAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var logRequest = bool.Parse(ConfigurationManager.AppSettings["LogRequest"].ToString());
            if (logRequest)
            {
                var controllerName = filterContext.RequestContext.RouteData.Values["Controller"];
                var actionName = filterContext.RequestContext.RouteData.Values["Action"];
                var requesttype = filterContext.RequestContext.HttpContext.Request.HttpMethod;
                log4net.Config.XmlConfigurator.Configure();
                var log = LogManager.GetLogger("MVC");

                var parameters = "";
                if (requesttype == "GET")
                {
                    parameters = filterContext.RequestContext.HttpContext.Request.QueryString.ToString();
                }
                else
                {
                    foreach (var parameter in filterContext.ActionParameters)
                    {
                        parameters += string.Format("{0}:{1}", parameter.Key, parameter.Value) + ", ";
                    }
                }

                log.Info(string.Format("{0}-->{1}:{2}/{3} " + System.Environment.NewLine + "Parameter: {4}"
                    , filterContext.RequestContext.HttpContext.Request.UserHostAddress
                    , requesttype, controllerName, actionName, parameters));
            }

            base.OnActionExecuting(filterContext);
        }
    }


    public class ApiActionLoggerAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var logRequest = bool.Parse(ConfigurationManager.AppSettings["LogRequest"].ToString());
            if (logRequest)
            {
                var url = context.Request.RequestUri.ToString();
                var ip = context.Request.GetClientIpAddress();

                log4net.Config.XmlConfigurator.Configure();
                var log = LogManager.GetLogger("API");

                var parameters = "";
                foreach (var parameter in context.ControllerContext.RouteData.Values)
                {
                    parameters += string.Format("{0}:{1}", parameter.Key, parameter.Value) + ", ";
                }

                log.Info(string.Format("{0}-->{1}:{2}" + System.Environment.NewLine + "Parameter: {3}"
                    , ip, context.Request.Method, url, parameters));
            }

            base.OnActionExecuting(context);
        }



    }

    public static class ApiExtenstion
    {
        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
            }
            if (request.Properties.ContainsKey("MS_OwinContext"))
            {
                return IPAddress.Parse(((OwinContext)request.Properties["MS_OwinContext"]).Request.RemoteIpAddress).ToString();
            }
            return null;
        }
    }
}
