
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
                    if (SpDatatable != null )
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


        //[HttpPost]
        //[Route("v2/Patient-My-VitalSign-Details-Get")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PatientMyVitalSignDetails(FormDataCollection col)
        //{
        //    GenericResponse resp = new GenericResponse();
            
        //        if (!string.IsNullOrEmpty(col["hospital_id"]) 
        //            && !string.IsNullOrEmpty(col["patient_reg_no"])
        //            && !string.IsNullOrEmpty(col["Vital"])
        //            )
        //        {
        //            var lang = "EN";
        //            if (!string.IsNullOrEmpty(col["lang"]))
        //                lang = col["lang"];

        //            var hospitaId = Convert.ToInt32(col["hospital_id"]);
        //            var patientId = Convert.ToInt32(col["patient_reg_no"]);
        //            var VitalSign = col["Vital"];

        //            if (!Util.OasisBranches.Contains(hospitaId))
        //            {
        //                var DataDb = new VitalSignDB();
        //                var SpDatatable = DataDb.GET_Patient_My_VitalSign_Detail_DT(lang, patientId ,  hospitaId ,VitalSign);
        //                if (SpDatatable != null && SpDatatable.Rows.Count > 0)
        //                {
        //                    resp.status = 1;
        //                    resp.msg = "Record Found";
        //                    resp.response = SpDatatable;
        //                }
        //                else
        //                {
        //                    resp.status = 0;
        //                    resp.msg = "NO Record Found";
        //                }
        //            }
        //            else
        //            {
        //                resp.status = 0;
        //                resp.msg = "NO Record Found";
        //            }
        //        }
        //        else
        //        {
        //            resp.status = 0;
        //            resp.msg = "Failed : Missing Parameters";
        //        }

            




        //    return Ok(resp);
        //}

        [HttpPost]
        [Route("v2/Patient-Device-VitalSign-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientDeviceVitalSignGet(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var patientId = Convert.ToInt32(col["patient_reg_no"]);

                var DataDb = new VitalSignDB();
                var SpDatatable = DataDb.GET_Patient_My_VitalSign_DT(lang, patientId, hospitaId);
                if (SpDatatable != null)
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
        [Route("v2/Patient-Device-VitalSign-Save")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientDeviceVitalSignSave(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["Vital_Name"])
                && !string.IsNullOrEmpty(col["Vital_Value"])
                && !string.IsNullOrEmpty(col["Sources"])
                )
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var patientId = col["patient_reg_no"];
                var VitalName = col["Vital_Name"];
                var VitalValue = col["Vital_Value"];
                var Source = col["Sources"];

                var VitalValue2 = "";
                var DeviceName = "";
                

                if (!string.IsNullOrEmpty(col["Vital_Value2"]))
                    VitalValue2 = col["Vital_Value2"].ToString();

                if (!string.IsNullOrEmpty(col["Device_Name"]))
                    DeviceName = col["Device_Name"].ToString();

                int iStatus = 0;
                string ErrorMsg = "";

                var DataDb = new VitalSignDB();
                var SpDatatable = DataDb.Save_Patient_Divice_VitalSign(patientId, hospitaId , VitalName,VitalValue , VitalValue2 , DeviceName , Source ,  ref iStatus , ref ErrorMsg);


                    resp.status = iStatus;
                    resp.msg = ErrorMsg;
                    return Ok(resp);    
            //if (SpDatatable != null && SpDatatable.Rows.Count > 0)
            //{
            //    resp.status = 1;
            //    resp.msg = "Record Found";
            //    resp.response = SpDatatable;
            //}
            //else
            //{
            //    resp.status = 0;
            //    resp.msg = "NO Record Found";
            //}

        }
            else
            {
                resp.status = 0;
                resp.msg = "Failed : Missing Parameters";
            }


            return Ok(resp);
        }

        [HttpPost]
        [Route("v2/Patient-VitalSign-Bulk-Save")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientDeviceVitalSignSave_Bulk(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])                
                && !string.IsNullOrEmpty(col["Sources"])
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"].ToString();
                
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var patientId = col["patient_reg_no"];                
                var Source = col["Sources"];

                
                var DeviceName = "";
                var HEIGHT = "";
                var HEART_RATE = "";
                var BODY_TEMPERATURE = "";                
                var BLOOD_PRESSURE_DIASTOLIC = "";
                var BLOOD_PRESSURE_SYSTOLIC =   "";
                var BLOOD_OXYGEN = "";
                var STEPS = "";
                var ACTIVE_ENERGY_BURNED = "";
                var BLOOD_GLUCOSE = "";
                var BODY_FAT_PERCENTAGE = "";
                var BODY_MASS_INDEX = "";
                var SLEEP_IN_BED = "";                
                var WATER = "";
                var WEIGHT = "";


                if (!string.IsNullOrEmpty(col["HEIGHT"]))
                    HEIGHT = col["HEIGHT"].ToString();

                if (!string.IsNullOrEmpty(col["HEART_RATE"]))
                    HEART_RATE = col["HEART_RATE"].ToString();

                if (!string.IsNullOrEmpty(col["BODY_TEMPERATURE"]))
                    BODY_TEMPERATURE = col["BODY_TEMPERATURE"].ToString();

                if (!string.IsNullOrEmpty(col["BLOOD_PRESSURE_DIASTOLIC"]))
                    BLOOD_PRESSURE_DIASTOLIC = col["BLOOD_PRESSURE_DIASTOLIC"].ToString();


                if (!string.IsNullOrEmpty(col["BLOOD_PRESSURE_SYSTOLIC"]))
                    BLOOD_PRESSURE_SYSTOLIC = col["BLOOD_PRESSURE_SYSTOLIC"].ToString();

                if (!string.IsNullOrEmpty(col["BLOOD_OXYGEN"]))
                    BLOOD_OXYGEN = col["BLOOD_OXYGEN"].ToString();

                if (!string.IsNullOrEmpty(col["STEPS"]))
                    STEPS = col["STEPS"].ToString();

                if (!string.IsNullOrEmpty(col["ACTIVE_ENERGY_BURNED"]))
                    ACTIVE_ENERGY_BURNED = col["ACTIVE_ENERGY_BURNED"].ToString();

                if (!string.IsNullOrEmpty(col["BLOOD_GLUCOSE"]))
                    BLOOD_GLUCOSE = col["BLOOD_GLUCOSE"].ToString();

                if (!string.IsNullOrEmpty(col["BODY_FAT_PERCENTAGE"]))
                    BODY_FAT_PERCENTAGE = col["BODY_FAT_PERCENTAGE"].ToString();

                if (!string.IsNullOrEmpty(col["BODY_MASS_INDEX"]))
                    BODY_MASS_INDEX = col["BODY_MASS_INDEX"].ToString();

                if (!string.IsNullOrEmpty(col["SLEEP_IN_BED"]))
                    SLEEP_IN_BED = col["SLEEP_IN_BED"].ToString();

                if (!string.IsNullOrEmpty(col["WATER"]))
                    WATER = col["WATER"].ToString();

                if (!string.IsNullOrEmpty(col["WEIGHT"]))
                    WEIGHT = col["WEIGHT"].ToString();

                if (!string.IsNullOrEmpty(col["Device_Name"]))
                    DeviceName = col["Device_Name"].ToString();

                int iStatus = 0;
                string ErrorMsg = "";

                var DataDb = new VitalSignDB();
                var SpDatatable = DataDb.Save_Patient_Divice_VitalSign_BULK(patientId, hospitaId,DeviceName,Source,
                    HEIGHT,HEART_RATE,BODY_TEMPERATURE,BLOOD_PRESSURE_DIASTOLIC,BLOOD_PRESSURE_SYSTOLIC,
                    BLOOD_OXYGEN,STEPS,ACTIVE_ENERGY_BURNED,BLOOD_GLUCOSE,BODY_FAT_PERCENTAGE,BODY_MASS_INDEX,SLEEP_IN_BED,
                    WEIGHT, WATER,ref iStatus, ref ErrorMsg);


                resp.status = iStatus;
                resp.msg = ErrorMsg;
                return Ok(resp);
                //if (SpDatatable != null && SpDatatable.Rows.Count > 0)
                //{
                //    resp.status = 1;
                //    resp.msg = "Record Found";
                //    resp.response = SpDatatable;
                //}
                //else
                //{
                //    resp.status = 0;
                //    resp.msg = "NO Record Found";
                //}

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
