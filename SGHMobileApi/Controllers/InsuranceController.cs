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
                    _resp.status = 0;
                    _resp.msg = "No Record Found";
                    return Ok(_resp);

                }

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

    }
}