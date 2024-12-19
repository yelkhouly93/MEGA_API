using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Net.Http.Formatting;
using System.Web;
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
    public class InsuranceController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();
        // GET: Insurance
        [HttpPost]
        [Route("v2/Patient-InsuranceInfo-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientInsuranceInfo(FormDataCollection col)
        {
            var resp = new GenericResponse();
            resp.status = 0;
            resp.msg = "";            
            if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
            {
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                if (hospitalId == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(_resp);
                }

                int errStatus = 0;
                string errMessage = "";

                InsuranceDB _patientDB = new InsuranceDB();
                var PatientInsuranceDT = _patientDB.GetPatientInsuranceInfo_DT(hospitalId, patientMrn, ref errStatus, ref errMessage);


                if (PatientInsuranceDT != null && PatientInsuranceDT.Rows.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = "Record(s) Found";
                    resp.response = PatientInsuranceDT;
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



        [HttpPost]
        [Route("v3/Patient-InsuranceInfo-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientInsuranceInfo_v3(FormDataCollection col)
        {
            var resp = new GenericResponse();
            resp.status = 0;
            resp.msg = "";
            //try
            //{
            if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
            {
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                //if (hospitalId == 9)
                //{
                //    resp.status = 0;
                //    resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                //    return Ok(resp);
                //}

                int errStatus = 0;
                string errMessage = "";
                resp.status = 0;
                resp.msg = "No Record Found";

                if (hospitalId == 9)
                {
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var PatientData = _loginApiCaller.GetPatientInsuranceByApi_NewDam(lang, patientMrn.ToString(), ref errStatus, ref errMessage);

                    if (PatientData != null && PatientData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientData;
                    }
                }
                else
                {
                    InsuranceDB _InsuranceDB = new InsuranceDB();
                    var PatientInsuranceDT = _InsuranceDB.GetPatientInsuranceInfo_DT(hospitalId, patientMrn, ref errStatus, ref errMessage);
                    if (PatientInsuranceDT != null && PatientInsuranceDT.Rows.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientInsuranceDT;
                    }
                }


            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter!";
            }


            return Ok(resp);
            //}
            //catch (Exception ex)
            //{
            //    var test = ex;
            //}

            //return Ok();
        }


        [HttpPost]
        [Route("v4/Patient-InsuranceInfo-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientInsuranceInfo_V4(FormDataCollection col)
        {
            var resp = new GenericResponse();
            resp.status = 0;
            resp.msg = "";
            //try
            //{
            if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
            {
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                //if (hospitalId == 9)
                //{
                //    resp.status = 0;
                //    resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                //    return Ok(resp);
                //}

                int errStatus = 0;
                string errMessage = "";
                resp.status = 0;
                resp.msg = "No Record Found";

                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var StrpatientMrn = col["patient_reg_no"];
                    var PatientData = _UAEApiCaller.GetPatientInsuranceByApi_NewUAE(lang, hospitalId, StrpatientMrn.ToString(), ref errStatus, ref errMessage);
                    if (PatientData != null && PatientData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientData;
                    }

                }
                else if (hospitalId == 9)
                {
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var PatientData = _loginApiCaller.GetPatientInsuranceByApi_NewDam(lang, patientMrn.ToString(), ref errStatus, ref errMessage);

                    if (PatientData != null && PatientData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientData;
                    }
                }
                else
                {
                    InsuranceDB _InsuranceDB = new InsuranceDB();
                    var PatientInsuranceDT = _InsuranceDB.GetPatientInsuranceInfo_DT(hospitalId, Convert.ToInt32(patientMrn), ref errStatus, ref errMessage);
                    if (PatientInsuranceDT != null && PatientInsuranceDT.Rows.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientInsuranceDT;
                    }
                }


            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter!";
            }


            return Ok(resp);
            //}
            //catch (Exception ex)
            //{
            //    var test = ex;
            //}

            //return Ok();
        }

        [HttpPost]
        [Route("v5/Patient-InsuranceInfo-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientInsuranceInfo_V5(FormDataCollection col)
        {
            var resp = new GenericResponse();
            resp.status = 0;
            resp.msg = "";
            //try
            //{
            if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
            {
                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                int errStatus = 0;
                string errMessage = "";
                resp.status = 0;
                resp.msg = "No Record Found";

                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var StrpatientMrn = col["patient_reg_no"];
                    var PatientData = _UAEApiCaller.GetPatientInsuranceByApi_NewUAE_V5(lang, hospitalId, StrpatientMrn.ToString(), ref errStatus, ref errMessage);
                    if (PatientData != null && PatientData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientData;
                    }

                }
                else if (hospitalId == 9)
                {
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var PatientData = _loginApiCaller.GetPatientInsuranceByApi_NewDam(lang, patientMrn.ToString(), ref errStatus, ref errMessage);

                    if (PatientData != null && PatientData.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientData;
                    }
                }
                else
                {
                    InsuranceDB _InsuranceDB = new InsuranceDB();
                    var PatientInsuranceDT = _InsuranceDB.GetPatientInsuranceInfo_DT(hospitalId, Convert.ToInt32(patientMrn), ref errStatus, ref errMessage);
                    if (PatientInsuranceDT != null && PatientInsuranceDT.Rows.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = PatientInsuranceDT;
                    }
                }


            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter!";
            }


            return Ok(resp);
            //}
            //catch (Exception ex)
            //{
            //    var test = ex;
            //}

            //return Ok();
        }


        [HttpPost]
        [Route("v2/IncApprovalStatus-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetInSuranceApprovalStatus(FormDataCollection col)
        {
            _resp = new GenericResponse();
            InsuranceDB CDB = new InsuranceDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                var CMRN = Convert.ToInt32(col["patient_reg_no"]);
                var BranchID = Convert.ToInt32(col["hospital_id"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var status = 0;
                var msg = "";


                var DTList = CDB.GetPatientInSuranceApprovalStatus_DT(lang, BranchID, CMRN, ref status, ref msg);


                if (DTList != null && DTList.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = DTList;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
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
        [Route("v3/IncApprovalStatus-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetInSuranceApprovalStatus_V3(FormDataCollection col)
        {
            _resp = new GenericResponse();
            InsuranceDB CDB = new InsuranceDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                var CMRN = Convert.ToInt32(col["patient_reg_no"]);
                var BranchID = Convert.ToInt32(col["hospital_id"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var status = 0;
                var msg = "";

                if (BranchID == 9)
                {

                    var PatientMRN = col["patient_reg_no"].ToString();

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();                    
                    var _Data = _loginApiCaller.GetPatientInsurnaceApprovalByApi_NewDam(lang, PatientMRN);

                    if (_Data != null)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = _Data;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No Record Found";
                    }
                    return Ok(_resp);

                }
                else if (BranchID > 300 && BranchID < 400) /*UAE BRanches*/
				{
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
                    return Ok(_resp);
                }
                else
				{
                    //KSA BRANCHES 
                    var DTList = CDB.GetPatientInSuranceApprovalStatus_DT(lang, BranchID, CMRN, ref status, ref msg);

                    if (DTList != null && DTList.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = DTList;

                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No Record Found";
                    }

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
        [Route("v2/UCAF-PDF-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetUCAFPDF(FormDataCollection col)
        {
            _resp = new GenericResponse();
            InsuranceDB CDB = new InsuranceDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["VisitID"]))
            {
                var lang = "EN";
                var VisitID = col["VisitID"];
                var BranchID = Convert.ToInt32(col["hospital_id"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var status = 0;
                var msg = "";

                if (BranchID == 9)
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
                    return Ok(_resp);

                }

                var tmpobj = new TestResultPDF();

                var FinalURL = ConfigurationManager.AppSettings["UCAFURLPDF"].ToString() + "?BranchID=" + BranchID + "&VisitId=" + VisitID;
                tmpobj.ReportUrl = Util.ConvertURL_to_PDF(FinalURL, VisitID);
                if (tmpobj.ReportUrl != null)
                {
                    _resp.status = 1;
                    _resp.msg = "Result found";
                    _resp.response = tmpobj;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Resutls Empty";
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
        [Route("v4/IncApprovalStatus-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetInSuranceApprovalStatus_V4(FormDataCollection col)
        {
            _resp = new GenericResponse();
            InsuranceDB CDB = new InsuranceDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                var CMRN = Convert.ToInt32(col["patient_reg_no"]);
                var BranchID = Convert.ToInt32(col["hospital_id"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var status = 0;
                var msg = "";

                if (BranchID == 9)
                {   
                    var PatientMRN = col["patient_reg_no"].ToString() ;

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();                    
                    var _Data = _loginApiCaller.GetPatientInsurnaceApprovalByApi_NewDam(lang, PatientMRN);
                    
                    if (_Data != null)
					{
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = _Data;
                    }
					{
                        _resp.status = 0;
                        _resp.msg = "No Record Found";
                    }                    
                    return Ok(_resp);

                }
                else if (BranchID > 300 && BranchID < 400) /*UAE BRanches*/
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found";

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var StrpatientMrn = col["patient_reg_no"];
                    var PatientData = _UAEApiCaller.GetPatientInsurnaceApprovalByApi_UAE_V4(lang, BranchID, StrpatientMrn.ToString());
                    
                    if (PatientData != null && PatientData.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = PatientData;
                    }

                    
                    return Ok(_resp);
                }
                else
				{
                    var DTList = CDB.GetPatientInSuranceApprovalStatus_DT(lang, BranchID, CMRN, ref status, ref msg);


                    if (DTList != null && DTList.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = DTList;

                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "No Record Found";
                    }
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