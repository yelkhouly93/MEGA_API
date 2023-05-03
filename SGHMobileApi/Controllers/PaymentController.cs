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
    /// <inheritdoc />
    [Authorize]
    [AuthenticationFilter]
    public class PaymentController : ApiController
    {
        private PaymentDB _paymentDb = new PaymentDB();
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };



        [HttpPost]
        [Route("v2/Invoice-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientInvoiceList(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var fromdate = "";
                if (!string.IsNullOrEmpty(col["fromdate"]))
                    fromdate = col["fromdate"];

                var todate = "";
                if (!string.IsNullOrEmpty(col["todate"]))
                    todate = col["todate"];

                var InvoiceType = "";
                if (!string.IsNullOrEmpty(col["InvoiceType"]))
                    InvoiceType = col["InvoiceType"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]); 
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                var allData = _paymentDb.GetPatientBillList(lang,hospitalId,registrationNo, fromdate, todate, InvoiceType);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
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



        [HttpPost]
        [Route("v2/Invoice-pdf-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetTestResultDetailPDF(FormDataCollection col)
        {
            var resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["Bill_id"]) && !string.IsNullOrEmpty(col["hospital_id"]))
            {
                var lang = col["lang"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var BillId = col["Bill_id"];
                var tmpobj = new InvoicePDF();

                string Str_Id = "OpBillID=" + BillId + "&BranchId=" + hospitaId.ToString();
                var ParmEnc = TripleDESImp.TripleDesEncrypt(Str_Id);
                var FinalURL = ConfigurationManager.AppSettings["InvoiceURL"].ToString() + ParmEnc;

                tmpobj.InvoiceUrl = Util.ConvertURL_to_PDF(FinalURL, BillId.ToString());
                //tmpobj.InvoiceUrl = "https://cxmw.sghgroup.com.sa/doctorsprofile/INVpdf/FOInvoiceDuplicateZATCA.pdf";
                if (tmpobj.InvoiceUrl != null)
                {
                    resp.status = 1;
                    resp.msg = "Inovice found";
                    resp.response = tmpobj;

                }
                else
                {
                    resp.status = 0;
                    resp.msg = "No Invoice Found";
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter";
            }
            return Ok(resp);
        }



        // GET: Payment
        [HttpPost]
        [Route("v2/consultaion-amount-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetConsultationAmount(FormDataCollection col)
        {
            //AHSAN TESTING need to Comment now
            //TESTENVOICES();

            _resp = new GenericResponse();

            //Ahsan Closing Date 12-02-2023 as per Payment Issues

            //_resp.status = 0;
            //_resp.msg = "Service Not Avaible.";
            //return Ok(_resp);

            //Ahsan Closing Date 12-02-2023 as per Payment Issues


            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["appointment_id"]) && !string.IsNullOrEmpty(col["bill_type"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Mobile_Payment_Test"]))
            {
                if (col["Mobile_Payment_Test"].ToString() != "1")
                {
                    _resp.status = 0;
                    _resp.msg = "Service Not Avaible.";
                    return Ok(_resp);
                }
                var HospitalId = Convert.ToInt32(col["hospital_id"]);
                var appointment_id = Convert.ToInt32(col["appointment_id"]);
                var BillType = col["bill_type"];                
                var Status = 0;
                var Msg = "";

                if (BillType != "I" && BillType != "C")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong BillType Format- It should be 'C' For Cash and 'I' for insurance";
                    return Ok(_resp);
                }

                var datatableAmount = _paymentDb.GetConsultationAmount(HospitalId, appointment_id, BillType, 1,ref Status,ref Msg);

                if (Status == 1 && datatableAmount.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = Msg;
                    _resp.response = datatableAmount;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = Msg;
                }




                //if (Error_Code == 0)
                //    _resp.status = 1;
                //else
                //    _resp.status = 0;

                //_resp.msg = MSG;
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
        [Route("v2/payment-confirmation")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PaymentConfirmation(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["appointment_id"]) && !string.IsNullOrEmpty(col["bill_type"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["OnlineTransaction_id"]) && !string.IsNullOrEmpty(col["Payment_Method"]))
            {

                var appointment_id = Convert.ToInt32(col["appointment_id"]);
                var hospitalID = Convert.ToInt32(col["hospital_id"]);
                var BillType = col["bill_type"];

                var OnlineTrasactionID = "";
                var PaidAmount = "0";
                // New Logic tracking ID on 08-03-2023 TRACKING ID
                var TrackID = 0;

                var PaymentMethod = "V2/API";
                if (!string.IsNullOrEmpty(col["OnlineTransaction_id"]))
                    OnlineTrasactionID = col["OnlineTransaction_id"];

                if (!string.IsNullOrEmpty(col["Paid_Amount"]))
                    PaidAmount = col["Paid_Amount"];

                if (!string.IsNullOrEmpty(col["Payment_Method"]))
                    PaymentMethod = col["Payment_Method"];

                if (!string.IsNullOrEmpty(col["TracK_ID"]))
                    TrackID = Convert.ToInt32(col["TracK_ID"].ToString());


                var Status = 0;
                var Msg = "";

                if (BillType != "I" && BillType != "C")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong BillType Format- It should be 'C' For Cash and 'I' for insurance";
                    return Ok(_resp);
                }

                SaveBillReturn ReturnObj;
                
                ReturnObj = _paymentDb.PaymentConfirmation_GenerateBill(hospitalID,appointment_id, BillType, 1, OnlineTrasactionID , PaidAmount, PaymentMethod, ref Status, ref Msg);
                
                //if (Status == 1 && datatableAmount.Rows.Count > 0)
                if (Status == 1 && ReturnObj != null)
                {
                    if (ReturnObj.GenerateEInvoice == "1")
                    {
                        var tempReturnHIS = GenerateInovice_HIS(hospitalID, ReturnObj.BillNo, false);
                        //Msg = Msg + tempReturnHIS.StatusCode.ToString() + " --" + tempReturnHIS.StatusMessage + "--" + tempReturnHIS.Details;
                        //Msg = Msg;
                    }
                    
                    _resp.status = 1;
                    _resp.msg = Msg;
                    _resp.response = ReturnObj;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = Msg;
                }
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        // GET: Payment
        [HttpPost]
        [Route("v2/services-amount-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetServicesAmount(FormDataCollection col)
        {
            _resp = new GenericResponse();

            //Ahsan Closing Date 12-02-2023 as per Payment Issues
            
            //_resp.status = 0;
            //_resp.msg = "Service Not Avaible.";
            //return Ok(_resp);

            //Ahsan Closing Date 12-02-2023 as per Payment Issues

            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["visit_id"]) && !string.IsNullOrEmpty(col["bill_type"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Mobile_Payment_Test"]))
            {

                if (col["Mobile_Payment_Test"].ToString() != "1")
                {
                    _resp.status = 0;
                    _resp.msg = "Service Not Avaible.";
                    return Ok(_resp);
                }


                var HospitalId = Convert.ToInt32(col["hospital_id"]);
                var Visit_id = Convert.ToInt32(col["visit_id"]);
                var BillType = col["bill_type"];
                var Status = 0;
                var Msg = "";

                if (BillType != "I" && BillType != "C")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong BillType Format- It should be 'C' For Cash and 'I' for insurance";
                    return Ok(_resp);
                }

                var Service_Ids = "";
                var Department_Ids = "";
                var Item_Ids = "";

                if (!string.IsNullOrEmpty(col["Service_Ids"]))
                    Service_Ids = col["Service_Ids"];

                if (!string.IsNullOrEmpty(col["Item_Ids"]))
                    Item_Ids = col["Item_Ids"];

                if (!string.IsNullOrEmpty(col["Department_Ids"]))
                    Department_Ids = col["Department_Ids"];

                




                var datatableAmount = _paymentDb.GetServicesAmount(HospitalId, Visit_id, BillType, 1,Service_Ids, Item_Ids, Department_Ids, ref Status, ref Msg);

                if (Status == 1 && datatableAmount.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = Msg;
                    _resp.response = datatableAmount;

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
        [Route("v2/payment-Services-confirmation")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PaymentServicesConfirmation(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["Visit_id"]) && !string.IsNullOrEmpty(col["bill_type"]) && !string.IsNullOrEmpty(col["hospital_id"]) 
                && !string.IsNullOrEmpty(col["Service_Ids"]) && !string.IsNullOrEmpty(col["Department_Ids"]) && !string.IsNullOrEmpty(col["Item_Ids"]) && !string.IsNullOrEmpty(col["OnlineTransaction_id"]) && !string.IsNullOrEmpty(col["Payment_Method"]))
            {
                var BillType = col["bill_type"];

                if (BillType != "I" && BillType != "C")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong BillType Format- It should be 'C' For Cash and 'I' for insurance";
                    return Ok(_resp);
                }

                var Visit_id = Convert.ToInt32(col["Visit_id"]);
                var hospitalID = Convert.ToInt32(col["hospital_id"]);
                
                var Service_Ids = col["Service_Ids"];
                var Department_Ids = col["Department_Ids"];
                var Item_Ids = col["Item_Ids"];

                var OnlineTrasactionID = "";
                var PaidAmount = "0";
                var PaymentMethod = "V2/API";
                var VisitTypeId = 1;

                if (!string.IsNullOrEmpty(col["OnlineTransaction_id"]))
                    OnlineTrasactionID = col["OnlineTransaction_id"];

                if (!string.IsNullOrEmpty(col["Paid_Amount"]))
                    PaidAmount = col["Paid_Amount"];
                
                if (!string.IsNullOrEmpty(col["VisitType_id"]))
                    VisitTypeId = Convert.ToInt32(col["VisitType_id"]);


                if (!string.IsNullOrEmpty(col["Payment_Method"]))
                    PaymentMethod = col["Payment_Method"];

                var Status = 0;
                var Msg = "";

                var ReturnObj = _paymentDb.PaymentServicesConfirmation_GenerateBill(hospitalID, Visit_id, VisitTypeId, BillType,Service_Ids,Department_Ids,Item_Ids, 1,OnlineTrasactionID,PaidAmount, PaymentMethod, ref Status, ref Msg);                
                
                
                if (Status == 1 && ReturnObj != null)
                {   
                    Generate_ALL_HIS_SERVICES_Inovice(ReturnObj, hospitalID);                    
                    _resp.status = 1;
                    _resp.msg = Msg;
                    _resp.response = ReturnObj;

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = Msg;
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }

        public void Generate_ALL_HIS_SERVICES_Inovice(List<SaveBillReturn> ObjList , int hospitalID)
        {

            if (ObjList.Count > 0)
            {
                foreach (var billModel in ObjList)
                {
                    // Temporary Bypass 
                    //if (billModel.GenerateEInvoice == "1")
                        GenerateInovice_HIS(hospitalID, billModel.BillNo, false);            
                }
            }
            return ;
        }
        public EInvoiceReturn GenerateInovice_HIS(int BranchId, string billNo, bool isCancellation)
        {
            var ObjList = _paymentDb.GetInvoiceInfo(BranchId, billNo, isCancellation);
            if (ObjList.Count > 0)
            {
               var temp =  GeneratingInvoice(ObjList , BranchId);
                return temp;
            }
            return null;
        }

        public EInvoiceReturn GeneratingInvoice(List<EInvoiceParam> ObjList ,int BranchId)
        {
            if (ObjList.Count > 0)
            {
                foreach (var billModel in ObjList)
                {
                    var temp = GenerateZATCAInvoice(billModel, BranchId);
                }
                
            }
            return null;
        }
        
        public EInvoiceReturn GenerateZATCAInvoice(EInvoiceParam param,int BranchId)
        {
            var apiReturnData = new EInvoiceReturn();
            var baseUrl = "";
            var apiMethod = "";

            baseUrl = ConfigurationManager.AppSettings["RCMAPIBaseUrl_"+ BranchId.ToString()].ToString();            
            apiMethod = ConfigurationManager.AppSettings["RCMAPIGenerateQRStandAlone"].ToString();


            //var apiQRType = "";
            //apiQRType = ConfigurationManager.AppSettings["RCMAPIQRFormatType"].ToString();
            //if (apiQRType == "1")
            //{
            //    apiMethod = ConfigurationManager.AppSettings["RCMAPIGenerateQRStandAlone"].ToString();
            //}
            //else
            //{
            //    apiMethod = ConfigurationManager.AppSettings["RCMAPIGenerateQRPDFLink"].ToString();
            //}

            var url = baseUrl + apiMethod;
            HttpStatusCode status;
            apiReturnData = RestUtility.CallServiceEInvoice <EInvoiceReturn>(url, null, param, "POST", out status) as EInvoiceReturn;
    
            if (apiReturnData != null)
            {
                if (apiReturnData.StatusCode == 200 & apiReturnData.Details != null)
                {
                }
                else
                {
                    apiReturnData = new EInvoiceReturn();                    
                    apiReturnData.StatusCode = 500;
                    apiReturnData.StatusMessage = "ERROR 500"; /*apiresult.ErrorMessage;*/
                    apiReturnData.Details = null;
                }
            }
            else
            {
                apiReturnData = new EInvoiceReturn();
                apiReturnData.StatusCode = 500;
                apiReturnData.StatusMessage = "ERROR 500"; /*apiresult.ErrorMessage;*/
                apiReturnData.Details = null;
            }
            
            
            return apiReturnData;
        }

        public void TESTENVOICES ()
        {
            var tempReturnHIS = GenerateInovice_HIS(1, "CR5643", false);
        }
    }
}