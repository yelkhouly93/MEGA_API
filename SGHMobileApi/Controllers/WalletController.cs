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
    public class WalletController : ApiController
    {

        private WalletDB _WalletDb = new WalletDB();
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };

        // GET: Wallet Balance
        [HttpPost]
        [Route("v2/Wallet-Balance-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetWalletBalance(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);


                var allData = _WalletDb.GetWalletBalance(lang, PatientMRN, BranchID);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = allData;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found:";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        // GET: Wallet Transaction Details
        [HttpPost]
        [Route("v2/Wallet-Transaction-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetWalletTransactionDetails(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);


                var allData = _WalletDb.GetWalletTransactionDetails(lang, PatientMRN, BranchID);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = allData;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found:";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        // GET: Wallet Beneficary List
        [HttpPost]
        [Route("v2/Wallet-BeneficiaryList-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetWalletBeneficaryList(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);


                var allData = _WalletDb.GetWalletBeneficaryList(lang, PatientMRN, BranchID);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = allData;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found:";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        // GET: Wallet Beneficary Add
        [HttpPost]
        [Route("v2/Wallet-Beneficiary-Add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult AddWalletBeneficaryList(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) 
                && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["B_MRN"])
                && !string.IsNullOrEmpty(col["B_BranchID"])
                && !string.IsNullOrEmpty(col["B_Name"])
                && !string.IsNullOrEmpty(col["B_NameAR"])
                && !string.IsNullOrEmpty(col["B_NID"])                
                && !string.IsNullOrEmpty(col["Sources"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);
                var B_MRN = Convert.ToInt32(col["B_MRN"]);
                var B_BranchId = Convert.ToInt32(col["B_BranchID"]);
                var B_Name = col["B_Name"].ToString();
                var B_NameAR = col["B_NameAR"].ToString();
                var B_NID = col["B_NID"].ToString();
                var Sources = col["Sources"].ToString();


                var errStatus = 0;
                var errMessage = "";

                _WalletDb.AddWalletBeneficary(PatientMRN, BranchID, B_MRN, B_BranchId, B_Name, B_NameAR, B_NID,Sources ,ref errStatus,ref errMessage);


                _resp.status = errStatus;
                _resp.msg = errMessage;
                

                //if (allData != null && allData.Rows.Count > 0)
                //{
                //    _resp.status = 1;
                //    _resp.msg = "Record Found";
                //    _resp.response = allData;

                //}
                //else
                //{
                //    _resp.status = 0;
                //    _resp.msg = "No Record Found:";
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
        [Route("v2/Wallet-Beneficiary-Remove")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult RemoveWalletBeneficary(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["B_ID"])                
                && !string.IsNullOrEmpty(col["Sources"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);
                var B_ID = Convert.ToInt32(col["B_ID"]);                
                var Sources = col["Sources"].ToString();


                var errStatus = 0;
                var errMessage = "";

                _WalletDb.RemoveWalletBeneficary(lang,PatientMRN, BranchID, B_ID,Sources, ref errStatus, ref errMessage);


                _resp.status = errStatus;
                _resp.msg = errMessage;

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        // GET: Vefiry Patient 
        [HttpPost]
        [Route("v2/Verify-Patient-Info")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetVerifyPatient(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) 
                && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["patient_NID"])
                )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);
                var PatientNID = col["patient_NID"];


                var allData = _WalletDb.VerifyPatientData(PatientMRN, BranchID , PatientNID);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = allData;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No Record Found:";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        // GET: Payment confirmation
        [HttpPost]
        [Route("v2/Wallet-onlinepayment-confirmation")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult WalletOnlinePaymentConfirmation(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["patient_reg_no"]) 
                && !string.IsNullOrEmpty(col["bill_type"]) 
                && !string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["OnlineTransaction_id"])
                && !string.IsNullOrEmpty(col["TracK_ID"])
                && !string.IsNullOrEmpty(col["Paid_Amount"])
                && !string.IsNullOrEmpty(col["Sources"])
                && !string.IsNullOrEmpty(col["Payment_Method"]))
            {

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);
                var hospitalID = Convert.ToInt32(col["hospital_id"]);
                var BillType = col["bill_type"];                
                var OnlineTrasactionID = col["OnlineTransaction_id"];
                var PaidAmount = col["Paid_Amount"];
                var PaymentMethod = col["Payment_Method"];
                var TrackID = Convert.ToInt32(col["TracK_ID"].ToString());
                var Sources = col["Sources"];
                

                var Status = 0;
                var Msg = "";

                if (BillType != "W")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong BillType Format.";
                    return Ok(_resp);
                }

                var _datatable = _WalletDb.OnlinePaymentConfirmation(lang,hospitalID, PatientMRN , BillType, OnlineTrasactionID, PaidAmount, PaymentMethod, TrackID, Sources, ref Status, ref Msg);

                _resp.status = Status;
                _resp.msg = Msg;
                _resp.response = _datatable;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }


        // GET: Payment confirmation
        [HttpPost]
        [Route("v2/Wallet-Amount-Transfer")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult WalletAmountTransfer(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["patient_reg_no"])                
                && !string.IsNullOrEmpty(col["hospital_id"])
                && !string.IsNullOrEmpty(col["Transfer_Amount"])
                && !string.IsNullOrEmpty(col["Sources"])
                && !string.IsNullOrEmpty(col["beneficiary_ID"])
                )
            {

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);
                var hospitalID = Convert.ToInt32(col["hospital_id"]);
                var beneficiaryID = Convert.ToInt32(col["beneficiary_ID"]);
                var TransferAmount = col["Transfer_Amount"];
                var Sources = col["Sources"];


                var Status = 0;
                var Msg = "";

                var _datatable = _WalletDb.WalletAmountTransfer(lang , hospitalID, PatientMRN, TransferAmount, beneficiaryID, Sources, ref Status, ref Msg);

                _resp.status = Status;
                _resp.msg = Msg;
                _resp.response = _datatable;
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