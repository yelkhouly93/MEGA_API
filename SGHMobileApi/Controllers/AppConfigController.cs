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

        private GenericResponse _resp = new GenericResponse();
        private AppConfigDB _AppconfigDb = new AppConfigDB();


        // GET: AppConfig
        [HttpPost]
        [Route("v2/app-Modules-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAppModuleList(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            resp.status = 0;
            resp.msg = "Failed : Missing Parameters";

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["ClientKey"]) )
                {
                    var ClientKey = col["ClientKey"].ToString();

                    var _ReturnModal = _AppconfigDb.GetClintModuleList (ClientKey);
                    if (_ReturnModal != null)
                    {
                        resp.status = 1;
                        resp.msg = "Record Found";
                        resp.response = _ReturnModal;
                    }
                }
            }
            return Ok(resp);
        }

    }
}