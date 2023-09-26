using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Extension;

namespace SmartBookingService.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [AuthenticationFilter]
    public class ClinicController : ApiController
    {

        private GenericResponse _resp = new GenericResponse();
        private ClinicDB _clinicDb = new ClinicDB();

        //private List<dynamic> _expandoList;


        /// <summary>
        /// Get All Clinics against hospital.
        /// </summary>
        /// <returns>Return list of all clinics against hospital</returns>
        [HttpPost]
        [Route("clinics")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAllClinics(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _clinicDb = new ClinicDB();
            if (!string.IsNullOrEmpty(col["hospital_id"]) )
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                int hospitalId = 0;
                try
                {
                    if (!string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = Convert.ToInt32(col["hospital_id"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }



                var pageNo = -1;
                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                var pageSize = 10;
                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                var allDataTable = _clinicDb.GetAllClinicsDataTable(lang, hospitalId, pageNo , pageSize);

                if (allDataTable.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = allDataTable;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Fail";
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
        [Route("clinics-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAllClinics_New(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _clinicDb = new ClinicDB();
            if (!string.IsNullOrEmpty(col["hospital_id"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                int hospitalId = 0;
                try
                {
                    if (!string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = Convert.ToInt32(col["hospital_id"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }
                


                var pageNo = -1;
                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                var pageSize = 10;
                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                var allDataTable = _clinicDb.GetAllClinicsDataTable(lang, hospitalId, pageNo, pageSize);

                if (allDataTable.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = allDataTable;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Fail";
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
        [Route("v2/clinics-get-all")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAllClinics_Newall_v2(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _clinicDb = new ClinicDB();
            var lang = "EN";
            var pageNo = -1;
            var pageSize = 10;
            var hospitalid = "";
            var groupentity = "";

            if (col != null)
            {
                groupentity = null;
                hospitalid = null;

                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];
                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);
                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);


                if (!string.IsNullOrEmpty(col["groupentity_id"]))
                    groupentity = col["groupentity_id"];

                if (!string.IsNullOrEmpty(col["hospital_id"]))
                    hospitalid = col["hospital_id"];

                
            }
            else
            {
                groupentity = null;
                hospitalid = null;
            }

            var allDataTable = _clinicDb.GetAllClinicsDataTable_V2(lang, hospitalid, groupentity, pageNo, pageSize);

            if (allDataTable.Rows.Count > 0)
            {
                _resp.status = 1;
                _resp.msg = "Success";
                _resp.response = allDataTable;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Fail";
            }

            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/clinics-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAllClinics_New_V2(FormDataCollection col)
        {
            _resp = new GenericResponse();
            _clinicDb = new ClinicDB();
            if (col != null && !string.IsNullOrEmpty(col["hospital_id"]))
            {
                var lang = "EN";
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                int hospitalId = 0;
                try
                {
                    if (!string.IsNullOrEmpty(col["hospital_id"]))
                        hospitalId = Convert.ToInt32(col["hospital_id"]);
                }
                catch (Exception e)
                {
                    _resp.status = 0;
                    _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                    return Ok(_resp);
                }


                //var ApiSource = "";
                //if (!string.IsNullOrEmpty(col["Sources"]))
                //    ApiSource = col["Sources"].ToString();


                var pageNo = -1;
                if (!string.IsNullOrEmpty(col["page_no"]))
                    pageNo = Convert.ToInt32(col["page_no"]);

                var pageSize = 10;
                if (!string.IsNullOrEmpty(col["page_size"]))
                    pageSize = Convert.ToInt32(col["page_size"]);

                var ApiSource = "MobileApp";
                if (!string.IsNullOrEmpty(col["Sources"]))
                    ApiSource = col["Sources"].ToString();

                if (!string.IsNullOrEmpty(col["Source"]))
                    ApiSource = col["Source"];

                DataTable allDataTable;
                /**/

                if (ApiSource.ToUpper() == "ARABOT")
                {
                    allDataTable = _clinicDb.GetAllClinicsDataTable_V2_ARA(lang, hospitalId, pageNo, pageSize);
                    
                }
                else
                {
                    allDataTable = _clinicDb.GetAllClinicsDataTable(lang, hospitalId, pageNo, pageSize, ApiSource);
                }

                

                if (allDataTable.Rows.Count > 0)
                {
                    _resp.status = 1;
                    _resp.msg = "Success";
                    _resp.response = allDataTable;
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Fail";
                }
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Failed : Missing Parameters. Please Provide the Branch ID";
            }
            return Ok(_resp);
        }

    }
}