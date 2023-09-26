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
    public class AgrementController : ApiController
    {
        private AgrementDB _AggreementDb = new AgrementDB();
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };


        [HttpPost]
        [Route("v2/Agreement-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetLegalAgreement(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["AgreementName"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                var AggrementName = "";
                if (!string.IsNullOrEmpty(col["AgreementName"]))
                    AggrementName = col["AgreementName"];

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                

                var allData = _AggreementDb.GetAgreementContent(lang, hospitalId, AggrementName);


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
        [Route("v2/Agreement-Acceptance-Save")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SaveLegalAgreement(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["AgreementName"]) 
                && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Source"]))
            {
                var AggrementName = "";
                if (!string.IsNullOrEmpty(col["AgreementName"]))
                    AggrementName = col["AgreementName"];

                var ActionId = 0;
                if (!string.IsNullOrEmpty(col["ActionId"]))
                    ActionId = Convert.ToInt32 (col["ActionId"]);

                var hospitalId = Convert.ToInt32(col["hospital_id"]);
                var MRN = Convert.ToInt32(col["patient_reg_no"]);
                var Source = col["Source"]; 



                var status = 0;
                var msg = "";

                _AggreementDb.SaveAgrrementAcceptance(hospitalId,AggrementName,MRN, ActionId,Source, ref status, ref msg);

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



    }
}
