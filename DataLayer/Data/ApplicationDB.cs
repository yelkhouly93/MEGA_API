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
    public class ApplicationDB
    {

        private readonly CustomDBHelper _db = new CustomDBHelper("RECEPTION");

        public DataTable GetApplicationVersion(string Lang, int APPID, string OS, ref int errStatus, ref string errMessage)
        {
            _db.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", Lang),
                new SqlParameter("@APPID", APPID),
                new SqlParameter("@OS", OS),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)
            };
            _db.param[3].Direction = ParameterDirection.Output;
            _db.param[4].Direction = ParameterDirection.Output;            

            var dt = _db.ExecuteSPAndReturnDataTable("DBO.[Get_MobileAppVersion_SP]");

            errStatus = Convert.ToInt32(_db.param[3].Value);
            errMessage = _db.param[4].Value.ToString();            

            return dt;
        }

    }
}
