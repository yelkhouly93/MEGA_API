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
    public class PatientPrescriptionApiCaller
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        CommonDB _commonDb = new CommonDB();

        string apiUserName = ConfigurationManager.AppSettings["MobileWebApi_UserName"].ToString();
        string apiPassword = ConfigurationManager.AppSettings["MobileWebApi_Password"].ToString();
        string apiFixedPatientId = ConfigurationManager.AppSettings["MobileWebApi_FixedPatientId"].ToString();

        public List<PatientPrescription> GetPatientPrescription(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg)
        {
            HttpStatusCode status;
            List<PatientPrescription> _listOfPatientMedications = new List<PatientPrescription>();

            string apiBasic = ConfigurationManager.AppSettings["MobileWebApi_BasicURL_" + hospitalId.ToString()].ToString();
            string GetPatientDiagnosisUrl = apiBasic + ConfigurationManager.AppSettings["MobileWebApi_FetchPatientMedications_" + hospitalId.ToString()].ToString();

            int patientIDFromDatabase = _commonDb.GetPateintIdAgainstMrn(hospitalId, resgistrationNo);

            GetPatientDiagnosisUrl = GetPatientDiagnosisUrl.Replace("{patientId}", patientIDFromDatabase.ToString());

            List<PatientMedicationsFromApi> _patientPrescriptionModel = RestUtility.CallService<List<PatientMedicationsFromApi>>(GetPatientDiagnosisUrl, null, null, "GET", apiUserName, apiPassword, out status) as List<PatientMedicationsFromApi>;

            _listOfPatientMedications = MapPatientPrescriptionModelApiToPatientPrescription(_patientPrescriptionModel);

            return _listOfPatientMedications;

        }

        private List<PatientPrescription> MapPatientPrescriptionModelApiToPatientPrescription(List<PatientMedicationsFromApi> _patientDaignosisModel)
        {
            try
            {
                List<PatientPrescription> _listOfPatientDiagnosis = new List<PatientPrescription>();

                if (_patientDaignosisModel != null && _patientDaignosisModel.Count > 0)
                {
                    _patientDaignosisModel.ForEach(p =>
                    {
                        string[] values = p.descriptionLines[0].line.Split(new char[0]);

                        string strDuration = values[values.Length - 2] + " " + values[values.Length - 1];
                        string strRoute = values[2];

                        _listOfPatientDiagnosis.Add(new PatientPrescription()
                        {
                            visit_id = 0,
                            doctor_name = "",
                            prescription_date = DateTime.Now,
                            drug_name = p.mainDescription,
                            route = strRoute,
                            duration = strDuration
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