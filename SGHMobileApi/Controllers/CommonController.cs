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
using System.Web;
using System.IO;
using System.Configuration;

namespace SGHMobileApi.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [AuthenticationFilter]
    public class CommonController : ApiController
    {
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };
        private CommonDB _commonDb = new CommonDB();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ///// <summary>
        ///// Get the list of all titles available in HIS.
        ///// </summary>
        ///// <returns>Return list of all titles available in HIS</returns>
        //[HttpPost]
        //[Route("api/get-titles")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PostGetTitles(FormDataCollection col)
        //{
        //    var lang = col["lang"];
        //    var hospitaId = Convert.ToInt32(col["hospital_id"]);

            
        //    _commonDB = new CommonDB();
        //    List<Titles> _allTitles = _commonDB.GetAllTitles(lang, hospitaId);


        //    _resp = new GenericResponse();

        //    if (_allTitles != null && _allTitles.Count > 0)
        //    {
        //        _resp.status = 1;
        //        _resp.msg = "Success";
        //        _resp.response = _allTitles;
                
        //    }
        //    else
        //    {
        //        _resp.status = 0;
        //        _resp.msg = "Fail";
                

        //    }

        //    return Ok(_resp);
        //}


        /// <summary>
        /// Get the list of all marital status codes available in HIS.
        /// </summary>
        /// <returns>Return list of all marital status codes available in HIS</returns>
        //[HttpPost]
        //[Route("api/get-maritalstatuscodes")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PostGetMaritalStatusCodes(FormDataCollection col)
        //{
        //    var lang = col["lang"];
        //    var hospitaId = Convert.ToInt32(col["hospital_id"]);

        //    _commonDB = new CommonDB();
        //    List<MaritalStatusCodes> _allMaritalStatusCodes = _commonDB.GetAllMaritalStatusCodes(lang, hospitaId);


        //    _resp = new GenericResponse();

        //    if (_allMaritalStatusCodes != null && _allMaritalStatusCodes.Count > 0)
        //    {
        //        _resp.status = 1;
        //        _resp.msg = "Success";
        //        _resp.response = _allMaritalStatusCodes;

        //    }
        //    else
        //    {
        //        _resp.status = 0;
        //        _resp.msg = "Fail";


        //    }

        //    return Ok(_resp);
        //}



        //[HttpPost]
        //[Route("api/verify-otp")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult GetVerificationCode(FormDataCollection col)
        //{
        //    //var lang = col["lang"];
        //    var hospitalId = Convert.ToInt32(col["hospital_id"]);
        //    var patientRegNo = col["patient_reg_no"];
        //    var patientPhone = col["patient_phone"];
        //    var verificationCode = col["verification_code"];

            
        //    PatientDB _patientDB = new PatientDB();
        //    var verifiedCode = _patientDB.VerifyOTP(hospitalId, patientRegNo, patientPhone, verificationCode);


        //    log.Info("Test for VerifyCode");
        //    log.Info("Branch Id: " + hospitalId);
        //    log.Info("Registratin No: " + patientRegNo);
        //    log.Info("Verification Code: " + verificationCode);
        //    log.Info("Cell No: " + patientPhone);
        //    log.Info("Verified Code: " + verifiedCode);
        //    log.Info("---------------");

        //    //RegisterPatientResponse resp = new RegisterPatientResponse();
        //    GenericResponse resp = new GenericResponse();

        //    if (verifiedCode != null && verifiedCode == verificationCode)
        //    {
        //        //string smsRes = Util.SendTestSMS(resendVerification.PCellNo, "Your OTP for SGH Mobile App Registration is " + resendVerification.ActivationNo);
        //        resp.msg = "OTP Verified";
        //        //resp.response = verificationCode;
        //        //resp.activation_num = Convert.ToInt32(verificationCode);
        //        //resp.smsResponse = smsRes;
        //        resp.status = 1;
        //    }
        //    else
        //    {
        //        resp.error_type = "invalid_or_mismatched_otp";
        //        resp.msg = "OTP Not Verified";
        //        //resp.response = registerPatientResFailure;
        //        resp.status = 0;
        //    }

        //    return Ok(resp);
        //}

        //[HttpPost]
        //[Route("api/resend-verification-code")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult ResendVerificationCode(FormDataCollection col)
        //{
        //    //var lang = col["lang"];
        //    var hospitalId = Convert.ToInt32(col["hospital_id"]);
        //    var patientRegNo = col["patient_reg_no"];
        //    var patientPhone = col["patient_phone"];

        //    PatientDB _patientDB = new PatientDB();
        //    var verificationCode = _patientDB.GetVerificaitonCode(hospitalId, patientRegNo, patientPhone);

        //    RegisterPatientResponse resp = new RegisterPatientResponse();
        //    //GenericResponse resp = new GenericResponse();

        //    if (verificationCode != null)
        //    {
        //        string smsRes = Util.SendTestSMS(patientPhone, "Your OTP for SGH Mobile App Registration is " + verificationCode);
        //        //resp.msg = "Activation code has been sent to the patient";
        //        //resp.response = verificationCode;
        //        resp.activation_num = Convert.ToInt32(verificationCode);
        //        //resp.smsResponse = smsRes;
        //        resp.status = 1;
        //    }
        //    else
        //    {
        //        resp.error_type = "no_valid_verification_code";
        //        resp.msg = "No Valid Verification Available";
        //        //resp.response = registerPatientResFailure;
        //        resp.status = 0;
        //    }

        //    return Ok(resp);
        //}


        [HttpPost]
        [Route("SMS")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SmsRecordDb(FormDataCollection col)
        {

            _resp = new GenericResponse();
            if (!string.IsNullOrEmpty(col["branch_id"]) && !string.IsNullOrEmpty(col["mobile_no"]) &&
                !string.IsNullOrEmpty(col["sms_content"]) && !string.IsNullOrEmpty(col["sms_source"]))
            {
                var hospitalId = 0;
                try
                {
                    hospitalId = Convert.ToInt32(col["branch_id"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                

                var mobileNo = col["mobile_no"];
                var smsText = col["sms_content"];
                var smsSource = col["sms_source"];
                
                _commonDb = new CommonDB();

                var errStatus = 0;
                var errMessage = "";
                
                _commonDb.SMSRequest(hospitalId, smsSource, mobileNo, smsText, ref errMessage, ref errStatus);


                _resp.msg = errMessage;
                _resp.status = errStatus != 0 ? 1 : 0;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";

            }

            return Ok(_resp);
        }

        /// <summary>
        /// Get the list of all nationalities available in HIS.
        /// </summary>
        /// <returns>Return list of all nationalities available in HIS</returns>
        [HttpPost]
        [Route("nationalities-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetHisNationalities(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]))
                {
                    var lang = col["lang"];
                    var hospitaId = 0;

                    try
                    {
                        hospitaId = Convert.ToInt32(col["hospital_id"]);
                    }
                    catch (Exception e)
                    {
                        _resp.status = 0;
                        _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                        return Ok(_resp);
                    }
                    

                    NationalityDB _NationalityDB = new NationalityDB();
                    List<Nationalities> _allNationalities = _NationalityDB.GetAllNationalities(lang, hospitaId);

                    if (_allNationalities != null && _allNationalities.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Success";
                        _resp.response = _allNationalities;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "Fail";
                    }
                }
                else
                {

                }

            }
            catch(Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }
            

            return Ok(_resp);
        }

        [HttpPost]
        [Route("v2/nationalities-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetNationalities(FormDataCollection col)
        {
            try
            {
                var lang = "EN";
                var hospitaId = 0;
                var OnlyTwo = 0;

                if (col != null)
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];
                    try
                    {
                        if (!string.IsNullOrEmpty(col["hospital_id"]))
                            hospitaId = Convert.ToInt32(col["hospital_id"]);

                        if (!string.IsNullOrEmpty(col["Only_Saudi"]))
                            OnlyTwo = Convert.ToInt32(col["Only_Saudi"]);
                    }
                    catch (Exception ex2)
                    {

                    }
                    
                }

                NationalityDB _NationalityDB = new NationalityDB();
                List<Nationalities> _allNationalities = _NationalityDB.GetAllNationalities_V2(lang, hospitaId, OnlyTwo);

                if (_allNationalities != null && _allNationalities.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = _allNationalities;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Fail";
                }



                //if (!string.IsNullOrEmpty(col["hospital_id"]))
                //{

                //}
                //else
                //{
                //    _resp.status = 0;
                //    _resp.msg = "Failed! Missing Parameters";
                //}

            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }


            return Ok(_resp);
        }




        [HttpPost]
        [Route("SGHDataRequest")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetGeneralDataApi(FormDataCollection col)
        {
            try
            {
                string errMessage = "";
                var errStatus = 0;
                if (!string.IsNullOrEmpty(col["DataSource_id"]) && !string.IsNullOrEmpty(col["hospital_id"]))
                {

                    var hospitalId = 0;
                    var SPName = 0;
                    try
                    {
                        hospitalId = Convert.ToInt32(col["hospital_id"]);
                        SPName = Convert.ToInt32(col["DataSource_id"]);
                    }
                    catch (Exception e)
                    {
                        _resp.status = 0;
                        _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                        return Ok(_resp);
                    }

                    var StrParameter = "";
                    IEnumerator<KeyValuePair<string, string>> pairs = col.GetEnumerator();
                    while (pairs.MoveNext())
                    {
                        KeyValuePair<string, string> pair = pairs.Current;
                        string strKey = pair.Key;
                        string strValue = pair.Value;

                        if (strKey != "hospital_id" && strKey != "DataSource_id")
                        {
                            if (StrParameter.Length > 0)
                                StrParameter += "~";

                            StrParameter += strValue;
                        }
                    }
                    
                    CommonDB CDB = new CommonDB();
                    var DataTable = CDB.GetGeneralData_DT(hospitalId, SPName, StrParameter, ref errStatus, ref errMessage);
                    
                    if (DataTable!= null && DataTable.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = errMessage;
                        _resp.response = DataTable;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = errMessage;
                    }

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }
            }
            catch (Exception ex)
            {
                _resp.status = 0;
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }

            return Ok(_resp);            
        }

        [HttpPost]
        [Route("V2/Comments-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewComments(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_name"]) && !string.IsNullOrEmpty(col["patient_Comments"]) 
                && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Type_ID"]))
            {

                var CtypeID =0;
                var PBranchID = 0;

                try
                {
                    CtypeID = Convert.ToInt32(col["Type_ID"]);
                    PBranchID = Convert.ToInt32(col["hospital_id"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                var PMobile = col["patient_phone"];
                var PName = col["patient_name"];
                var PComments = col["patient_Comments"];                
                var PEmail = col["patient_Email"];
                var CProfileID = 0;
                var CMRN = 0;
                var PatientBranch = 0;
                var status = 0;
                // Logic change according new submission 
                var PCTypeListID = 1;
                var msg = "";
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                if (!string.IsNullOrEmpty(col["Profile_ID"]))
                    CProfileID = Convert.ToInt32(col["Profile_ID"]);

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    CMRN = Convert.ToInt32(col["patient_reg_no"]);

                if (!string.IsNullOrEmpty(col["patient_hospital_id"]))
                    PatientBranch  = Convert.ToInt32(col["patient_hospital_id"]);

                if (CMRN !=  0 && PatientBranch == 0)
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Missing Parameters";
                    return Ok(_resp);
                }
                

                CommonDB CDB = new CommonDB();
                CDB.SavePatientComments(CtypeID ,PCTypeListID, PComments , PMobile , PName , PEmail , CProfileID ,CMRN ,  PBranchID, PatientBranch,ref status , ref msg);                
                //CDB.SendEmail();

                if (status > 0 )
                {
                    _resp.status = status;
                    _resp.msg = msg;
                    if (lang == "EN" || lang == "en")
                    {
                        _resp.msg = "Thank you for the feedback to help us improve your experience, We will work on your feedback and update you within 3 working Days";
                    }
                    else
                    {
                        _resp.msg = "نشكركم على تواصلكم معنا لتحسين تجربتكم، سيتم العمل على إفادتكم والتواصل معكم خلال ٣ أيام عمل";
                    }

                    
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed to record the comment";
                    if (lang == "EN")
                    {
                        _resp.msg = "Failed to record your comment";
                    }
                    else
                    {
                        _resp.msg = "تم تسجيل إفادتكم وإرسالها للقسم المعني";
                    }
                    _resp.msg = "لم يتم حفظ التعليق";
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
        [Route("V3/Comments-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewComments_V3(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_name"]) && !string.IsNullOrEmpty(col["patient_Comments"])
                && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Sources"]) && !string.IsNullOrEmpty(col["Type_ID"]))
            {

                var CtypeID = 0;
                var PBranchID = 0;

                try
                {
                    CtypeID = Convert.ToInt32(col["Type_ID"]);
                    PBranchID = Convert.ToInt32(col["hospital_id"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                var PMobile = col["patient_phone"];
                var PName = col["patient_name"];
                var PComments = col["patient_Comments"];
                var PEmail = col["patient_Email"];
                var CProfileID = 0;
                var CMRN = 0;
                var PatientBranch = 0;
                var status = 0;
                
                // Logic change according new submission 
                var PCTypeListID = 1;
                var msg = "";
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                if (!string.IsNullOrEmpty(col["Profile_ID"]))
                    CProfileID = Convert.ToInt32(col["Profile_ID"]);

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    CMRN = Convert.ToInt32(col["patient_reg_no"]);

                if (!string.IsNullOrEmpty(col["patient_hospital_id"]))
                    PatientBranch = Convert.ToInt32(col["patient_hospital_id"]);

                if (CMRN != 0 && PatientBranch == 0)
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Missing Parameters";
                    return Ok(_resp);
                }


                var Sources = col["Sources"];


                CommonDB CDB = new CommonDB();
                CDB.SavePatientComments_V3(CtypeID, PCTypeListID, PComments, PMobile, PName, PEmail, CProfileID, CMRN, PBranchID, PatientBranch, Sources, ref status, ref msg);
                //CDB.SendEmail();

                if (status > 0)
                {
                    _resp.status = status;
                    _resp.msg = msg;
                    if (lang == "EN" || lang == "en")
                    {
                        _resp.msg = "Thank you for the feedback to help us improve your experience, We will work on your feedback and update you within 3 working Days";
                    }
                    else
                    {
                        _resp.msg = "نشكركم على تواصلكم معنا لتحسين تجربتكم، سيتم العمل على إفادتكم والتواصل معكم خلال ٣ أيام عمل";
                    }


                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed to record the comment";
                    if (lang == "EN")
                    {
                        _resp.msg = "Failed to record your comment";
                    }
                    else
                    {
                        _resp.msg = "تم تسجيل إفادتكم وإرسالها للقسم المعني";
                    }
                    _resp.msg = "لم يتم حفظ التعليق";
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
        [Route("v2/Comments-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetComments(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["patient_hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

               //var PMobile = col["patient_phone"];

                var CMRN = Convert.ToInt32(col["patient_reg_no"]);
                var PatientBranch = Convert.ToInt32(col["patient_hospital_id"]); ;

                var allCommentsList = CDB.GetPatientComments_DT(CMRN, PatientBranch, lang);


                if (allCommentsList != null)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = allCommentsList;

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
        [Route("v2/CType-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetCommentTypes(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            var lang = "EN";

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

            }



            var allCommentsList = CDB.GetCommentType_DT(lang);


            if (allCommentsList != null)
            {
                _resp.status = 1;
                _resp.msg = "Record(s) Found";
                _resp.response = allCommentsList;

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "No Record Found";
            }


            return Ok(_resp);

        }

        [HttpPost]
        [Route("v2/CType-List-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetCommentTypeLists(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["Ctype_Id"]))
            {

                var lang = "EN";
                var CtypeID = Convert.ToInt32(col["Ctype_Id"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];
                             



                var allCommentsList = CDB.GetCommentTypeList_DT(lang , CtypeID);


                if (allCommentsList != null)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = allCommentsList;

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
                _resp.msg = "Missing Parameter.";
            }

               


            return Ok(_resp);

        }

        [HttpPost]
        [Route("v2/IncApprovalStatus-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetInSuranceApprovalStatus(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
            {
                var lang = "EN";
                var CMRN = Convert.ToInt32(col["patient_reg_no"]);
                var BranchID = Convert.ToInt32(col["hospital_id"]);

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var status = 0;
                var msg = "";


                var DTList = CDB.GetPatientInSuranceApprovalStatus_DT(lang, BranchID,CMRN, ref status, ref msg);


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
            CommonDB CDB = new CommonDB();

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


        //[HttpPost]
        //[Route("api/GetData")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult DynamicApi(FormDataCollection col)
        //{
        //    try
        //    {
        //        // first Get the Store Procedure Name from API
        //        // 
        //        var parameters = new List<SqlParameter>();
        //        IEnumerator<KeyValuePair<string, string>> pairs = col.GetEnumerator();
        //        while (pairs.MoveNext())
        //        {
        //            KeyValuePair<string, string> pair = pairs.Current;
        //            string strKey = pair.Key;
        //            string strValue = pair.Value;
        //            SqlParameter p = new SqlParameter(strKey, strValue);
        //            parameters.Add(p);
        //        }
        //        //SqlParameter[] sqlParameter;
        //        //sqlParameter = parameters.ToArray();





        //        GenericResponse resp = new GenericResponse();
        //        var spName = col["sp_Name"];
        //        if (spName != null)
        //        {
        //            var dbCommonDb = new CommonDB();

        //            var dtp = dbCommonDb.GetSpParameters(spName);

        //            foreach (DataRow row in dtp.Rows)
        //            {
        //                //dt.Rows[i][valueField].ToString();



        //            }

        //            var lang = col["lang"];
        //            var hospitalId = Convert.ToInt32(col["hospital_id"]);
        //            var clinicId = Convert.ToInt32(col["clinic_id"]);
        //            var pageNo = Convert.ToInt32(col["page_no"]);
        //            var pageSize = Convert.ToInt32(col["page_size"]);

        //            //DBO.[Get_Doctors_SP]

        //            var expandoList = new List<dynamic>();

        //            //CommentFor Testing //if (loginDb.ValidateOTP(regno, hospitalId, otpid))  // Validate OTP First 
        //            if (true) // OTP Success
        //            {

        //                var dt = dbCommonDb.GetDynamicDataTable(spName);

        //                if (expandoList.Count > 0)
        //                {
        //                    resp.status = 1;
        //                    resp.msg = "errMessage";
        //                    resp.response = dt;
        //                }
        //                else
        //                {
        //                    resp.status = 0;
        //                    resp.msg = "No data Found";
        //                    resp.response = null;
        //                }
        //                return Ok(resp);
        //            }
        //            else
        //            {
        //                // OTP Fail condition                    
        //                resp.status = 0;
        //                resp.msg = "OTP Not Match, Please enter the Correct OTP";
        //                resp.response = null;
        //                resp.error_type = "OTP Miss-match";
        //                return Ok(resp);
        //            }
        //        }
        //        else
        //        {
        //            // no SP Name Found 
        //            resp.status = 0;
        //            resp.msg = "SP Parameter missing";
        //            resp.response = null;
        //            resp.error_type = "SP Parameter missing";
        //            return Ok();
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        Log.Error(ex);
        //    }

        //    return Ok();
        //}






        //[HttpGet]
        //[Route("api/DynamicApi-test-V2")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult DynamicApi(FormDataCollection col)
        //{
        //    try
        //    {
        //        // first Get the Store Procedure Name from API
        //        // 
        //        GenericResponse resp = new GenericResponse();
        //        var spName = col["sp_Name"];
        //        if (spName != null)
        //        {
        //            var dbCommonDb = new CommonDB();

        //            var dtp = dbCommonDb.GetSpParameters(spName);

        //            foreach (DataRow row in dtp.Rows)
        //            {
        //                //dt.Rows[i][valueField].ToString();



        //            }

        //            var lang = col["lang"];
        //            var hospitalId = Convert.ToInt32(col["hospital_id"]);
        //            var clinicId = Convert.ToInt32(col["clinic_id"]);
        //            var pageNo = Convert.ToInt32(col["page_no"]);
        //            var pageSize = Convert.ToInt32(col["page_size"]);

        //            //DBO.[Get_Doctors_SP]

        //            var expandoList = new List<dynamic>();

        //            //CommentFor Testing //if (loginDb.ValidateOTP(regno, hospitalId, otpid))  // Validate OTP First 
        //            if (true) // OTP Success
        //            {

        //                var dt = dbCommonDb.GetDynamicDataTable(spName);

        //                if (expandoList.Count > 0)
        //                {
        //                    resp.status = 1;
        //                    resp.msg = "errMessage";
        //                    resp.response = dt;
        //                }
        //                else
        //                {
        //                    resp.status = 0;
        //                    resp.msg = "No data Found";
        //                    resp.response = null;
        //                }
        //                return Ok(resp);
        //            }
        //            else
        //            {
        //                // OTP Fail condition                    
        //                resp.status = 0;
        //                resp.msg = "OTP Not Match, Please enter the Correct OTP";
        //                resp.response = null;
        //                resp.error_type = "OTP Miss-match";
        //                return Ok(resp);
        //            }
        //        }
        //        else
        //        {
        //            // no SP Name Found 
        //            resp.status = 0;
        //            resp.msg = "SP Parameter missing";
        //            resp.response = null;
        //            resp.error_type = "SP Parameter missing";
        //            return Ok();
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        Log.Error(ex);
        //    }

        //    return Ok();
        //}

        [HttpPost]
        [Route("v2/RequestTypes-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetReqyestTypes(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            var lang = "EN";
            var TypeLevel = 0;

            if (!string.IsNullOrEmpty(col["Rtype_id"]) )
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var Rtype_ID = 0;                
                try
                {
                    Rtype_ID = Convert.ToInt32(col["Rtype_id"]);
                    if (!string.IsNullOrEmpty(col["LevelType"]))
                        TypeLevel = Convert.ToInt32(col["LevelType"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                

                var DTList = CDB.GetRequestType_DT(lang, Rtype_ID , TypeLevel);

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
                _resp.msg = "Missing Parameter, Main Request Type Required";
            }

            return Ok(_resp);
        }
        
        [HttpPost]
        [Route("v2/NewRequest-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult AddNewRequest(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();
           

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["req_type"]))
            {
                var status = 0;
                var msg = "";
                var requestIDetails = "";
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                var CMRN = Convert.ToInt32(col["patient_reg_no"]);
                var BranchID = Convert.ToInt32(col["hospital_id"]);
                var requestType = Convert.ToInt32(col["req_type"]);
                var Comments = col["patient_comments"];


                if (!string.IsNullOrEmpty(col["req_detail"]))
                    requestIDetails = col["req_detail"];

                CDB.SaveNewRequest(lang, BranchID, CMRN,requestType,requestIDetails, ref status, ref msg);

                if (status > 0)
                {
                    _resp.status = 1;

                    if (lang == "EN" || lang == "en")
                    {
                        _resp.msg = "Your request has been saved and forwarded to relevant department.";
                    }
                    else
                    {
                        _resp.msg = "تم تسجيل إفادتكم وإرسالها للقسم المعني ";
                    }
                    //_resp.msg = msg;
                }                    
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Your Request save Unsuccessfully.";
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
        [Route("v2/Request-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetRequest(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var PMrn = Convert.ToInt32( col["patient_reg_no"]);
                var branchID = Convert.ToInt32(col["hospital_id"]);

                var allReportList = CDB.GetRequestList_DT(PMrn, branchID,lang);


                if (allReportList != null && allReportList.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Found";
                    _resp.response = allReportList;

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
        [Route("v2/Send-OTP")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SendOTPRequest(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_phone"]))
            {
                //var lang = "EN";
                var CMRN = "0";
                var ReasonCode = 2;
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var PatientPhone = col["patient_phone"];
                
                var National_ID = col["patient_national_id"];

                if (!string.IsNullOrEmpty(col["reason_code"]))
                    ReasonCode = Convert.ToInt32(col["reason_code"]); 

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    CMRN = col["patient_reg_no"];

                var ActivationOTP = 0;
                var Error_Code = 1;
                var MSG = "";

                CDB.SENT_OTP(hospitaId, PatientPhone, CMRN, National_ID, ReasonCode, ref ActivationOTP, ref Error_Code, ref MSG);

                //if (hospitaId != 201 && !Util.UaeBranches.Contains(hospitaId))
                //    Util.SendTestSMS(PatientPhone, ActivationOTP + " is your OTP for SGH Mobile App");
                //else if (hospitaId == 201)
                //    Util.SendSMS_Cairo(PatientPhone, ActivationOTP + " is your OTP for SGH Mobile App");
                //else if (Util.UaeBranches.Contains(hospitaId))                
                //    Util.SendSMS_UAE(hospitaId, PatientPhone, ActivationOTP + " is your OTP for SGH Mobile App");

                //var MsgContent = "<#> SGHC OTP Code " + ActivationOTP + " ";
                var MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + ActivationOTP + " ";                
                MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();


                if (hospitaId != 201 && !Util.UaeBranches.Contains(hospitaId))
                    Util.SendTestSMS(PatientPhone, MsgContent);
                else if (hospitaId == 201)
                    Util.SendSMS_Cairo(PatientPhone, MsgContent);
                else if (Util.UaeBranches.Contains(hospitaId))
                    Util.SendSMS_UAE(hospitaId, PatientPhone, MsgContent);




                if (Error_Code == 0)
                    _resp.status = 1;
                else
                    _resp.status = 0;

                _resp.msg = MSG;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        [HttpPost]
        [Route("v2/gen-Verify-OTP")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult VerifyOTPRequest(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["reason_code"]) && !string.IsNullOrEmpty(col["verification_code"]))
            {   
                var hospitaId = 0;
                var ReasonCode = 3;

                var PatientPhone = col["patient_phone"];
                var ActivationCode = col["verification_code"];

                try
                {
                    hospitaId = Convert.ToInt32(col["hospital_id"]);
                    ReasonCode = Convert.ToInt32(col["reason_code"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }



                if (CDB.VerifyOTP_Mobile(hospitaId, PatientPhone, ActivationCode, ReasonCode))
                {
                    _resp.status = 1;
                    _resp.msg = "OTP Verified";
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed: OTP not Verify";
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
        [Route("v2/UpdateData-OTP")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult VerifyOTP_And_UpdateData(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) 
                && !string.IsNullOrEmpty(col["verification_code"]) && !string.IsNullOrEmpty(col["patient_DOB"]) && !string.IsNullOrEmpty(col["patient_Gender"]) && !string.IsNullOrEmpty(col["Marital_Status"])
                )
            {  
                //&& !string.IsNullOrEmpty(col["patient_nationality_id"])
                var CMRN = col["patient_reg_no"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);
                var PatientPhone = col["patient_phone"];
                var patient_DOB = col["patient_DOB"];
                var ActivationCode = col["verification_code"];
                var patient_Gender = col["patient_Gender"];
                var Marital_Status = col["Marital_Status"];
                //var PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                var PatientNationalityId = 0;
                var EMail = col["patient_Email"];

                var patient_Gender_i = 1;
                var Status = 0;
                var Msg = "";
                var Sources = ConfigurationManager.AppSettings["API_SOURCE_KEY"].ToString();

                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];

                if (!string.IsNullOrEmpty(col["Source"]))
                    Sources = col["Source"];


                DateTime PDateOfBirth;
                if (!DateTime.TryParse(patient_DOB, out PDateOfBirth))
                {
                    // handle parse failure
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Date Format";
                    return Ok(_resp);
                }
                if (PDateOfBirth > DateTime.Now)
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Date";
                    return Ok(_resp);
                }
                
                if (patient_Gender != "F" && patient_Gender != "M")
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Gender Format";
                    return Ok(_resp);
                }

                if (Marital_Status != "S" && Marital_Status != "M" )
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Wrong Marital Status Format";
                    return Ok(_resp);
                }
                var iMarital_Status = 6;

                if (Marital_Status == "S")
                    iMarital_Status = 6;

                if (Marital_Status == "M")
                    iMarital_Status = 2;



                if (CDB.VerifyOTP_Mobile(hospitaId, PatientPhone,  ActivationCode))
                {
                    PatientDB patientDb = new PatientDB();
                    if (patient_Gender == "F")
                        patient_Gender_i = 1;
                    else
                        patient_Gender_i = 2;
                    var RegistrationNo = Convert.ToInt32(CMRN); 

                    patientDb.UpdatePatientData(hospitaId, RegistrationNo, PDateOfBirth, patient_Gender_i, PatientPhone, iMarital_Status, PatientNationalityId, EMail, Sources, ref Status, ref Msg);
                    _resp.status = Status;
                    _resp.msg = Msg;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! :  Wrong OTP ";
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


        [HttpPost]
        [Route("v2/Notification-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetNotification(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();


            var lang = "EN";
            var MRN = "";
            var FCMToken = "";
            var hospitalID = 0;
            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    MRN = col["patient_reg_no"];

                if (!string.IsNullOrEmpty(col["APP_Token"]))
                    FCMToken = col["APP_Token"];

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalID = Convert.ToInt32(col["hospital_id"]);

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Missing Parameter";
                return Ok(_resp);
            }

            var allList = CDB.GetNotificationList_DT(lang , MRN , FCMToken , hospitalID);


            if (allList != null)
            {
                _resp.status = 1;
                _resp.msg = "Record(s) Found";
                _resp.response = allList;

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "No Record Found";
            }

            

            return Ok(_resp);

        }


        [HttpPost]
        [Route("v2/Notification-Unread-Count-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetNotificationUnReadCount(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();


            var lang = "EN";
            var MRN = "";
            var FCMToken = "";

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]))
                {

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);

                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    MRN = col["patient_reg_no"];

                    if (!string.IsNullOrEmpty(col["APP_Token"]))
                        FCMToken = col["APP_Token"];


                    var allList = CDB.GetNotificationUnReadCOunt_DT(lang, MRN, FCMToken, hospitaId);
                    if (allList != null)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allList;
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
                    _resp.msg = "Missing Parameters";
                    return Ok(_resp);
                }

                

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Missing Parameters";
                return Ok(_resp);
            }

           
            return Ok(_resp);
        }



        [HttpPost]
        [Route("v2/Notification-read")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult NotificationRead(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();
            var AppTokken = "";
            var NotificationID = 0;

            if (!string.IsNullOrEmpty(col["APP_Token"]) && !string.IsNullOrEmpty(col["Notification_ID"]))
            {
                //if (!string.IsNullOrEmpty(col["lang"]))
                //    lang = col["lang"];
                try
                {
                    if (!string.IsNullOrEmpty(col["Notification_ID"]))
                        NotificationID = Convert.ToInt32(col["Notification_ID"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }


                if (!string.IsNullOrEmpty(col["APP_Token"]))
                    AppTokken = col["APP_Token"];

                var status = 0;
                var msg = "";
                CDB.UpdateNotificationRead(AppTokken, NotificationID, ref status, ref msg);


                if (status == 1)
                {
                    _resp.status = 1;
                    _resp.msg = "Record(s) Updated";
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failded No record Updatede";
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
        [Route("V2/Register-device")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Registerdevice(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["APP_Token"]))
            {
                var AppTokken = col["APP_Token"];

                var PBranchID = 1;

               
                var CMRN = "0";
                var status = 0;
                var msg = "";
                var DeviceType = "";
                var MobileModal = "";
                var MobileOS = "";
                var MobileOsVersion = "";

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    PBranchID = Convert.ToInt32(col["hospital_id"]);

                if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    CMRN = col["patient_reg_no"];


                if (!string.IsNullOrEmpty(col["DeviceType"]))
                    DeviceType = col["DeviceType"];
                if (!string.IsNullOrEmpty(col["MobileModal"]))
                    MobileModal = col["MobileModal"];
                if (!string.IsNullOrEmpty(col["MobileOS"]))
                    MobileOS = col["MobileOS"];
                if (!string.IsNullOrEmpty(col["MobileOsVersion"]))
                    MobileOsVersion = col["MobileOsVersion"];

                CommonDB CDB = new CommonDB();
                CDB.SaveDeviceRegistration(CMRN, PBranchID, AppTokken, DeviceType,MobileModal,MobileOS,MobileOsVersion, ref status, ref msg);


                if (status > 0)
                {
                    _resp.status = status;
                    _resp.msg = msg;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed to save record.";
                }

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);
        }


        [Route("V2/UploadProfileIamge")]
        [HttpPost]
        public IHttpActionResult UploadFiles(string patient_reg_no, int hospital_id)
        {
            CommonDB CDB = new CommonDB();
            ////Fetch the File.
            //HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

            //CDB.UploadDocImg(postedFile, patient_reg_no, hospital_id);


            
            //Create the Directory.
            //string path = HttpContext.Current.Server.MapPath("~/Uploads/");
            string path = ConfigurationManager.AppSettings["USerProfileImage_"].ToString(); // Read Jed if not found in App setting
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //Fetch the File.
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

            //Fetch the File Name.
            //string fileName = HttpContext.Current.Request.Form["fileName"] + Path.GetExtension(postedFile.FileName);
            string fileName = patient_reg_no + "_" + hospital_id.ToString()  + Path.GetExtension(postedFile.FileName);

            //Save the File.
            postedFile.SaveAs(path + fileName);

            var Status = 0;
            var Msg = "";

            // Save to DB the File Name 
            CDB.UpdateImagePath(fileName, patient_reg_no, hospital_id, ref Status, ref Msg);


            var URpfile = new UserProfileURL();
            URpfile.Image_URL = ConfigurationManager.AppSettings["USerProfileImage_URL"].ToString() + fileName.ToString();

            _resp.status = 1;
            _resp.msg = "Success";
            _resp.response = URpfile;
            //Send OK Response to Client.
            return Ok(_resp);
        }

        //[HttpPost()]
        //public string UploadFiles()
        //{
        //    int iUploadedCnt = 0;

        //    // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
        //    string sPath = "";
        //    sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

        //    System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

        //    // CHECK THE FILE COUNT.
        //    for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
        //    {
        //        System.Web.HttpPostedFile hpf = hfc[iCnt];

        //        if (hpf.ContentLength > 0)
        //        {
        //            // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
        //            if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
        //            {
        //                // SAVE THE FILES IN THE FOLDER.
        //                hpf.SaveAs(sPath + Path.GetFileName(hpf.FileName));
        //                iUploadedCnt = iUploadedCnt + 1;
        //            }
        //        }
        //    }

        //    // RETURN A MESSAGE (OPTIONAL).
        //    if (iUploadedCnt > 0)
        //    {
        //        return iUploadedCnt + " Files Uploaded Successfully";
        //    }
        //    else
        //    {
        //        return "Upload Failed";
        //    }
        //}

        [HttpPost]
        [Route("v2/CancelationReason-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetCancelationReason(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]))
                {
                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);



                    var ApiSource = "";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    CommonDB CDB = new CommonDB();
                    var DataResponse= CDB.GetCanelationReason_DT(lang, hospitaId , ApiSource);

                    if (DataResponse != null && DataResponse.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Success";
                        _resp.response = DataResponse;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "Fail";
                    }
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }

            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }


            return Ok(_resp);
        }
        
        [HttpPost]
        [Route("v2/heardAboutUs-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetheardAboutUs(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["hospital_id"]))
                {
                    var lang = col["lang"];
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);


                    CommonDB CDB = new CommonDB();
                    var DataResponse = CDB.GetHeardAboutUs_DT(lang, hospitaId);

                    if (DataResponse != null && DataResponse.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Success";
                        _resp.response = DataResponse;
                    }
                    else
                    {
                        _resp.status = 0;
                        _resp.msg = "Fail";
                    }
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed! Missing Parameters";
                }

            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }


            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/MobileUpdateRequest-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult AddNewMobileUpdateRequest(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_national_id"]) && !string.IsNullOrEmpty(col["patient_name"]) && !string.IsNullOrEmpty(col["patient_phone"]))
            {
                var status = 0;
                var msg = "";

                var BranchID = Convert.ToInt32(col["hospital_id"]);                
                var NationalID = col["patient_national_id"];
                var PatientName = col["patient_name"];
                var PatientPhone = col["patient_phone"];                

                CDB.SaveMobileUpdateRequest(BranchID, NationalID,PatientName, PatientPhone, ref status, ref msg);

                _resp.status = status;
                _resp.msg = msg;

            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        [HttpPost]
        [Route("v2/Serviceitem-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetServiceItemList(FormDataCollection col)
        {
            _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]))
            {                
                var ServiceType = "";
                var ServiceId = 0;
             
                var BranchID = Convert.ToInt32(col["hospital_id"]);
                if (col["Service_Type"] != null)
                    ServiceType = col["Service_Type"];
                
                if (col["Service_Id"] != null)
                    ServiceId = Convert.ToInt32(col["Service_Id"]);

                var dt = CDB.GetServiceItemList(BranchID, ServiceType, ServiceId);

                if (dt != null && dt.Rows.Count > 0 )
                {
                    _resp.status = 1;
                    _resp.msg = "Data Found";
                    _resp.response = dt;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "No data Found";                    
                }
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }



        //[HttpPost]
        //[Route("v2/CMS-USER-list-get")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult GetCMSUserList_ForERP(FormDataCollection col)
        //{
        //    try
        //    {
        //        var EMPID = col["EMP_ID"];
        //        var hospitaId = col["hospital_id"];


        //        CommonDB CDB = new CommonDB();
        //        var DataResponse = CDB.GetCMSUSERLIST_DT(hospitaId, EMPID);

        //        if (DataResponse != null && DataResponse.Rows.Count > 0)
        //        {
        //            _resp.status = 1;
        //            _resp.msg = "Success";
        //            _resp.response = DataResponse;
        //        }
        //        else
        //        {
        //            _resp.status = 0;
        //            _resp.msg = "Fail";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _resp.msg = ex.ToString();
        //        Log.Error(ex);
        //    }


        //    return Ok(_resp);
        //}



        // EXEC[dbo].[Get_ServiceItemsList_SP]

        // @BranchId = 1, 

        //@ServiceType = ‘OP’,

        //@ServiceId = 3

    }
}