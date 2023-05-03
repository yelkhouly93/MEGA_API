using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Linq;
using SGHMedicalApi.Models;
using SGHMedicalApi.Common;
using DataLayer.Data.SGHMedicalDAL;

namespace SGHMedicalApi.Controllers
{
    [RoutePrefix("api/q")]
    public class LoginApiController : SGHMedicalBaseApiController
    {
        private EncryptDecrypt EncryDecry = new EncryptDecrypt();
        WebApiLoginModel responsemodel = new WebApiLoginModel();


        [Route("login")]
        public HttpResponseMessage PostLogin(_logWebApi model)
        {
            LoginDAL log = new LoginDAL();
            
            bool IsComplete = false;
            if (ModelState.IsValid)
            {

                log.Username = model.EmployeeId;
                log.Password = model.Password;
                log.ClinicType = "1";


                /* I separate Try statement to capture each error on decrypt method;
                 */

                try //ModuleKey Decryption
                {
                    var modkey = EncryDecry.Decrypt(model.ModuleKey.ToString(), true);//W1TjdtW3Tpk=
                    if (string.Compare("0" ?? "", EncryDecry.Decrypt(model.ModuleKey.ToString(), true), false) != 0)
                    {
                        responsemodel.IsComplete = IsComplete;
                        responsemodel.message = "Wrong Module Key.";
                        var response2 = Request.CreateResponse(HttpStatusCode.InternalServerError, responsemodel);
                        return response2;
                    }

                }
                catch (Exception ex)
                {
                    responsemodel.IsComplete = IsComplete;
                    responsemodel.message = "ModuleKey = " + ex.Message;
                    var response2 = Request.CreateResponse(HttpStatusCode.InternalServerError, responsemodel);
                    return response2;
                }

                try //APP ID Decryption
                {

                    if (string.Compare("HISLOGINDoctorApp" ?? "", EncryDecry.Decrypt(model.AppID.ToString(), true), false) != 0)
                    {
                        responsemodel.IsComplete = IsComplete;
                        responsemodel.message = "Wrong APP ID.";
                        var response2 = Request.CreateResponse(HttpStatusCode.InternalServerError, responsemodel);
                        return response2;
                    }
                }
                catch (Exception ex)
                {
                    responsemodel.IsComplete = IsComplete;
                    responsemodel.message = "AppID = " + ex.Message;
                    var response2 = Request.CreateResponse(HttpStatusCode.InternalServerError, responsemodel);
                    return response2;
                }

            }
            else
            {
                //ModelState Error
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                responsemodel.IsComplete = IsComplete;
                responsemodel.message = messages;
                var response2 = Request.CreateResponse(HttpStatusCode.InternalServerError, responsemodel);
                return response2;
            }

            try
            {

                if (log.mobileLogin())
                {
                    IsComplete = true;

                    responsemodel.IsComplete = IsComplete;

                    SetupFormsAuthTicket_API(log.EmployeeID, model.EmployeeId.ToString(), log.Employee, log.DivisionID, log.DepartmentID, log.DepartmentName);

                    var response = Request.CreateResponse(HttpStatusCode.OK, responsemodel);
                    var AppID = EncryDecry.Decrypt(model.AppID.ToString(), true);//15bzWRSMTvJrdeHHYY0uqpYuAEjJl2Sw
                    response.Headers.Add("X-HIS-APP", AppID);
                    return response;

                }
                else
                {
                    responsemodel.IsComplete = IsComplete;
                    responsemodel.message = "Please check your credentials.";
                    var response2 = Request.CreateResponse(HttpStatusCode.InternalServerError, responsemodel);
                    return response2;
                }

            }
            catch (Exception ex)
            {
                responsemodel.IsComplete = IsComplete;
                responsemodel.message = ex.Message;
                var response2 = Request.CreateResponse(HttpStatusCode.InternalServerError, responsemodel);
                return response2;
            }
        }



        private void SetupFormsAuthTicket_API(string EmployeeID, string EmpID, string EmployeeName, string DivisionID, string DepartmentID, string DepartmentName)
        {
            var userData = EmployeeID.ToString(CultureInfo.InvariantCulture);

            CustomPrincipalSerializedModel serializeModel = new CustomPrincipalSerializedModel();
            serializeModel.Id = int.Parse(EmployeeID);
            serializeModel.FullName = EmployeeName;
            serializeModel.Department = DepartmentID;
            serializeModel.Email = "";
            serializeModel.UserRole = (int)1;
            serializeModel.UserRoleDesc = "mobile";
            serializeModel.IpAddress = "";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string userData2 = serializer.Serialize(serializeModel);
            var cryptor = new EncryptDecrypt();
            var encryptedEmployee = cryptor.Encrypt(userData2, true);
            responsemodel.__sghis = encryptedEmployee;

            List<WebApiLoginModel_EmployeeDetails> emplist = new List<WebApiLoginModel_EmployeeDetails>();
            emplist.Add(new WebApiLoginModel_EmployeeDetails
            {
                ID = EmployeeID,
                EmployeeId = EmpID,
                FullName = EmployeeName,
                DepartmentId = DepartmentID,
                DepartmentName = DepartmentName
            });

            responsemodel.EmployeeDetails = emplist;


        }


    }
}
