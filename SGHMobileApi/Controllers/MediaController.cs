using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Formatting;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Description;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Common;
using SGHMobileApi.Extension;
using SmartBookingService.Controllers.ClientApi;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class MediaController : ApiController
    {
        // GET: Media
        
        [HttpPost]
        [Route("v2/News-Notification-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult NewsNotificationGet(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["content_type"]))
            {
                var lang = col["lang"];
                var ContentType = Convert.ToInt32(col["content_type"]);
                var hospitaId = col["hospital_id"];

                MediaDB _MediaDB = new MediaDB();

                var _allDTResults = _MediaDB.GetNewsAnnoucementDT(lang, hospitaId, ContentType);
                if (_allDTResults != null && _allDTResults.Rows.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = "Records Found(s)";
                    resp.response = _allDTResults;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "No Record Found";
                }

            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameters";
            }


            return Ok(resp);
        }


        [HttpPost]
        [Route("v2/Media-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult MediaGalleryGet(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["content_type"]))
            {
                var lang = col["lang"];
                var ContentType = Convert.ToInt32(col["content_type"]);
                var hospitaId = col["hospital_id"];

                MediaDB _MediaDB = new MediaDB();
                var _allDTResults = _MediaDB.GetMediaDT(lang, hospitaId, ContentType);

                if (_allDTResults != null && _allDTResults.Rows.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = "Records Found(s)";
                    resp.response = _allDTResults;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "No Record Found";
                }

            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameters";
            }


            return Ok(resp);
        }


        [HttpPost]
        [Route("v4/News-Notification-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult NewsNotificationGet_V4(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["content_type"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Country_id"]))
            {
                var lang = col["lang"];
                var ContentType = Convert.ToInt32(col["content_type"]);
                var hospitaId = col["hospital_id"];
                var CountryID = col["Country_id"];

                MediaDB _MediaDB = new MediaDB();

                var _allDTResults = _MediaDB.GetNewsAnnoucementDT_V4(lang, hospitaId, ContentType, CountryID);
                if (_allDTResults != null && _allDTResults.Rows.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = "Records Found(s)";
                    resp.response = _allDTResults;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "No Record Found";
                }

            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameters";
            }


            return Ok(resp);
        }

    }
}