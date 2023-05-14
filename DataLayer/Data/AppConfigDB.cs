using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataLayer.Common;
using DataLayer.Model;

using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace DataLayer.Data
{
	
	public class AppConfigDB
	{
        private readonly CustomDBHelper _db = new CustomDBHelper("RECEPTION");
   //     public List<AppConfigModule> GetClintModuleList (string CKey)
   //     {
			//var _AppConfigModule = new List<AppConfigModule>();

   //         _db.param = new SqlParameter[]
   //         {
   //             new SqlParameter("@ClientKey", CKey)                
   //         };

   //         _AppConfigModule = _db.ExecuteSPAndReturnDataTable("AppConfig.Get_Client_App_Modules").ToListObject<AppConfigModule>();

   //         return _AppConfigModule;
   //     }

        public DataTable GetClintModuleList(string CKey)
        {
            //var _AppConfigModule = new List<AppConfigModule>();

            _db.param = new SqlParameter[]
            {
                new SqlParameter("@ClientKey", CKey)
            };

            var _AppConfigModule = _db.ExecuteSPAndReturnDataTable("AppConfig.Get_Client_App_Modules");

            return _AppConfigModule;
        }

    }
}
