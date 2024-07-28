using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Extension;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class HospitalController : ApiController
    {
        private HospitalDB _hospitalDb = new HospitalDB();
        private GenericResponse _resp = new GenericResponse();
        private CommonDB _dbCommonDb = new CommonDB();
        private List<dynamic> _expandoList;

        /// <summary>
        /// Get all hospitals .
        /// </summary>
        /// <returns>Return list of all hospital's branches</returns>
        [HttpPost]
        [Route("hospitals")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetHospitalsList(FormDataCollection col)
        {

            _resp = new GenericResponse();
            _hospitalDb = new HospitalDB();
            var groupentityid = 1;
            var lang = "EN";
            var Ispayment = 0;

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                if (!string.IsNullOrEmpty(col["groupentity_id"]))
                    groupentityid = Convert.ToInt32(col["groupentity_id"]);

                if (!string.IsNullOrEmpty(col["PaymentDetails"]))
                    Ispayment = Convert.ToInt32(col["PaymentDetails"]);
            }            

            _expandoList = new List<dynamic>();

            var allHospitals = _hospitalDb.GetAllHospitalsDataTable(lang, groupentityid , Ispayment) ;
            //_expandoList = _dbCommonDb.MappingDT_dynamicList(allHospitals);

            if (allHospitals.Rows.Count > 0)
            {
                _resp.status = 1;
                _resp.msg = "Success";
                _resp.response = allHospitals;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Fail";
            }

            return Ok(_resp);
        }


        [HttpPost]
        [Route("v2/hospitals-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetHospitalsList_v2(FormDataCollection col)
        {

            _resp = new GenericResponse();
            _hospitalDb = new HospitalDB();
            var groupentityid = "";
            var lang = "EN";
            var Ispayment = 0;
            var longitude = 0.00;
            var latitude = 0.00;
            var CallingArea = "";
            var For_TEST = "1";

            var CountryID = 0;

            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];


                if (!string.IsNullOrEmpty(col["groupentity_id"]))
                    groupentityid = col["groupentity_id"];

                if (!string.IsNullOrEmpty(col["PaymentDetails"]))
                    Ispayment = Convert.ToInt32(col["PaymentDetails"]);

                if (!string.IsNullOrEmpty(col["country_ID"]))
                    CountryID = Convert.ToInt32(col["country_ID"]);


                if (!string.IsNullOrEmpty(col["latitude"]))
                    latitude = Convert.ToDouble(col["latitude"]);

                if (!string.IsNullOrEmpty(col["longitude"]))
                    longitude = Convert.ToDouble(col["longitude"]);

                if (!string.IsNullOrEmpty(col["Area"]))
                    CallingArea = col["Area"].ToString();

                if (!string.IsNullOrEmpty(col["FOR_TEST"]))
                    For_TEST = col["FOR_TEST"].ToString();


            }
            else
            {
                groupentityid = null;
                lang = null;
            }


            var ApiSource = "";
            if (!string.IsNullOrEmpty(col["Sources"]))
                ApiSource = col["Sources"].ToString();


            var allHospitals = _hospitalDb.GetAllHospitalsDataTable_v2(lang, groupentityid, Ispayment , CountryID, latitude , longitude , ApiSource , CallingArea , For_TEST);
            //_expandoList = _dbCommonDb.MappingDT_dynamicList(allHospitals);

            if (allHospitals.Rows.Count > 0)
            {
                _resp.status = 1;
                _resp.msg = "Success";
                _resp.response = allHospitals;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Fail";
            }

            return Ok(_resp);
        }



        [HttpPost]
        [Route("v2/hospital-country-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetHospitalCountryList_v2(FormDataCollection col)
        {

            _resp = new GenericResponse();
            _hospitalDb = new HospitalDB();            
            var lang = "EN";
            if (col != null)
            {
                if (!string.IsNullOrEmpty(col["lang"]))
                    lang = col["lang"];
            }

            var allCountryList = _hospitalDb.GetAllHospitalsCountry_DT(lang);
            if (allCountryList.Rows.Count > 0)
            {
                _resp.status = 1;
                _resp.msg = "Success";
                _resp.response = allCountryList;
            }
            else
            {
                _resp.status = 0;
                _resp.msg = "Fail";
            }

            return Ok(_resp);
        }

    }
}