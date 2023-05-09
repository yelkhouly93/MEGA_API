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
	
	class AppConfigDB
	{
        private readonly CustomDBHelper _db = new CustomDBHelper("RECEPTION");
        public AppConfigModule GetClientModuleList (string CKey){

			AppConfigModule _AppConfigModule = new AppConfigModule();

            _db.param = new SqlParameter[]
            {
                new SqlParameter("@BranchId", CKey)                
            };            

            var flag = _db.ExecuteSP("AppConfig.[Get_Client_App_Modules]");

            return _AppConfigModule;
        }
	}
}
