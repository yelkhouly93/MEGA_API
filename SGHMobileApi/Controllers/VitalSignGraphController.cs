
using System.Collections.Generic;
using System.Web.Http;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System.Web.Http.Description;
using DataLayer.Data;
using System;
using System.Net.Http.Formatting;
using SmartBookingService.Controllers.ClientApi;
using SGHMobileApi.Common;
using System.Configuration;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class VitalSignGraphController : ApiController
    {
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [Route("Patient-ViatalSign-MainGraph-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientVitalSignMainGraph(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Source"]))
                {
                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);                    
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);
                    var Source = col["Source"].ToString();

                    var errStatus = 0;
                    var errMessage = "";
                    
                    

                    if (!Util.OasisBranches.Contains(hospitaId))
                    {
                        var DataDb = new VitalSignDB();
                        var SpDatatable= DataDb.GET_Patient_VitalSign_GraphData(lang, hospitaId, patientId, Source);
                        if (SpDatatable != null && SpDatatable.Rows.Count > 0 )
						{
                            errStatus = 1;
                            errMessage = "Record Found(s)";
                        }
                    }
                    else
                    {
                        errStatus = 0;
                        errMessage = "Not Avaialble in Current Branch.";
                    }
                    if (errStatus != 0)
                    {   
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = errStatus;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed : Missing Parameters";
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
        [Route("v2/Patient-My-VitalSign-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientMyVitalSign(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();            
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
                {
                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);

                    var DataDb = new VitalSignDB();
                    var SpDatatable = DataDb.GET_Patient_My_VitalSign_DT(lang, patientId, hospitaId );
                    if (SpDatatable != null && SpDatatable.Rows.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record Found";
                        resp.response = SpDatatable;
                    }
                    else
					{
                        resp.status = 0;
                        resp.msg = "NO Record Found";
                    }
                    
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed : Missing Parameters";
                }


            return Ok(resp);
        }


        [HttpPost]
        [Route("v2/Patient-My-VitalSign-Details-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientMyVitalSignDetails(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            
                if (!string.IsNullOrEmpty(col["hospital_id"]) 
                    && !string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["Vital"])
                    )
                {
                    var lang = "EN";
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);
                    var patientId = Convert.ToInt32(col["patient_reg_no"]);
                    var VitalSign = col["Vital"];

                    if (!Util.OasisBranches.Contains(hospitaId))
                    {
                        var DataDb = new VitalSignDB();
                        var SpDatatable = DataDb.GET_Patient_My_VitalSign_Detail_DT(lang, patientId ,  hospitaId ,VitalSign);
                        if (SpDatatable != null && SpDatatable.Rows.Count > 0)
                        {
                            resp.status = 1;
                            resp.msg = "Record Found";
                            resp.response = SpDatatable;
                        }
                        else
                        {
                            resp.status = 0;
                            resp.msg = "NO Record Found";
                        }
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "NO Record Found";
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Failed : Missing Parameters";
                }

            




            return Ok(resp);
        }


    }
}
