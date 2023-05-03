using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Filters;
using System.Web.UI;
using SGHMobileApi.Common;
using WebApi.AuthenticationFilter;
using System.Net.Http;

namespace SGHMobileApi.Extension
{
    //public class AuthorizationAttributeExtension
    //{
    //}
    public class AuthenticationFilter : AuthenticationFilterAttribute
    {

        public override void OnAuthentication(HttpAuthenticationContext context)
        {
            System.Net.Http.Formatting.MediaTypeFormatter jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            var ci = context.Principal.Identity as ClaimsIdentity;

            //First of all we are going to check that the request has the required Authorization header. If not set the Error
            var authHeader = context.Request.Headers.Authorization;
            //Change "Bearer" for the needed schema
            if (authHeader == null || authHeader.Scheme.ToString().ToLower() != "bearer")
            {

                context.ErrorResult = context.ErrorResult = new AuthenticationFailureResult("unauthorized", context.Request,
                    new { Error = new { Code = 401, Message = "Request require authorization" } });
            }
            //If the token has expired the property "IsAuthenticated" would be False, then set the error
            else if (!ci.IsAuthenticated)
            {
                context.ErrorResult = new AuthenticationFailureResult("ExPired", context.Request,
                    new { Error = new { Code = 401, Message = "The Token has expired" } });
            }

            if (ci.IsAuthenticated)
            {
                // Log Here Authenticate

                //Request.GetOwinContext().Request.RemoteIpAddress
                //DataLog

                //DataLog_DB.ADDLOGS_DB("", "", "", "");

                // ci is role admin bypass
                // else 

                var apiCall = context.Request.RequestUri.LocalPath;
                var APiMethod = context.Request.Method.ToString();
                var APIOrignalURL = context.Request.RequestUri.OriginalString.ToString();
                var APIUserAgent = context.Request.Headers.UserAgent.ToString();

                //var requestMessage = context.Request.Content.ReadAsByteArrayAsync();
                //HttpRequestMessage request = context.Request;
                //var requestMessage = await request.Content.ReadAsByteArrayAsync();
                //var requestMessage = context.Request.Content.ReadAsByteArrayAsync().ToString();
                //var APIParameters = Encoding.UTF8.GetString(requestMessage);
                
                
                



                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                var claimUserId = claims
                    ?.FirstOrDefault(x => x.Type.Equals("UserName", StringComparison.OrdinalIgnoreCase))?.Value;
                var claimrole = claims
                    ?.FirstOrDefault(x => x.Type.Equals("Role", StringComparison.OrdinalIgnoreCase))?.Value;

                //ClaimsPrincipal principal = context.Principal as ClaimsPrincipal;                
                //string Claimrole = principal.Claims.FirstOrDefault(
                //                        c => c.Type == ClaimTypes.Role)?.Value;
                if (claimrole != null)
                {
                    DataLog_DB.SAVE_API_CALL_LOGS_DB(apiCall, APiMethod, APIOrignalURL, APIUserAgent, "", claimUserId, claimrole);

                    if (claimrole == "Admin") return;
                    if (!(DataLog_DB.CheckAccess(apiCall, claimUserId)))
                    {
                        context.ErrorResult = new AuthenticationFailureResult("DB Access Denied", context.Request,
                            new { Error = new { Code = 401, Message = "DB Access Denied : You don't have access to this API" } });
                    }
                }
                else
                {
                    context.ErrorResult = new AuthenticationFailureResult("Claim Access Denied", context.Request,
                        new { Error = new { Code = 401, Message = "You don't have access to this API" } });
                }




            }


        }

        //public override void OnAuthentication(HttpAuthenticationContext context)
        //{
        //    System.Net.Http.Formatting.MediaTypeFormatter jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
        //    var ci = context.Principal.Identity as ClaimsIdentity;

        //    //First of all we are going to check that the request has the required Authorization header. If not set the Error
        //    var authHeader = context.Request.Headers.Authorization;
        //    //Change "Bearer" for the needed schema
        //    if (authHeader == null || authHeader.Scheme.ToString().ToLower() != "bearer")
        //    {
        //        context.ErrorResult = context.ErrorResult = new AuthenticationFailureResult("unauthorized", context.Request,
        //            new { Error = new { Code = 401, Message = "Request require authorization" } });
        //    }
        //    //If the token has expired the property "IsAuthenticated" would be False, then set the error
        //    else if (!ci.IsAuthenticated)
        //    {
        //        context.ErrorResult = new AuthenticationFailureResult("unauthorized", context.Request,
        //            new { Error = new { Code = 401, Message = "The Token has expired" } });
        //    }


        //}

    }
}


