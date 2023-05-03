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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using SGHMobileApi.Common;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class PatientLabAndRADResultController : ApiController
    {
        /// <summary>
        /// Get Patient Diagnosis against patient registration No.
        /// </summary>
        /// <returns>Return Patient Diagnosis against patient registration No</returns>
        //[HttpPost]
        //[Route("api/get-patientlabnradresults")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult Post(FormDataCollection col)
        //{
        //    var lang = col["lang"];
        //    var hospitaId = Convert.ToInt32(col["hospital_id"]);
        //    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
            
        //    int errStatus = 0;
        //    string errMessage = "";

        //    LABResultDB _labResultDB = new LABResultDB();
        //    PateintTestsModel _patientLABResults = _labResultDB.GetPatientLABnRADResults(lang, hospitaId, registrationNo, ref errStatus, ref errMessage);

        //    GenericResponse resp = new GenericResponse();

        //    if (_patientLABResults != null)
        //    {
        //        resp.status = 1;
        //        resp.msg = errMessage;
        //        resp.response = _patientLABResults;
                
        //    }
        //    else
        //    {
        //        resp.status = 0;
        //        resp.msg = errMessage;
                

        //    }

        //    return Ok(resp);
        //}

        //[HttpPost]
        //[Route("api/DownloadPdfFile")]
        //[ResponseType(typeof(List<GenericResponse>))]
        //public IHttpActionResult DownloadPdfFile(FormDataCollection col)
        //{
        //    IHttpActionResult response;

        //    var lang = col["lang"];
        //    var hospitaId = Convert.ToInt32(col["hospital_id"]);
        //    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
        //    var filename = Convert.ToString(col["file_name"]);
        //    var filetype = Convert.ToString(col["file_type"]);

        //    if (filename == null)
        //        return BadRequest();

        //    try
        //    {
        //        var file = Util.DownloadFileFTP(Convert.ToString(filename), Convert.ToString(filetype));//.GetFile();

        //        HttpResponseMessage responseMsg = new HttpResponseMessage(HttpStatusCode.OK);
        //        responseMsg.Content = new ByteArrayContent(file);
        //        responseMsg.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //        responseMsg.Content.Headers.ContentDisposition.FileName = Convert.ToString(filename);
        //        responseMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        //        response = ResponseMessage(responseMsg);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        GenericResponse resp = new GenericResponse();
        //        resp.status = 0;
        //        resp.msg = "Report File not Available";

        //        //return resp;
        //        return BadRequest("Report File not Available");
        //    }
        //    //return Ok(response);
        //}

         
    }
}