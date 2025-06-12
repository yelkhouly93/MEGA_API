using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer.Data
{
	public class HomeCareDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public DataTable SavePatientInfo(string PatientMRN, int BranchID, string fullname,
          string Gender, string Age, string DOB, string Email, string MobileNUmber, 
          string Address , string buildingNo , string Appartment , string LandMark , string LocationURL , string lang
          ,ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@RegistrationNo", PatientMRN),
                new SqlParameter("@FullName", fullname),
                new SqlParameter("@Gender", Gender),
                new SqlParameter("@Age", Age),
                new SqlParameter("@DOB", DOB),
                new SqlParameter("@Email", Email),
                new SqlParameter("@MobileNumber", MobileNUmber),
                new SqlParameter("@Address", Address),
                new SqlParameter("@BuildingNo", buildingNo),
                new SqlParameter("@AppartmentNumber", Appartment),
                new SqlParameter("@LandMark", LandMark),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                new SqlParameter("@LocationURL", LocationURL),
                new SqlParameter("@Lang", lang)

            };

            DB.param[12].Direction = ParameterDirection.Output;
            DB.param[13].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Save_HC_PatientInfo_SP]");

            errStatus = Convert.ToInt32(DB.param[12].Value);
            errMessage = DB.param[13].Value.ToString();

            return dataTable;
        }



        public DataTable GetMasterList(string lang, int BranchID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchID", BranchID),                
                new SqlParameter("@Lang", lang)
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_HC_MasterList_SP]");
            return dataTable;
        }


        public DataTable GetPatientAddress(string lang, int BranchID , string MRN )
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@Lang", lang),
                new SqlParameter("@RegistraionNo", MRN)
                
            };

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_HC_Patient_Addres_SP]");

            return dataTable;
        }



        public DataTable Save_Patient_HC_Request(string PatientMRN, int BranchID, string AppoitmentDate,
          string AppoitmentTime, string ServiceIds, string Comments
          , ref int errStatus, ref string errMessage , ref string AdditionalOutput)
        {
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@RegistrationNo", PatientMRN),
                new SqlParameter("@AppoitmentDate", AppoitmentDate),
                new SqlParameter("@AppoitmentTime", AppoitmentTime),
                new SqlParameter("@ServiceIds", ServiceIds),
                new SqlParameter("@Comments", Comments),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                new SqlParameter("@Additional", SqlDbType.NVarChar, 500)
            };

            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;
            DB.param[8].Direction = ParameterDirection.Output;

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Save_HC_Patient_Request_SP]");

            errStatus = Convert.ToInt32(DB.param[6].Value);
            errMessage = DB.param[7].Value.ToString();
            AdditionalOutput = DB.param[8].Value.ToString();

            return dataTable;
        }


        public List<HC_PatientRequest> GetPatientRequestList(string lang, int BranchID, string MRN)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@Lang", lang),
                new SqlParameter("@RegistraionNo", MRN)

            };

            var dataTableSet = DB.ExecuteSPAndReturnDataSet ("[dbo].[Get_HC_Patient_Request_SP]");
            var DataList = MappingHCRequestList(dataTableSet);
            return DataList;
        }
        private List<HC_PatientRequest> MappingHCRequestList(DataSet patientDataset)
        {
            try
            {
                var DataList = new List<HC_PatientRequest>();

                if (patientDataset != null && patientDataset.Tables[0] != null && patientDataset.Tables[0].Rows.Count > 0)
                {
                    var ReportrequestTable = patientDataset.Tables[0].ToListObject<HC_PatientRequest>();
                    foreach (var row in ReportrequestTable)
                    {
                        var ReportData = new HC_PatientRequest();

                        ReportData.AddDate = row.AddDate;
                        ReportData.BranchID = row.BranchID;
                        ReportData.RegistrationNo = row.RegistrationNo;
                        ReportData.RequestID = row.RequestID;
                        ReportData.ShowCancel = row.ShowCancel;
                        ReportData.ShowConfirm = row.ShowConfirm;
                        ReportData.ShowPay = row.ShowPay;
                        ReportData.StatusID = row.StatusID;
                        ReportData.StatusValue = row.StatusValue;
                        ReportData.TotalAmount = row.TotalAmount;
                        ReportData.CurrnecyFlag = row.CurrnecyFlag;


                        // NOW Adding Service list
                        DataRow[] RRsubDataTableRow = patientDataset.Tables[1].Select("RequestID=" + row.RequestID);
                        if (RRsubDataTableRow != null)
                        {
                            var AttachList = new List<HC_PatientRequestService>();
                            foreach (DataRow Subrow in RRsubDataTableRow)
                            {
                                var HCServiceItems = new HC_PatientRequestService();

                                HCServiceItems.BranchID = (int)Subrow["BranchID"];
                                HCServiceItems.MSID = (int)Subrow["MSID"];
                                HCServiceItems.RegistrationNo = Subrow["RegistrationNo"].ToString();
                                HCServiceItems.RequestID = (int)Subrow["RequestID"];
                                HCServiceItems.ServiceID = (int)Subrow["ServiceID"];
                                HCServiceItems.ServiceName = Subrow["ServiceName"].ToString();
                                

                                AttachList.Add(HCServiceItems);
                            }
                            ReportData.ServicesList = AttachList;
                        }
                        DataList.Add(ReportData);
                    }
                }

                return DataList;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }


        public DataTable Cancel_Patient_HC_Request(string PatientMRN, int BranchID, int RequestID, string Lang , string Reason
                         , ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@RegistrationNo", PatientMRN),
                new SqlParameter("@RequestID", RequestID),
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@Reason", Reason),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)                
            };

            DB.param[5].Direction = ParameterDirection.Output;
            DB.param[6].Direction = ParameterDirection.Output;
            

            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Cancel_HC_Patient_Request_SP]");

            errStatus = Convert.ToInt32(DB.param[5].Value);
            errMessage = DB.param[6].Value.ToString();
           

            return dataTable;
        }



        public DataTable Update_Patient_Other_Mobile(string PatientMRN, int BranchID, string OtherMobile, string Lang)
        {
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@RegistrationNo", PatientMRN),                
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@OtherMobile", OtherMobile)                
            };



            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Update_HC_Patient_OtherMobile_SP]");



            return dataTable;
        }

        public DataTable Update_Patient_RequestType(string PatientMRN, int BranchID, int RequestID, string Lang, string RequestType)
        {
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@RegistrationNo", PatientMRN),
                new SqlParameter("@RequestID", RequestID),
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@RequestType", RequestType)
            };



            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Update_HC_Patient_RequestType_SP]");



            return dataTable;
        }


        public DataTable PaymentConfirmation_Patient_RequestType(string PatientMRN, int BranchID, int RequestID, string Lang, string PaymentAmount , string PaymentRef )
        {
            DB.param = new SqlParameter[]
            {

                new SqlParameter("@BranchID", BranchID),
                new SqlParameter("@RegistrationNo", PatientMRN),
                new SqlParameter("@RequestID", RequestID),
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@PaymentAmount", PaymentAmount),
                new SqlParameter("@PaymentRef", PaymentRef)
            };



            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[PaymentConfirmation_HC_Patient_Request_SP]");



            return dataTable;
        }


    }
}
