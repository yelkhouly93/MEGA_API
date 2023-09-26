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

// ReSharper disable once CheckNamespace
namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class PatientTestResultsController : ApiController
    {
        /// <summary>
        /// Get Patient Diagnosis against patient registration No.
        /// </summary>
        /// <returns>Return Patient Diagnosis against patient registration No</returns>
        //[HttpPost]
        //[Route("api/get-patient-testreports")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult Post(FormDataCollection col)
        //{
        //    var lang = col["lang"];
        //    var hospitaId = Convert.ToInt32(col["hospital_id"]);
        //    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

        //    int errStatus = 0;
        //    string errMessage = "";

        //    PatientDB _patientDB = new PatientDB();
        //    List<PateintTests> _allPatientDiagnosis = _patientDB.GetPatientTestResults(lang, hospitaId, registrationNo, ref errStatus, ref errMessage);

        //    GenericResponse resp = new GenericResponse();

        //    if (_allPatientDiagnosis != null && _allPatientDiagnosis.Count > 0)
        //    {
        //        resp.status = 1;
        //        resp.msg = errMessage;
        //        resp.response = _allPatientDiagnosis;

        //    }
        //    else
        //    {
        //        resp.status = 0;
        //        resp.msg = errMessage;


        //    }

        //    return Ok(resp);
        //}


        //[HttpPost]
        //[Route("api/get-patient-testreportsnew")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PostTestOrder(FormDataCollection col)
        //{
        //    var lang = col["lang"];
        //    var hospitaId = Convert.ToInt32(col["hospital_id"]);
        //    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

        //    int errStatus = 0;
        //    string errMessage = "";

        //    PatientDB _patientDB = new PatientDB();
        //    List<PateintTests> _allPatientDiagnosis = _patientDB.GetPatientTestResultsNew(lang, hospitaId, registrationNo, ref errStatus, ref errMessage);

        //    GenericResponse resp = new GenericResponse();

        //    if (_allPatientDiagnosis != null && _allPatientDiagnosis.Count > 0)
        //    {
        //        resp.status = 1;
        //        resp.msg = errMessage;
        //        resp.response = _allPatientDiagnosis;

        //    }
        //    else
        //    {
        //        resp.status = 0;
        //        resp.msg = errMessage;


        //    }

        //    return Ok(resp);
        //}



        [HttpPost]
        [Route("test-reports-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientTestReports(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) ) 
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                int errStatus = 0;
                string errMessage = "";

                var patientDb = new PatientDB();
                var allPatientDiagnosis = patientDb.GetPatientTestResultsNew(lang, hospitaId, registrationNo, ref errStatus, ref errMessage);                

                if (allPatientDiagnosis != null && allPatientDiagnosis.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = errMessage;
                    resp.response = allPatientDiagnosis;

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




        //[HttpPost]
        //[Route("api/get-patient-testresult")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PostTestResult(FormDataCollection col)
        //{
        //    var lang = col["lang"];
        //    var hospitaId = Convert.ToInt32(col["hospital_id"]);
        //    var testId = Convert.ToInt32(col["test_id"]);

        //    int errStatus = 0;
        //    string errMessage = "";

        //    PatientDB _patientDB = new PatientDB();
        //    PatientLabResultsApiCaller _apiCaller = new PatientLabResultsApiCaller();

        //    TestResultMain _allPatientResults = _apiCaller.GetPatientLabResultsByApi(lang, hospitaId, testId, ref errStatus, ref errMessage);

        //    GenericResponse resp = new GenericResponse();

        //    if (_allPatientResults != null)
        //    {
        //        resp.status = 1;
        //        resp.msg = errMessage;
        //        resp.response = _allPatientResults;

        //    }
        //    else
        //    {
        //        resp.status = 0;
        //        resp.msg = errMessage;


        //    }

        //    return Ok(resp);
        //}



        [HttpPost]
        [Route("get-testresult")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResult(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var testId = Convert.ToInt32(col["test_id"]);

            int errStatus = 0;
            string errMessage = "";

            var patientDb = new PatientDB();
            var apiCaller = new PatientLabResultsApiCaller();

            var allPatientResults = apiCaller.GetPatientTestResultsAPI(lang, hospitaId, testId, ref errStatus, ref errMessage);

            var resp = new GenericResponse();

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

            return Ok(resp);
        }


        [HttpPost]
        [Route("testresult-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResult_Copy(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var testId = Convert.ToInt32(col["test_id"]);

            int errStatus = 0;
            string errMessage = "";

            var patientDb = new PatientDB();
            var apiCaller = new PatientLabResultsApiCaller();

            var allPatientResults = apiCaller.GetPatientTestResultsAPI(lang, hospitaId, testId, ref errStatus, ref errMessage);

            var resp = new GenericResponse();

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

            return Ok(resp);
        }




        [HttpPost]
        [Route("v2/test-List-get")]
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
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                var allPatientDiagnosis = patientDb.GetPatientTestResultsNew(lang, hospitaId, registrationNo, ref errStatus, ref errMessage, ApiSource);

                if (allPatientDiagnosis != null && allPatientDiagnosis.Count > 0)
                {
                    if (ApiSource.ToLower() == "saleforce")
					{
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = allPatientDiagnosis;
                    }
                    else
					{
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = allPatientDiagnosis;
                    }

                    

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
        [Route("v2/test-resultdetails-get")]
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
        [Route("v2/testresult-pdf-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetailPDF(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["test_id"]))
            {
                var lang = col["lang"];
                //var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var testId = col["test_id"];
                var tmpobj = new TestResultPDF();

                string Str_Id = "Test_Id=" + testId;
                var ParmEnc = TripleDESImp.TripleDesEncrypt(Str_Id);
                var FinalURL = ConfigurationManager.AppSettings["LabResultURL"].ToString() + ParmEnc;
                
                tmpobj.ReportUrl = Util.ConvertURL_to_PDF(FinalURL , testId.ToString());
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
        [Route("v2/GetSys-MedReports-pdf-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetMedReportResultDetailPDF(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["Report_id"]) && !string.IsNullOrEmpty(col["Visit_ID"]))
            {
                var lang = col["lang"];
                //var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var ReportID = col["Report_id"];
                var VisitID = col["Visit_ID"];
                var tmpobj = new TestResultPDF();

                var FinalURL = ConfigurationManager.AppSettings["MEDReportURLPDF"].ToString() + ReportID;
                tmpobj.ReportUrl = Util.ConvertURL_to_PDF(FinalURL, VisitID);
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
        [Route("v2/GetSys-MedReports")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPreDefineReports(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) )
            {
                var lang = col["lang"];
                var hospitalId = Convert.ToInt32( col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                var patientDb = new PatientDB();
                var allReportList = patientDb.GetPreDefineMedicalReports(lang, hospitalId, registrationNo);

                
                if (allReportList != null && allReportList.Count > 0 )
                {
                    var FinalReportList = MapDefineMedicalReport(allReportList , hospitalId);
                    resp.status = 1;
                    resp.msg = "Report list found";
                    resp.response = FinalReportList;

                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Report list not found";
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter";
            }
            return Ok(resp);
        }


        private List<PreDefineMedReport> MapDefineMedicalReport(List<PreDefineMedReport> PreDefineReport , int BranchID)
        {
            List<PreDefineMedReport> _ObjList = new List<PreDefineMedReport>();
            foreach (var row in PreDefineReport)
            {
                PreDefineMedReport _MedicalReport = new PreDefineMedReport();

                _MedicalReport.FromDate = row.FromDate;
                _MedicalReport.NoOfDays = row.NoOfDays;
                _MedicalReport.Reason_AR = row.Reason_AR;
                _MedicalReport.RegistrationNo = row.RegistrationNo;
                _MedicalReport.ReportType = row.ReportType;
                _MedicalReport.ToDate = row.ToDate;
                _MedicalReport.VisitID = row.VisitID;
                _MedicalReport.VisitiDateTime = row.VisitiDateTime;
                _MedicalReport.VisitType = row.VisitType;

                _MedicalReport.DepartmentName = row.DepartmentName;
                _MedicalReport.DoctorName = row.DoctorName;

                _MedicalReport.URL = row.URL;
                var VisitID = "Visit_Id=" + row.VisitID.ToString() + "&MRN=" + row.RegistrationNo + "&BranchId=" + BranchID;

                var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
                ParmEnc = ParmEnc.Replace("+" , "$$$");

                _MedicalReport.ReportID_pram = ParmEnc;
                
                var FinalURL = ConfigurationManager.AppSettings["MEDReportURL"].ToString() + ParmEnc;

                _MedicalReport.URL = FinalURL;

                //_MedicalReport.URL = FinalURL;
                _MedicalReport.URL = "";

                _ObjList.Add(_MedicalReport);
            }
            return _ObjList;
        }




    }
}