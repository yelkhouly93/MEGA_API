using System.Collections.Generic;
using System.Web.Http;
using DataLayer.Model;
using DataLayer.Reception.Business;
using SGHMobileApi.Extension;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using DataLayer.Data;
using System;
using System.Data;
using System.Net.Http.Formatting;
using SGHMobileApi.Common;
using System.Data.SqlClient;
using System.Configuration;
using RestClient;
using System.Net;


namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class HomeCareController : ApiController
    {
        private HomeCareDB _HomeCareDb = new HomeCareDB();
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };

        
        [HttpPost]
        [Route("v2/HC/Save-Patient-Info")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SavePatientInfo(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (    !string.IsNullOrEmpty(col["hospital_id"])       && !string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["patient_fullname"]) && !string.IsNullOrEmpty(col["patient_Gender"])
                    && !string.IsNullOrEmpty(col["patient_Email"]) && !string.IsNullOrEmpty(col["patient_Mobile"])
                    && !string.IsNullOrEmpty(col["patient_Address"]) 
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = col["patient_reg_no"];
                var fullname = col["patient_fullname"];
                var Gender = col["patient_Gender"];
                var Email = col["patient_Email"];
                var MobileNUmber = col["patient_Mobile"];

                var Address = col["patient_Address"];

                var Age = "";
                if (!string.IsNullOrEmpty(col["patient_Age"]))
                    Age = col["patient_Age"];

                var DOB = "";
                if (!string.IsNullOrEmpty(col["patient_DBO"]))
                    DOB = col["patient_DBO"];
                
                var buildingNo = "";
                if (!string.IsNullOrEmpty(col["patient_building"]))
                    buildingNo = col["patient_building"];

                var Appartment = "";
                if (!string.IsNullOrEmpty(col["patient_Appartment"]))
                    Appartment = col["patient_Appartment"];

                var LandMark = "";
                if (!string.IsNullOrEmpty(col["patient_LandMark"]))
                    LandMark = col["patient_LandMark"];

                var LocationURL = "";
                if (!string.IsNullOrEmpty(col["patient_LocationURL"]))
                    LocationURL = col["patient_LocationURL"];

                

                var errStatus = 0;
                var errMessage = "";

                var allData = _HomeCareDb.SavePatientInfo(  PatientMRN, BranchID , fullname , Gender , Age,
                    DOB,Email, MobileNUmber , Address , buildingNo , Appartment, LandMark, LocationURL, lang, ref errStatus, ref errMessage);


                if ( errStatus > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Save";
                    _resp.response = allData;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed To Save Record";
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
        [Route("v2/HC/Get-MasterList")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GEtMasterList(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) 
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                
                var allData = _HomeCareDb.GetMasterList(lang, BranchID);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = allData;

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
        [Route("v2/HC/Get-PatientAddress")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientAddress(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var MRN = col["patient_reg_no"].ToString();

                var allData = _HomeCareDb.GetPatientAddress(lang, BranchID , MRN);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = allData;

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
        [Route("v2/HC/Save-Patient-Request")]
        [ResponseType(typeof(List<GenericResponse_New>))]
        public IHttpActionResult SavePatientHCRequest(FormDataCollection col)
        {
            var resp = new GenericResponse_New();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["Appoitment_Date"]) && !string.IsNullOrEmpty(col["Appoitment_Time"])
                && !string.IsNullOrEmpty(col["Service_Ids"])
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var MRN = col["patient_reg_no"].ToString();

                var APpDate = col["Appoitment_Date"].ToString();
                var Apptime = col["Appoitment_Time"].ToString();
                var ServiceIds = col["Service_Ids"].ToString();

                var Comments = "";
                if (!string.IsNullOrEmpty(col["Comments"]))
                    Comments = col["Comments"];

                var errStatus = 0;
                var errMessage = "";
                var AddionolOutPut = "";

                var allData = _HomeCareDb.Save_Patient_HC_Request(MRN, BranchID,APpDate, Apptime,ServiceIds,Comments,ref errStatus, ref errMessage , ref AddionolOutPut);


                if (errStatus > 0)
                {
                    resp.status = 1;
                    resp.msg = errMessage;
                    resp.additional = AddionolOutPut;
                    resp.response = allData;

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
        [Route("v2/HC/Get-Patient-Requests")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientRequestList(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var MRN = col["patient_reg_no"].ToString();

                var allData = _HomeCareDb.GetPatientRequestList(lang, BranchID, MRN);


                if (allData != null && allData.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = allData;

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
        [Route("v2/HC/Cancel-Patient-Request")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult CancelPatientHCRequest(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["Request_ID"])
                
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var MRN = col["patient_reg_no"].ToString();

                var RequestID = Convert.ToInt32(col["Request_ID"]);
                

                var Reason = "";
                if (!string.IsNullOrEmpty(col["Reason"]))
                    Reason = col["Reason"];

                var errStatus = 0;
                var errMessage = "";
                var AddionolOutPut = "";

                var allData = _HomeCareDb.Cancel_Patient_HC_Request(MRN, BranchID, RequestID,lang,Reason, ref errStatus, ref errMessage);

                    resp.status = errStatus;
                    resp.msg = errMessage;

                //if (errStatus > 0)
                //{
                //    resp.status = 1;
                //    resp.msg = errMessage;

                //    resp.response = allData;

                //}
                //else
                //{
                //    resp.status = 0;
                //    resp.msg = "Failed to Save Request Please try Again Latter";
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
        [Route("v2/HC/Update-Patient-OtherMobile")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SavePatientMobile(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["patient_OtherMobile"])
                    
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = col["patient_reg_no"];
                var otherMobile = col["patient_OtherMobile"];
                


                var errStatus = 0;
                var errMessage = "";
                var tempa = _HomeCareDb.Update_Patient_Other_Mobile(PatientMRN, BranchID, otherMobile, lang);

                _resp.status = 1;
                _resp.msg = "Record has been updated Sccussfully";

                //var allData = _HomeCareDb.Update_Patient_Other_Mobile(PatientMRN, BranchID, fullname, Gender, Age,
                //    DOB, Email, MobileNUmber, Address, buildingNo, Appartment, LandMark, LocationURL, lang, ref errStatus, ref errMessage);


                //if (allData != null && allData.Rows.Count > 0 && errStatus > 0)
                //{
                //    _resp.status = 1;
                //    _resp.msg = "Record Save";
                //    _resp.response = allData;

                //}
                //else
                //{
                //    _resp.status = 0;
                //    _resp.msg = "Failed To Save Record";
                //}

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }


        [HttpPost]
        [Route("v2/HC/Update-Patient-Request")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult UpdatePatientRequest(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["Request_id"]) && !string.IsNullOrEmpty(col["Request_type"])

                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = col["patient_reg_no"];
                var RequestType = col["Request_type"].ToString();
                var RequestID = Convert.ToInt32(col["Request_id"]);

                if (RequestType.ToLower ()  != "accepted" && RequestType.ToLower() != "confirmed")
				{
                    _resp.status = 0;
                    _resp.msg = "Wrong Input Type";
                    return Ok(_resp);
                }

                var errStatus = 0;
                var errMessage = "";

                var allData = _HomeCareDb.Update_Patient_RequestType(PatientMRN, BranchID,RequestID, lang,RequestType);

                _resp.status = 1;
                _resp.msg = "Record Updated Scuessfull";

                //if (allData != null && allData.Rows.Count > 0 && errStatus > 0)
                //{
                //    _resp.status = 1;
                //    _resp.msg = "Record Save";
                //    _resp.response = allData;

                //}
                //else
                //{
                //    _resp.status = 0;
                //    _resp.msg = "Failed To Save Record";
                //}

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        [HttpPost]
        [Route("v2/HC/Patient-Request-PaymentConfirmation")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PaymentConfirmationPatientRequest(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["Request_id"]) && !string.IsNullOrEmpty(col["Payment_Amount"])
                    && !string.IsNullOrEmpty(col["Payment_Ref"])

                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = col["patient_reg_no"];
                var PaymentAmount = col["Payment_Amount"].ToString();
                var RequestID = Convert.ToInt32(col["Request_id"]);
                var PaymentRef = col["Payment_Ref"].ToString();

                

                var errStatus = 0;
                var errMessage = "";

                var allData = _HomeCareDb.PaymentConfirmation_Patient_RequestType(PatientMRN, BranchID, RequestID, lang, PaymentAmount,PaymentRef);

                _resp.status = 1;
                _resp.msg = "Record Updated Scuessfull";

                //if (allData != null && allData.Rows.Count > 0 && errStatus > 0)
                //{
                //    _resp.status = 1;
                //    _resp.msg = "Record Save";
                //    _resp.response = allData;

                //}
                //else
                //{
                //    _resp.status = 0;
                //    _resp.msg = "Failed To Save Record";
                //}

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
