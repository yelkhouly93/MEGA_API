using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace DataLayer.Data
{
    public class ClinicDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        //public List<Clinics> GetAllClinics(string lang, int hospitalID, int pageno = -1, int pagesize = 10)
        //{
        //    DB.param = new SqlParameter[]
        //    {
        //        new SqlParameter("@Lang", lang),
        //        new SqlParameter("@BranchId", hospitalID),
        //        new SqlParameter("@PageNo", pageno),
        //        new SqlParameter("@PageSize", pagesize)
        //    };

        //    var _allClinics = new List<Clinics>();

        //    _allClinics = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Clinics_SP]").ToListObject<Clinics>();

        //    return _allClinics;

        //}

        public List<Clinics> GetAllClinicsRefillMedication(string lang, int hospitalID,int medRefillReq, int pageno = -1, int pagesize = 10)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@PageNo", pageno),
                new SqlParameter("@PageSize", pagesize),
                new SqlParameter("@MedRefillReq", medRefillReq),
            };

            var _allClinics = new List<Clinics>();

            _allClinics = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Clinics_SP]").ToListObject<Clinics>();

            return _allClinics;

        }


        
            public DataTable GetAllClinicsDataTable_V2_ARA(string lang, int hospitalID, int pageno = -1, int pagesize = 10)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@PageNo", pageno),
                    new SqlParameter("@PageSize", pagesize)
                };

            var allClinicsDt
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Clinics_V2_ARA_SP]");

            return allClinicsDt;

        }

        public DataTable GetAllClinicsDataTable(string lang, int hospitalID, int pageno = -1, int pagesize = 10 , string ApiSources = "MobileApp")
        {                
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@PageNo", pageno),
                    new SqlParameter("@PageSize", pagesize)
                };


            string DB_SP_Name = "DBO.[Get_Clinics_SP]";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.Get_Clinics_SP";


            var allClinicsDt
                = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

            return allClinicsDt;

        }


        public DataTable GetAllClinicsDataTable_V2(string lang, string hospitalID, string GroupEntity , int pageno = -1, int pagesize = 10)
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@GroupEntityId", GroupEntity),
                    new SqlParameter("@PageNo", pageno),
                    new SqlParameter("@PageSize", pagesize)
                };

            var allClinicsDt
                = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Clinics_V2_SP]");

            return allClinicsDt;

        }

        public DataTable GetClinicsByBodyAreaDataTable(string lang, int hospitalID,string BodyArea,int age , string gender,  string ApiSources = "MobileApp")
        {
            DB.param = new SqlParameter[]
                {
                    new SqlParameter("@Lang", lang),
                    new SqlParameter("@BranchId", hospitalID),
                    new SqlParameter("@BodyArea", BodyArea),
                    new SqlParameter("@Age", age),
                    new SqlParameter("@Gender", gender)
                };

            string DB_SP_Name = "DBO.[Get_Clinics_byBodyArea_SP]";

            var allClinicsDt
                = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);

            return allClinicsDt;

        }

    }
}
