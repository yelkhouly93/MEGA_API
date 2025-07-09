using DataLayer.Common;
using DataLayer.Data;
using DataLayer.Data.FCM;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;


namespace SGHMobileApi.Controllers
{
    public class SGHERPController : ApiController
    {
        private readonly EncryptDecrypt_New util = new EncryptDecrypt_New();
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };
        private CommonDB _commonDb = new CommonDB();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET: SGHERP
        [HttpPost]
        [Route("v2/CMS-USER-list-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetCMSUserList_ForERP(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["clientid"]) && !string.IsNullOrEmpty(col["username"]) && !string.IsNullOrEmpty(col["password"]))
                {
                    if (col["clientid"] == "301C25F7-602E-4348-A032-E8CBC7794BB6" && col["username"] == "sgherp" && col["password"] == "YXf5LXkXh4B15KlDBqqyhA==")
                    {
                        var EMPID = col["EMP_ID"];
                        var hospitaId = col["hospital_id"];
                        CommonDB CDB = new CommonDB();
                        var DataResponse = CDB.GetCMSUSERLIST_DT(hospitaId, EMPID);

                        if (DataResponse != null && DataResponse.Rows.Count > 0)
                        {
                            _resp.status = 1;
                            _resp.msg = "Success";
                            _resp.response = DataResponse;
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
                        _resp.msg = "Authentication Failed";
                    }                   

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Missing Parameters";
                }
                

            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }


            return Ok(_resp);
        }


        // GET: SGHERP
        [HttpPost]
        [Route("FCM/Get-Incomplete-Reminder")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetIncompleteAppoitmentReminders_ForFCM(FormDataCollection col)
        {
            try
            {
                if (!string.IsNullOrEmpty(col["clientid"]) && !string.IsNullOrEmpty(col["username"]) && !string.IsNullOrEmpty(col["password"]))
                {
                    if (col["clientid"] == "301C25F7-602E-4348-A032-E8CBC7794BB6" && col["username"] == "FCMData" && col["password"] == "YXf5LXkXh4B15KlDBqqyhA==")
                    {   
                        fcmDB CDB = new fcmDB();
                        var DataResponse = CDB.Get_IncompleteReminder_DT();

                        if (DataResponse != null && DataResponse.Rows.Count > 0)
                        {
                            _resp.status = 1;
                            _resp.msg = "Success";
                            _resp.response = DataResponse;
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
                        _resp.msg = "Authentication Failed";
                    }

                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Missing Parameters";
                }


            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }


            return Ok(_resp);
        }


        // GET: SGHERP
        [HttpPost]
        [Route("vid/Get-ZoomCall-Details")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetZoomCallInfo_ForVC(FormDataCollection col)
        {
            _resp.status = 0;

            try
            {
                if (!string.IsNullOrEmpty(col["username"]))
                {
                    var EncId = col["username"].ToString();
                    //var dycID = util.Encrypt(EncId, true);
                    var dycID = EncId;

                    fcmDB CDB = new fcmDB();
                    if (!String.IsNullOrEmpty(dycID))
					{
                        var DataResponse = CDB.VC_GetZoomCallInfo(dycID);
                        if (DataResponse != null )
						{
                            _resp.status = 1;
                            _resp.response = DataResponse;
                            return Ok(_resp);
                        }
                    }
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Failed : Missing Parameters";
                }


            }
            catch (Exception ex)
            {
                _resp.msg = ex.ToString();
                Log.Error(ex);
            }


            return Ok(_resp);
        }



    }
}