using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class LoginController : ApiController
    {
        /// <summary>
        /// Validate the patient login credentials.
        /// </summary>
        /// <returns>Return the user info on successfull validation of patient login credentials</returns>
        [HttpPost]
        [Route("api/patient-login")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult Post(FormDataCollection col)
        {
            var lang = col["lang"];
            var hospitaId = Convert.ToInt32(col["hospital_id"]);
            var userName = col["patient_user_name"];
            var password = col["patient_password"];

            int errStatus = 0;
            string errMessage = "";

            LoginDB _loginDB = new LoginDB();
            UserInfo _userInfo = _loginDB.ValidateLoginUser(lang, hospitaId, userName, password, ref errStatus, ref errMessage);


            GenericResponse resp = new GenericResponse();

            if (errStatus == 0 && _userInfo != null)
            {
                resp.status = 1;
                resp.msg = errMessage;
                resp.response = _userInfo;


            }
            else
            {
                resp.status = 0;
                resp.msg = errMessage;
                resp.response = null;
                resp.error_type = errStatus.ToString();

            }

            return Ok(resp);
        }


        [HttpGet]
        [Route("check-token")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PostCheckLogin(FormDataCollection col)
        {
            return Ok();
        }
    }
}