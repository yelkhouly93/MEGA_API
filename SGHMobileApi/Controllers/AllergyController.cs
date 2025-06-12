using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using DataLayer.Data;
using DataLayer.Model;
using SGHMobileApi.Common;
using SGHMobileApi.Extension;
using SmartBookingService.Controllers.ClientApi;

namespace SmartBookingService.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class AllergyController : ApiController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private GenericResponse _resp = new GenericResponse();
        private PatientDB _patientDb = new PatientDB();


        [HttpPost]
        [Route("v2/Food-AllergyList-Patient-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAllergyListBy_Patient(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["Sources"])
                    && !string.IsNullOrEmpty(col["hospital_id"])
                    )
                {
                    var Lang = "EN";
                    if (!string.IsNullOrEmpty(col["lang"]))
                        Lang = col["lang"];

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);

                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (Lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

                    if (hospitaId == 9)
                    {
                        _resp.status = 0;
                        if (Lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }


                    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                    var Source = col["Sources"].ToString();
                    int errStatus = 0;
                    string errMessage = "";

                    var EpisodeType = "OP";
                    var EpisodeID = 0;

                    if (!string.IsNullOrEmpty(col["Episode_Type"]))
                        EpisodeType = col["Episode_Type"];
                    if (!string.IsNullOrEmpty(col["Episode_Id"]))
                        EpisodeID = Convert.ToInt32(col["Episode_Id"]);


                    if (EpisodeType.ToUpper() != "OP" && EpisodeType.ToUpper() != "IP")
                    {
                        _resp.status = 0;
                        _resp.msg = "WRONG Episode Type";
                        return Ok(_resp);
                    }


                    string consentMessage = string.Empty;
                    var _FoodAllergyList = _patientDb.GET_FoodAllergyList_ByPatient(Lang, registrationNo, hospitaId, Source, EpisodeType, EpisodeID);

                    if (_FoodAllergyList != null && _FoodAllergyList.Rows.Count > 0)
                    {
                        resp.status = errStatus;
                        resp.msg = errMessage;
                        resp.response = _FoodAllergyList;
                    }
                    else
                    {
                        resp.status = errStatus;
                        resp.msg = errMessage;
                        resp.response = null;
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //log.Error(ex);

            }
            return Ok();
        }

        //Save_FoodAllergyList_ByPatient
        [HttpPost]
        [Route("v2/PatientFood-AllergyList-save")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientFood_AllergyList_Save(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["Sources"])
                    && !string.IsNullOrEmpty(col["hospital_id"])
                    && !string.IsNullOrEmpty(col["FoodIds"])
                    )
                {
                    var Lang = "EN";
                    if (!string.IsNullOrEmpty(col["lang"]))
                        Lang = col["lang"];
                    //               try
                    //{
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);

                    if (hospitaId == 9)
                    {
                        _resp.status = 0;
                        if (Lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }
                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        _resp.status = 0;
                        if (Lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }

                    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                    var Source = col["Sources"].ToString();
                    var FoodIds = col["FoodIds"].ToString();
                    int errStatus = 0;
                    string errMessage = "Fail to Update";

                    string consentMessage = string.Empty;
                    _patientDb.Save_FoodAllergyList_ByPatient(registrationNo, hospitaId, FoodIds, Source, ref errStatus, ref errMessage);

                    resp.status = errStatus;
                    resp.msg = errMessage;
                    //               }
                    //               catch(Exception ex)
                    //{
                    //                   resp.status = 0;
                    //                   resp.msg = "Wrong Parameter!";
                    //               }
                    resp.response = null;
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //log.Error(ex);

            }
            return Ok();
        }



        [HttpPost]
        [Route("v3/Food-AllergyList-Patient-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAllergyListBy_Patient_v3(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["Sources"])
                    && !string.IsNullOrEmpty(col["hospital_id"])
                    )
                {
                    var Lang = "EN";
                    if (!string.IsNullOrEmpty(col["lang"]))
                        Lang = col["lang"];

                    int errStatus = 0;
                    string errMessage = "";

                    var hospitaId = Convert.ToInt32(col["hospital_id"]);

                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {
                        //_resp.status = 0;
                        //if (Lang == "EN")
                        //    _resp.msg = "Sorry this service not available";
                        //else
                        //    _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        //return Ok(_resp);

						string UAEMRN = col["patient_reg_no"].ToString();
						ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        
                        var _NewData = _UAEApiCaller.GetAllergyList_NewUAE(Lang, hospitaId, UAEMRN, ref errStatus, ref errMessage);

						if (_NewData.Count > 0)
						{
							_resp.status = 1;
							_resp.msg = "record Found(s)";
							_resp.response = _NewData;
							return Ok(_resp);
						}
                        else
						{
                            _resp.status = 1;
                            _resp.msg = "record Found(s)";
                            _resp.response = _NewData;
                            return Ok(_resp);
                        }
					}

                    if (hospitaId == 9)
                    {
                        _resp.status = 0;
                        if (Lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }


                    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                    var Source = col["Sources"].ToString();
                    

                    var EpisodeType = "OP";
                    var EpisodeID = 0;

                    if (!string.IsNullOrEmpty(col["Episode_Type"]))
                        EpisodeType = col["Episode_Type"];
                    if (!string.IsNullOrEmpty(col["Episode_Id"]))
                        EpisodeID = Convert.ToInt32(col["Episode_Id"]);


                    if (EpisodeType.ToUpper() != "OP" && EpisodeType.ToUpper() != "IP")
                    {
                        _resp.status = 0;
                        _resp.msg = "WRONG Episode Type";
                        return Ok(_resp);
                    }


                    string consentMessage = string.Empty;
                    var _FoodAllergyList = _patientDb.GET_FoodAllergyList_ByPatient_V3(Lang, registrationNo, hospitaId, Source, EpisodeType, EpisodeID);

                    if (_FoodAllergyList != null && _FoodAllergyList.Count > 0)
                    {
                        resp.status = errStatus;
                        resp.msg = errMessage;
                        resp.response = _FoodAllergyList;
                    }
                    else
                    {
                        resp.status = errStatus;
                        resp.msg = errMessage;
                        resp.response = null;
                    }
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //log.Error(ex);

            }
            return Ok();
        }

        //Save_FoodAllergyList_ByPatient
        [HttpPost]
        [Route("v3/PatientFood-AllergyList-save")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult PatientFood_AllergyList_Save_v3(FormDataCollection col)
        {
            GenericResponse resp = new GenericResponse();
            try
            {
                if (!string.IsNullOrEmpty(col["patient_reg_no"])
                    && !string.IsNullOrEmpty(col["Sources"])
                    && !string.IsNullOrEmpty(col["hospital_id"])
                    && !string.IsNullOrEmpty(col["FoodIds"])
                    )
                {
                    var Lang = "EN";
                    if (!string.IsNullOrEmpty(col["lang"]))
                        Lang = col["lang"];
                    //               try
                    //{
                    var hospitaId = Convert.ToInt32(col["hospital_id"]);

                    var registrationNo = Convert.ToInt32(col["patient_reg_no"]);
                    var registrationNo_str = col["patient_reg_no"].ToString();
                    var Source = col["Sources"].ToString();
                    var FoodIds = col["FoodIds"].ToString();
                    int errStatus = 0;
                    string errMessage = "Fail to Update";

                    string consentMessage = string.Empty;



                    if (hospitaId == 9)
                    {
                        _resp.status = 0;
                        if (Lang == "EN")
                            _resp.msg = "Sorry this service not available";
                        else
                            _resp.msg = "عذرا هذه الخدمة غير متوفرة";
                        return Ok(_resp);
                    }
                    if (hospitaId >= 301 && hospitaId < 400) /*for UAE BRANCHES*/
                    {   
                        ApiCallerUAE _UAEApiCaller = new ApiCallerUAE();
                        var _NewData = _UAEApiCaller.SaveAllergyList_NewUAE(hospitaId, registrationNo_str, FoodIds, ref errStatus, ref errMessage);

                        _resp.status = 1;
                        _resp.msg = "Record has been updated successfully.";
                        return Ok(_resp);
                        
                    }
                    else
					{
                        _patientDb.Save_FoodAllergyList_ByPatient(registrationNo, hospitaId, FoodIds, Source, ref errStatus, ref errMessage);

                        resp.status = errStatus;
                        resp.msg = errMessage;
                        resp.response = null;
                        return Ok(resp);
                    }

                    
                    
                }
                else
                {
                    resp.status = 0;
                    resp.msg = "Missing Parameter!";
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //log.Error(ex);

            }
            return Ok();
        }



    }
}
