using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SGHMobileApi.Common;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Description;


namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class AppConfigController : ApiController
    {
        // GET: AppConfig
        public ActionResult Index()
        {
            return View();
        }
    }
}