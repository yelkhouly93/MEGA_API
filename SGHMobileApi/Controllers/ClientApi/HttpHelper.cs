using DataLayer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RestClient
{
    public static class RestUtility
    {
        public static string Msg = string.Empty;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static object CallService<T>(string url, string operation, object requestBodyObject, string method, string username, string password, out HttpStatusCode status, string apiToken = null) where T : class
        {
            try
            {

                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Method = method;
                webReq.Accept = "application/json";

                //Add basic authentication header if username is supplied
                if (apiToken != null) 
                {
                    webReq.Headers["Authorization"] = "Bearer " + apiToken;
                }
                else if (!string.IsNullOrEmpty(username))
                {
                    webReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                }

                //Add key to header if operation is supplied
                if (!string.IsNullOrEmpty(operation))
                {
                    webReq.Headers["Operation"] = operation;
                }

                //Serialize request object as JSON and write to request body
                if (requestBodyObject != null)
                {
                    
                    var requestBody = JsonConvert.SerializeObject(requestBodyObject);
                    
                    webReq.ContentLength = requestBody.Length;
                    webReq.ContentType = "application/json";

                    var streamWriter = new StreamWriter(webReq.GetRequestStream(), Encoding.ASCII);
                    streamWriter.Write(requestBody);
                    streamWriter.Close();
                }

                var response = webReq.GetResponse();

                status = ((HttpWebResponse)response).StatusCode;

                if (response == null)
                {
                    return null;
                }

                status = ((HttpWebResponse)response).StatusCode;

                var streamReader = new StreamReader(response.GetResponseStream());

                var responseContent = streamReader.ReadToEnd().Trim();

                var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

                return jsonObject;
            }
            catch(WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorContennt = reader.ReadToEnd().Trim();
                            var jsonObject = JsonConvert.DeserializeObject<ErrorResponseObject>(errorContennt);
                            
                            status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
                            Msg = jsonObject.details;
                            
                            return null;
                        }
                    }
                    
                }

                status = HttpStatusCode.InternalServerError;
                
                return null;
            }
            
        }

        public static object CallServiceEInvoice<T>(string url, string operation, object requestBodyObject, string method,out HttpStatusCode status, string apiToken = null) where T : class
        {
            try
            {

                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Method = method;
                webReq.Accept = "application/json";

                //Add basic authentication header if username is supplied
                //if (apiToken != null)
                //{
                //    webReq.Headers["Authorization"] = "Bearer " + apiToken;
                //}
                //else if (!string.IsNullOrEmpty(username))
                //{
                //    webReq.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                //}

                //Add key to header if operation is supplied
                if (!string.IsNullOrEmpty(operation))
                {
                    webReq.Headers["Operation"] = operation;
                }

                //Serialize request object as JSON and write to request body
                if (requestBodyObject != null)
                {

                    var requestBody = JsonConvert.SerializeObject(requestBodyObject);

                    webReq.ContentLength = requestBody.Length;
                    webReq.ContentType = "application/json";
                    var streamWriter = new StreamWriter(webReq.GetRequestStream(), Encoding.ASCII);
                    streamWriter.Write(requestBody);
                    streamWriter.Close();
                }

                var response = webReq.GetResponse();

                status = ((HttpWebResponse)response).StatusCode;

                if (response == null)
                {
                    return null;
                }

                status = ((HttpWebResponse)response).StatusCode;

                var streamReader = new StreamReader(response.GetResponseStream());

                var responseContent = streamReader.ReadToEnd().Trim();

                var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

                return jsonObject;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorContennt = reader.ReadToEnd().Trim();
                            var jsonObject = JsonConvert.DeserializeObject<ErrorResponseObject>(errorContennt);

                            status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
                            Msg = jsonObject.details;

                            return null;
                        }
                    }

                }

                status = HttpStatusCode.InternalServerError;

                return null;
            }

        }


        public static object CallAPI<T>(string url, KeyValuePair<string, string>[] ObjFormCol)
        {
            using (var client = new HttpClient())
            {

                //var apiToken = ConfigurationManager.AppSettings["MobileApi_Token"].ToString();
                // ahsan New Token Logic
                

                //client.DefaultRequestHeaders.Authorization =
                //    new AuthenticationHeaderValue("Bearer", apiToken);

                //client.BaseAddress = new Uri(url);
                var newURI = new Uri(url);
                //var content = new FormUrlEncodedContent(ObjFormCol);
                //var result = client.PostAsync("", content).Result;
                var result = client.GetAsync(newURI).Result;


                string resultContent = result.Content.ReadAsStringAsync().Result;


                var resp = new GenericResponse();

                resp = JsonConvert.DeserializeObject<GenericResponse>(resultContent);
                //var jsonObject = JsonConvert.DeserializeObject<T>(resp.response);
                if (resp.status == 1)
                {
                    //var jsonObject = (T)resp.response;
                    var jsonObject = JsonConvert.DeserializeObject<T>(resp.response.ToString());
                    return jsonObject;
                }
                else
                {
                    //var Errorresp = new ErrorResponse_ERROR();

                    // Errorresp = JsonConvert.DeserializeObject<ErrorResponse_ERROR>(resultContent);

                    // AHSNA Comments as crashing and code not Completed
                    //if (Errorresp.ERROR.Code == 401 || Errorresp.ERROR.Message == "The Token has expired")
                    //{
                    //    var srt = "Genrate the token and recall";

                    //     srt = "Token Genetrated";
                    //}

                }

                return null;

            }

        }


        public static object CallAPI_Perscription<T>(string url, KeyValuePair<string, string>[] ObjFormCol)
        {
            using (var client = new HttpClient())
            {   
                var newURI = new Uri(url);                
                var result = client.GetAsync(newURI).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                var resp = new GenericResponse();
                var DamResp = new DamPerscriptionResponse();
                resp = JsonConvert.DeserializeObject<GenericResponse>(resultContent);
                DamResp = JsonConvert.DeserializeObject<DamPerscriptionResponse>(resultContent);
                var jsonObject = JsonConvert.DeserializeObject<T>(DamResp.prescriptions.ToString());


                return jsonObject;

            }

        }
        public static object CallAPI_POST<T>(string url, object requestBodyObject, out HttpStatusCode status) where T : class
        {
            try
            {

                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.Method = "POST";
                webReq.Accept = "application/json";
                
                //Serialize request object as JSON and write to request body
                if (requestBodyObject != null)
                {

                    var requestBody = JsonConvert.SerializeObject(requestBodyObject);

                    webReq.ContentLength = requestBody.Length;
                    webReq.ContentType = "application/json";

                    var streamWriter = new StreamWriter(webReq.GetRequestStream(), Encoding.ASCII);
                    streamWriter.Write(requestBody);
                    streamWriter.Close();
                }

                var response = webReq.GetResponse();

                status = ((HttpWebResponse)response).StatusCode;

                if (response == null)
                {
                    return null;
                }

                status = ((HttpWebResponse)response).StatusCode;

                var streamReader = new StreamReader(response.GetResponseStream());

                var responseContent = streamReader.ReadToEnd().Trim();

                var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

                return jsonObject;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)wex.Response)
                    {
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorContennt = reader.ReadToEnd().Trim();
                            var jsonObject = JsonConvert.DeserializeObject<PostResponse>(errorContennt);

                            status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
                            Msg = jsonObject.errorMessage;

                            return jsonObject;
                        }
                    }

                }

                status = HttpStatusCode.InternalServerError;

                return null;
            }

        }





    }
}