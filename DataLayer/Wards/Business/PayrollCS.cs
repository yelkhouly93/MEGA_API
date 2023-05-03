using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using DataLayer.Wards.Model;

namespace DataLayer.Wards.Business
{
    public class PayrollCS
    {
        DBHelper DB = new DBHelper("Reception");
        public DataSet LeavePayProvision()
        {
            try
            {
                SqlParameter[] sqlParam = new SqlParameter[1];
                sqlParam[0] = new SqlParameter("@start", "12/1/2011");
                DataSet ds = dl.ExecuteSQLDS("PAYROLL.HIS_LEAVEPAY_PROVISION", sqlParam);                
                return ds;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                //return false;
            }
        }
    }
}
