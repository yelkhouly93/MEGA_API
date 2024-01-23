using DataLayer.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLayer.Data
{
	public class NotificationDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        /********* Patient Perscription NOTIFICATION AREA ***********/
        public DataTable Get_Notification_PatientPrescription_DT(string lang, int hospitaId, int PatintMRN, int VisitId, int PrescriptionId, int DrugId, string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@PatientMRN", PatintMRN),
                new SqlParameter("@VisitId", VisitId),
                new SqlParameter("@PrescriptionId", PrescriptionId),
                new SqlParameter("@DrugId", DrugId),
                new SqlParameter("@Source", Source),                
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Notification_PatientPrescription_Details_SP]");

            errStatus = Convert.ToInt32(DB.param[6].Value);
            errMessage = DB.param[7].Value.ToString();

            return dt;
        }

        public void SAVE_Notification_PatientPrescription_DT(string lang,int hospitaId, int PatintMRN, int VisitId, int PrescriptionId, int DrugId, string Source,DateTime StartDate , DateTime StartTime, string StartTimeName, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@PatientMRN", PatintMRN),
                new SqlParameter("@VisitId", VisitId),
                new SqlParameter("@PrescriptionId", PrescriptionId),
                new SqlParameter("@DrugId", DrugId),
                new SqlParameter("@Source", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                new SqlParameter("@StartDate", StartDate),
                new SqlParameter("@StartTimeName", StartTimeName),
                new SqlParameter("@StartTime", StartTime),
            };

            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[SAVE_Notification_PatientPrescription_SP]");

            errStatus = Convert.ToInt32(DB.param[6].Value);
            errMessage = DB.param[7].Value.ToString();

            //return dt;
        }

        public void Cancel_Notification_PatientPrescription_DT(string lang, int hospitaId, int PatintMRN, int VisitId, int PrescriptionId, int DrugId, string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@PatientMRN", PatintMRN),
                new SqlParameter("@VisitId", VisitId),
                new SqlParameter("@PrescriptionId", PrescriptionId),
                new SqlParameter("@DrugId", DrugId),
                new SqlParameter("@Source", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            DB.ExecuteSP("[dbo].[Cancel_Notification_PatientPrescription_SP]");

            errStatus = Convert.ToInt32(DB.param[6].Value);
            errMessage = DB.param[7].Value.ToString();

            
        }

        public void Cancel_ALL_Notification_PatientPrescription(string lang, int hospitaId, int PatintMRN, string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@PatientMRN", PatintMRN),                
                new SqlParameter("@Source", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 500)
            };

            DB.param[3].Direction = ParameterDirection.Output;
            DB.param[4].Direction = ParameterDirection.Output;

            DB.ExecuteSP("[dbo].[Cancel_All_Notification_PatientPrescription_SP]");

            errStatus = Convert.ToInt32(DB.param[3].Value);
            errMessage = DB.param[4].Value.ToString();


        }

        /**** ///////// END Patient Perscription NOTIFICATION AREA /////// ***/


        //public DataTable Get_Notification_MissAppointment_DT(string lang, int hospitaId, int PatintMRN, string Source, ref int errStatus, ref string errMessage)
        //{
        //    DB.param = new SqlParameter[]
        //    {
        //        new SqlParameter("@BranchId", hospitaId),
        //        new SqlParameter("@PatientMRN", PatintMRN),                
        //        new SqlParameter("@Source", Source),
        //        new SqlParameter("@status", SqlDbType.Int),
        //        new SqlParameter("@msg", SqlDbType.NVarChar, 500)
        //    };

        //    DB.param[3].Direction = ParameterDirection.Output;
        //    DB.param[4].Direction = ParameterDirection.Output;

        //    var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Notification_MissAppoitment_List_SP]");

        //    errStatus = Convert.ToInt32(DB.param[3].Value);
        //    errMessage = DB.param[4].Value.ToString();

        //    return dt;
        //}

        public DataTable Get_Notification_PatientSuggestion_DT(string lang, int hospitaId, int PatintMRN, string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {                
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitaId),
                new SqlParameter("@RegistrationNo", PatintMRN),
                //new SqlParameter("@Source", Source)
            };

            //DB.param[3].Direction = ParameterDirection.Output;
            //DB.param[4].Direction = ParameterDirection.Output;

            var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_Notification_SuggestedAppoitment_List_SP]");

            //errStatus = Convert.ToInt32(DB.param[3].Value);
            //errMessage = DB.param[4].Value.ToString();

            return dt;
        }



        public DataTable Get_FCMNotification_DT(string TagData, string TagID)
        {
            DB.param = new SqlParameter[]
            {   
                new SqlParameter("@TagData", TagData),
                new SqlParameter("@TagID", TagID)                
            };

            var dt = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_FCM_Notification_SP]");

            return dt;
        }

    }
}
