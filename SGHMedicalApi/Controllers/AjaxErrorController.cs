using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace SGHMedicalApi.Controllers
{
    /// <summary>
    /// Used when an ajax request throws error.
    /// This enable you return a json response.
    /// </summary>
    public class AjaxErrorController : Controller
    {
        public JsonResult GetJsonError(Exception ex)
        {
            //you can also manipulate your exception before sending back to user.
            //e.g. log to text fles, return custom error message or etc.
            return Json(new { Success = false, ex.Message, ex.StackTrace }, JsonRequestBehavior.AllowGet);
        }
    }

}
