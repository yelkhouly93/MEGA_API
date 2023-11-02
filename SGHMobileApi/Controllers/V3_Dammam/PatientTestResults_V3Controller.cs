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
    public class PatientTestResults_V3Controller : ApiController
    {


        [HttpPost]
        [Route("v3/test-List-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientTestReportsList(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                int errStatus = 0;
                string errMessage = "";

                var patientDb = new PatientDB();

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Source"]))
                    ApiSource = col["Source"].ToString();

                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();


                var EpisodeType = "OP";
                var EpisodeID = 0;

                if (!string.IsNullOrEmpty(col["Episode_Type"]))
                    EpisodeType = col["Episode_Type"];
                if (!string.IsNullOrEmpty(col["Episode_Id"]))
                    EpisodeID = Convert.ToInt32(col["Episode_Id"]);

                if (EpisodeType.ToUpper() != "OP" && EpisodeType.ToUpper() != "IP")
                {
                    resp.status = 0;
                    resp.msg = "WRONG Episode Type";
                    return Ok(resp);
                }

                var PatientData = new List<PateintTests>();
                if (hospitaId == 9)
				{
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    PatientData = _loginApiCaller.GetPatientLabRadiologyByApi_NewDam(lang, registrationNo.ToString(), ref errStatus, ref errMessage);
                }
                else
				{
                    PatientData = patientDb.GetPatientTestResultsNew(lang, hospitaId, registrationNo, ref errStatus, ref errMessage, ApiSource, EpisodeType, EpisodeID);                    
                }

                if (PatientData != null && PatientData.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = errMessage;
                    resp.response = PatientData;
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
            return Ok(resp);
        }



        [HttpPost]
        [Route("v3/test-resultdetails-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetail(FormDataCollection col)
        {
            var resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["test_id"]))
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var testId = Convert.ToInt32(col["test_id"]);

                int errStatus = 0;
                string errMessage = "";

                var patientDb = new PatientDB();
                var apiCaller = new PatientLabResultsApiCaller();

                //var allPatientResults = apiCaller.GetPatientTestResultsAPI(lang, hospitaId, testId, ref errStatus, ref errMessage);
                if (testId == 0)
				{
                    resp.status = 0;
                    resp.msg = "Currently Not Available For dammam.";
                    return Ok(resp);
                }
                var allPatientResults = apiCaller.GetPatientLabResultsByApi(lang, hospitaId, testId, ref errStatus, ref errMessage);

                if (allPatientResults != null)
                {
                    resp.status = 1;
                    resp.msg = errMessage;
                    resp.response = allPatientResults;

                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Resutls Empty";
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter.";
            }


            return Ok(resp);
        }



        [HttpPost]
        [Route("v3/testresult-pdf-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetailPDF(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["test_id"]))
            {
                var lang = col["lang"];
                //var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var testId = col["test_id"];
                
                if (Convert.ToInt32(testId) == 0)
                {
                    resp.status = 0;
                    resp.msg = "Currently Not Available For dammam.";
                    return Ok(resp);
                }

                var tmpobj = new TestResultPDF();

                string Str_Id = "Test_Id=" + testId;
                var ParmEnc = TripleDESImp.TripleDesEncrypt(Str_Id);
                var FinalURL = ConfigurationManager.AppSettings["LabResultURL"].ToString() + ParmEnc;

                tmpobj.ReportUrl = Util.ConvertURL_to_PDF(FinalURL, testId.ToString());
                if (tmpobj.ReportUrl != null)
                {
                    resp.status = 1;
                    resp.msg = "Result found";
                    resp.response = tmpobj;

                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Resutls Empty";
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter";
            }
            return Ok(resp);
        }

    }
}
