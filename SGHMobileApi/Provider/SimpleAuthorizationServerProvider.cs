using System;
using DataLayer;
using DataLayer.Data;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;

namespace SGHMobileApi.Provider
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            
            string clientId = string.Empty;
            if (context.Parameters.Where(f => f.Key == "clientid").Select(f => f.Value).SingleOrDefault() != null)
                clientId = context.Parameters.Where(f => f.Key == "clientid").Select(f => f.Value).SingleOrDefault()[0];

            context.OwinContext.Set<string>("ClientId", clientId);

            context.Validated(); //   
        }

        //public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{
        //    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
        //    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
        //    CommonDB _db = new CommonDB();

        //    string _clientId = context.OwinContext.Get<string>("ClientId");
        //    //var clientInfo = _db.GetClientInfo(_clientId);

        //    string _userName = ConfigurationManager.AppSettings["UserName"].ToString();
        //    string _password = ConfigurationManager.AppSettings["Password"].ToString();


        //    if (_userName != null && _password != null)
        //    {
        //        //string _userName = clientInfo.UserName;
        //        //string _password = clientInfo.Password;

        //        EncryptDecrypt _encrptDecryptObj = new EncryptDecrypt();
        //        _password = _encrptDecryptObj.Decrypt(_password, true);



        //        if (_userName != "" && _password != "")
        //        {
        //            if (_userName.ToLower() == context.UserName.ToLower() && _password == context.Password)
        //            {
        //                identity.AddClaim(new Claim("userName", "_userName"));

        //                var props = new AuthenticationProperties(new Dictionary<string, string>
        //                    {
        //                        {
        //                            "userdisplayname", context.UserName
        //                        },
        //                        {
        //                             "role", "admin"
        //                        }
        //                     });

        //                var ticket = new AuthenticationTicket(identity, props);
        //                context.Validated(ticket);
        //            }
        //            else
        //            {
        //                context.SetError("invalid_grant", "Provided username and password is incorrect");
        //                context.Rejected();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        context.SetError("invalid_clientId", "Provided ClientId is incorrect");
        //        context.Rejected();
        //    }

        //    return;

        //}



        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //bool b_isAuthenticate = false;
            //string StrLog_Comments = "", StrLog_Discription = "";


            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var db = new CommonDB();

            var clientId = context.OwinContext.Get<string>("ClientId");
            var userName = context.OwinContext.Get<string>("username");

            //StrLog_Comments = "UserName : " + userName + " ---- ClientID: " + clientId;

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
            {
                var clientInfo = db.GetClientInfo(clientId);
                if (!string.IsNullOrEmpty(clientInfo.UserName) && !string.IsNullOrEmpty(clientInfo.Password))
                {
                    if (string.Equals(clientInfo.UserName.ToLower(), context.UserName.ToLower(),
                        StringComparison.Ordinal) && string.Equals(clientInfo.Password, context.Password,
                        StringComparison.Ordinal))
                    {


                        identity.AddClaim(new Claim("UserName", clientInfo.UserName));
                        identity.AddClaim(new Claim("Role", clientInfo.Role));
                        identity.AddClaim(new Claim("UserID", clientInfo.UserID.ToString()));

                        //StrLog_Comments += " Role : " + clientInfo.Role;

                        var props = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                {
                                    "userdisplayname", context.UserName
                                },
                                {
                                     "role", clientInfo.Role
                                }
                             });

                        var ticket = new AuthenticationTicket(identity, props);
                        context.Validated(ticket);

                        /*
                                                b_isAuthenticate = true;
                        */

                    }
                    else
                    {
                        //StrLog_Discription = "Rejected token generation: User or Pwd not match";
                        context.SetError("invalid_authentication", "Provided username and password is incorrect");
                        context.Rejected();
                    }

                }
                else
                {
                    //StrLog_Discription = "Rejected token generation: Invalid User or Pwd Info";
                    context.SetError("invalid_client details", "Provided Client details is invalid.");
                    context.Rejected();
                }
            }
            else
            {

                //StrLog_Discription = "Rejected token generation: Invalid Client ID";
                context.SetError("invalid_clientId", "Provided ClientId is incorrect");
                context.Rejected();
            }

            // Rejected 



            //DataLog_DB.ADDLOGS_DB("token", StrLog_Discription, _clientId, "RequestMethod", StrLog_Comments);

            return;

        }
    }
}