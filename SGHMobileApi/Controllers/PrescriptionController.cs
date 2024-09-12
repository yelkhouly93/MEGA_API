using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SGHMobileApi.Common;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Extension;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Description;

namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class Prescription_TEStController : ApiController
    {
        // GET: Prescription
        GenericResponse _resp = new GenericResponse();
        [HttpPost]
        [Route("v2/MadicalRefill-List-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetMedicalRefill_List(FormDataCollection col)
        {            
            _resp.status = 0;
            _resp.msg = "Failed : Missing Parameters";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = "EN";

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang  = col["lang"];
                

                var hospitaId = 0;
                var registrationNo = "";
                int errStatus = 0;
                string errMessage = "";
                PatientDB _patientDB = new PatientDB();


                var EpisodeId = 0;
                var EpisodeType = "OP";
                try
                {
                    hospitaId = Convert.ToInt32(col["hospital_id"]);
                    registrationNo = col["patient_reg_no"];

                    if (!string.IsNullOrEmpty(col["Episode_Id"]))
                        EpisodeId = Convert.ToInt32(col["Episode_Id"]);

                    if (!string.IsNullOrEmpty(col["Episode_Type"]))
                        EpisodeType = col["Episode_Type"].ToString();

                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;

                    _resp.msg = "Sorry this service not available";

                    return Ok(_resp);
                }
                if (hospitaId == 9) /*for Dammam BRANCHES*/
                {
                    _resp.status = 0;

                    _resp.msg = "Sorry this service not available";

                    return Ok(_resp);
                }

                if (EpisodeType.ToUpper() != "OP" && EpisodeType.ToUpper() != "IP")
                {
                    _resp.status = 0;
                    _resp.msg = "WRONG Episode Type";
                    return Ok(_resp);
                }



                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();


                var _allPatientMedDT = _patientDB.GetPatient_RefillPrescriptionDT(lang, hospitaId, registrationNo, ref errStatus, ref errMessage, ApiSource, EpisodeId, EpisodeType);


                if (_allPatientMedDT != null && _allPatientMedDT.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = errMessage;
                    _resp.response = _allPatientMedDT;
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
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/MadicalRefill-RequestList-Get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetMedicalRefill_RequestList(FormDataCollection col)
        {

            _resp.status = 0;
            _resp.msg = "Failed : Missing Parameters";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) && col["patient_reg_no"] != "0")
            {
                var lang = col["lang"];
                var hospitaId = 0;
                var registrationNo = "";
                int errStatus = 0;
                string errMessage = "";
                PatientDB _patientDB = new PatientDB();


                var EpisodeId = 0;
                var EpisodeType = "OP";
                try
                {
                    hospitaId = Convert.ToInt32(col["hospital_id"]);
                    registrationNo = col["patient_reg_no"];

                    if (!string.IsNullOrEmpty(col["Episode_Id"]))
                        EpisodeId = Convert.ToInt32(col["Episode_Id"]);

                    if (!string.IsNullOrEmpty(col["Episode_Type"]))
                        EpisodeType = col["Episode_Type"].ToString();

                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;

                    _resp.msg = "Sorry this service not available";

                    return Ok(_resp);
                }
                if (hospitaId == 9) /*for Dammam BRANCHES*/
                {
                    _resp.status = 0;

                    _resp.msg = "Sorry this service not available";

                    return Ok(_resp);
                }

                if (EpisodeType.ToUpper() != "OP" && EpisodeType.ToUpper() != "IP")
                {
                    _resp.status = 0;
                    _resp.msg = "WRONG Episode Type";
                    return Ok(_resp);
                }



                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();


                var _allPatientMedDT = _patientDB.GetPatient_RefillRequestDT(lang, hospitaId, registrationNo, ref errStatus, ref errMessage, ApiSource, EpisodeId, EpisodeType);


                if (_allPatientMedDT != null && _allPatientMedDT.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = errMessage;
                    _resp.response = _allPatientMedDT;
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
                _resp.msg = "Failed : Missing Parameters";
            }
            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/MadicalRefill-Request-Add")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult AddMedicalRefill_Request(FormDataCollection col)
        {

            _resp.status = 0;
            _resp.msg = "Failed : Missing Parameters";

            if (!string.IsNullOrEmpty(col["hospital_id"]) && !string.IsNullOrEmpty(col["patient_reg_no"]) 
                && col["patient_reg_no"] != "0"
                && !string.IsNullOrEmpty(col["Visit_Id"])
                && !string.IsNullOrEmpty(col["Doctor_Name"])
                && !string.IsNullOrEmpty(col["Drug_Name"])
                && !string.IsNullOrEmpty(col["DEPT_NAME"])
                && !string.IsNullOrEmpty(col["Prescription_Id"])
                && !string.IsNullOrEmpty(col["Drug_Id"])
                )
            {
                var lang = col["lang"];
                var hospitaId = 0;
                var registrationNo = "";
                int errStatus = 0;
                string errMessage = "";
                PatientDB _patientDB = new PatientDB();

                var Visit_Id = 0;
                var Doctor_Name = "";
                var Drug_Name = "";
                var DEPT_NAME = "";
                var Prescription_Id = 0;
                var Drug_Id = 0;


                var EpisodeId = 0;
                var EpisodeType = "OP";
                try
                {
                    hospitaId = Convert.ToInt32(col["hospital_id"]);
                    registrationNo = col["patient_reg_no"];
                    Visit_Id = Convert.ToInt32(col["Visit_Id"]);
                    Doctor_Name = col["Doctor_Name"];
                    Drug_Name = col["Drug_Name"];
                    DEPT_NAME = col["DEPT_NAME"];
                    Prescription_Id = Convert.ToInt32(col["Prescription_Id"]);
                    Drug_Id = Convert.ToInt32(col["Drug_Id"]);

                    //if (!string.IsNullOrEmpty(col["Episode_Id"]))
                    //    EpisodeId = Convert.ToInt32(col["Episode_Id"]);

                    //if (!string.IsNullOrEmpty(col["Episode_Type"]))
                    //    EpisodeType = col["Episode_Type"].ToString();

                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                {
                    _resp.status = 0;

                    _resp.msg = "Sorry this service not available";

                    return Ok(_resp);
                }
                if (hospitaId == 9) /*for Dammam BRANCHES*/
                {
                    _resp.status = 0;

                    _resp.msg = "Sorry this service not available";

                    return Ok(_resp);
                }

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();


                var _allPatientMedDT = _patientDB.Save_Patient_RefillRequest(hospitaId, registrationNo,Visit_Id,Doctor_Name,Drug_Name,DEPT_NAME,Prescription_Id,Drug_Id, ref errStatus, ref errMessage, ApiSource);

                _resp.status = errStatus;
                _resp.msg = errMessage;

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