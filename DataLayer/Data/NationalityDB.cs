using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace DataLayer.Data
{
    public class NationalityDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public List<Nationalities> GetAllNationalities(string lang, int hospitalID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID)
            };

            var _allNationalities = new List<Nationalities>();

            _allNationalities = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Nationalities_SP]").ToListObject<Nationalities>();

            return _allNationalities;

        }

        public List<Nationalities> GetAllNationalities_V2(string lang, int hospitalID , int Only_Saudi = 0)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@OnlyTwo", Only_Saudi)
                
            };

            var _allNationalities = new List<Nationalities>();

            _allNationalities = DB.ExecuteSPAndReturnDataTable("DBO.[Get_Nationalities_V2_SP]").ToListObject<Nationalities>();

            return _allNationalities;

        }
    }
}