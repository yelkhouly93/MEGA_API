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
using System.Web;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class MedicalReport_SickleaveController : ApiController
    {
        // GET: MedicalReport_Sickleave
        [HttpPost]
        [Route("v2/GetSys-MedReports")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPreDefineReports(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = col["lang"];
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                if (hospitalId == 9)
                {
                    var PatientMRN = col["patient_reg_no"].ToString();
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var _Data = _loginApiCaller.GetSysMedicalReport_SickLeaveByApi_NewDam(lang, PatientMRN);
                    //Ahsan 

                    if (_Data != null && _Data.Count > 0)
                    {
                        var FinalReportList = MapDefineMedicalReport_Dammam(_Data, hospitalId, PatientMRN);

                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = FinalReportList;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No Record Found";
                    }
                    return Ok(resp);
                }
                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    resp.status = 0;
                    if (lang == "EN")
                        resp.msg = "Sorry this service not available";
                    else
                        resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(resp);
                }

                var patientDb = new PatientDB();
                var allReportList = patientDb.GetPreDefineMedicalReports(lang, hospitalId, registrationNo);


                if (allReportList != null && allReportList.Count > 0)
                {
                    var FinalReportList = MapDefineMedicalReport(allReportList, hospitalId);
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


        [HttpPost]
        [Route("v3/GetSys-MedReports")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPreDefineReports_V3(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = col["lang"];
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                int errStatus = 0;
                string errMessage = "";


                if (hospitalId == 9)
                {
                    var PatientMRN = col["patient_reg_no"].ToString();
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var _Data = _loginApiCaller.GetSysMedicalReport_SickLeaveByApi_NewDam(lang, PatientMRN);
                    //Ahsan 

                    if (_Data != null && _Data.Count > 0)
                    {
                        var FinalReportList = MapDefineMedicalReport_Dammam(_Data, hospitalId, PatientMRN);

                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = FinalReportList;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "No Record Found";
                    }
                    return Ok(resp);
                }
                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                    var registrationNo_str = col["patient_reg_no"].ToString();

                    var _data = _UAEApiCaller.Get_SysMedical_ReportList_NewUAE(lang, hospitalId, registrationNo_str, ref errStatus, ref errMessage);

                    if (_data != null && _data.Count > 0 )
					{
                        var FinalReportList = MapDefineMedicalReport_UAE(_data , hospitalId);
                        resp.status = 1;
                        resp.msg = "Report list found";
                        resp.response = FinalReportList;
                        return Ok(resp);
                    }
                    else
					{
                        resp.status = 0;                        
                        resp.msg = "No Data Found";
                        return Ok(resp);

                    }

                    //resp.status = 0;
                    //if (lang == "EN")
                    //    resp.msg = "Sorry this service not available";
                    //else
                    //    resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    //return Ok(resp);
                }
                else
				{
                    var patientDb = new PatientDB();
                    var allReportList = patientDb.GetPreDefineMedicalReports(lang, hospitalId, registrationNo);

                    if (allReportList != null && allReportList.Count > 0)
                    {
                        var FinalReportList = MapDefineMedicalReport2(allReportList, hospitalId);
                        resp.status = 1;
                        resp.msg = "Report list found";
                        resp.response = FinalReportList;                        
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = "Report list not found";
                    }
                    return Ok(resp);
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
        [Route("v2/GetSys-MedReports_TEMP")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPreDefineReports_TEMP(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = col["lang"];
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);


                if (hospitalId == 9)
                {
                    var PatientMRN = col["patient_reg_no"].ToString();
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var _DataSick = _loginApiCaller.GetSysMedicalReport_SickLeaveByApi_NewDam(lang, PatientMRN);
                    // AHSAN TEMP
                    var _DataMedical = _loginApiCaller.GetSysMedicalReport_MedicalReportByApi_NewDam(lang, PatientMRN);

                    var _Data = new List<GetSys_MedicalReport>();

                    _Data.AddRange(_DataSick);
                    _Data.AddRange(_DataMedical);


                    if (_Data != null && _Data.Count > 0)
                    {
                        var FinalReportList = MapDefineMedicalReport_Dammam(_Data, hospitalId, PatientMRN);

                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = FinalReportList;
                    }
                    {
                        resp.status = 0;
                        resp.msg = "No Record Found";
                    }
                    return Ok(resp);
                }
                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    resp.status = 0;
                    if (lang == "EN")
                        resp.msg = "Sorry this service not available";
                    else
                        resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(resp);
                }


                var patientDb = new PatientDB();
                var allReportList = patientDb.GetPreDefineMedicalReports_Temp(lang, hospitalId, registrationNo);


                if (allReportList != null)
                {
                    //var FinalReportList = MapDefineMedicalReport(allReportList, hospitalId);
                    resp.status = 1;
                    resp.msg = "Report list found";
                    resp.response = allReportList;

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
        [Route("v3/GetSys-MedReports-pdf-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetMedReportResultDetailPDF_V3(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["Report_id"]) && !string.IsNullOrEmpty(col["Visit_ID"])
                && !string.IsNullOrEmpty(col["hospital_id"])
                )
            {
                var lang = col["lang"];
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var ReportID = col["Report_id"];
                var VisitID = col["Visit_ID"];
                var tmpobj = new TestResultPDF();

                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
				{
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                    //var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
                    //ParmEnc = ParmEnc.Replace("+", "$$$");

                    var TemParmEnc = ReportID.Replace("$$$" , "+");
                    var ParmEnc = TripleDESImp.TripleDesDecrypt(TemParmEnc.ToString());

                    string  msgreturn = "";
                    int erstatus = 0;
                    if (ParmEnc != "")
                    {
                        Uri myUri = new Uri("http://www.example.com?" + ParmEnc);
                        string param1 = HttpUtility.ParseQueryString(myUri.Query).Get("Visit_Id");
                        string param2 = HttpUtility.ParseQueryString(myUri.Query).Get("lang");
                        string param3 = HttpUtility.ParseQueryString(myUri.Query).Get("MRN");
                        string param4 = HttpUtility.ParseQueryString(myUri.Query).Get("BranchId");


                        var _NewData = _UAEApiCaller.GetPatientSickLeave_UAE_PDF(hospitalId, param3, param1, ref erstatus, ref msgreturn);

                        tmpobj = new TestResultPDF();

                        if (_NewData != null)
                        {
                            if (!string.IsNullOrEmpty(_NewData.Base64))
                            {
                                var PdfURL = Util.Convert_Base64_to_PDF(_NewData.Base64, VisitID);
                                tmpobj.ReportUrl = PdfURL;

                                resp.status = 1;
                                resp.msg = "Result found";
                                resp.response = tmpobj;
                                return Ok(resp);
                            }

                        }
                    }




                    //tmpobj.ReportUrl = "https://cxmw.sghgroup.net/doctorsprofile/temp/123test.pdf";
                    //resp.status = 1;
                    //resp.msg = "Result found";
                    //resp.response = tmpobj;
                    resp.status = 0;
                    resp.msg = "Resutls Empty";
                }
                else
				{
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
                
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter";
            }
            return Ok(resp);
        }

        private List<PreDefineMedReport2> MapDefineMedicalReport_Dammam(List<GetSys_MedicalReport> PreDefineReport, int BranchID, string MRN)
        {
            List<PreDefineMedReport2> _ObjList = new List<PreDefineMedReport2>();
            foreach (var row in PreDefineReport)
            {
                PreDefineMedReport2 _MedicalReport = new PreDefineMedReport2();

                _MedicalReport.FromDate = row.fromDate;
                _MedicalReport.NoOfDays = row.noOfDays.ToString();
                _MedicalReport.Reason_AR = row.Reason_AR;
                //_MedicalReport.RegistrationNo = Convert.ToInt32(MRN);
                _MedicalReport.RegistrationNo = MRN;
                _MedicalReport.ReportType = row.reportType;
                _MedicalReport.ToDate = row.toDate;
                _MedicalReport.VisitID = row.EPISODE_NO.ToString();
                _MedicalReport.VisitiDateTime = row.visitDateTime;

                //_MedicalReport.VisitType = row.visitType;
                _MedicalReport.VisitType = "OP";

                _MedicalReport.DepartmentName = row.departmentName;
                _MedicalReport.DoctorName = row.doctorName;



                //_MedicalReport.URL = row.URL;
                var VisitID = "Visit_Id=" + row.EPISODE_NO.ToString() + "&MRN=" + MRN + "&BranchId=" + BranchID;

                var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
                ParmEnc = ParmEnc.Replace("+", "$$$");

                _MedicalReport.ReportID_pram = ParmEnc;

                var FinalURL = ConfigurationManager.AppSettings["MEDReportURL"].ToString() + ParmEnc;

                _MedicalReport.URL = FinalURL;

                //_MedicalReport.URL = FinalURL;
                _MedicalReport.URL = "";

                _ObjList.Add(_MedicalReport);
            }
            return _ObjList;
        }


        
        private List<PreDefineMedReport> MapDefineMedicalReport(List<PreDefineMedReport> PreDefineReport, int BranchID)
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
                ParmEnc = ParmEnc.Replace("+", "$$$");

                _MedicalReport.ReportID_pram = ParmEnc;

                var FinalURL = ConfigurationManager.AppSettings["MEDReportURL"].ToString() + ParmEnc;

                _MedicalReport.URL = FinalURL;

                //_MedicalReport.URL = FinalURL;
                _MedicalReport.URL = "";

                _ObjList.Add(_MedicalReport);
            }
            return _ObjList;
        }

        private List<PreDefineMedReport2> MapDefineMedicalReport2(List<PreDefineMedReport> PreDefineReport, int BranchID)
        {
            List<PreDefineMedReport2> _ObjList = new List<PreDefineMedReport2>();
            foreach (var row in PreDefineReport)
            {
                PreDefineMedReport2 _MedicalReport = new PreDefineMedReport2();

                _MedicalReport.FromDate = row.FromDate;
                _MedicalReport.NoOfDays = row.NoOfDays.ToString();
                _MedicalReport.Reason_AR = row.Reason_AR;
                _MedicalReport.RegistrationNo = row.RegistrationNo.ToString();
                _MedicalReport.ReportType = row.ReportType;
                _MedicalReport.ToDate = row.ToDate;
                _MedicalReport.VisitID = row.VisitID.ToString();
                _MedicalReport.VisitiDateTime = row.VisitiDateTime;
                _MedicalReport.VisitType = row.VisitType;

                _MedicalReport.DepartmentName = row.DepartmentName;
                _MedicalReport.DoctorName = row.DoctorName;

                _MedicalReport.URL = row.URL;
                var VisitID = "Visit_Id=" + row.VisitID.ToString() + "&MRN=" + row.RegistrationNo + "&BranchId=" + BranchID;

                var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
                ParmEnc = ParmEnc.Replace("+", "$$$");

                _MedicalReport.ReportID_pram = ParmEnc;

                var FinalURL = ConfigurationManager.AppSettings["MEDReportURL"].ToString() + ParmEnc;

                _MedicalReport.URL = FinalURL;

                //_MedicalReport.URL = FinalURL;
                _MedicalReport.URL = "";

                _ObjList.Add(_MedicalReport);
            }
            return _ObjList;
        }



        private List<PreDefineMedReport2> MapDefineMedicalReport_UAE(List<MedicalReportList_UAE> PreDefineReport, int BranchID)
        {
            List<PreDefineMedReport2> _ObjList = new List<PreDefineMedReport2>();
            foreach (var row in PreDefineReport)
            {
                PreDefineMedReport2 _MedicalReport = new PreDefineMedReport2();
                if (row.fromDate != null)
                    _MedicalReport.FromDate = DateTime.Parse (row.fromDate);
                _MedicalReport.NoOfDays = row.noOfDays;
                _MedicalReport.Reason_AR = row.reason_AR;
                _MedicalReport.RegistrationNo = row.registrationNo;
                _MedicalReport.ReportType = row.reportType;

                if (row.toDate != null)
                    _MedicalReport.ToDate = DateTime.Parse(row.toDate);
                _MedicalReport.VisitID = row.visitID;
                if (row.visitiDateTime != null)
                    _MedicalReport.VisitiDateTime = DateTime.Parse(row.visitiDateTime);
                _MedicalReport.VisitType = row.visitType;

                _MedicalReport.DepartmentName = row.departmentName;
                _MedicalReport.DoctorName = row.doctorName;

                _MedicalReport.URL = row.url;
                var VisitID = "Visit_Id=" + row.FILEID.ToString() + "&MRN=" + row.registrationNo + "&BranchId=" + BranchID;

                var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
                ParmEnc = ParmEnc.Replace("+", "$$$");

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
