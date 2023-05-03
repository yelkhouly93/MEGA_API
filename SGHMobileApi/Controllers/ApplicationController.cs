using DataLayer.Model;
using SGHMobileApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLayer.Data;


using DataLayer.Reception.Business;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using System.Data;
using System.Net.Http.Formatting;
using SGHMobileApi.Common;
using System.Data.SqlClient;
using System.Web;
using System.IO;
using System.Configuration;


namespace SGHMobileApi.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class ApplicationController : ApiController
    {
        private GenericResponse _resp = new GenericResponse()
        {
            status = 0
        };
        private ApplicationDB _Db = new ApplicationDB();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [HttpPost]
        [Route("v2/AppVersion-get")]
        [ResponseType(typeof(List<GenericResponse>))]
        public IHttpActionResult GetAppVersion(FormDataCollection col)
        {
            try
            {
                var lang = "EN";
                var AppID = 1;
                var OS = "Android";
                //var OS = "iOS";
                if (!string.IsNullOrEmpty(col["OS"]) && !string.IsNullOrEmpty(col["App_id"]))
                {
                    if (!string.IsNullOrEmpty(col["lang"]))
                        lang = col["lang"];
                    try
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(col["App_id"]))
                                AppID = Convert.ToInt32(col["App_id"]);
                        }
                        catch (Exception e)
                        {
                            _resp.status = 0;
                            _resp.msg = "Parameter in Wrong Format : -- " + e.Message;
                            return Ok(_resp);
                        }


                        if (!string.IsNullOrEmpty(col["OS"]))
                            OS = col["OS"];
                        
                    }
                    catch (Exception ex2)
                    {

                    }

                    var ErStatus = 0;
                    var ErMsg = "";

                    var DataTable = _Db.GetApplicationVersion(lang, AppID, OS, ref ErStatus, ref ErMsg);

                    if (DataTable != null && DataTable.Rows.Count > 0)
                    {
                        _resp.status = ErStatus;
                        _resp.msg = ErMsg;
                        _resp.response = DataTable;
                    }
                    else
                    {
                        _resp.status = ErStatus;
                        _resp.msg = ErMsg;
                    }
                }
                else
                {
                    _resp.status = 0;
                    _resp.msg = "Missing Parameters.";
                }

                



                //if (!string.IsNullOrEmpty(col["hospital_id"]))
                //{

                //}
                //else
                //{
                //    _resp.status = 0;
                //    _resp.msg = "Failed! Missing Parameters";
                //}

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