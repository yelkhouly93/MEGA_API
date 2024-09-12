using DataLayer.Common;
using DataLayer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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


        public static object CallAPI<T>(string url, KeyValuePair<string, string>[] ObjFormCol , bool IsForUAE = false)
        {
            try
			{
                using (var client = new HttpClient())
                {
                    if (IsForUAE)
                    {
                        var apiToken = GetToken("UAE");
                        if (apiToken == null)
                            return null;

                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", apiToken);
                    }

                    var newURI = new Uri(url);
                    var result = client.GetAsync(newURI).Result;


                    string resultContent = result.Content.ReadAsStringAsync().Result;


                    var resp = new GenericResponse();

                    resp = JsonConvert.DeserializeObject<GenericResponse>(resultContent);
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
            catch(Exception ex)
			{
                return null;
			}
            

        }


        public static object CallAPI_Perscription<T>(string url, KeyValuePair<string, string>[] ObjFormCol)
        {
            try
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
                    if (DamResp.prescriptions != null)
					{
                        var jsonObject = JsonConvert.DeserializeObject<T>(DamResp.prescriptions.ToString());


                        return jsonObject;
                    }
                    return null;
                    

                }

            }
            catch(Exception ex)
			{

			}
            return null;

        }
        public static object CallAPI_POST<T>(string url, object requestBodyObject, out HttpStatusCode status, bool IsForUAE = false) where T : class
        {
            try
            {

                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                
                webReq.Method = "POST";
                webReq.Accept = "application/json";




                if (IsForUAE)
                {
                    var apiToken = GetToken("UAE");

                    if (apiToken != null)
                    {
                        webReq.Headers["Authorization"] = "Bearer " + apiToken;
                    }
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


        public static object CallAPI_POST_UAE<T>(string url, object requestBodyObject, out GenericResponse responseOut, bool IsForUAE = false) where T : class
        {
            try
            {
                responseOut = new GenericResponse();
                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);

                webReq.Method = "POST";
                webReq.Accept = "application/json";




                if (IsForUAE)
                {
                    var apiToken = GetToken("UAE");

                    if (apiToken != null)
                    {
                        webReq.Headers["Authorization"] = "Bearer " + apiToken;
                    }
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

                responseOut = null;
                var response = webReq.GetResponse();

                var status = ((HttpWebResponse)response).StatusCode;

                if (response == null)
                {
                    
                    return null;
                }

                status = ((HttpWebResponse)response).StatusCode;

                var streamReader = new StreamReader(response.GetResponseStream());

                var responseContent = streamReader.ReadToEnd().Trim();

                if (status== HttpStatusCode.OK)
				{

                    //AHSAN NEW Change 
                    var resp = new GenericResponse();

                    resp = JsonConvert.DeserializeObject<GenericResponse>(responseContent);
                    //var jsonObject = JsonConvert.DeserializeObject<T>(resp.response);
                    responseOut = resp;

                    if (resp.status == 1)
                    {
                        //var jsonObject = (T)resp.response;
                        if (resp.response != null)
						{
                            var jsonObject2 = JsonConvert.DeserializeObject<T>(resp.response.ToString());
                            return jsonObject2;
                        }
                        
                    }
                    else
                    {
                        //var Errorresp = new ErrorResponse_ERROR();

                        //Errorresp = JsonConvert.DeserializeObject<ErrorResponse_ERROR>(resultContent);
                        return null;

                    }
                }
                else
				{
                    var resp = new GenericResponse();
                    resp.status = (int)status;
                    resp.msg = Msg;
                    responseOut = resp;
                }

                return null;


                //var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

                //return jsonObject;
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

                            var status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
                            Msg = jsonObject.errorMessage;
                            var resp = new GenericResponse();
                            resp.status = (int)status;
                            resp.msg = Msg;
                            responseOut = resp;
                            //return jsonObject;
                            return null;
                        }
                    }

                }

                //status = HttpStatusCode.InternalServerError;
                var resp2 = new GenericResponse();
                resp2.status = (int)HttpStatusCode.InternalServerError;
                try
				{
                    resp2.msg = wex.Message;

                }
                catch(Exception ex)
				{
                    resp2.msg = "UAE InternalServerError";
                }
              
                
                responseOut = resp2;

                return null;
            }

        }

        public static object CallAPI_POST_UAE_LABPDF<T>(string url, object requestBodyObject,  bool IsForUAE = true) where T : class
        {
            try
            {
                
                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);

                webReq.Method = "POST";
                webReq.Accept = "application/json";




                if (IsForUAE)
                {
                    var apiToken = GetToken("UAE");

                    if (apiToken != null)
                    {
                        webReq.Headers["Authorization"] = "Bearer " + apiToken;
                    }
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

                var status = ((HttpWebResponse)response).StatusCode;

                if (response == null)
                {

                    return null;
                }

                status = ((HttpWebResponse)response).StatusCode;

                var streamReader = new StreamReader(response.GetResponseStream());

                var responseContent = streamReader.ReadToEnd().Trim();

                if (status == HttpStatusCode.OK)
                {

                    //AHSAN NEW Change 
                    var resp = new LabRad_PDF_UAE_Response();

                    resp = JsonConvert.DeserializeObject<LabRad_PDF_UAE_Response>(responseContent);
                    //var jsonObject = JsonConvert.DeserializeObject<T>(resp.response);
                    

                    if (resp.Reports != null)
                    {
                        //var jsonObject = (T)resp.response;
                        if (resp.Reports != null)
                        {
                            //var jsonObject3 = JsonConvert.DeserializeObject<List<LabRad_PDF_UAE>>(resp.Reports.ToString());
                            var jsonObject2 = JsonConvert.DeserializeObject<T>(resp.Reports.ToString());
                            return jsonObject2;
                        }

                    }
                    else
                    {
                        //var Errorresp = new ErrorResponse_ERROR();

                        //Errorresp = JsonConvert.DeserializeObject<ErrorResponse_ERROR>(resultContent);
                        return null;

                    }
                }
                else
                {
                    //var resp = new GenericResponse();
                    //resp.status = (int)status;
                    //resp.msg = Msg;
                    //responseOut = resp;
                    return null;
                }

                return null;
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

                            var status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
                            Msg = jsonObject.errorMessage;
                            var resp = new GenericResponse();
                            resp.status = (int)status;
                            resp.msg = Msg;
                            
                            //return jsonObject;
                            return null;
                        }
                    }

                }

                //status = HttpStatusCode.InternalServerError;
                var resp2 = new GenericResponse();
                resp2.status = (int)HttpStatusCode.InternalServerError;
                try
                {
                    resp2.msg = wex.Message;

                }
                catch (Exception ex)
                {
                    resp2.msg = "UAE InternalServerError";
                }

                return null;
            }

        }

        public static object CallAPI_POST_UAE_Appointment<T>(string url, object requestBodyObject, out AppointmentPostResponse responseOut, bool IsForUAE = false) where T : class
        {
            try
            {
                responseOut = new AppointmentPostResponse();
                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);

                webReq.Method = "POST";
                webReq.Accept = "application/json";




                if (IsForUAE)
                {
                    var apiToken = GetToken("UAE");

                    if (apiToken != null)
                    {
                        webReq.Headers["Authorization"] = "Bearer " + apiToken;
                    }
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

                responseOut = null;
                var response = webReq.GetResponse();

                var status = ((HttpWebResponse)response).StatusCode;

                if (response == null)
                {

                    return null;
                }

                status = ((HttpWebResponse)response).StatusCode;

                var streamReader = new StreamReader(response.GetResponseStream());

                var responseContent = streamReader.ReadToEnd().Trim();

                if (status == HttpStatusCode.OK)
                {

                    //AHSAN NEW Change 
                    var resp = new AppointmentPostResponse();

                    resp = JsonConvert.DeserializeObject<AppointmentPostResponse>(responseContent);

                    var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);



                    responseOut = resp;

                    if (resp.Error == "False" || resp.Error == "false")
                    {
                        var jsonObject2 = JsonConvert.DeserializeObject<T>(responseContent);
                        return jsonObject2;
                    }
                    else
                    {
                        //var Errorresp = new ErrorResponse_ERROR();

                        //Errorresp = JsonConvert.DeserializeObject<ErrorResponse_ERROR>(resultContent);
                        return null;

                    }
                }









                return null;


                //var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

                //return jsonObject;
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

                            var status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
                            Msg = jsonObject.errorMessage;
                            var resp = new AppointmentPostResponse();
                            //resp.status = (int)status;
                            //resp.msg = Msg;
                            responseOut = resp;
                            //return jsonObject;
                            return null;
                        }
                    }

                }

                //status = HttpStatusCode.InternalServerError;
                var resp2 = new AppointmentPostResponse();
                //resp2.status = (int)HttpStatusCode.InternalServerError;
                //resp2.msg = "InternalServerError";
                responseOut = resp2;

                return null;
            }

        }




        public static object CallAPI_POST_UAE_Add_Patient<T>(string url, object requestBodyObject, out GenericResponse_NEWUAE_registration responseOut, bool IsForUAE = false) where T : class
        {
            try
            {
                responseOut = new GenericResponse_NEWUAE_registration();
                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);

                webReq.Method = "POST";
                webReq.Accept = "application/json";

                var resp = new GenericResponse_NEWUAE_registration();



                if (IsForUAE)
                {
                    var apiToken = GetToken("UAE");

                    if (apiToken != null)
                    {
                        webReq.Headers["Authorization"] = "Bearer " + apiToken;
                    }
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

                responseOut = null;
                var response = webReq.GetResponse();

                var status = ((HttpWebResponse)response).StatusCode;

                if (response == null)
                {

                    return null;
                }

                status = ((HttpWebResponse)response).StatusCode;

                var streamReader = new StreamReader(response.GetResponseStream());

                var responseContent = streamReader.ReadToEnd().Trim();

                if (status == HttpStatusCode.OK)
                {

                    //AHSAN NEW Change 
                    

                    resp = JsonConvert.DeserializeObject<GenericResponse_NEWUAE_registration>(responseContent);
                    //var jsonObject = JsonConvert.DeserializeObject<T>(resp.response);
                    responseOut = resp;

                    if (resp.Error == false && resp.Mrn != null)
                    {
                        //var jsonObject = (T)resp.response;
                        if (resp != null)
                        {                            
                            return resp;
                        }

                    }
                    else
                    {

                        resp.Error = true;
                        return resp;

                    }
                }







                resp.Error = true;

                return resp;


                //var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

                //return jsonObject;
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

                            var status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
                            Msg = jsonObject.errorMessage;
                            var resp = new GenericResponse_NEWUAE_registration();
                            resp.Error = false;
                            resp.Message = Msg;
                            responseOut = resp;
                            //return jsonObject;
                            return null;
                        }
                    }

                }

                //status = HttpStatusCode.InternalServerError;
                var resp2 = new GenericResponse_NEWUAE_registration();
                resp2.Error = false;
                resp2.Message = "InternalServerError";
                responseOut = resp2;

                return null;
            }

        }

        public static object CallAPI_SMS_POST<T>(string url, object requestBodyObject, out HttpStatusCode status, bool IsForUAE = false) where T : class
        {
            try
            {

                // Initialize an HttpWebRequest for the current URL.
                var webReq = (HttpWebRequest)WebRequest.Create(url);

                webReq.Method = "POST";
                webReq.Accept = "application/json";




                if (IsForUAE)
                {
                    var apiToken = GetToken_SMSAPI("UAE");

                    if (apiToken != null)
                    {
                        webReq.Headers["Authorization"] = "Bearer " + apiToken;                        
                    }
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

        //public static object CallAPI_SMS_POST<T>(string url, object requestBodyObject, out GenericResponse_NEWUAE_registration responseOut, bool IsForUAE = false) where T : class
        //{
        //    try
        //    {
        //        responseOut = new GenericResponse_NEWUAE_registration();
        //        // Initialize an HttpWebRequest for the current URL.
        //        var webReq = (HttpWebRequest)WebRequest.Create(url);

        //        webReq.Method = "POST";
        //        webReq.Accept = "application/json";

        //        var resp = new GenericResponse_NEWUAE_registration();



        //        if (IsForUAE)
        //        {
        //            var apiToken = GetToken_SMSAPI("UAE");

        //            if (apiToken != null)
        //            {   
        //                webReq.Headers["Authorization"] = "Bearer " + apiToken;
        //            }
        //        }



        //        //Serialize request object as JSON and write to request body
        //        if (requestBodyObject != null)
        //        {

        //            var requestBody = JsonConvert.SerializeObject(requestBodyObject);

        //            webReq.ContentLength = requestBody.Length;
        //            webReq.ContentType = "application/json";

        //            var streamWriter = new StreamWriter(webReq.GetRequestStream(), Encoding.ASCII);
        //            streamWriter.Write(requestBody);
        //            streamWriter.Close();
        //        }

        //        responseOut = null;
        //        var response = webReq.GetResponse();

        //        var status = ((HttpWebResponse)response).StatusCode;

        //        if (response == null)
        //        {

        //            return null;
        //        }

        //        status = ((HttpWebResponse)response).StatusCode;

        //        var streamReader = new StreamReader(response.GetResponseStream());

        //        var responseContent = streamReader.ReadToEnd().Trim();

        //        if (status == HttpStatusCode.OK)
        //        {

        //            //AHSAN NEW Change 


        //            resp = JsonConvert.DeserializeObject<GenericResponse_NEWUAE_registration>(responseContent);
        //            //var jsonObject = JsonConvert.DeserializeObject<T>(resp.response);
        //            responseOut = resp;

        //            if (resp.Error == false && resp.Mrn != null)
        //            {
        //                //var jsonObject = (T)resp.response;
        //                if (resp != null)
        //                {
        //                    return resp;
        //                }

        //            }
        //            else
        //            {

        //                resp.Error = true;
        //                return resp;

        //            }
        //        }







        //        resp.Error = true;

        //        return resp;


        //        //var jsonObject = JsonConvert.DeserializeObject<T>(responseContent);

        //        //return jsonObject;
        //    }
        //    catch (WebException wex)
        //    {
        //        if (wex.Response != null)
        //        {
        //            using (var errorResponse = (HttpWebResponse)wex.Response)
        //            {
        //                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
        //                {
        //                    string errorContennt = reader.ReadToEnd().Trim();
        //                    var jsonObject = JsonConvert.DeserializeObject<PostResponse>(errorContennt);

        //                    var status = ((System.Net.HttpWebResponse)(wex.Response)).StatusCode;
        //                    Msg = jsonObject.errorMessage;
        //                    var resp = new GenericResponse_NEWUAE_registration();
        //                    resp.Error = false;
        //                    resp.Message = Msg;
        //                    responseOut = resp;
        //                    //return jsonObject;
        //                    return null;
        //                }
        //            }

        //        }

        //        //status = HttpStatusCode.InternalServerError;
        //        var resp2 = new GenericResponse_NEWUAE_registration();
        //        resp2.Error = false;
        //        resp2.Message = "InternalServerError";
        //        responseOut = resp2;

        //        return null;
        //    }

        //}


        public static string GetToken(string ForCompany)
        {

            //GetToken_SMSAPI("");


            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");
            var SQL_Qry = "  Select  TOP 1 " +
                           "CASE WHEN(DATEDIFF (SECOND, Add_Date, GETDATE ())) > (Expire_In - 300) THEN" +
                            "'' ELSE AccessToken END AccessToken   From [API_TOKEN] WHERE FORCompany = '" + ForCompany + "' ORDER BY ID Desc";


            var Token = _DB.ExecuteSQLScalar(SQL_Qry);
            if (Token == "")
            {
                //Ahsan Currently HardCode 
                // Later from DB the Token Details 
                // As may be Multilple Comapny can Intregate
                string SUBUrlToken = ConfigurationManager.AppSettings["UAEApi_BasicURL"].ToString() + "token";
                var content_Token = new[]{
                    new KeyValuePair<string, string>("grant_type", "password"),                    
                    new KeyValuePair<string, string>("username", "mobileapp"),
                    new KeyValuePair<string, string>("Password", "k0MV1NRt1W0Wt2r453JT")
                };

                var _tokenList = RestUtility.CallAPIToken<TokenModal>(SUBUrlToken, content_Token) as TokenModal;
                
                if (_tokenList != null)
				{
                    Token = _tokenList.access_token;

                    _DB.param = new SqlParameter[]
                    {
                            new SqlParameter("@AccessToken", _tokenList.access_token),
                            new SqlParameter("@Expire_In", _tokenList.expires_in),
                            new SqlParameter("@ForCompany", ForCompany),
                    };

                    _DB.ExecuteSP("[dbo].[Update_API_TOKEN_SP]");
                }
            }

            return Token;
        }



        public static TokenModal GetTokenTESTING(string ForCompany)
        {

            //GetToken_SMSAPI("");
            var ReturnTokken = new TokenModal();


            CustomDBHelper _DB = new CustomDBHelper("RECEPTION");
            var SQL_Qry = "  Select  TOP 1 " +
                           "CASE WHEN(DATEDIFF (SECOND, Add_Date, GETDATE ())) > (Expire_In - 300) THEN" +
                            "'' ELSE AccessToken END AccessToken   From [API_TOKEN_TEST] WHERE FORCompany = '" + ForCompany + "' ORDER BY ID Desc";


            var Token = _DB.ExecuteSQLScalar(SQL_Qry);
            if (Token == "")
            {
                //Ahsan Currently HardCode 
                // Later from DB the Token Details 
                // As may be Multilple Comapny can Intregate
                string SUBUrlToken = ConfigurationManager.AppSettings["UAEApi_BasicURL"].ToString() + "token";
                var content_Token = new[]{
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "mobileapp"),
                    new KeyValuePair<string, string>("Password", "k0MV1NRt1W0Wt2r453JT")
                };

                var _tokenList = RestUtility.CallAPIToken_TEST<TokenModal>(SUBUrlToken, content_Token) as TokenModal;

                if (_tokenList != null)
                {
                    Token = _tokenList.access_token;

                    _DB.param = new SqlParameter[]
                    {
                            new SqlParameter("@AccessToken", _tokenList.access_token),
                            new SqlParameter("@Expire_In", _tokenList.expires_in),
                            new SqlParameter("@ForCompany", ForCompany),
                    };

                    _DB.ExecuteSP("[dbo].[Update_API_TOKEN_TEST_SP]");
                }
                return _tokenList;
            }
            else
			{
                ReturnTokken.access_token = Token;
                ReturnTokken.token_type = "DB Token";                

            }

            return ReturnTokken;
        }




        public static string GetToken_SMSAPI(string ForCompany)
        {
            var Token = "";
            if (Token == "")
            {
                //string SUBUrlToken = ConfigurationManager.AppSettings["UAEApi_SMSURL"].ToString() + "token";
                string SUBUrlToken = "";
                SUBUrlToken = "https://smartmessaging.etisalat.ae:5676/marvel/login/user";
                var content_Token = new[]{                    
                    new KeyValuePair<string, string>("username", "itdeveloper6"),
                    new KeyValuePair<string, string>("password", "Zx10r2026!")
                };

                var _tokenList = RestUtility.CallAPI_SMSToken<UAE_SMS_TokenModal>(SUBUrlToken, content_Token) as UAE_SMS_TokenModal;

                if (_tokenList != null)
                {
                    Token = _tokenList.token;
                }
            }

            return Token;
        }

        public static string Send_UAE_SMS ()
		{
            string SUBUrlToken = "https://smartmessaging.etisalat.ae:5676/campaigns/submissions/sms/nb/";
            var content_SMS = new[]{
                    new KeyValuePair<string, string>("desc", "This is the description for campaign"),
                    new KeyValuePair<string, string>("campaignName", "KSA campaign!"),
                    new KeyValuePair<string, string>("msgCategory", "4.5"),
                    new KeyValuePair<string, string>("contentType", "3.2"),
                    new KeyValuePair<string, string>("dndCategory", "campaign"),
                    new KeyValuePair<string, string>("priority", "1"),
                    new KeyValuePair<string, string>("clientTxnId", "11234658798800"),
                    new KeyValuePair<string, string>("senderAddr", "SGHUAE"),
                    new KeyValuePair<string, string>("recipient", "966581178188"),
                    new KeyValuePair<string, string>("msg", "test sms KSA")
                };

            
            //var resp = new GenericResponse_NEWUAE_registration();
            //var _NewData = RestUtility.CallAPI_SMS_POST<RegistrationPostResponse>(SUBUrlToken, content_SMS, out resp, true);
            ////CallAPI_SMS_POST


            HttpStatusCode status;
            string RegistrationUrl = "https://smartmessaging.etisalat.ae:5676/campaigns/submissions/sms/nb/";
            UAE_SMS_Template _accData = new UAE_SMS_Template();
            //_accData = new UAE_SMS_Template()
            //{
            //    doctorID = doctorID,
            //    clinicCode = clinicCode,
            //    patient_document_id = patient_document_id,
            //    time = time,
            //    fileNumber = MRN
            //};
            var _NewData = RestUtility.CallAPI_SMS_POST<PostResponse>(RegistrationUrl, _accData, out status, true);

            var ReturnObject = new PostResponse();
            ReturnObject = _NewData as PostResponse;
            if (status == HttpStatusCode.OK)
                return "";





            return "";
		}

        public static TokenModal CallAPIToken<T>(string url, KeyValuePair<string, string>[] ObjFormCol)
        {
            try
			{
                var handler = new HttpClientHandler
                {
                    UseCookies = false
                };

                using (var client = new HttpClient())
                {
                    //CacheControlHeaderValue varHeader = new CacheControlHeaderValue();
                    //varHeader.NoCache = true;
                    
                    //client.DefaultRequestHeaders.CacheControl = varHeader;

                    client.BaseAddress = new Uri(url);
                    var content = new FormUrlEncodedContent(ObjFormCol);

					//System.Net.ServicePointManager.SecurityProtocol =
					//	SecurityProtocolType.Tls | SecurityProtocolType.Tls11
					//	| SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
					//ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };



					var result = client.PostAsync("", content).Result;

                    

                    string resultContent = result.Content.ReadAsStringAsync().Result;                    

                    var resp = JsonConvert.DeserializeObject<TokenModal>(resultContent);

                    if (resp.access_token != null)
                    {
                        return resp;
                    }
                    return null;
                }
            }
            catch(Exception ex)
			{
                return null;
			}

            return null;
        }


        public static TokenModal CallAPIToken_TEST<T>(string url, KeyValuePair<string, string>[] ObjFormCol)
        {
            
                var handler = new HttpClientHandler
                {
                    UseCookies = false
                };

                using (var client = new HttpClient())
                {
                    //CacheControlHeaderValue varHeader = new CacheControlHeaderValue();
                    //varHeader.NoCache = true;

                    //client.DefaultRequestHeaders.CacheControl = varHeader;

                    client.BaseAddress = new Uri(url);
                    var content = new FormUrlEncodedContent(ObjFormCol);

                    //System.Net.ServicePointManager.SecurityProtocol =
                    //	SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                    //	| SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                    //ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };



                    var result = client.PostAsync("", content).Result;



                    string resultContent = result.Content.ReadAsStringAsync().Result;

                    var resp = JsonConvert.DeserializeObject<TokenModal>(resultContent);

                    if (resp.access_token != null)
                    {
                        return resp;
                    }
                    return null;
                }
            
            

            return null;
        }





        public static UAE_SMS_TokenModal CallAPI_SMSToken<T>(string url, KeyValuePair<string, string>[] ObjFormCol)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    UseCookies = false
                };

                using (var client = new HttpClient())
                {
                    //CacheControlHeaderValue varHeader = new CacheControlHeaderValue();
                    //varHeader.NoCache = true;

                    //client.DefaultRequestHeaders.CacheControl = varHeader;

                    client.BaseAddress = new Uri(url);
                    var content = new FormUrlEncodedContent(ObjFormCol);

                    //System.Net.ServicePointManager.SecurityProtocol =
                    //	SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                    //	| SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                    //ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };


                    
                    var result = client.PostAsync("", content).Result;



                    string resultContent = result.Content.ReadAsStringAsync().Result;

                    var resp = JsonConvert.DeserializeObject<UAE_SMS_TokenModal>(resultContent);

                    if (resp.token != null)
                    {
                        return resp;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }



    }
}