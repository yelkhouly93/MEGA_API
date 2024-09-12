using System.Collections.Generic;
using System.Web.Http;
using DataLayer.Model;
using DataLayer.Reception.Business;
using SGHMobileApi.Extension;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using DataLayer.Data;
using System;
using System.Data;
using System.Net.Http.Formatting;
using SGHMobileApi.Common;
using System.Data.SqlClient;
using System.Configuration;
using RestClient;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Renci.SshNet;
using HeyRed.Mime;

namespace SGHMobileApi.Controllers
{
    public class FileDownloadController : ApiController
    {

        [HttpGet]
        [Route("v2/uae_RR_fILE")]
        [ResponseType(typeof(HttpResponseMessage))]
        public IHttpActionResult Get_UAE_Report_File(string FileName)
        {
            //SFTP
            string _sftpHost = "130.6.211.148";
            string _sftpUsername = "cmsadmin";
            string _sftpPassword = "Passshared2024";
            string _remoteFilePath = "/ReqRpt/" + FileName;


            string fileName = Path.GetFileName(FileName);
            // Determine the MIME type of the file
            var  contentType = MimeTypesMap.GetMimeType(FileName);

            byte[] fileBytes;
            using (var sftp = new SftpClient(_sftpHost, _sftpUsername, _sftpPassword))
            {
                sftp.Connect();
                using (var memoryStream = new MemoryStream())
                {
                    // Download file from SFTP server
                    sftp.DownloadFile(_remoteFilePath, memoryStream);
                    fileBytes = memoryStream.ToArray();
                }
                sftp.Disconnect();

                // Create HttpResponseMessage
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileBytes)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };

                return ResponseMessage(response);
            }
            return null;

        }
    }
}