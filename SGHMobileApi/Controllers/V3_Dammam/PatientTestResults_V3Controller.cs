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
using DataLayer.Data.ORACLE;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class PatientTestResults_V3Controller : ApiController
    {

        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };

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
        [Route("v2/test-dammam")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult TESTDAMAM(FormDataCollection col)
        {

            _resp = new GenericResponse();
            ORACLECS_DB _OraDb = new ORACLECS_DB();
            _OraDb.TESTEXECUTE();

            return Ok(_resp);
        }


        [HttpPost]
        [Route("v3/test-resultdetails-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetail(FormDataCollection col)
        {
            var resp = new GenericResponse();
            //if (!string.IsNullOrEmpty(col["test_id"]) && !string.IsNullOrEmpty(col["hospital_id"])

            //    )

            if (!string.IsNullOrEmpty(col["test_id"]) )
                {
                var lang = col["lang"];
                //var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var hospitaId = 0;
                var testId = Convert.ToInt32(col["test_id"]);
                var ReportType = "Lab";

                if (!string.IsNullOrEmpty(col["Report_type"]))
                    ReportType = col["Report_type"].ToString();



                int errStatus = 0;
                string errMessage = "";

                var patientDb = new PatientDB();
                var apiCaller = new PatientLabResultsApiCaller();

                
    //            if (hospitaId == 9)
				//{
    //                ORACLECS_DB _OraDb = new ORACLECS_DB();
    //                string OraSQL = "";
                    
    //                var dataResults = _OraDb.EXECUTE_SQL_DT(OraSQL);
    //                if (dataResults != null)
				//	{
    //                    resp.status = 1;
    //                    resp.msg = "Data Found";
    //                    resp.response = dataResults;
    //                }
    //                else
				//	{
    //                    resp.status = 0;
    //                    resp.msg = "No data Found.";
    //                }
    //                return Ok(resp);
    //            }
                
                
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


        [HttpPost]
        [Route("v4/test-List-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientTestReportsList_V4(FormDataCollection col)
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

                //var PatientData = new List<PateintTests>();
                if (hospitaId > 300 && hospitaId < 400)
				{
                    // UAE
                    var UAEMRN = col["patient_reg_no"].ToString();
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var _NewData = _UAEApiCaller.GetPatientTestList_NewUAE(lang, hospitaId, UAEMRN, ref errStatus, ref errMessage);
                    
                    if (_NewData != null && _NewData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = _NewData;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
                    return Ok(resp);
                }
                else if (hospitaId > 200 && hospitaId < 300)
                {
                    // EYGPT
                    var EYGPTMRN = col["patient_reg_no"].ToString();
                    ApiCallerEygpt _EYGApiCaller = new ApiCallerEygpt();
                    var _NewData = _EYGApiCaller.GetPatientTestList_NewEYG(lang, hospitaId, EYGPTMRN, ref errStatus, ref errMessage);

                    if (_NewData != null && _NewData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = _NewData;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
                    return Ok(resp);
                }
                else if (hospitaId == 9)
                {
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var _NewData = _loginApiCaller.GetPatientLabRadiologyByApi_NewDam_V4(lang, registrationNo.ToString(), ref errStatus, ref errMessage);
                    if (_NewData != null && _NewData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = _NewData;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
                    return Ok(resp);
                }
                else
                {
                    var PatientData = patientDb.GetPatientTestResultsNew_V4(lang, hospitaId, registrationNo, ref errStatus, ref errMessage, ApiSource, EpisodeType, EpisodeID);
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
                    return Ok(resp);

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
        [Route("v4/test-resultdetails-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetail_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["test_id"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Report_type"]))
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                
                //var testId = Convert.ToInt32(col["test_id"]);
                var testId = col["test_id"].ToString();

                var ReportType = "Lab";
                if (!string.IsNullOrEmpty(col["Report_type"]))
                    ReportType = col["Report_type"].ToString();



                int errStatus = 0;
                string errMessage = "";

                var patientDb = new PatientDB();
                var apiCaller = new PatientLabResultsApiCaller();


                var allPatientResults = new TestResultMain();

                if (hospitaId > 300 && hospitaId < 400)
				{

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var _NewData = _UAEApiCaller.GetPatientTestResultsList_NewUAE(lang, hospitaId, testId, ref errStatus, ref errMessage);

                    //allPatientResults = apiCaller.GetPatientLabResultsByApi_UAE(lang, hospitaId, testId, ref errStatus, ref errMessage);

                    if (_NewData != null)
                    {
                        resp.status = 1;
                        resp.msg = "Data Found";
                        resp.response = _NewData;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No data Found.";
                    }
                    return Ok(resp);
                }
                else if (hospitaId == 9)
				{
					ORACLECS_DB _OraDb = new ORACLECS_DB();
					string OraSQL = "";

					var dataResults = _OraDb.GetPatientLabResultsByApi_Dam(testId.ToString() , ReportType );
					if (dataResults != null )
					{
						resp.status = 1;
						resp.msg = "Data Found";
						resp.response = dataResults;
					}
					else
					{
						resp.status = 0;
						resp.msg = "No data Found.";
					}
					return Ok(resp);
				}
                else
				{
                    allPatientResults = apiCaller.GetPatientLabResultsByApi_V4(lang, hospitaId, testId, ref errStatus, ref errMessage);
                }

                //var allPatientResults = apiCaller.GetPatientLabResultsByApi(lang, hospitaId, testId, ref errStatus, ref errMessage);

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
        [Route("v5/test-resultdetails-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetail_V5(FormDataCollection col)
        {
            var resp = new GenericResponse();
            
            if (!string.IsNullOrEmpty(col["test_id"]) && !string.IsNullOrEmpty(col["hospital_id"]) 
                && !string.IsNullOrEmpty(col["Report_type"])
                && !string.IsNullOrEmpty(col["patient_reg_no"])
                )
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);

                var MRN= Convert.ToInt32(col["patient_reg_no"]).ToString();
                var testId = col["test_id"].ToString();

                var ReportType = "Lab";
                if (!string.IsNullOrEmpty(col["Report_type"]))
                    ReportType = col["Report_type"].ToString();



                int errStatus = 0;
                string errMessage = "";

                var patientDb = new PatientDB();
                var apiCaller = new PatientLabResultsApiCaller();


                var allPatientResults = new TestResultMain();

                if (hospitaId > 300 && hospitaId < 400)
                {

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var _NewData = _UAEApiCaller.GetPatientTestResultsList_NewUAE(lang, hospitaId, testId, ref errStatus, ref errMessage);
                    
                    if (_NewData != null)
                    {
                        resp.status = 1;
                        resp.msg = "Data Found";
                        resp.response = _NewData;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No data Found.";
                    }
                    return Ok(resp);
                }
                else if (hospitaId > 200 && hospitaId < 300)
                {

                    ApiCallerEygpt _EYGApiCaller = new ApiCallerEygpt();
                    var _NewData = _EYGApiCaller.GetPatientTestResultsList_NewEYG(lang, hospitaId, testId, ref errStatus, ref errMessage);

                    if (_NewData != null)
                    {
                        resp.status = 1;
                        resp.msg = "Data Found";
                        resp.response = _NewData;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No data Found.";
                    }
                    return Ok(resp);
                }
                else if (hospitaId == 9)
                {
                    ORACLECS_DB _OraDb = new ORACLECS_DB();
                    string OraSQL = "";

                    var dataResults = _OraDb.GetPatientLabResultsByApi_Dam(testId.ToString(), ReportType);
                    if (dataResults != null)
                    {
                        resp.status = 1;
                        resp.msg = "Data Found";
                        resp.response = dataResults;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No data Found.";
                    }
                    return Ok(resp);
                }
                else
                {
                    allPatientResults = apiCaller.GetPatientLabResultsByApi_V4(lang, hospitaId, testId, ref errStatus, ref errMessage);
                }

                //var allPatientResults = apiCaller.GetPatientLabResultsByApi(lang, hospitaId, testId, ref errStatus, ref errMessage);

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
        [Route("v4/testresult-pdf-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetailPDF_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["test_id"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Report_type"]))
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var testId = col["test_id"];

                var MRN = "";
                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    MRN = col["patient_reg_no"].ToString();

                var ReportType = "Lab";
                if (!string.IsNullOrEmpty(col["Report_type"]))
                    ReportType = col["Report_type"].ToString();

                resp.status = 0;
                resp.msg = "No data Found.";


                if (hospitaId > 300 && hospitaId < 400)
                {

                    if (string.IsNullOrEmpty(MRN))
					{
                        resp.status = 0;
                        resp.msg = "Wrong MRN.";
                        return Ok(resp);
                    }
                    
                    int errStatus = 0;
                    string errMessage = "";
                    
                    // As per New TestId from UAE 12-09-2024
                    int underscoreIndex = testId.IndexOf('_');
                    testId = testId.Substring(0, underscoreIndex);

                    //testId = testId.Replace("_RAD", "").Replace("_LAB", "");
                    


                    if (ReportType.ToUpper() == "LAB")
                        ReportType = "Lab"; 
                    else
                        ReportType = "Rad";

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var _NewData = _UAEApiCaller.GetPatientLabRAD_UAE_PDF(hospitaId, MRN, testId, ReportType, ref errStatus, ref errMessage);

                    var tmpobj = new TestResultPDF();
                    if (_NewData != null)
					{
                        if (!string.IsNullOrEmpty(_NewData.Base64))
                        {
                            var PdfURL = Util.Convert_Base64_to_PDF(_NewData.Base64, testId);
                            tmpobj.ReportUrl = PdfURL;

                            resp.status = 1;
                            resp.msg = "Result found";
                            resp.response = tmpobj;
                            return Ok(resp);
                        }

                    }
                    
                    
                    resp.status = 0;
                    resp.msg = "No Result found";                    
                    return Ok(resp);
                }
                else if (hospitaId == 9)
				{
                    var tmpobj = new TestResultPDF();
                    string Str_Id = "Test_Id=" + testId.ToString();
                    Str_Id += "&hospital_id=" + hospitaId.ToString();
                    Str_Id += "&Report_type=" + ReportType.ToString();
                    Str_Id += "&lang=EN" ;

                    var ParmEnc = TripleDESImp.TripleDesEncrypt(Str_Id);
                    var FinalURL = ConfigurationManager.AppSettings["LabResultURL_V4"].ToString() + ParmEnc;

                    tmpobj.ReportUrl = Util.ConvertURL_to_PDF(FinalURL, testId.ToString());
                    if (tmpobj.ReportUrl != null)
                    {
                        resp.status = 1;
                        resp.msg = "Result found";
                        resp.response = tmpobj;

                    }                   


                    //resp.status = 0;
                    //resp.msg = "No data Found.";
                }
                else
				{
                    var tmpobj = new TestResultPDF();
                    string Str_Id = "Test_Id=" + testId.ToString();
                    Str_Id += "&hospital_id=" + hospitaId.ToString();
                    Str_Id += "&Report_type=" + ReportType.ToString();
                    Str_Id += "&lang=EN";
                    var ParmEnc = TripleDESImp.TripleDesEncrypt(Str_Id);
                    var FinalURL = ConfigurationManager.AppSettings["LabResultURL_V4"].ToString() + ParmEnc;

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
                
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter";
            }
            return Ok(resp);
        }


        [HttpPost]
        [Route("v4/testresult-pdf-get-STRING")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetailPDF_V4_STRING(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["test_id"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Report_type"]))
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var testId = col["test_id"];

                var ReportType = "Lab";
                if (!string.IsNullOrEmpty(col["Report_type"]))
                    ReportType = col["Report_type"].ToString();

                if (hospitaId > 300 && hospitaId < 400)
                {
                    resp.status = 0;
                    resp.msg = "No data Found.";
                }
                else if (hospitaId == 9)
                {
                    var tmpobj = new TestResultPDF();
                    string Str_Id = "Test_Id=" + testId.ToString();
                    Str_Id += "&hospital_id=" + hospitaId.ToString();
                    Str_Id += "&Report_type=" + ReportType.ToString();
                    Str_Id += "&lang=EN";

                    var ParmEnc = TripleDESImp.TripleDesEncrypt(Str_Id);
                    var FinalURL = ConfigurationManager.AppSettings["LabResultURL_V4"].ToString() + ParmEnc;

                    //tmpobj.ReportUrl = Util.ConvertURL_to_PDF(FinalURL, testId.ToString());
                    tmpobj.ReportUrl = FinalURL;
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


                    resp.status = 0;
                    resp.msg = "No data Found.";
                }
                else
                {
                    var tmpobj = new TestResultPDF();
                    string Str_Id = "Test_Id=" + testId.ToString();
                    Str_Id += "&hospital_id=" + hospitaId.ToString();
                    Str_Id += "&Report_type=" + ReportType.ToString();
                    Str_Id += "&lang=EN";
                    var ParmEnc = TripleDESImp.TripleDesEncrypt(Str_Id);
                    var FinalURL = ConfigurationManager.AppSettings["LabResultURL_V4"].ToString() + ParmEnc;

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
