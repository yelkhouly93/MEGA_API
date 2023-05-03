using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataLayer.Data.SGHMedicalDAL
{
    public class LoginDAL
    {
        public int StationID { get; set; }
        public int UserID { get; set; }
        public string IPID { get; set; }
        public string PIN { get; set; }

        public string Employee { get; set; }
        public string EmployeeID { get; set; }

        public string DivisionID { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }


        public string Username { get; set; }
        public string Password { get; set; }

        public string ClinicType { get; set; }
        public string ClinicDeptID { get; set; }


        DBHelper DB = new DBHelper("HIS");
        EncryptDecrypt util = new EncryptDecrypt();

        public Boolean mobileLogin()
        {
            try
            {


                StringBuilder sql1 = new StringBuilder();
                sql1.Append(@"   
                                     if not exists(select  * from sys.tables where name = 'HIS_MOBILE_LOGIN_LOG')
                                     BEGIN
                                    CREATE TABLE [HISGLOBAL].[HIS_MOBILE_LOGIN_LOG](
	                                    [ID] [int] IDENTITY(1,1) NOT NULL,
	                                    [UserID] [int] NULL,
	                                    [IPAddress] [varchar](20) NULL,
	                                    [HostName] [varchar](100) NULL,
	                                    [DepartmentId] [int] NULL,
	                                    [LoginDate] [datetime] NULL,
	                                    [IsLogInCorrect] [bit] NULL
                                    ) ON [MasterFile]

                                     END
                ");
                DB.ExecuteSQL(sql1.ToString());
                sql1.Clear();


                DataTable ds = DB.ExecuteSQLAndReturnDataTable("select a.id, rtrim(FirstName) + ' ' + rtrim(Lastname) as Name, password, DepartmentID,b.DivisionId,b.name as deptname from HIS..Employee A LEFT JOIN Department b on a.departmentid = b.id where A.employeeid = '" + Username + "' and a.Deleted = 0");
                if (ds.Rows.Count != 0)
                {
                    var userid = ds.Rows[0]["id"].ToString();
                    var departmentId = ds.Rows[0]["DepartmentID"].ToString();

                    bool isLoginCorrect = false;

                    if (string.Compare(Password ?? "", util.DecryptPassword(ds.Rows[0]["password"].ToString()), false) != 0)
                    {

                        StringBuilder sql = new StringBuilder();
                        sql.Clear();
                        sql.Append("  insert into hisglobal.HIS_MOBILE_LOGIN_LOG ( DepartmentId,UserID, IPAddress,HostName,LoginDate,IsLogInCorrect) values  ( '" + departmentId + "', '" + userid + "', '123','123',GETDATE(),'" + isLoginCorrect + "')  ");
                        DB.ExecuteSQL(sql.ToString());

                        return false;
                    }
                    else
                    {
                        isLoginCorrect = true;
                        this.Employee = ds.Rows[0]["name"].ToString();
                        this.EmployeeID = ds.Rows[0]["id"].ToString();
                        this.DivisionID = ds.Rows[0]["divisionid"].ToString();
                        this.DepartmentName = ds.Rows[0]["deptname"].ToString();
                        this.DepartmentID = ds.Rows[0]["DepartmentID"].ToString();


                        StringBuilder sql = new StringBuilder();
                        sql.Clear();
                        sql.Append("  insert into hisglobal.HIS_MOBILE_LOGIN_LOG ( DepartmentId,UserID, IPAddress,HostName,LoginDate,IsLogInCorrect) values  ( '" + departmentId + "', '" + userid + "', '123','123',GETDATE(),'" + isLoginCorrect + "')  ");
                        DB.ExecuteSQL(sql.ToString());

                        return true;
                    }


                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }

}

