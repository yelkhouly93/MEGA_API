using DataLayer.Data;
using DataLayer.Data.DoctorAPI;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using SmartBookingService.Controllers.ClientApi;
using SGHMobileApi.Common;
using System.Configuration;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class DocClinincController : ApiController
    {
        private GenericResponse _resp = new GenericResponse() { status = 0 };
        private DoctorAPIDB __MainDb = new DoctorAPIDB();

        [HttpPost]
        [Route("docapp/Validate-Doctor")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult ValidateDoctor(FormDataCollection col)
		{
            if (!string.IsNullOrEmpty(col["Hospital_ID"]) && !string.IsNullOrEmpty(col["Employee_ID"]) 
                && col["National_ID"] != "0" 
                )
			{
                var EmployeeID = "";
                var BranchID = 0;
                var NationalID = "";
                try
				{
                    BranchID = Convert.ToInt32(col["Hospital_ID"]);
                    NationalID = col["National_ID"];
                    EmployeeID = col["Employee_ID"];
                }
                catch(Exception ex)
				{
                    _resp.status = 0;
                    _resp.msg = "Wrong Format ! Please check Hospital ID OR National ID should be Integer.";
                    _resp.error_type = "0";
                    return Ok(_resp);
                }
                
                if (NationalID.Length != 10 || (NationalID[0].ToString() != "1" && NationalID[0].ToString() != "2"))
				{
                    _resp.status = 0;
                    _resp.msg = "Wrong National ID Format.";
                    _resp.error_type = "0";
                    return Ok(_resp);
                }
                
                var lang = "EN";

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                int errStatus = 0;
                string errMessage = "";

                var _AllDateTable = __MainDb.Validate_DotorInfo(lang,BranchID.ToString(),EmployeeID,NationalID,ApiSource,ref errStatus ,ref errMessage);

                if (errStatus != 0 && _AllDateTable != null)
				{
                    _resp.status = 1;
                    _resp.msg = errMessage;
                    _resp.response = _AllDateTable;
                }
                else
				{
                    _resp.status = 0;
                    _resp.msg = "Wrong information OR Doctor not found.";
                }


            }
            else
            {
                _resp.msg = "Failed! Missing Parameter";
            }
            return Ok(_resp);

        }

        [HttpPost]
        [Route("docapp/Doctor-Information-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetDoctorInformatioin(FormDataCollection col)
        {
            if (!string.IsNullOrEmpty(col["Hospital_ID"]) && !string.IsNullOrEmpty(col["Employee_ID"]))
            {
                var EmployeeID = "";
                var BranchID = 0;                
                try
                {
                    BranchID = Convert.ToInt32(col["Hospital_ID"]);                    
                    EmployeeID = col["Employee_ID"];
                }
                catch (Exception ex)
                {
                    _resp.status = 0;
                    _resp.msg = "Wrong Format ! Please check Hospital ID OR National ID should be Integer.";
                    _resp.error_type = "0";
                    return Ok(_resp);
                }
                
                var lang = "EN";

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                var _AllDateTable = __MainDb.GetDoctorInformation(lang, BranchID.ToString(), EmployeeID, ApiSource);

                if (_AllDateTable != null)
                {
                    _resp.status = 1;
                    _resp.msg = "Record Found";
                    _resp.response = _AllDateTable;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Wrong information OR Doctor not found.";
                }
            }
            else
            {
                _resp.msg = "Failed! Missing Parameter";
            }
            return Ok(_resp);

        }

        
    }
}
