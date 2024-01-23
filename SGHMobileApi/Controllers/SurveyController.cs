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
    public class SurveyController : ApiController
    {
        private SurveyDB _SurveyDb = new SurveyDB();
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };


        [HttpPost]
        [Route("v2/Survey-Questions-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetSurveyQuestionsList(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["SurveyId"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];

                var SurveyId = Convert.ToInt32(col["SurveyId"]);


                var ServiceID = 0;
                if (!string.IsNullOrEmpty(col["ServiceId"]))
                    ServiceID = Convert.ToInt32(col["ServiceId"]);




                var allData = _SurveyDb.GetSurveyQuestions(lang, SurveyId, ServiceID);


                if (allData != null && allData.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Data Found Success";
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
        [Route("v2/Survey-Answer-Save")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult SaveLegalAgreement(FormDataCollection col)
        {
            _resp = new GenericResponse();

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["QAnswersQuery"])
                && !string.IsNullOrEmpty(col["patient_reg_no"]) && !string.IsNullOrEmpty(col["Source"])
                && !string.IsNullOrEmpty(col["SurveyId"]) && !string.IsNullOrEmpty(col["ServiceId"])
                )
            {

                
    //            try
				//{
                    var AnswerQuery = col["QAnswersQuery"];
                    var Source = col["Source"];
                    var ActionId = Convert.ToInt32(col["ServiceId"]);
                    var hospitalId = Convert.ToInt32(col["hospital_id"]);
                    var MRN = Convert.ToInt32(col["patient_reg_no"]);
                    var SurveyID = Convert.ToInt32(col["SurveyId"]);
                    var ServiceId = Convert.ToInt32(col["ServiceId"]);
                    var status = 0;
                    var msg = "";

                    _SurveyDb.SaveSurveyQuestionAnswers(hospitalId, MRN, AnswerQuery, Source, SurveyID, ServiceId, ref status, ref msg);
                    
                    _resp.status = status;
                    _resp.msg = msg;
                //}
    //            catch(Exception ex)
				//{
    //                _resp.status = 0;
    //                _resp.msg = "Failed : Please Provide the Valid Parameters";
    //            }
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
