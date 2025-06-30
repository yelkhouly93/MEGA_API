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
using System.Web.Routing;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class TrackingLogsController : ApiController
    {
        private TrackingLogsDB _TrackLogDB = new TrackingLogsDB();

        // GET: TrackingLogs
        [HttpPost]
        [Route("v2/app-tracking-logs")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppRating(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            resp.status = 0;
            resp.msg = "Missing Parameter!";


            // Get the route data
            var routeData = Request.GetRouteData();

            // Get the route template string
            var routeTemplate = routeData.Route.RouteTemplate;


            try
            {
                if (col != null)
                {
                    if (
                            !string.IsNullOrEmpty(col["hospital_id"]) && 
                            !string.IsNullOrEmpty(col["patient_reg_no"]) && 
                            !string.IsNullOrEmpty(col["APP_JOURNEY_FLAG"]) &&                            
                            !string.IsNullOrEmpty(col["APP_JOURNEY_DEPTID"]) &&
                            !string.IsNullOrEmpty(col["APP_JOURNEY_DOCID"]) &&
                            !string.IsNullOrEmpty(col["Entry_Purpose"]) &&
                            !string.IsNullOrEmpty(col["App_ID"]) &&
                            !string.IsNullOrEmpty(col["Lang"]) &&
                            !string.IsNullOrEmpty(col["Sources"])
                        )
                    {
                        var hospitaId = Convert.ToInt32(col["hospital_id"]);
                        
                        var patient_reg_no = col["patient_reg_no"];
                        var APP_JOURNEY_FLAG = col["APP_JOURNEY_FLAG"];
                        var APP_JOURNEY_DEPTID = col["APP_JOURNEY_DEPTID"];
                        var APP_JOURNEY_DOCID = col["APP_JOURNEY_DOCID"];
                        var Entry_Purpose = col["Entry_Purpose"];
                        var App_ID = col["App_ID"];
                        var Lang = col["Lang"];


                        var patient_Name = "";
                        if (!string.IsNullOrEmpty(col["patient_name"]))
                            patient_Name = col["patient_name"];


                        var Sources = col["Sources"].ToString();

                        var IdentiferName = "APP_JOURNEY_FLAG~APP_JOURNEY_BRANCH~APP_JOURNEY_MRN~APP_JOURNEY_DEPTID~APP_JOURNEY_DOCID~APP_JOURNEY_PATIENT_NAME";
                        var IdentiferValues = APP_JOURNEY_FLAG.ToString()+'~'+ hospitaId + '~' + patient_reg_no + '~' + APP_JOURNEY_DEPTID + '~' + APP_JOURNEY_DOCID + '~' + patient_Name ;
                        var isUpdate = true;
                        
                        if (APP_JOURNEY_DEPTID == "0" && APP_JOURNEY_DOCID == "0")
                            isUpdate = false;

                        _TrackLogDB.SaveTrackingLogs(Entry_Purpose,hospitaId.ToString(), patient_reg_no, IdentiferName, IdentiferValues, App_ID, Lang, isUpdate);

                        resp.status = 1;
                        resp.msg = "Record has been Added updated Successfully.";

                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "Failed : Missing Parameters";
                    }

                }

            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                resp.status = 0;
            }

            return Ok(resp);
        }

    }
}