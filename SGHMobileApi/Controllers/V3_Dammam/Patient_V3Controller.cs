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
    public class Patient_V3Controller : ApiController
    {
        [HttpPost]
        [Route("v3/patient-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientGET_v3(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]))
                {

                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];

                    var hospitalId = Convert.ToInt32(col["hospital_id"]);

                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                    var errStatus = 0;
                    var errMessage = "";

                    PatientDB patientDb = new PatientDB();
                    // Change as per Mehmode Request
                    //var dt = patientDb.GetPatientDataDT_V2(lang, hospitalId, patientMrn, ref errStatus, ref errMessage);

                    var loginDb = new Login2DB();


                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    UserInfo_New userInfo = new UserInfo_New();

                    if (hospitalId == 9)
                    {
                        var apiCaller = new PatientApiCaller();
                        var IdType = "";
                        var IdValue = "";
                        IdType = "MRN";
                        IdValue = patientMrn.ToString();
                        
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                    }
                    else
                    {
                        userInfo = loginDb.ValidateLoginUser_New(lang, hospitalId, null, col["patient_reg_no"].ToString(), null, ref errStatus, ref errMessage, ApiSource);
                    }




                    if (errStatus == 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        //resp.response = dt;
                        resp.response = userInfo;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        resp.error_type = errStatus.ToString();
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }


                return Ok(resp);
            }
            catch (Exception ex)
            {

                //Log.Error(ex);
            }

            return Ok();
        }

        /// <summary>
        /// Get Patient Diagnosis against patient registration No.
        /// </summary>
        /// <returns>Return Patient Diagnosis against patient registration No</returns>
        [HttpPost]
        [Route("v3/patient-prescription-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientPrescription(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = col["lang"];
                var hospitaId = 0;
                var registrationNo = 0;
                int errStatus = 0;
                string errMessage = "No record Found";
                PatientDB _patientDB = new PatientDB();


                var EpisodeId = 0;
                var EpisodeType = "OP";
                try
                {
                    hospitaId = Convert.ToInt32(col["hospital_id"]);
                    registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                    if (!string.IsNullOrEmpty(col["Episode_Id"]))
                        EpisodeId = Convert.ToInt32(col["Episode_Id"]);

                    if (!string.IsNullOrEmpty(col["Episode_Type"]))
                        EpisodeType = col["Episode_Type"].ToString();

                }
                catch (Exception e)
                {
                    resp.status = 0;
                    resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(resp);
                }

                if (EpisodeType.ToUpper() != "OP" && EpisodeType.ToUpper() != "IP")
                {
                    resp.status = 0;
                    resp.msg = "WRONG Episode Type";
                    return Ok(resp);
                }



                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                if (hospitaId==9 && EpisodeId > 0 )
				{
                    resp.status = 0;
                    resp.msg = "NO Data Found";
                    return Ok(resp);
                }

                var _allPatientMedDT = new DataTable();
                if (hospitaId == 9)
				{

                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var userPerscription = _loginApiCaller.GetPatientPerscriptionByApi_NewDam(lang, registrationNo.ToString(), ref errStatus, ref errMessage);
                    if (userPerscription.Count > 0)
					{
                        resp.status = 1;
                        resp.msg = "Record Found";
                        resp.response = userPerscription;
                    }

                }
                else
				{
                    _allPatientMedDT = _patientDB.GetPatientPrescriptionDT(lang, hospitaId, registrationNo, ref errStatus, ref errMessage, ApiSource, EpisodeId, EpisodeType);
                    if (_allPatientMedDT != null && _allPatientMedDT.Rows.Count > 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = _allPatientMedDT;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                    }
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
        [Route("v3/patient-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewPatientRegistration_V2(FormDataCollection col)
        {
            var registerPatient = new RegisterPatient2();
            List<RegistePatientResponseFailure> registerPatientResFailure;
            RegisterPatientResponse resp = new RegisterPatientResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_first_name"]) && !string.IsNullOrEmpty(col["patient_last_name"])
                && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_national_id"]) && !string.IsNullOrEmpty(col["patient_birthday"])
                && !string.IsNullOrEmpty(col["patient_gender"]) && !string.IsNullOrEmpty(col["patient_nationality_id"]) && !string.IsNullOrEmpty(col["patient_marital_status_id"]))
            {

                var checkdateTime = Convert.ToDateTime(col["patient_birthday"]);
                var YEARPART = checkdateTime.Year;
                if (YEARPART <= 1900)
                {
                    resp.status = 0;
                    resp.msg = "Birth Date should be in Gregorian : -- ";
                    return Ok(resp);
                }


                try
                {
                    registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);

                    registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);


                    registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
                    registerPatient.PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                    registerPatient.PatientMaritalStatusId = Convert.ToInt32(col["patient_marital_status_id"]);
                }
                catch (Exception e)
                {
                    resp.status = 0;
                    resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(resp);
                }


                //var Sources = "MobileApp";
                var Sources = ConfigurationManager.AppSettings["API_SOURCE_KEY"].ToString();
                if (!string.IsNullOrEmpty(col["Sources"]))
                    Sources = col["Sources"];


                registerPatient.Lang = col["lang"];

                if (!string.IsNullOrEmpty(col["patient_middle_name"]))
                {
                    registerPatient.PatientMiddleName = col["patient_middle_name"];
                }
                else
                    registerPatient.PatientMiddleName = "";




                registerPatient.PatientTitleId = 0; // Convert.ToInt32(col["patient_title_id"]);
                registerPatient.PatientFirstName = col["patient_first_name"];

                registerPatient.PatientLastName = col["patient_last_name"];
                registerPatient.PatientFamilyName = null; // col["patient_family_name"];
                registerPatient.PatientPhone = col["patient_phone"];
                registerPatient.PatientEmail = null; // col["patient_email"];
                registerPatient.PatientNationalId = col["patient_national_id"];


                registerPatient.PatientAddress = null; // col["patient_address"];

                //registerPatient.PatientMaritalStatusId = 0; // Convert.ToInt32(col["patient_marital_status_id"]);

                registerPatient.skipDuplicateCheck = false;
                //registerPatient.PatientNationalityId = Convert.ToInt32(col["Nationality_id"]);

                var Patient_Pwd = "";

                if (!string.IsNullOrEmpty(col["Patient_Pwd"]))
                    Patient_Pwd = col["Patient_Pwd"];

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var status = 0;
                var msg = "";
                var successType = "";
                var registrationNo = 0;
                var activationNo = 0;
                var errorType = "";

                var patientDb = new PatientDB();


                if (registerPatient.HospitaId == 9)
                {
                    status = 0;
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    PostResponse ReturnObject;
                    var APiResilts = _loginApiCaller.PatientAddApi_NewDammam(registerPatient , out ReturnObject);

                    if (APiResilts)
                        status = 1;

                    if (status != 1)
                    {
                        resp.error_type = errorType;
                        //resp.msg = "Failed - Patient File Not created. Please try Again.";
                        resp.msg = ReturnObject.errorMessage;
                        resp.response = "";
                        resp.status = 0;
                    }
                }
                else
				{
                    registerPatientResFailure = patientDb.RegisterNewPatient_V2(registerPatient, ref status, ref msg, ref errorType, ref successType, ref registrationNo, ref activationNo, Sources);
                    if (status != 1 )
					{
                        resp.error_type = errorType;
                        resp.msg = msg;
                        resp.response = registerPatientResFailure;
                        resp.status = 0;
                    }
                }

                





                if (status == 1)
                {
                    /*******
                     ******* New Changes 06-July-2022 
                     ******* Save Patient Password on Successful registration
                     ********/
                    var PWDstatus = 0;
                    var PWDmsg = "";
                    if (Patient_Pwd != "")
                    {
                        patientDb.Save_PatientPwd(lang, registerPatient.HospitaId, registrationNo, registerPatient.PatientNationalId, Patient_Pwd, ref PWDstatus, ref PWDmsg);
                    }


                    //add LOGS
                    patientDb.AddPatient_Log(registerPatient.HospitaId, registrationNo, registerPatient.PatientNationalId, registerPatient.PatientFirstName, registerPatient.PatientLastName, registerPatient.PatientPhone, Sources);



                    string PName = (registerPatient.PatientFirstName == null ? "" : registerPatient.PatientFirstName + " ") +
                        (registerPatient.PatientMiddleName == null ? "" : registerPatient.PatientMiddleName + " ") +
                        (registerPatient.PatientLastName == null ? "" : registerPatient.PatientLastName);

                    //string branchName = (registerPatient.HospitaId == 1 ? "Jeddah" : (registerPatient.HospitaId == 2 ? "Riyadh" : (registerPatient.HospitaId == 3 ? "Madinah" : (registerPatient.HospitaId == 10 ? "Dammam" : (registerPatient.HospitaId == 8 ? "Hail" : (registerPatient.HospitaId == 101 ? "Beverly" : (registerPatient.HospitaId == 201 ? "Cairo" : "Branch")))))));

                    string branchName = "Sadui German Health";

                    branchName = patientDb.GetBranchName(registerPatient.HospitaId);
                    if (string.IsNullOrEmpty(branchName))
                        branchName = "Sadui German Health";

                    string smsRes = "";

                    if (registerPatient.HospitaId != 201 && !Util.OasisBranches.Contains(registerPatient.HospitaId))
                        smsRes = Util.SendTestSMS(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                    else if (registerPatient.HospitaId == 201)
                    {
                        registerPatient.PatientPhone = registerPatient.PatientPhone.Replace("+966", "");
                        smsRes = Util.SendSMS_Cairo(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                    }
                    else if (Util.OasisBranches.Contains(registerPatient.HospitaId))
                    {

                        registerPatient.PatientPhone = registerPatient.PatientPhone.Replace("+966", "");
                        smsRes = Util.SendTestSMS(registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with  " + branchName + ". Please contact with hospital to get your File No.");
                    }

                    //string smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, "Your new File No. for selected SGH Branch is : " + registrationNo + ".\nUse this OTP to login SGH Mobile App: " + activationNo);
                    var tempPatient_Name = registerPatient.PatientFirstName + ' ' + registerPatient.PatientMiddleName + ' ' + registerPatient.PatientLastName;
                    resp.activation_num = registerPatient.HospitaId == 10 ? 9114 : activationNo;
                    resp.msg = registerPatient.HospitaId == 10 ? "Thank you " + PName + " for opening a file with  " + branchName + ". Please contact with hospital to get your File No." : msg;
                    resp.response = new RegistePatientResponseSuccess() { registration_no = registerPatient.HospitaId == 10 ? 155623 : registrationNo, national_id = registerPatient.PatientNationalId, phone = registerPatient.PatientPhone, name = tempPatient_Name, name_ar = tempPatient_Name };
                    resp.smsResponse = smsRes;
                    resp.status = 1;
                    resp.success_type = successType;

                    //Log.Info("\n" + resp.msg);
                    //Log.Info("\n" + resp.activation_num);
                    //Log.Info("\n" + resp.smsResponse);

                }
                else
                {
                    //resp.error_type = errorType;
                    //resp.msg = msg;
                    //resp.response = registerPatientResFailure;
                    //resp.status = 0;
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Failed! Missing Parameters";
            }


            return Ok(resp);
        }

        //[HttpPost]
        //[Route("v3/Patient-LatestPrescription-get")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult PatientLatestPrescriptions(FormDataCollection col)
        //{
        //    var resp = new GenericResponse();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Order_ID"]))
        //        {
        //            var hospitalId = Convert.ToInt32(col["hospital_id"]);
        //            var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
        //            var Lang = "EN";
        //            if (!string.IsNullOrEmpty(col["Lang"]))
        //                Lang = col["Lang"];

        //            // Not the Order_ID  prameter = Visit Id 
        //            var OrderID = Convert.ToInt32(col["Order_ID"]);

        //            var VisitID = "Order_ID=" + OrderID.ToString() + "&MRN=" + patientMrn + "&BranchId=" + hospitalId + "&Lang=" + Lang;

        //            var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
        //            var FinalURL = ConfigurationManager.AppSettings["MEDPerscriptionURLPDF"].ToString() + ParmEnc;


        //            var FinalURLNEW = Util.ConvertURL_to_PDF(FinalURL, OrderID.ToString());

        //            MedicalPrescriptions ObjReturn = new MedicalPrescriptions();

        //            ObjReturn.PdfURL = FinalURLNEW;

        //            if (ObjReturn != null)
        //            {
        //                resp.status = 1;
        //                resp.msg = "Record(s) Found";
        //                resp.response = ObjReturn;
        //            }
        //            else
        //            {
        //                resp.status = 0;
        //                resp.msg = "No Record Found";
        //            }
        //        }
        //        else
        //        {
        //            resp.status = 0;
        //            resp.msg = "Missing Parameter!";
        //        }
        //        return Ok(resp);
        //    }
        //    catch (Exception ex)
        //    {
        //        var test = ex;
        //    }

        //    return Ok();
        //}



        [HttpPost]
        [Route("v3/BookingPatientFamily-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetBookingPatientFamilyList(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";



                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Booking_hospital_id"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];



                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var BookinghospitalId = Convert.ToInt32(col["Booking_hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    var errStatus = 0;
                    var errMessage = "";

                    PatientDB _patientDB = new PatientDB();

                    resp.status = 0;
                    resp.msg = "NO Record Found";

                    if (BookinghospitalId ==9) // Booking IN DAMMAM
					{
                        // Call dammam API Function fill list
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        UserInfo_New userInfo = new UserInfo_New();

                        var IdType = "";
                        var IdValue = "";
                        if (hospitalId == 9) // Patient From Damam
						{
                            
                            //var apiCaller = new PatientApiCaller();
                                IdType = "MRN";
                                IdValue = patientMrn.ToString();
                                userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                            IdType = "MOB";
                            IdValue = userInfo.phone.ToString();
                        }
                                                    
                        List<login_check_modal> _damuserInfo = new List<login_check_modal>();
                       _damuserInfo = _loginApiCaller.ValidateLoginUserByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);

                        if (_damuserInfo.Count == 0)
                        {
                            resp.status = 0;
                            resp.msg = errMessage;
                            resp.error_type = "0";

                        }
                        else
                        {
                            resp.status = 1;
                            resp.msg = errMessage;
                            resp.response = _damuserInfo;
                        }

                    }
                    else
					{
                        // checked in  HIS
                        var PatientFamilyDT = _patientDB.GetBookingPatientFamily_List(lang, hospitalId, patientMrn, BookinghospitalId, ref errStatus, ref errMessage);
                        if (errStatus == 1)
                        {
                            resp.status = 0;
                            resp.msg = errMessage;
                            resp.error_type = "0";

                        }
                        else
                        {
                            resp.status = 1;
                            resp.msg = errMessage;
                            resp.response = PatientFamilyDT;
                        }
                    }
                    
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }


                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //log.Error(ex);
            }

            return Ok();
        }

        [HttpPost]
        [Route("v3/PatientFamily-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientFamilyList(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                var lang = "EN";
                var hospitalId = 0;

                // For Now Only Mobily Number 
                //if (!string.IsNullOrEmpty(col["patient_reg_no"]) || !string.IsNullOrEmpty(col["patient_national_id"]) || !string.IsNullOrEmpty(col["patient_phone"]))
                if (!string.IsNullOrEmpty(col["patient_phone"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];


                    if (string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = 0;
                    else
                        hospitalId = Convert.ToInt32(col["hospital_id"]);


                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);

                    var PatientNationId = col["patient_national_id"];

                    var PCell = col["patient_phone"];


                    var loginApiCaller = new LoginApiCaller();
                    var errStatus = 0;
                    var errMessage = "";
                    //var OTP = "";

                    if (!string.IsNullOrEmpty(col["patient_reg_no"]))
                    {
                        if (string.IsNullOrEmpty(col["hospital_id"]))
                        {

                            resp.status = 0;
                            resp.msg = "Missing Prameter. With MRN Please Provide Hospital ID";
                            resp.error_type = errStatus.ToString();
                            return Ok(resp);

                        }
                    }

                    if (!string.IsNullOrEmpty(col["patient_national_id"]))
                    {
                        if (PatientNationId.Length != 10)
                        {
                            resp.status = 0;
                            resp.msg = "Wrong input! Invalid National ID";
                            resp.error_type = errStatus.ToString();
                            return Ok(resp);
                        }
                    }
                    PatientDB _patientDB = new PatientDB();

                    var PatientFamilyDT = _patientDB.GetPatientFamily_List(lang, hospitalId, PCell, PatientNationId, patientMrn, ref errStatus, ref errMessage);

                    if (errStatus == 0)
                    {
                        resp.status = 1;
                        resp.msg = errMessage;
                        resp.response = PatientFamilyDT;
                    }
                    else
                    {
                        resp.status = 0;
                        resp.msg = errMessage;
                        resp.error_type = errStatus.ToString();
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }


                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //log.Error(ex);
            }

            return Ok();
        }




        [HttpPost]
        [Route("v3/patient-visits-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientVisits(FormDataCollection col)
        {
            var _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                
                var registrationNo = Convert.ToInt32(col["patient_reg_no"]);

                int errStatus = 0;
                string errMessage = "";


                if (hospitalId == 9)
                {
                    _resp.status = 0;                    
                    _resp.msg = "No Record Found";                    
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    var PatientData = _loginApiCaller.GetPatientVisitByApi_NewDam(lang, registrationNo.ToString(), ref errStatus, ref errMessage);

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

                    var allAppointmnetList = patientDb.GetPatientVisits(lang, hospitalId, registrationNo);


                    if (allAppointmnetList != null && allAppointmnetList.Rows.Count > 0)
                    {
                        _resp.status = 1;
                        _resp.msg = "Record(s) Found";
                        _resp.response = allAppointmnetList;

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
        [Route("v3/Patient-LatestPrescription-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientLatestPrescriptions(FormDataCollection col)
        {
            var resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["Order_ID"]))
                {
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var patientMrn = Convert.ToInt32(col["patient_reg_no"]);
                    
                    if (hospitalId == 9)
					{
                        resp.status = 0;
                        resp.msg = "No Record Found";
                        return Ok(resp);
                    }
                    
                    var Lang = "EN";
                    if (!string.IsNullOrEmpty(col["Lang"]))
                        Lang = col["Lang"];

                    // Not the Order_ID  prameter = Visit Id 
                    var OrderID = Convert.ToInt32(col["Order_ID"]);

                    var VisitID = "Order_ID=" + OrderID.ToString() + "&MRN=" + patientMrn + "&BranchId=" + hospitalId + "&Lang=" + Lang;

                    var ParmEnc = TripleDESImp.TripleDesEncrypt(VisitID.ToString());
                    var FinalURL = ConfigurationManager.AppSettings["MEDPerscriptionURLPDF"].ToString() + ParmEnc;


                    var FinalURLNEW = Util.ConvertURL_to_PDF(FinalURL, OrderID.ToString());

                    MedicalPrescriptions ObjReturn = new MedicalPrescriptions();

                    ObjReturn.PdfURL = FinalURLNEW;

                    if (ObjReturn != null)
                    {
                        resp.status = 1;
                        resp.msg = "Record(s) Found";
                        resp.response = ObjReturn;
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
            catch (Exception ex)
            {
                var test = ex;
            }

            return Ok();
        }


        [HttpPost]
        [Route("v3/Patient-InsuranceInfo-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientInsuranceInfo(FormDataCollection col)
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
                    PatientDB _patientDB = new PatientDB();
                    var PatientInsuranceDT = _patientDB.GetPatientInsuranceInfo_DT(hospitalId, patientMrn, ref errStatus, ref errMessage);
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


    }


}
