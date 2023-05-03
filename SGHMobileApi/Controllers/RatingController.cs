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
    public class RatingController : ApiController
    {

        private RatingDB _RatingDB = new RatingDB();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [Route("v2/app-rating-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostAppRating(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (col != null)
                {
                    if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Screen_Name"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Service_Date"]))
                    {

                        var lang = "EN";
                        if (!string.IsNullOrEmpty(col["lang"]))
                            lang = col["lang"].ToString();

                        var hospitaId = Convert.ToInt32(col["hospital_id"]);
                        var ScreenName = col["Screen_Name"].ToString();
                        var patient_reg_no = Convert.ToInt32(col["patient_reg_no"]);
                        var Service_Date = Convert.ToDateTime(col["Service_Date"]);


                        var RatingId = 0;
                        if (!string.IsNullOrEmpty(col["RatingId"]))
                            RatingId = Convert.ToInt32(col["RatingId"]);

                        decimal StarRate = 0;
                        if (!string.IsNullOrEmpty(col["Star_Rate"]))
                            StarRate = Convert.ToDecimal(col["Star_Rate"]);

                        string Comments = "";
                        if (!string.IsNullOrEmpty(col["Comments"]))
                            Comments = col["Comments"].ToString();

                        bool Compeleted = false;
                        if (!string.IsNullOrEmpty(col["Compeleted"]))
                        {
                            if (col["Compeleted"].ToString() == "1")
                                Compeleted = true;
                        }
                            

                        int Service_RecordID = 0;
                        if (!string.IsNullOrEmpty(col["Service_RecordID"]))
                            Service_RecordID = Convert.ToInt32(col["Service_RecordID"]);

                        var errStatus = 0;
                        var errMessage = "";
                        
                        _RatingDB.SaveAppRating(lang, RatingId, hospitaId, patient_reg_no, ScreenName, StarRate, Comments, Compeleted, Service_RecordID, Service_Date, ref errStatus, ref errMessage);


                        resp.status = errStatus;
                        resp.msg = errMessage;
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
                Log.Error(ex);
                resp.status = 0;
            }

            return Ok(resp);
        }



        [HttpPost]
        [Route("v2/app-Rating-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAppRatingList(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];



                    var hospitalId = Convert.ToInt32(col["hospital_id"]);                
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    //var errStatus = 0;
                    //var errMessage = "";

                    
                    var DTList = _RatingDB.GetUserRating_List(lang, hospitalId, patientMrn);

                    if (DTList != null && DTList.Rows.Count > 1)
                    {
                        resp.status = 1;
                        resp.msg = "Record Found";
                        resp.response = DTList;
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
                    resp.msg = "Missing Parameter!";
                }


                return Ok(resp);
            }
            catch (Exception ex)
            {

                Log.Error(ex);
            }

            return Ok();
        }


        [HttpPost]
        [Route("v2/app-rating-update")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult UpdateAppRating(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (col != null)
                {
                    if (!string.IsNullOrEmpty(col["RatingId"]) && !string.IsNullOrEmpty(col["Star_Rate"]) && !string.IsNullOrEmpty(col["Comments"]) && Convert.ToInt32(col["RatingId"]) != 0)
                    {
                        var RatingId = Convert.ToInt32(col["RatingId"]);
                        decimal StarRate = Convert.ToDecimal(col["Star_Rate"]);
                        string Comments = col["Comments"].ToString();
                        var errStatus = 0;
                        var errMessage = "";

                        _RatingDB.UpdateAppRating(RatingId,StarRate, Comments, ref errStatus, ref errMessage);
                        


                        resp.status = errStatus;
                        resp.msg = errMessage;
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
                Log.Error(ex);
                resp.status = 0;
            }

            return Ok(resp);
        }


    }
}
