
using DataLayer.Common;
using DataLayer.Data;
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
    public class PatientDiagnosisApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        CommonDB _commonDb = new CommonDB();

        string apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
        string apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
        string apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

        public List<PatientDiagnosis> GetPatientDiagnosis(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;
            List<PatientDiagnosis> _listOfPatientDiagnosis = new List<PatientDiagnosis>();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalId.ToString()].ToString();
            string GetPatientDiagnosisUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_FetchPatientDiagnosis_" + hospitalId.ToString()].ToString();

            int patientIDFromDatabase = _commonDb.GetPateintIdAgainstMrn(hospitalId, resgistrationNo);

            GetPatientDiagnosisUrl = GetPatientDiagnosisUrl.Replace("{patientId}", patientIDFromDatabase.ToString());

            List<PatientDiagnosisFromApi> _patientDiagnosisModel = RestUtility.CallService<List<PatientDiagnosisFromApi>>(GetPatientDiagnosisUrl, null, null, "GET", apiUserName, apiPassword, out status) as List<PatientDiagnosisFromApi>;

            _listOfPatientDiagnosis = MapPatientDiagnosisModelApiToPatientDiagnosis(_patientDiagnosisModel);

            return _listOfPatientDiagnosis;

        }

        private List<PatientDiagnosis> MapPatientDiagnosisModelApiToPatientDiagnosis(List<PatientDiagnosisFromApi> _patientDiagnosisModel)
        {
            try
            {
                List<PatientDiagnosis> _listOfPatientDiagnosis = new List<PatientDiagnosis>();

                if (_patientDiagnosisModel != null && _patientDiagnosisModel.Count > 0)
                {
                    _patientDiagnosisModel.ForEach(p =>
                    {
                        _listOfPatientDiagnosis.Add(new PatientDiagnosis()
                        {
                            visit_id = 0,
                            visit_type = "Out-Patient",
                            diagnosis_desc = p.DESC1 + (p.DESC2 != null ? " " + p.DESC2 : "") + (p.DESC3 != null ? " " + p.DESC3 : ""),
                            diagnosis_datetime = p.DATE_RECORDED
                        });
                    });
                }

                return _listOfPatientDiagnosis;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
