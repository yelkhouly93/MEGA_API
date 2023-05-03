using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.DirectoryServices;
using DataLayer;


namespace SGHMobileApi.Extension
{
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string _userName = ConfigurationManager.AppSettings["UserName"].ToString();
            string _password = ConfigurationManager.AppSettings["Password"].ToString();

            EncryptDecrypt _encrptDecryptObj = new EncryptDecrypt();
            _password = _encrptDecryptObj.Decrypt(_password, true);
             
            if (userName != _userName || password != _password)
            {
                // No user with userName/password exists.
                return null;
            }

            // Create a ClaimsIdentity with all the claims for this user.
            Claim nameClaim = new Claim(ClaimTypes.Name, userName);
            List<Claim> claims = new List<Claim> { nameClaim };

            // important to set the identity this way, otherwise IsAuthenticated will be false
            // see: http://leastprivilege.com/2012/09/24/claimsidentity-isauthenticated-and-authenticationtype-in-net-4-5/
            ClaimsIdentity identity = new ClaimsIdentity(claims, "AuthenticationTypes.Basic");

            var principal = new ClaimsPrincipal(identity);
            return principal;
        }

    }
}