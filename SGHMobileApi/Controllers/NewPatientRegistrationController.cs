using System.Collections.Generic;
using System.Web.Http;
using DataLayer.Model;
using DataLayer.Reception.Business;
using SGHMobileApi.Extension;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using DataLayer.Data;
using System;
using System.Net.Http.Formatting;
using System.Configuration;
using SGHMobileApi.Common;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestClient;
using SmartBookingService.Controllers.ClientApi;


namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class NewPatientRegistrationController : ApiController
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Register new user (without HIS PIN) for SGH mobile App
        /// </summary>
        /// <returns>Return success/fail response with SMS response</returns>
        [HttpPost]
        [Route("api/new-patient-registration")]
        [ResponseType(typeof(List<GenericResponse>))]
        public async Task<IHttpActionResult> PostRegisterNewPatientWithoutPIN(FormDataCollection col)
        {
            RegisterPatient _registerPatient = new RegisterPatient();

            _registerPatient.Lang = col["lang"];
            _registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);
            _registerPatient.PateintUserName = col["patient_user_name"];
            _registerPatient.PatientPassword = col["patient_password"];
            _registerPatient.PatientTitleId = Convert.ToInt32(col["patient_title_id"]);
            _registerPatient.PatientFirstName = col["patient_first_name"];
            _registerPatient.PatientMiddleName = col["patient_middle_name"];
            _registerPatient.PatientLastName = col["patient_last_name"];
            _registerPatient.PatientFamilyName = col["patient_family_name"];
            _registerPatient.PatientPhone = col["patient_phone"];
            _registerPatient.PatientEmail = col["patient_email"];
            _registerPatient.PatientNationalId = col["patient_national_id"];
            _registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);
            _registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
            _registerPatient.PatientAddress = col["patient_address"];
            _registerPatient.PatientNationalityId =  Convert.ToInt32(col["patient_nationality_id"]);
            _registerPatient.PatientMaritalStatusId = Convert.ToInt32(col["patient_marital_status_id"]);
            _registerPatient.SkipDuplicateEmail = col["skip_duplicate_email"];
            _registerPatient.SkipDuplicatePhone = col["skip_duplicate_phone"];
            _registerPatient.ActivationNum = col["activation_num"];
            _registerPatient.SkipSendActivationNum = col["skip_send_activation_num]"];
            
            int status = 0;
            string msg = "";
            string successType = "";
            int registrationNo = 0;
            int activationNo = 0;
            string error_type = "";


            PatientDB _patientDB = new PatientDB();
            var registerPatientResFailure = _patientDB.RegisterNewPatient(_registerPatient, ref status, ref msg, ref error_type, ref successType, ref registrationNo, ref activationNo);

            RegisterPatientResponse resp = new RegisterPatientResponse();

            if (status == 1)
            {
                string smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, activationNo + " is your OTP for SGH Mobile App Registration");
                resp.activation_num = activationNo;
                resp.msg = msg;
                resp.response = null;
                resp.smsResponse = smsRes;
                resp.status = 1;
                resp.success_type = successType;

            }
            else
            {
                resp.error_type = error_type;
                resp.msg = msg;
                resp.response = "";//registerPatientResFailure;
                resp.status = 0;
            }

            return Ok(resp);
        }

        [HttpPost]
        [Route("api/new-patient-registration2")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostNewPatientRegistration(FormDataCollection col)
        {
            RegisterPatient2 _registerPatient = new RegisterPatient2();
            List<RegistePatientResponseFailure> registerPatientResFailure;

            _registerPatient.Lang = col["lang"];
            _registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);
            //_registerPatient.PateintUserName = col["patient_user_name"];
            //_registerPatient.PatientPassword = col["patient_password"];
            _registerPatient.PatientTitleId = 0; // Convert.ToInt32(col["patient_title_id"]);
            _registerPatient.PatientFirstName = col["patient_first_name"];
            _registerPatient.PatientMiddleName = col["patient_middle_name"];
            _registerPatient.PatientLastName = col["patient_last_name"];
            _registerPatient.PatientFamilyName = null; // col["patient_family_name"];
            _registerPatient.PatientPhone = col["patient_phone"];
            _registerPatient.PatientEmail = null; // col["patient_email"];
            _registerPatient.PatientNationalId = col["patient_national_id"];
            _registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);
            _registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
            _registerPatient.PatientAddress = null; // col["patient_address"];
            _registerPatient.PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
            _registerPatient.PatientMaritalStatusId = 0; // Convert.ToInt32(col["patient_marital_status_id"]);
            _registerPatient.skipDuplicateCheck = Convert.ToBoolean(col["skip_duplicate_check"]);
            
            NewPatientRegistrationApiCaller _apiCaller = new NewPatientRegistrationApiCaller();

            int status = 0;
            string msg = "";
            string successType = "";
            int registrationNo = 0;
            int activationNo = 0;
            string error_type = "";

            if (!Util.OasisBranches.Contains(_registerPatient.HospitaId))
            {
                PatientDB _patientDB = new PatientDB();
                registerPatientResFailure = _patientDB.RegisterNewPatient(_registerPatient, ref status, ref msg, ref error_type, ref successType, ref registrationNo, ref activationNo);
            }
            else
            {
                
                registerPatientResFailure = _apiCaller.RegisterNewPatient(_registerPatient, ref status, ref msg, ref error_type, ref successType, ref registrationNo, ref activationNo);
            }
            

            RegisterPatientResponse resp = new RegisterPatientResponse();

            if (status == 1)
            {
                string PName = (_registerPatient.PatientFirstName == null ? "" : _registerPatient.PatientFirstName + " ") + 
                    (_registerPatient.PatientMiddleName == null ? "" : _registerPatient.PatientMiddleName + " ") + 
                    (_registerPatient.PatientLastName == null ? "" : _registerPatient.PatientLastName);
                string branchName = (_registerPatient.HospitaId == 1 ? "Jeddah" : (_registerPatient.HospitaId == 2 ? "Riyadh" : (_registerPatient.HospitaId == 3 ? "Madinah" : (_registerPatient.HospitaId == 10 ? "Dammam" : (_registerPatient.HospitaId == 8 ? "Hail" : (_registerPatient.HospitaId == 101 ? "Beverly" : (_registerPatient.HospitaId == 201 ? "Cairo" : "Branch")))))));
                
                string smsRes = "";

                if (_registerPatient.HospitaId != 201 && !Util.OasisBranches.Contains(_registerPatient.HospitaId))
                    smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with SGH " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                else if (_registerPatient.HospitaId == 201)
                {
                    _registerPatient.PatientPhone = _registerPatient.PatientPhone.Replace("+966", "");
                    smsRes = Util.SendSMS_Cairo(_registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with SGH " + branchName + ". Your new File No. is : " + registrationNo + ".\nPlease use this OTP to login SGH Mobile App: " + activationNo);
                }
                else if (Util.OasisBranches.Contains(_registerPatient.HospitaId))
                    {
                    
                    _registerPatient.PatientPhone = _registerPatient.PatientPhone.Replace("+966", "");
                    smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, "Thank you " + PName + " for opening a file with SGH " + branchName + ". Please contact with hospital to get your File No.");
                }

                //string smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, "Your new File No. for selected SGH Branch is : " + registrationNo + ".\nUse this OTP to login SGH Mobile App: " + activationNo);
                resp.activation_num = _registerPatient.HospitaId == 10 ? 9114 : activationNo;
                resp.msg = _registerPatient.HospitaId == 10 ? "Thank you " + PName + " for opening a file with SGH " + branchName + ". Please contact with hospital to get your File No." : msg;
                resp.response = new RegistePatientResponseSuccess() { registration_no = _registerPatient.HospitaId == 10 ? 155623 :registrationNo, national_id = _registerPatient.PatientNationalId, phone = _registerPatient.PatientPhone, name = null, name_ar = null };
                resp.smsResponse = smsRes;
                resp.status = 1;
                resp.success_type = successType;

                log.Info("\n" + resp.msg);
                log.Info("\n" + resp.activation_num);
                log.Info("\n" + resp.smsResponse);

            }
            else
            {
                resp.error_type = error_type;
                resp.msg = msg;
                resp.response = registerPatientResFailure;
                resp.status = 0;
            }

            return Ok(resp);
        }

        /// <summary>
        /// Register new user (with existing HIS PIN) for SGH mobile App
        /// </summary>
        /// <returns>Return success/fail response with SMS response</returns>
        [HttpPost]
        [Route("api/old-patient-registration")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostRegisterNewPatientWithPIN(FormDataCollection col)
        {
            RegisterPatient _registerPatient = new RegisterPatient();
            RegisterPatientResponse resp = new RegisterPatientResponse();

            _registerPatient.Lang = col["lang"];
            _registerPatient.HospitaId = Convert.ToInt32(col["hospital_id"]);
            _registerPatient.PatientId = Convert.ToInt32(col["patient_id"]);
            _registerPatient.PateintUserName = col["patient_user_name"];
            _registerPatient.PatientPassword = col["patient_password"];
            _registerPatient.PatientTitleId = Convert.ToInt32(col["patient_title_id"]);
            _registerPatient.PatientFirstName = col["patient_first_name"];
            _registerPatient.PatientMiddleName = col["patient_middle_name"];
            _registerPatient.PatientLastName = col["patient_last_name"];
            _registerPatient.PatientFamilyName = col["patient_family_name"];
            _registerPatient.PatientPhone = col["patient_phone"];
            _registerPatient.PatientEmail = col["patient_email"];
            _registerPatient.PatientNationalId = col["patient_national_id"];
            _registerPatient.PatientBirthday = Convert.ToDateTime(col["patient_birthday"]);
            _registerPatient.PatientGender = Convert.ToInt32(col["patient_gender"]);
            _registerPatient.PatientAddress = col["patient_address"];
            _registerPatient.PatientNationalityId = Convert.ToInt32(col["patient_nationality_id"]);
            _registerPatient.PatientMaritalStatusId = Convert.ToInt32(col["patient_marital_status_id"]);
            _registerPatient.ActivationNum = col["activation_num"];
            _registerPatient.SkipSendActivationNum = col["skip_send_activation_num]"];

            int status = 0;
            string msg = "";
            string successType = "";
            int registrationNo = 0;
            int activationNo = 0;
            string error_type = "";

            if (_registerPatient.PatientId != null && _registerPatient.PatientId != 0)
            {
                PatientDB _patientDB = new PatientDB();
                var registerPatientResFailure = _patientDB.RegisterNewPatient(_registerPatient, ref status, ref msg, ref error_type, ref successType, ref registrationNo, ref activationNo);

                if (status == 1)
                {
                    string smsRes = Util.SendTestSMS(_registerPatient.PatientPhone, "Your OTP for SGH Mobile App Registration is " + activationNo);
                    resp.activation_num = activationNo;
                    resp.msg = msg;
                    resp.response = null;
                    resp.smsResponse = smsRes;
                    resp.status = 1;
                    resp.success_type = successType;
                }
                else
                {
                    resp.error_type = error_type;
                    resp.msg = msg;
                    resp.response = registerPatientResFailure;
                    resp.status = 0;
                }

                
            }
            else
            {
                resp.error_type = "invalid_patient_id";
                resp.msg = "Patient Id is invalid. Please provide the valid patient Id";
                resp.response = "";
                resp.status = 0;
                
            }
            return Ok(resp);
            
        }



        /// <summary>
        /// Resend the activation code to patient as SMS
        /// </summary>
        /// <returns>Return success/fail response with SMS response</returns>
        [HttpPost]
        [Route("api/resend-activation-num")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostResendActivationNumber(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitalId = Convert.ToInt32(col["hospital_id"]);
            var patientUserName = col["patient_user_name"];
            var patientPhone = col["patient_phone"];
            
            PatientDB _patientDB = new PatientDB();
            var resendVerification = _patientDB.ResendVerificaitonCode(lang, hospitalId, patientUserName, patientPhone);

            RegisterPatientResponse resp = new RegisterPatientResponse();

            if (resendVerification != null)
            {
                string smsRes = Util.SendTestSMS(resendVerification.PCellNo, "Your OTP for SGH Mobile App Registration is " + resendVerification.ActivationNo);
                resp.msg = "Activation code has been sent to the patient";
                resp.response = "";
                resp.smsResponse = smsRes;
                resp.status = 1;
            }
            else
            {
                resp.error_type = "username_incorrect";
                resp.msg = "Invalid UserName";
                //resp.response = registerPatientResFailure;
                resp.status = 0;
            }

            return Ok(resp);
        }


        /// <summary>
        /// Send the password again to patient's Mobile
        /// </summary>
        /// <returns>Return success/fail response with SMS response</returns>
        [HttpPost]
        [Route("api/patient-forget-password")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostForgetPassword(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitalId = Convert.ToInt32(col["hospital_id"]);
            var patientUserName = col["patient_user_name"];
            var patientPhone = col["patient_phone"];

            PatientDB _patientDB = new PatientDB();
            var resendVerification = _patientDB.ResendVerificaitonCode(lang, hospitalId, patientUserName, patientPhone);

            RegisterPatientResponse resp = new RegisterPatientResponse();

            if (resendVerification != null)
            {
                string smsRes = Util.SendTestSMS(resendVerification.PCellNo, "Your Passwrod for SGH Mobile App Registration is " + resendVerification.AppPassword);
                resp.msg = "Password has been sent to the patient";
                resp.response = "";
                resp.smsResponse = smsRes;
                resp.status = 1;
            }
            else
            {
                resp.error_type = "username_incorrect";
                resp.msg = "Invalid UserName";
                //resp.response = registerPatientResFailure;
                resp.status = 0;
            }

            return Ok(resp);
        }


        /// <summary>
        /// Change the password of the patient for mobile APP
        /// </summary>
        /// <returns>Return success/fail response</returns>
        [HttpPost]
        [Route("api/change-patient-password")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostChangePassword(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitalId = Convert.ToInt32(col["hospital_id"]);
            var patientId = col["patient_id"];
            var patientUserName = col["patient_user_name"];
            var patientOldPassword = col["patient_old_password"];
            var patientNewPassword = col["patient_new_password"];

            int status = 0;
            string msg = "";
            string error_type = "";

            PatientDB _patientDB = new PatientDB();
            _patientDB.ChangePassword(lang, hospitalId, patientId, patientUserName, patientOldPassword, patientNewPassword, ref status, ref msg, ref error_type);

            GenericResponse resp = new GenericResponse();

            if (status > 0)
            {
                resp.msg = msg;
                resp.response = "";
                resp.status = 1;
            }
            else
            {
                resp.error_type = error_type;
                resp.msg = msg;
                //resp.response = registerPatientResFailure;
                resp.status = 0;
            }

            return Ok(resp);
        }

    }
}