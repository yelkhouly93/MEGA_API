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
    public class HospitalDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public List<Hospitals> GetAllHospitals(string lang, int groupentityid = 1)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@GroupEntityId", groupentityid),
            };

            var _allHospitals = new List<Hospitals>();

            _allHospitals = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Hospitals_SP]").ToListObject<Hospitals>();

            return _allHospitals;

        }


        public DataTable GetAllHospitalsDataTable(string lang, int groupentityid = 1 , int IsPaymentDetail = 0)
        {
            var showPaydetails = 0;
            if (IsPaymentDetail == 1)
                showPaydetails = 1;

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@GroupEntityId", groupentityid),
                new SqlParameter("@IncludePaymentGW", showPaydetails),                
            };

            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Hospitals2_SP]");

            return dt;
        }


        public DataTable GetAllHospitalsDataTable_v2(string lang, string groupentityid ,  int IsPaymentDetail = 0 , int CountryID = 0 , double lat = 0, double lng = 0 , string ApiSources = "MobileApp" , string CallingArea = "")
        {
            var showPaydetails = 0;
            
            if (IsPaymentDetail == 1)
                showPaydetails = 1;

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@GroupEntityId", groupentityid),
                new SqlParameter("@IncludePaymentGW", showPaydetails),
                new SqlParameter("@CountryID", CountryID),
                new SqlParameter("@lat", lat),
                new SqlParameter("@lng", lng),
                new SqlParameter("@CallingArea", CallingArea)                
            };


            string DB_SP_Name = "DBO.Get_Hospitals2_SP";

            if (ApiSources.ToLower() == "saleforce")
                DB_SP_Name = "SF.Get_Hospitals2_SP";
            var dt = DB.ExecuteSPAndReturnDataTable(DB_SP_Name);
            //var totaldistance = GetDistance(21.607480, 39.156425, 21.597247317731494, 39.13322550523333);
            return dt;

        }
        private double GetDistance (double lat , double lng , double lat2, double lng2)
        {
            double e = lat * (Math.PI / 180);
            double f = lng * (Math.PI / 180);
            double g = lat2 * (Math.PI / 180);
            double h = lng2 * (Math.PI / 180);
            double i =
                (Math.Cos(e) * Math.Cos(g) * Math.Cos(f) * Math.Cos(h)
                + Math.Cos(e) * Math.Sin(f) * Math.Cos(g) * Math.Sin(h)
                + Math.Sin(e) * Math.Sin(g));
            double j = Math.Acos(i);
            double k = (6371 * j);  //Distance in KM

            return k;
        }

        public DataTable GetAllHospitalsCountry_DT(string lang)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang)                
            };
            var dt = DB.ExecuteSPAndReturnDataTable("DBO.[Get_BranchCounties_SP]");

            return dt;

        }

    }
}
