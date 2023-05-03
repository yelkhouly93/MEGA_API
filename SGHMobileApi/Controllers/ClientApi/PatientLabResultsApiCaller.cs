using DataLayer.Common;
using DataLayer.Model;
using Newtonsoft.Json.Linq;
using RestClient;
using SGHMobileApi.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace SmartBookingService.Controllers.ClientApi
{
    public class PatientLabResultsApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public TestResultMain GetPatientLabResultsByApi(string lang, int hospitalID, int testId, ref int Er_Status, ref string Msg)
        {

            HttpStatusCode status;
            TestResultMain testResult = new TestResultMain();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_LabResult"];
            string apiKey = ConfigurationManager.AppSettings["MobileWebApi_ApiToken"];

            string GetLabResultUrl = apiBasic + testId.ToString();

            var testOrders = RestUtility.CallService<TestResult>(GetLabResultUrl, null, "", "POST", "", "", out status, apiKey) as TestResult;

            if (testOrders.responseCode == 0)
            {
                testResult = MapTestResultModelToTestResultMain(testOrders);
            }
            

            return testResult;

        }

        private TestResultMain MapTestResultModelToTestResultMain(TestResult testOrders)
        {
            TestResultMain testResultMain = new TestResultMain();
            List<TestResultParameter> testParameters = new List<TestResultParameter>();

            testResultMain.testCode = testOrders.entity.testCode;
            testResultMain.testName = testOrders.entity.testName;
            testResultMain.section = testOrders.entity.section;
            
            testResultMain.sample_name = testOrders.entity.sampleName;
            testResultMain.collected_date = testOrders.entity.collectedDt;

            
            // For Testing
            var icount = testOrders.entity.testParameters.Count();
            //var itst = 1;

            if (testOrders.entity.testParameters.Count > 0)
            {
                foreach (var param in testOrders.entity.testParameters)
                {
                    
                    TestResultParameter parameter = new TestResultParameter();

                    if (param.parameterName == "Sendout File Result")
                    {
                        continue;
                    }


                    parameter.parameter_name = param.parameterName ?? "";
                    parameter.result = param.result ?? "";
                    parameter.unit = param.unit ?? "";
                    parameter.range = param.limitRange ?? "";
                    parameter.ResultValueCategory = param.ResultValueCategory ?? "N";

                    //parameter.severityID = "N";
                    parameter.rating = param.rating;
                    parameter.severityID = param.rating;

                    if (parameter.result != "")
                    {
                        var tempResult = parameter.result;

                        if (tempResult.Substring(0, 1) == ".")
                        {
                            parameter.result = "0" + tempResult;
                        }
                        else if (tempResult.EndsWith("."))
                        {
                            parameter.result = tempResult + "0";
                        }
                    }



                    if (param.rating == null || param.rating.Trim() == "")
                        parameter.severityID = "N";

                    parameter.Weightage = 0;

                    if (parameter.severityID == "N")
                        parameter.Weightage = 0;
                    else if (parameter.severityID == "H")
                        parameter.Weightage = 50;
                    else if (parameter.severityID == "L")
                        parameter.Weightage = 50;
                    else if (parameter.severityID == "P")
                        parameter.Weightage = 100;


                    if (parameter.parameter_name == "RAD. REPORT")
                        parameter.parameter_name = "RADIOLOGY REPORT";

                    parameter.parameter_name = parameter.parameter_name.Replace(":", "").Replace(".","");

                    //if (parameter.parameter_name != "EXAMINATION :" && parameter.parameter_name != "CLINICAL DATA :")
                    //{

                    //    if (itst <= 1)
                    //        parameter.severityID = "P";
                    //    else if (itst <= 2)
                    //        parameter.severityID = "L";
                    //    else if (itst <= 3)
                    //        parameter.severityID = "H";


                    //}
                    //else
                    //{
                    //    //if (itst <= 1 )
                    //    //    parameter.severityID = "P";
                    //    //else if (itst <= 2)
                    //    //    parameter.severityID = "L";
                    //    //else if (itst <= 3)
                    //    //    parameter.severityID = "H";


                    //}


                    testParameters.Add(parameter);

                    //itst += 1;

                }
            }

            testParameters = testParameters.OrderByDescending(o => o.Weightage).ToList();

            testResultMain.parameters = testParameters;

            return testResultMain;

        }


        public TestResult GetPatientTestResultsAPI(string lang, int hospitalID, int testId, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;
            TestResultMain testResult = new TestResultMain();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_LabResult"];
            string apiKey = ConfigurationManager.AppSettings["MobileWebApi_ApiToken"];
            TestResult testOrders = new TestResult();

            string GetLabResultUrl = apiBasic + testId.ToString();
            try
            {
                testOrders = RestUtility.CallService<TestResult>(GetLabResultUrl, null, "", "POST", "", "", out status, apiKey) as TestResult;
            }
            catch
            {

            }
            

            return testOrders;

        }


    }
}