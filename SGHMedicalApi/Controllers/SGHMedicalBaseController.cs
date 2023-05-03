using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SGHMedicalApi.Common;

namespace SGHMedicalApi.Controllers
{
    [MvcExceptionHandler]
    [MvcActionLogger]
    [AllowCrossSite]
    [System.Web.Mvc.Authorize]
    public class SGHMedicalBaseController : Controller
    {
        public SGHMedicalBaseController()
        {

        }

        protected string GetUserIp()
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }
    }

    [ApiExceptionHandler]
    [ApiActionLogger]
    [System.Web.Http.Authorize]
    public class SGHMedicalBaseApiController : ApiController
    {
        public SGHMedicalBaseApiController()
        {

        }

    }
}