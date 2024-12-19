using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace DataLayer.Data
{
	public class VitalSignDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public DataTable GET_Patient_VitalSign_GraphData(string Lang, int PatientMRN, int BranchID, string Source)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@Source", Source),                    
                    new SqlParameter("@Er_Status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
                };
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientVitals_SP]");
            return ReturnDataTable;
        }


        public DataSet GET_Patient_My_VitalSign_DT(string Lang, int PatientMRN, int BranchID)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID)                    
                };

            var ReturnDataTable = DB.ExecuteSPAndReturnDataSet("[dbo].[Get_PatientVitalDevice_SP]");
            return ReturnDataTable;
        }

        public DataTable GET_Patient_My_VitalSign_Detail_DT(string Lang, int PatientMRN, int BranchID ,  string VitalSign)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", Lang),
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@Vital", VitalSign)
                };

            var ReturnDataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_PatientMyVitals_Details_SP]");
            return ReturnDataTable;
        }

        public bool Save_Patient_Divice_VitalSign(string PatientMRN, int BranchID , string VitalName , string VitalValue, string VitalValue2 , string DeviceName,string Source, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
                {                    
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@VitalName", VitalName),
                    new SqlParameter("@VitalValue", VitalValue),
                    new SqlParameter("@VitalValue2", VitalValue2),
                    new SqlParameter("@DeveiceName", DeviceName),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 500),
                    new SqlParameter("@Source", Source)
                };
            
            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteNonQuerySP("[dbo].[Save_PatientDeviceVitals_SP]");
            
            errStatus = Convert.ToInt32(DB.param[6].Value);
            errMessage = DB.param[7].Value.ToString();

            if (errStatus == 1)
                return true;


            return false;
        }


        public bool Save_Patient_Divice_VitalSign_BULK(string PatientMRN, int BranchID, 
            string DeviceName, 
            string Source, 
            string HEIGHT,
            string HEART_RATE,
            string BODY_TEMPERATURE,
            string BLOOD_PRESSURE_DIASTOLIC,
            string BLOOD_PRESSURE_SYSTOLIC,
            string BLOOD_OXYGEN,
            string STEPS,
            string ACTIVE_ENERGY_BURNED,
            string BLOOD_GLUCOSE,
            string BODY_FAT_PERCENTAGE,
            string BODY_MASS_INDEX,
            string SLEEP_IN_BED,
            string WEIGHT,
            string WATER,
            ref int errStatus,
            ref string errMessage)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@RegistrationNo", PatientMRN),
                    new SqlParameter("@BranchID", BranchID),
                    new SqlParameter("@HEIGHT", HEIGHT),
                    new SqlParameter("@HEART_RATE", HEART_RATE),
                    new SqlParameter("@BODY_TEMPERATURE", BODY_TEMPERATURE),
                    new SqlParameter("@BLOOD_PRESSURE_DIASTOLIC", BLOOD_PRESSURE_DIASTOLIC),
                    new SqlParameter("@BLOOD_PRESSURE_SYSTOLIC", BLOOD_PRESSURE_SYSTOLIC),
                    new SqlParameter("@BLOOD_OXYGEN", BLOOD_OXYGEN),
                    new SqlParameter("@STEPS", STEPS),
                    new SqlParameter("@ACTIVE_ENERGY_BURNED", ACTIVE_ENERGY_BURNED),
                    new SqlParameter("@BLOOD_GLUCOSE", BLOOD_GLUCOSE),
                    new SqlParameter("@BODY_FAT_PERCENTAGE", BODY_FAT_PERCENTAGE),
                    new SqlParameter("@BODY_MASS_INDEX", BODY_MASS_INDEX),
                    new SqlParameter("@SLEEP_IN_BED", SLEEP_IN_BED),
                    new SqlParameter("@WEIGHT", WEIGHT),
                    new SqlParameter("@WATER", WATER),
                    new SqlParameter("@Source", Source),
                    new SqlParameter("@DeveiceName", DeviceName),
                    new SqlParameter("@status", SqlDbType.Int),
                    new SqlParameter("@msg", SqlDbType.NVarChar, 500)
                    
                };

            DB.param[18].Direction = ParameterDirection.Output;
            DB.param[19].Direction = ParameterDirection.Output;

            var ReturnDataTable = DB.ExecuteNonQuerySP("[dbo].[Save_PatientDeviceVitals_Bulk_SP]");

            errStatus = Convert.ToInt32(DB.param[18].Value);
            errMessage = DB.param[19].Value.ToString();

            if (errStatus == 1)
                return true;


            return false;
        }

    }
}
