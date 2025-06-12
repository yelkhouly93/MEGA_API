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
                    PostResponse_AddPatient ReturnObject;
                    var APiResilts = _loginApiCaller.PatientAddApi_NewDammam(registerPatient , out ReturnObject);

                    if (APiResilts)
					{
                        status = 1;
                        try
						{
                            registrationNo = Convert.ToInt32(ReturnObject.data.mrn);
                        }
                        catch(Exception e)
						{

						}
                        
                    }
                        

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

                    //string branchName = (registerPatient.HospitaId == 1 ? "Jeddah" : (registerPatient.HospitaId == 2 ? "Riyadh" : (registerPatient.HospitaId == 3 ? "Madinah" : (registerPatient.HospitaId == 9 ? "Dammam" : (registerPatient.HospitaId == 8 ? "Hail" : (registerPatient.HospitaId == 101 ? "Beverly" : (registerPatient.HospitaId == 201 ? "Cairo" : "Branch")))))));

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


                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    UserInfo_New userInfo = new UserInfo_New();
                    var IdType = "";
                    var IdValue = "";
                    if (BookinghospitalId ==9) // Booking IN DAMMAM
					{
                        // Call dammam API Function fill list
                        

                        
                        if (hospitalId == 9) // Patient From Damam
						{
                            
                            //var apiCaller = new PatientApiCaller();
                                IdType = "MRN";
                                IdValue = patientMrn.ToString();
                                userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                            IdType = "MOB";
                            IdValue = userInfo.phone.ToString();
                        }
                        else
						{
                            var loginDb = new Login2DB();
                            var userInfo_NEW = loginDb.ValidateLoginUser_New(lang, hospitalId, null, col["patient_reg_no"].ToString(), null, ref errStatus, ref errMessage, "MOBILEAPP");
                            IdType = "MOB";
                            IdValue = userInfo_NEW.phone.ToString();
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
                        var PatientFamilyDT = new DataTable();
                        if (hospitalId == 9)
						{
                             IdType = "MRN";
                             IdValue = patientMrn.ToString();
                             userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);

                            PatientFamilyDT = _patientDB.GetBookingPatientFamily_List(lang, 0, 0, BookinghospitalId, ref errStatus, ref errMessage , userInfo.phone);
                            //public List<login_check_modal> login_check(string Lang, int hospitalId, string pCellNo, string nationalId, int registrationNo, string Source, ref int erStatus, ref string msg, bool IsEncrypt = true)

                        }
                        else
						{
                            PatientFamilyDT = _patientDB.GetBookingPatientFamily_List(lang, hospitalId, patientMrn, BookinghospitalId, ref errStatus, ref errMessage);
                        }


                        // checked in  HIS
                        
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
        [Route("v4/patient-visits-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientVisits_V4(FormDataCollection col)
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


                if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                {
                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var StrpatientMrn = col["patient_reg_no"];
					var PatientData = _UAEApiCaller.GetPatientVisitByApi_NewUAE(lang, hospitalId, StrpatientMrn.ToString(), ref errStatus, ref errMessage);
					if (PatientData != null && PatientData.Count > 0)
					{
						_resp.status = 1;
						_resp.msg = "Record(s) Found";
						_resp.response = PatientData;
					}
                    else
					{
                        _resp.status = 0;
                        _resp.msg = "No Record Found";
                    }
                    //_resp.response = PatientData;
                    return Ok(_resp);
                }
				else if (hospitalId == 9)
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
        [Route("v4/patient-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientGET_v4(FormDataCollection col)
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

                    var errStatus = 1;
                    var errMessage = "";

                    PatientDB patientDb = new PatientDB();
                    // Change as per Mehmode Request
                    //var dt = patientDb.GetPatientDataDT_V2(lang, hospitalId, patientMrn, ref errStatus, ref errMessage);

                    var loginDb = new Login2DB();


                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Sources"]))
                        ApiSource = col["Sources"].ToString();


                    UserInfo_New userInfo = new UserInfo_New();
                    if (hospitalId >= 301 && hospitalId < 400) /*for UAE BRANCHES*/
                    {
                        try
						{
                            ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                            var IdType = "";
                            var IdValue = "";
                            IdType = "MRN";
                            IdValue = col["patient_reg_no"].ToString();
                            userInfo = _UAEApiCaller.GetPatientDataByApi_NewUAE(lang, IdValue, IdType, hospitalId, ref errStatus, ref errMessage);
                            errStatus = 0;
                        }
                        catch
						{
                            errStatus = 1;
                            errMessage = "No Record Found. Please Try Again Later.";
                            return Ok(resp);
                        }
                        
                    }
                    else if (hospitalId == 9)
                    {
                        var apiCaller = new PatientApiCaller();
                        var IdType = "";
                        var IdValue = "";
                        IdType = "MRN";
                        IdValue = patientMrn.ToString();

                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                        if (userInfo != null)
                            errStatus = 0;
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



        [HttpPost]
        [Route("v4/BookingPatientFamily-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetBookingPatientFamilyList_V4(FormDataCollection col)
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


                    LoginApiCaller _loginApiCaller = new LoginApiCaller();
                    UserInfo_New userInfo = new UserInfo_New();
                    var IdType = "";
                    var IdValue = "";
                    IdType = "MRN";
                    IdValue = patientMrn.ToString();
                    //checked for DUBAI Patient 
                    if (hospitalId >= 301 && hospitalId < 400)
					{
                        var UAEMRN = col["patient_reg_no"].ToString();
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                        var _NewData =  _UAEApiCaller.GetPatientFamilyListForBooking_NewUAE(lang, hospitalId, UAEMRN, BookinghospitalId, ref errStatus, ref  errMessage);

                        if (_NewData.Count> 0)
						{
                            resp.status = 1;
                            resp.msg = errMessage;
                            resp.response = _NewData;
                            return Ok(resp);
                        }
                        



                    }
                    else
					{
                        if (BookinghospitalId == 9) // Booking IN DAMMAM
                        {
                            // Call dammam API Function fill list
                            if (hospitalId == 9) // Patient From Damam
                            {
                                //var apiCaller = new PatientApiCaller();
                                userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                                IdType = "MOB";
                                IdValue = userInfo.phone.ToString();
                            }
                            else
                            {
                                var loginDb = new Login2DB();
                                var userInfo_NEW = loginDb.ValidateLoginUser_New(lang, hospitalId, null, col["patient_reg_no"].ToString(), null, ref errStatus, ref errMessage, "MOBILEAPP");
                                IdType = "MOB";
                                IdValue = userInfo_NEW.phone.ToString();
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
                            var PatientFamilyDT = new DataTable();
                            if (hospitalId == 9)
                            {
                                IdType = "MRN";
                                IdValue = patientMrn.ToString();
                                userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);

                                PatientFamilyDT = _patientDB.GetBookingPatientFamily_List(lang, 0, 0, BookinghospitalId, ref errStatus, ref errMessage, userInfo.phone);
                                //public List<login_check_modal> login_check(string Lang, int hospitalId, string pCellNo, string nationalId, int registrationNo, string Source, ref int erStatus, ref string msg, bool IsEncrypt = true)

                            }
                            else
                            {
                                PatientFamilyDT = _patientDB.GetBookingPatientFamily_List(lang, hospitalId, patientMrn, BookinghospitalId, ref errStatus, ref errMessage);
                            }


                            // checked in  HIS

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
        [Route("v4/PatientFamily-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientFamilyList_V4(FormDataCollection col)
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


                    //IdType = "MRN";
                    //IdValue = patientMrn.ToString();
                    //checked for DUBAI Patient 
                    if (hospitalId >= 301 && hospitalId < 400)
                    {
                        var UAEMRN = col["patient_reg_no"].ToString();
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();

                        var _NewData = _UAEApiCaller.GetPatientFamilyList_NewUAE(lang, hospitalId, UAEMRN,  ref errStatus, ref errMessage);

                        if (_NewData.Count > 0)
                        {
                            resp.status = 1;
                            resp.msg = errMessage;
                            resp.response = _NewData;
                            return Ok(resp);
                        }
                    }
                    else
					{
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
        [Route("v4/patient-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewPatientRegistration_V4(FormDataCollection col)
        {
            var registerPatient = new RegisterPatient2();
            List<RegistePatientResponseFailure> registerPatientResFailure;
            RegisterPatientResponse resp = new RegisterPatientResponse();
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_first_name"]) && !string.IsNullOrEmpty(col["patient_last_name"])
                && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_national_id"]) && !string.IsNullOrEmpty(col["patient_birthday"])
                && !string.IsNullOrEmpty(col["patient_gender"]) && !string.IsNullOrEmpty(col["patient_nationality_id"]) && !string.IsNullOrEmpty(col["patient_marital_status_id"])
                && !string.IsNullOrEmpty(col["country_ID"])
                )
            {
                var CountryID = Convert.ToInt32(col["country_ID"]);
                var str_New_MRN = "";

                if (CountryID == 3) /*for UAE*/
                {
                    if (    
                        string.IsNullOrEmpty(col["Current_City"]) 
                        || string.IsNullOrEmpty(col["patient_address"]) 
                        || string.IsNullOrEmpty(col["IdType"])
                        || string.IsNullOrEmpty(col["IdExpiry"])
                        )
					{
                        resp.status = 0;
                        resp.msg = "Failed! Missing Parameters";
                        return Ok(resp);

                    }


                }


                var checkdateTime = Convert.ToDateTime(col["patient_birthday"]);
                var YEARPART = checkdateTime.Year;
                if (YEARPART <= 1900)
                {
                    resp.status = 0;
                    resp.msg = "Birth Date should be in Gregorian : -- ";
                    return Ok(resp);
                }
                // Testing
                //registerPatient.PatientMaritalStatusId = 6;
                try
                {
                    registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);

                    registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);


                    registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
                    registerPatient.PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                    if (CountryID == 2) /*for KSA*/
                        try
						{
                            registerPatient.PatientMaritalStatusId = Convert.ToInt32(col["patient_marital_status_id"]);
                        }
                        catch (Exception e)
						{
                            registerPatient.PatientMaritalStatusId = 6;

                        }
                    else
					{
                        if (col["patient_marital_status_id"].ToString().ToLower() == "married")
                            registerPatient.PatientMaritalStatusId = 2;
                        else
                            registerPatient.PatientMaritalStatusId = 1;
                    }
                        
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

                if (!string.IsNullOrEmpty(col["patient_address"]))
                    registerPatient.PatientAddress = col["patient_address"].ToString();
                else
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
                var registrationNo = "";
                var activationNo = 0;
                var errorType = "";

                var patientDb = new PatientDB();

                if (CountryID == 3) /*for UAE*/
				{
                    var Countey_city = col["Current_City"].ToString();
                    var IDType = Convert.ToInt32(col["IdType"].ToString());
                    var IdExpiry = Convert.ToDateTime(col["IdExpiry"]);

                    var registerPatientUAE = new RegisterPatientUAE();                    
                    registerPatientUAE.CurrentCity = Countey_city;
                    registerPatientUAE.HospitaId = registerPatient.HospitaId;
                    registerPatientUAE.IdExpiry = IdExpiry;
                    registerPatientUAE.IdType = IDType;                    
                    registerPatientUAE.PatientAddress = registerPatient.PatientAddress;
                    registerPatientUAE.PatientBirthday = registerPatient.PatientBirthday;
                    registerPatientUAE.PatientEmail = registerPatient.PatientEmail;
                    registerPatientUAE.PatientFamilyName = registerPatient.PatientFamilyName;
                    registerPatientUAE.PatientFirstName = registerPatient.PatientFirstName;
                    registerPatientUAE.PatientGender = registerPatient.PatientGender;
                    registerPatientUAE.PatientId = registerPatient.PatientId.ToString();
                    registerPatientUAE.PatientLastName = registerPatient.PatientLastName;
                    registerPatientUAE.PatientMaritalStatusId = registerPatient.PatientMaritalStatusId;
                    registerPatientUAE.PatientMiddleName = registerPatient.PatientMiddleName;
                    registerPatientUAE.PatientNationalId = registerPatient.PatientNationalId;
                    registerPatientUAE.PatientNationalityId = registerPatient.PatientNationalityId;
                    registerPatientUAE.PatientPhone = registerPatient.PatientPhone;
                    registerPatientUAE.PatientTitleId = registerPatient.PatientTitleId;
                    registerPatientUAE.skipDuplicateCheck = registerPatient.skipDuplicateCheck;

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    RegistrationPostResponse ReturnObject;
                    var APiResilts = _UAEApiCaller.PatientAddApi_NewUAE(registerPatientUAE, out ReturnObject);

                    if (APiResilts && !string.IsNullOrEmpty (ReturnObject.Mrn))
                    {
                        //status = 1;
                        try
                        {
                            
                            registrationNo = ReturnObject.Mrn;
                            str_New_MRN = registrationNo;

                            _UAEApiCaller.GenerateOTP_V3(registerPatient.HospitaId.ToString(), registerPatient.PatientPhone, str_New_MRN, registerPatient.PatientNationalId, "MobileApp", ref activationNo,ref status,ref msg);

                            LoginApiCaller _loginApiCaller = new LoginApiCaller();
                            _loginApiCaller.GenerateOTP_V3(registerPatient.HospitaId.ToString(), registerPatient.PatientPhone, str_New_MRN, registerPatient.PatientNationalId, "MobileApp", ref activationNo, ref status, ref msg);



                        }
                        catch (Exception e)
                        {

                        }
                        status = 1;
                    }
                    else
					{
                        resp.error_type = errorType;
                        resp.msg = ReturnObject.Message;
                        resp.response = "";
                        resp.status = 0;
                        return Ok(resp);
                    }


                }
                else  // Currently For KSA
				{ 
                    if (registerPatient.HospitaId == 9)
                    {
                        status = 0;
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        PostResponse_AddPatient ReturnObject;
                        var APiResilts = _loginApiCaller.PatientAddApi_NewDammam(registerPatient, out ReturnObject);

                        if (APiResilts)
                        {
                            status = 1;
                            try
                            {
                                registrationNo = ReturnObject.data.mrn;
                                str_New_MRN = registrationNo;
                            }
                            catch (Exception e)
                            {

                            }

                        }


                        if (status != 1)
                        {
                            resp.error_type = errorType;                            
                            resp.msg = ReturnObject.errorMessage;
                            resp.response = "";
                            resp.status = 0;
                        }
                    }
                    else
                    {

                        var NEwIntregistrationNo = 0;
                        registerPatientResFailure = patientDb.RegisterNewPatient_V2(registerPatient, ref status, ref msg, ref errorType, ref successType, ref NEwIntregistrationNo, ref activationNo, Sources);
                        registrationNo = NEwIntregistrationNo.ToString();
                        str_New_MRN = registrationNo;
                        if (status != 1)
                        {
                            resp.error_type = errorType;
                            resp.msg = msg;
                            resp.response = registerPatientResFailure;
                            resp.status = 0;
                        }
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
                        patientDb.Save_PatientPwd_NewForUAE(lang, registerPatient.HospitaId, registrationNo, registerPatient.PatientNationalId, Patient_Pwd, ref PWDstatus, ref PWDmsg);
                    }


                    ////add LOGS
                    //patientDb.AddPatient_Log(registerPatient.HospitaId, registrationNo, registerPatient.PatientNationalId, registerPatient.PatientFirstName, registerPatient.PatientLastName, registerPatient.PatientPhone, Sources);
                    ////add LOGS
                    patientDb.AddPatient_Log_newUAE(registerPatient.HospitaId, registrationNo, registerPatient.PatientNationalId, registerPatient.PatientFirstName, registerPatient.PatientLastName, registerPatient.PatientPhone, Sources);




                    string PName = (registerPatient.PatientFirstName == null ? "" : registerPatient.PatientFirstName + " ") +
                        (registerPatient.PatientMiddleName == null ? "" : registerPatient.PatientMiddleName + " ") +
                        (registerPatient.PatientLastName == null ? "" : registerPatient.PatientLastName);

                    //string branchName = (registerPatient.HospitaId == 1 ? "Jeddah" : (registerPatient.HospitaId == 2 ? "Riyadh" : (registerPatient.HospitaId == 3 ? "Madinah" : (registerPatient.HospitaId == 9 ? "Dammam" : (registerPatient.HospitaId == 8 ? "Hail" : (registerPatient.HospitaId == 101 ? "Beverly" : (registerPatient.HospitaId == 201 ? "Cairo" : "Branch")))))));

                    string branchName = "Sadui German Health";

                    branchName = patientDb.GetBranchName(registerPatient.HospitaId);
                    if (string.IsNullOrEmpty(branchName))
                        branchName = "Sadui German Health";

                    string smsRes = "";
                    var SMSTEXT_Content = "Thank you " + PName + " for opening a file with " + branchName + ".Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo;

                    if (registerPatient.HospitaId > 300 && registerPatient.HospitaId < 400)
					{
                        var CBC = new CommonDB();
                        CBC.InsertUAESMSTABLE(registerPatient.PatientPhone, SMSTEXT_Content);
                    }
                    else if (registerPatient.HospitaId != 201 && !Util.OasisBranches.Contains(registerPatient.HospitaId))
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
                    var tempRegistrarion = 0;
                    tempRegistrarion = Convert.ToInt32(registrationNo);

                    //resp.response = new RegistePatientResponseSuccess() { registration_no = registerPatient.HospitaId == 10 ? 155623 : tempRegistrarion, national_id = registerPatient.PatientNationalId, phone = registerPatient.PatientPhone, name = tempPatient_Name, name_ar = tempPatient_Name };
                    resp.response = new RegistePatientResponseSuccess_NEw_After_UAE() { registration_no = registerPatient.HospitaId == 10 ? "155623" : str_New_MRN, national_id = registerPatient.PatientNationalId, phone = registerPatient.PatientPhone, name = tempPatient_Name, name_ar = tempPatient_Name };
                    
                    resp.smsResponse = smsRes;
                    resp.status = 1;
                    resp.success_type = successType;
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

        [HttpPost]
        [Route("v5/patient-add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewPatientRegistration_V5(FormDataCollection col)
        {
            var registerPatient = new RegisterPatient2();
            List<RegistePatientResponseFailure> registerPatientResFailure;
            
            RegisterPatientResponse resp = new RegisterPatientResponse();
            
            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_first_name"]) && !string.IsNullOrEmpty(col["patient_last_name"])
                && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_national_id"]) && !string.IsNullOrEmpty(col["patient_birthday"])
                && !string.IsNullOrEmpty(col["patient_gender"]) && !string.IsNullOrEmpty(col["patient_nationality_id"]) && !string.IsNullOrEmpty(col["patient_marital_status_id"])
                && !string.IsNullOrEmpty(col["country_ID"])
                )
            {
                var CountryID = Convert.ToInt32(col["country_ID"]);
                var str_New_MRN = "";

                if (CountryID == 3) /*for UAE*/
                {
                    if (
                        string.IsNullOrEmpty(col["Current_City"])
                        || string.IsNullOrEmpty(col["patient_address"])
                        || string.IsNullOrEmpty(col["IdType"])
                        || string.IsNullOrEmpty(col["IdExpiry"])
                        )
                    {
                        resp.status = 0;
                        resp.msg = "Failed! Missing Parameters";
                        return Ok(resp);

                    }
                }


                var checkdateTime = Convert.ToDateTime(col["patient_birthday"]);
                var YEARPART = checkdateTime.Year;
                if (YEARPART <= 1900)
                {
                    resp.status = 0;
                    resp.msg = "Birth Date should be in Gregorian : -- ";
                    return Ok(resp);
                }
                // Testing
                //registerPatient.PatientMaritalStatusId = 6;
                try
                {
                    registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);

                    registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);


                    registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
                    registerPatient.PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                    if (CountryID == 2) /*for KSA*/
                        try
                        {
                            registerPatient.PatientMaritalStatusId = Convert.ToInt32(col["patient_marital_status_id"]);
                        }
                        catch (Exception e)
                        {
                            registerPatient.PatientMaritalStatusId = 6;

                        }
                    else
                    {
                        if (col["patient_marital_status_id"].ToString().ToLower() == "married")
                            registerPatient.PatientMaritalStatusId = 2;
                        else
                            registerPatient.PatientMaritalStatusId = 1;
                    }

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
                
                registerPatient.PatientNationalId = col["patient_national_id"];

                 

                if (!string.IsNullOrEmpty(col["patient_email"]))
                    registerPatient.PatientEmail = col["patient_email"].ToString();
                else
                    registerPatient.PatientEmail = null; // col["patient_email"];


                if (!string.IsNullOrEmpty(col["patient_address"]))
                    registerPatient.PatientAddress = col["patient_address"].ToString();
                else
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
                var registrationNo = "";
                var activationNo = 0;
                var errorType = "";

                var patientDb = new PatientDB();

                if (CountryID == 3) /*for UAE*/
                {
                    var Countey_city = col["Current_City"].ToString();
                    var IDType = Convert.ToInt32(col["IdType"].ToString());
                    var IdExpiry = Convert.ToDateTime(col["IdExpiry"]);

                    var registerPatientUAE = new RegisterPatientUAE();
                    registerPatientUAE.CurrentCity = Countey_city;
                    registerPatientUAE.HospitaId = registerPatient.HospitaId;
                    registerPatientUAE.IdExpiry = IdExpiry;
                    registerPatientUAE.IdType = IDType;
                    registerPatientUAE.PatientAddress = registerPatient.PatientAddress;
                    registerPatientUAE.PatientBirthday = registerPatient.PatientBirthday;
                    registerPatientUAE.PatientEmail = registerPatient.PatientEmail;
                    registerPatientUAE.PatientFamilyName = registerPatient.PatientFamilyName;
                    registerPatientUAE.PatientFirstName = registerPatient.PatientFirstName;
                    registerPatientUAE.PatientGender = registerPatient.PatientGender;
                    registerPatientUAE.PatientId = registerPatient.PatientId.ToString();
                    registerPatientUAE.PatientLastName = registerPatient.PatientLastName;
                    registerPatientUAE.PatientMaritalStatusId = registerPatient.PatientMaritalStatusId;
                    registerPatientUAE.PatientMiddleName = registerPatient.PatientMiddleName;
                    registerPatientUAE.PatientNationalId = registerPatient.PatientNationalId;
                    registerPatientUAE.PatientNationalityId = registerPatient.PatientNationalityId;
                    registerPatientUAE.PatientPhone = registerPatient.PatientPhone;
                    registerPatientUAE.PatientTitleId = registerPatient.PatientTitleId;
                    registerPatientUAE.skipDuplicateCheck = registerPatient.skipDuplicateCheck;

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    RegistrationPostResponse ReturnObject;
                    //var APiResilts = _UAEApiCaller.PatientAddApi_NewUAE(registerPatientUAE, out ReturnObject);
                    //var status = 0;
                    //var msg = "";

              

                    var APiResilts = patientDb.Save_Patient_RegistrationData(registerPatientUAE.HospitaId, registerPatientUAE.PatientFirstName , registerPatientUAE.PatientMiddleName 
                        , registerPatientUAE.PatientLastName, registerPatientUAE.PatientFamilyName, registerPatientUAE.PatientPhone, registerPatientUAE.PatientEmail
                        , registerPatientUAE.PatientNationalId , registerPatientUAE.PatientBirthday.ToString(), registerPatientUAE.PatientGender, registerPatientUAE.PatientAddress
                        , registerPatientUAE.PatientNationalityId, registerPatientUAE.PatientMaritalStatusId , registerPatientUAE.IdExpiry.ToString() , registerPatientUAE.IdType
                        , registerPatientUAE.CurrentCity, CountryID , Patient_Pwd, ref status, ref msg
                        );

                    if (status > 0 )
                    {
                        int tempRegId = status;
                            registrationNo = status.ToString();
                            str_New_MRN = registrationNo;
                            LoginApiCaller _loginApiCaller = new LoginApiCaller();
                            _loginApiCaller.GenerateOTP_V3(registerPatient.HospitaId.ToString(), registerPatient.PatientPhone, str_New_MRN, registerPatient.PatientNationalId, "MobileApp", ref activationNo, ref status, ref msg , CountryID , 6);

                        // Encrpt the Data here For condition if 1 record Found
                        var PhoneNumber = registerPatient.PatientPhone;                        
                        var OTP = activationNo.ToString();
                        string MsgContent = "";
                        MsgContent = ConfigurationManager.AppSettings["SMS_InitalText_UAE"].ToString() + OTP + " ";
                        MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                        //Util.SendTestSMS(PhoneNumber, MsgContent);
                        var CBC = new CommonDB();
                        CBC.InsertUAESMSTABLE(PhoneNumber, MsgContent);
                        
                        var RetunData = new RegistrationData_MW();

                        RetunData.BranchID = registerPatient.HospitaId;
                        RetunData.DataID = tempRegId;
                        RetunData.PatientNationalID = registerPatient.PatientNationalId;
                        RetunData.PatientPhone = registerPatient.PatientPhone;


                        resp.msg = "OTP has been send to Register Mobile Number";
                        resp.response = RetunData;
                        resp.status = 1;
                        return Ok(resp);

                    }
                    else // failed 
                    {
                        resp.error_type = "Failed";
                        resp.msg = "Failed to Add Record, Please Try again";
                        resp.response = "";
                        resp.status = 0;
                        return Ok(resp);
                    }


                }
                else  // Currently For KSA
                {   
                        status = 0;
                        LoginApiCaller _loginApiCaller = new LoginApiCaller();
                        PostResponse_AddPatient ReturnObject;

                        var APiResilts = patientDb.Save_Patient_RegistrationData(registerPatient.HospitaId, registerPatient.PatientFirstName, registerPatient.PatientMiddleName
                        , registerPatient.PatientLastName, registerPatient.PatientFamilyName, registerPatient.PatientPhone, registerPatient.PatientEmail
                        , registerPatient.PatientNationalId, registerPatient.PatientBirthday.ToString(), registerPatient.PatientGender, registerPatient.PatientAddress
                        , registerPatient.PatientNationalityId, registerPatient.PatientMaritalStatusId, "", 0
                        , "", CountryID, Patient_Pwd, ref status, ref msg
                        );
                        
                        if (status > 0)
                        {
                            // SENT OTP 
                            int activationCode = 0, ErrorCode =0;                            
                            _loginApiCaller.GenerateOTP_V3(registerPatient.HospitaId.ToString(), registerPatient.PatientPhone,
                                status.ToString(), registerPatient.PatientNationalId, "Mobile", ref activationCode, ref ErrorCode, ref msg , CountryID , 6);
                            var PhoneNumber = registerPatient.PatientPhone;                            
                            var OTP = activationCode.ToString();
                            string MsgContent = "";                            
                            if (OTP != "6465" && OTP != "1122")
                            {
                                MsgContent = ConfigurationManager.AppSettings["SMS_InitalText"].ToString() + OTP + " ";
                                MsgContent += ConfigurationManager.AppSettings["SMS_Signature"].ToString();
                                Util.SendTestSMS(PhoneNumber, MsgContent);
                            }

                            var RetunData = new RegistrationData_MW();

                            RetunData.BranchID = registerPatient.HospitaId;
                            RetunData.DataID = status;
                            RetunData.PatientNationalID = registerPatient.PatientNationalId;
                            RetunData.PatientPhone = registerPatient.PatientPhone;
                            resp.msg = "OTP has been send to Register Mobile Number";
                            resp.response = RetunData;
                            resp.status = 1;
                            return Ok(resp);

                        }
                        else
						{
                            resp.error_type = "Failed";
                            resp.msg = "Failed to Add Record, Please Try again";
                            resp.response = "";
                            resp.status = 0;
                            return Ok(resp);
                        }                    
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Failed! Missing Parameters";
            }


            return Ok(resp);
        }


        [HttpPost]
        [Route("v4/Patient-BasicData-update")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Update_PatientBasicData(FormDataCollection col)
        {
            var resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_phone"]) && !string.IsNullOrEmpty(col["patient_reg_no"])
                && !string.IsNullOrEmpty(col["patient_DOB"]) && !string.IsNullOrEmpty(col["Sources"]))
            {
                var Status = 0;
                var Msg = "";

                var CMRN = col["patient_reg_no"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);

                if (hospitaId == 9)
                {
                    resp.status = 0;
                    resp.msg = "Sorry this service not available - عذرا هذه الخدمة غير متوفرة";
                    return Ok(resp);
                }

                var PatientPhone = col["patient_phone"];
                var patient_DOB = col["patient_DOB"];
                var patient_Gender = col["patient_Gender"];
                var Marital_Status = col["Marital_Status"];
                var PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
                var EMail = col["patient_Email"];
                var Sources = col["Sources"];

                DateTime PDateOfBirth;
                if (!DateTime.TryParse(patient_DOB, out PDateOfBirth))
                {
                    // handle parse failure
                    resp.status = 0;
                    resp.msg = "Failed : Wrong Date Format";
                    return Ok(resp);
                }
                if (PDateOfBirth > DateTime.Now)
                {
                    resp.status = 0;
                    resp.msg = "Failed : Wrong Date";
                    return Ok(resp);
                }

                var patient_Gender_i = 0;
                if (!string.IsNullOrEmpty(patient_Gender))
                {
                    if (patient_Gender != "F" && patient_Gender != "M")
                    {
                        resp.status = 0;
                        resp.msg = "Failed : Wrong Gender Format";
                        return Ok(resp);
                    }
                    if (patient_Gender == "F")
                        patient_Gender_i = 1;
                    else
                        patient_Gender_i = 2;
                }

                var iMarital_Status = 0;
                if (!string.IsNullOrEmpty(Marital_Status))
                {
                    if (Marital_Status != "S" && Marital_Status != "M")
                    {
                        resp.status = 0;
                        resp.msg = "Failed : Wrong Marital Status Format";
                        return Ok(resp);
                    }

                    if (Marital_Status == "S")
                        iMarital_Status = 6;

                    if (Marital_Status == "M")
                        iMarital_Status = 2;
                }

                if (hospitaId >= 301 && hospitaId < 400)
				{


                    int errStatus = 0;
                    string errMessage = "";

                    ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                    var _NewData = _UAEApiCaller.UpdatePatientBasicData_NewUAE(hospitaId, CMRN, PDateOfBirth.ToString ("dd-MM-yyyy"), Marital_Status, patient_Gender, PatientPhone , EMail ,  ref errStatus, ref errMessage);

                    resp.status = errStatus;
                    resp.msg = errMessage;
                    return Ok(resp);
                }
                else if(hospitaId == 9)

                {
                    resp.status = 0;
                    resp.msg = "Service not available ";
                    return Ok(resp);
                }
                else
				{
                    PatientDB patientDb = new PatientDB();
                    var RegistrationNo = Convert.ToInt32(CMRN);
                    patientDb.UpdatePatientData(hospitaId, RegistrationNo, PDateOfBirth, patient_Gender_i, PatientPhone, iMarital_Status, PatientNationalityId, EMail, Sources, ref Status, ref Msg);
                    resp.status = Status;
                    resp.msg = Msg;
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
        [Route("v4/Patient-pwd-update")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SaveUpdate_PatientPwd_v4(FormDataCollection col)
        {
            var _resp = new GenericResponse();
            CommonDB CDB = new CommonDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["patient_national_id"])
                && !string.IsNullOrEmpty(col["patient_Pwd"]))
            {
                var Status = 0;
                var Msg = "";

                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var CMRN = col["patient_reg_no"];
                var hospitaId = Convert.ToInt32(col["hospital_id"]);


                // Open For Dammam
                //if (hospitaId == 9)
                //{
                //    _resp.status = 0;
                //    if (lang == "EN")
                //        _resp.msg = "Sorry this service not available";
                //    else
                //        _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                //    return Ok(_resp);
                //}

                var PatientNationalalID = col["patient_national_id"];
                var patient_pwd = col["patient_Pwd"];
                PatientDB patientDb = new PatientDB();
                //var RegistrationNo = Convert.ToInt32(CMRN);

                if (!string.IsNullOrEmpty(col["patient_national_id"]))
                {
                    if (hospitaId > 300 && hospitaId < 400)
					{
                        // UAE Case By Pass
					}
                    else if (PatientNationalalID.Length != 10 || !Util.IsDigitsOnly(PatientNationalalID))
                    {
                        _resp.status = 0;
                        _resp.msg = "Wrong Format! Invalid National ID Number.";
                        _resp.error_type = "0";
                        return Ok(_resp);
                    }
                }

                patientDb.Save_PatientPwd_V4(lang, hospitaId, CMRN, PatientNationalalID, patient_pwd, ref Status, ref Msg);
                _resp.status = Status;
                _resp.msg = Msg;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters";
            }

            return Ok(_resp);

        }


        [HttpPost]
        [Route("v2/InPatient-Visit-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetInPatientVisitSign(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["patient_reg_no"])                
                && !string.IsNullOrEmpty(col["hospital_id"])
                )
            {
                var Lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    Lang = col["lang"];

                var hospitaId = Convert.ToInt32(col["hospital_id"]);

                if (hospitaId == 9)
                {
                    resp.status = 0;
                    if (Lang == "EN")
                        resp.msg = "Sorry this service not available";
                    else
                        resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    resp.status = 0;
                    if (Lang == "EN")
                        resp.msg = "Sorry this service not available";
                    else
                        resp.msg = "عذرا هذه الخدمة غير متوفرة";
                    return Ok(resp);
                }


                var registrationNo = col["patient_reg_no"];

                int errStatus = 0;
                string errMessage = "No record Found";
                PatientDB _patientDb = new PatientDB();
                var _DataList = _patientDb.GeT_InPatient_Visit(Lang, registrationNo, hospitaId);

                if (_DataList != null && _DataList.Rows.Count > 0)
                {
                    resp.status = 1;
                    resp.msg = "Record Found";
                    resp.response = _DataList;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "No Record Found";
                    resp.response = null;
                }
            }
            else
            {
                resp.status = 0;
                resp.msg = "Missing Parameter!";
            }
            return Ok(resp);

            //return Ok();
        }


    }


}
