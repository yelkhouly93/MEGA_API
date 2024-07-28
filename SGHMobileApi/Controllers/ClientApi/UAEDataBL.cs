using DataLayer.Common;
using Newtonsoft.Json;
using RestClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace SmartBookingService.Controllers.ClientApi
{
    public class UAEDataBL
    {
        private CustomDBHelper _DB = new CustomDBHelper("SurveyDb");
        public int AppUserID = 0;

  //      public PatientDataView InitializePatientDataView()
  //      {
  //          PatientDataView ObjView = new PatientDataView();
  //          _DB = new CustomDBHelper("SurveyDb");
  //          ObjView.ddl_BranchList = FuncCommon.GetListFormDB(_DB, "Get_Hospitals2_SP");
  //          return ObjView;
  //      }


  //      public PatientInformation GetPatientInformation(int BranchID, string MRN)
  //      {
  //          PatientInformation _Obj = new PatientInformation();

  //          string apiBasicURL = ConfigurationManager.AppSettings["MobileApi_BasicURL"].ToString();
  //          string SUBUrl = apiBasicURL + ConfigurationManager.AppSettings["MobileApi_PatientData"].ToString();

  //          var content = new[]{
  //                  new KeyValuePair<string, string>("hospital_id", BranchID.ToString()),
  //                  new KeyValuePair<string, string>("patient_reg_no", MRN)
  //          };
  //          _Obj = RestUtility.CallAPI<PatientInformation>(SUBUrl, content) as PatientInformation;

  //          return _Obj;
  //      }

  //      public List<LabTest> GetPatientLabReports (int BranchID, string MRN)
  //      {

  //          var _List = new List<LabTest>();

  //          string apiBasicURL = ConfigurationManager.AppSettings["MobileApi_BasicURL"].ToString();
  //          string SUBUrl = apiBasicURL + ConfigurationManager.AppSettings["MobileApi_LabReport"].ToString();

  //          var content = new[]{
  //                  new KeyValuePair<string, string>("hospital_id", BranchID.ToString()),
  //                  new KeyValuePair<string, string>("patient_reg_no", MRN)
  //          };
  //          var _Listtem = RestUtility.CallAPI<List<LabTest>>(SUBUrl, content) as List<LabTest>;


  //          //var results = (from myobject in _Listtem
  //          //              where myobject.report_type == "LAB"
  //          //              select myobject) as List<LabTest>;


  //          //_List = _Listtem.Where(o => o.report_type == "LAB") as List<LabTest>;

  //          return _Listtem;
  //      }

  //      public List<MedicalReport> GetPatientMedicalReports(int BranchID, string MRN)
  //      {
  //          string apiBasicURL = ConfigurationManager.AppSettings["MobileApi_BasicURL"].ToString();
  //          string SUBUrl = apiBasicURL + ConfigurationManager.AppSettings["MobileApi_MedicalReport"].ToString();

  //          var content = new[]{
  //                  new KeyValuePair<string, string>("hospital_id", BranchID.ToString()),
  //                  new KeyValuePair<string, string>("patient_reg_no", MRN)
  //          };
  //          var _Listtem = RestUtility.CallAPI<List<MedicalReport>>(SUBUrl, content) as List<MedicalReport>;
  //          return _Listtem;
  //      }

  //      public bool ArabotWebhook_insurance_sent_request()
  //      {
  //          var insuranceObj = new insurance_sent_request();

  //          // Inject Varaible Area
  //          List<inject_variables> objListinject = new List<inject_variables>();
  //          inject_variables ObjInject = new inject_variables();
  //          inject_variables_values ObjInjectValues = new inject_variables_values();

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "doctor";
  //          //ObjInjectValues.values = "2970";
  //          ObjInjectValues.value = "Mahmoud Salah";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);


  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "speciality";
  //          ObjInjectValues.value = "Vascular Surgery";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "day";
  //          ObjInjectValues.value = "Monday";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();

  //          ObjInject.name = "date";
  //          ObjInjectValues.value = "30 Jan 2023";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);


  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "time";
  //          ObjInjectValues.value = "03:15 PM";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "branch";
  //          ObjInjectValues.value = "Jeddah";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "global.branch_id";
  //          ObjInjectValues.value = "1";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "global.speciality_id";
  //          ObjInjectValues.value = "158";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "global.app_id";
  //          ObjInjectValues.value = "3612040";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "global.mrn";
  //          ObjInjectValues.value = "1597857";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "global.doctor_id";
  //          ObjInjectValues.value = "2970";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);
            

  //          ObjInject = new inject_variables();
  //          ObjInjectValues = new inject_variables_values();
  //          ObjInject.name = "global.date";
  //          ObjInjectValues.value = "01/30/2023";
  //          ObjInject.values = ObjInjectValues;
  //          objListinject.Add(ObjInject);



            


  //          insuranceObj.inject_variables = objListinject;

  //          language oBJlANG = new language();
  //          oBJlANG.code = "en";
  //          insuranceObj.template_information.language = oBJlANG;


  //          string[] objParameter = new string[] { };
  //          //objParameter = objParameter.Append("ar").ToArray();
  //          objParameter = objParameter.Append("Dr. Mahmoud Salah").ToArray();
  //          objParameter = objParameter.Append("Vascular Surgery").ToArray();
  //          objParameter = objParameter.Append("Monday").ToArray();
  //          objParameter = objParameter.Append("30 Jan 2023").ToArray();
  //          objParameter = objParameter.Append("03:15 PM").ToArray();
  //          objParameter = objParameter.Append("Jeddah").ToArray();
            

  //          insuranceObj.template_information.parameters = objParameter;


  //          List<component_parameters> ListObjComponent = new List<component_parameters>();
  //          component_parameters ObjComponent = new component_parameters();


  //          List<component_parameters_button> ListObjbuttons = new List<component_parameters_button>();
  //          component_parameters_button objButton = new component_parameters_button();
  //          objButton = new component_parameters_button();
  //          objButton.index = "0";
  //          objButton.payload = "postback_payload_1827";
  //          objButton.sub_type = "quick_reply";
  //          ListObjbuttons.Add(objButton);
  //          //ObjComponent.buttons = ListObjbuttons;
  //          //ListObjComponent.Add(ObjComponent);

  //          objButton = new component_parameters_button();
  //          objButton.index = "1";
  //          objButton.payload = "postback_payload_1832";
  //          objButton.sub_type = "quick_reply";
  //          ListObjbuttons.Add(objButton);
  //          //ObjComponent.buttons = ListObjbuttons;
  //          //ListObjComponent.Add(ObjComponent);

  //          objButton = new component_parameters_button();
  //          objButton.index = "2";
  //          objButton.payload = "postback_payload_1652";
  //          objButton.sub_type = "quick_reply";
  //          ListObjbuttons.Add(objButton);
            
            
            
  //          ObjComponent.buttons = ListObjbuttons;
  //          ListObjComponent.Add(ObjComponent);

  //          insuranceObj.template_information.component_parameters = ListObjComponent;


  //          insuranceObj.token = "new_appointment";
  //          insuranceObj.template_information.name = "new_appointment";


  //          var requestBody = JsonConvert.SerializeObject(insuranceObj);

  //          HttpStatusCode status;


  //          var _Listtem = RestUtility.CallWhatsappWebhook<object>("https://stclab.arabot.io/receiver/intent_webhook/bots/216/intents/1746", insuranceObj, out status);
  //          if (status != null)
  //          {

  //          }
  //          return true;
  //      }

  //      public bool UpdatePatient_MRN_CMS(UpdateMobileCMS ObjViewPost)
		//{   
  //          var qry = " UPDATE [HIS_DTGW_CNTRL].UNI_HIS.[dbo].Patient_stg SET PCellNo = '"+ ObjViewPost.Mobile  + "' , LastMobileUpdateDateTime = GEtDate() WHERE registrationno = '" + ObjViewPost.PatientMRN + "' and branchID = '"+ ObjViewPost.BranchId  + "'";
  //          var tnmp = _DB.ExecuteNonQuery(qry);
  //          var strqry = "Update Patient Mobile Patient ID = " + ObjViewPost.PatientMRN + " -  Branch Id = " + ObjViewPost.BranchId.ToString() + " - NEw Number = " + ObjViewPost.Mobile;
  //          var Username = HttpContext.Current.User.Identity.Name;
  //          var logID = FormCookiesValues.GetLogID(Username);
  //          var userid = FormCookiesValues.GetUserID(Username);
  //          var QueryURL = HttpContext.Current.Request.Url.AbsoluteUri;

  //          _DB = new CustomDBHelper();
  //          _DB.param = new SqlParameter[]
  //          {
  //                      new SqlParameter("@UserId", userid),
  //                      new SqlParameter("@LogID", logID),
  //                      new SqlParameter("@Query", strqry),
  //                      new SqlParameter("@QType", "UpdateMobileINcms"),
  //                      new SqlParameter("@AppID", 1),
  //                      new SqlParameter("@QURL", QueryURL)
  //          };
  //          _DB.ExecuteSP("DBO.[Insert_transaction_log_SP]");

            
  //          return true;
		//}
    }
}