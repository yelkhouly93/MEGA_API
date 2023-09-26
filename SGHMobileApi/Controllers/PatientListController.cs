using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SGHMobileApi.Extension;
using DataLayer.Model;
using DataLayer.Data;
using SGHMobileApi.Common;
using System.Web.Http.Description;
using System.Net.Http.Formatting;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class PatientListController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();


        [HttpPost]
        [Route("v2/patientList-byDate-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientListByDateRange(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["StartDate"]) && !string.IsNullOrEmpty(col["EndDate"]) )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                
                try
                {
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var StartDate = Convert.ToDateTime(col["StartDate"]);
                    var EndDate = Convert.ToDateTime(col["EndDate"]);

                    var errStatus = 0;
                    var errMessage = "";

                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    var allPatientList = patientDb.Get_Patient_List_By_DateRange(lang, hospitalId, StartDate, EndDate, ref errStatus, ref errMessage, ApiSource);
                    _resp.status = errStatus;
                    _resp.msg = errMessage;
                    _resp.response = allPatientList;
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }                
                
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        [HttpPost]
        [Route("v2/patientUpdatedList-byDate-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientUpdatedListByDateRange(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["UpdatedDate"])
                && !string.IsNullOrEmpty(col["StartDate"]) && !string.IsNullOrEmpty(col["EndDate"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                try
                {
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var StartDate = Convert.ToDateTime(col["StartDate"]);
                    var EndDate = Convert.ToDateTime(col["EndDate"]);
                    var UpdatedDate = Convert.ToDateTime(col["UpdatedDate"]);

                    var errStatus = 0;
                    var errMessage = "";

                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();

                    var allPatientList = patientDb.Get_Patient_Updated_List_By_DateRange(lang, hospitalId, StartDate, EndDate, UpdatedDate, ref errStatus, ref errMessage, ApiSource);
                    _resp.status = errStatus;
                    _resp.msg = errMessage;
                    _resp.response = allPatientList;
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }


        [HttpPost]
        [Route("v2/patientFamilyProfile-List-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientFamilyProfileList(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Source"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                try
                {
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);                    

                    var errStatus = 0;
                    var errMessage = "";

                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Source"]))
                        ApiSource = col["Source"].ToString();

                    var allPatientList = patientDb.GetPatientFamilyProfile_List(lang, hospitalId,PatientMRN, ApiSource, ref errStatus, ref errMessage);
                    _resp.status = errStatus;
                    _resp.msg = errMessage;
                    _resp.response = allPatientList;
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }


    }
}
