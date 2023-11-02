using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SGHMobileApi.Extension;
using DataLayer.Model;
using DataLayer.Data;
using SGHMobileApi.Common;
using System.Web.Http.Description;
using System.Net.Http.Formatting;
using SmartBookingService.Controllers.ClientApi;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class PatientListing_Family_V3_Controller : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();

        [HttpPost]
        [Route("v3/patientFamilyProfile-List-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetPatientFamilyProfileList(FormDataCollection col)
        {
            _resp = new GenericResponse();
            var patientDb = new PatientDB();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Source"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                try
                {
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var PatientMRN = Convert.ToInt32(col["patient_reg_no"]);

                    var errStatus = 0;
                    var errMessage = "";

                    var ApiSource = "MobileApp";
                    if (!string.IsNullOrEmpty(col["Source"]))
                        ApiSource = col["Source"].ToString();

                    UserInfo_New userInfo = new UserInfo_New();
                    var PatientListFromDammam = new List<PatientFamilyList>();
                    var PatientListFromHIS = new List<PatientFamilyList>();
                    var IdType = "";
                    var IdValue = "";
                    LoginApiCaller _loginApiCaller = new LoginApiCaller();

                    if (hospitalId == 9)
					{
                        
                        IdType = "MRN";
                        IdValue = PatientMRN.ToString();
                        
                        userInfo = _loginApiCaller.GetPatientDataByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);


                        IdType = "MOB";
                        IdValue = userInfo.phone.ToString();
                        PatientListFromDammam = _loginApiCaller.GetPatientListsByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);

                        PatientListFromHIS = patientDb.GetPatientFamilyProfile_List_V3_ByMobile(lang, PatientListFromDammam[0].PCellno, ApiSource, ref errStatus, ref errMessage);

                    }
                    else
					{
                        PatientListFromHIS = patientDb.GetPatientFamilyProfile_List_V3(lang, hospitalId, PatientMRN, ApiSource, ref errStatus, ref errMessage);
                        if (PatientListFromHIS.Count > 0)
                            IdValue = PatientListFromHIS[0].PCellno;
                        else
						{
                            var loginDb = new Login2DB();
                            userInfo = loginDb.ValidateLoginUser_New(lang, hospitalId, null, PatientMRN.ToString(), null, ref errStatus, ref errMessage, ApiSource);
                            IdValue = userInfo.phone;
                        }

                        IdType = "MOB";                        
                        PatientListFromDammam = _loginApiCaller.GetPatientListsByApi_NewDam(lang, IdValue, IdType, ref errStatus, ref errMessage);
                    }

                    PatientListFromHIS.AddRange(PatientListFromDammam);

                    _resp.status = 0;
                    _resp.msg = "No Record Found";

                    if (PatientListFromHIS.Count > 0)
					{
                        _resp.status = 1;
                        _resp.msg = "Record Found";
                        _resp.response = PatientListFromHIS;
                    }
                    

                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
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
