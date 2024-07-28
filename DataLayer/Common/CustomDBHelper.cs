using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DataLayer.Common
{
    public class CustomDBHelper : DataLayer.DBHelper
    {

        public new SqlParameter[] param = null;

        public new Boolean Successful { get; private set; }
        public new string ErrorMessage { get; private set; }
        public new string ErrorStackTrace { get; private set; }


        public CustomDBHelper(string ModuleName = "SGH_HIS")
        {
            
        }

        public new string ExecuteSQLScalar(string sql = "")
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    this.Successful = true;
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        var tempstr = cmd.ExecuteScalar();
                        
                        if (tempstr == null)
                            return "";

                        return tempstr.ToString();

                        //return cmd.ExecuteScalar().ToString();


                    }
                }
                catch (Exception ex)
                {
                    this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(sql, ex.Message, ex.StackTrace);
                    throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return false;
                }
            }
        }

        public int ExecuteNonQuery(string sql = "")
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    this.Successful = true;
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(sql, ex.Message, ex.StackTrace);
                    throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return false;
                }
            }
        }

        public int ExecuteNonQuerySP(string spName)
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(spName, CN))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300;
                        if (param != null)
                        {
                            foreach (SqlParameter item in param)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        this.Successful = true;
                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                     this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(spName, ex.Message, ex.StackTrace);
                    throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return false;
                }
            }
        }


        public new Boolean ExecuteSQL(string sql = "")
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        this.Successful = true;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(sql, ex.Message, ex.StackTrace);
                    throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return false;
                }
            }
        }

        public new Boolean ExecuteSP(string spName = "")
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(spName, CN))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300;
                        if (param != null)
                        {
                            foreach (SqlParameter item in param)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        this.Successful = true;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(spName, ex.Message, ex.StackTrace);
                    throw new ApplicationException("<b>From:</b> " + spName + "<br/><br/><b>Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return false;
                }
            }
        }

        public new DataTable ExecuteSPAndReturnDataTable(string SPName = "")
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(SPName, CN))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300;
                        if (param != null)
                        {
                            foreach (SqlParameter item in param)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        this.Successful = true;
                        SqlDataReader rs = cmd.ExecuteReader();
                        dt.Load(rs);
                        rs.Close();
                        rs.Dispose();
                        
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    CN.Close();
                    CN.Dispose();
                    this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;

                    Log_SP_ERROR(SPName, ex.Message, ex.StackTrace);
                    throw new ApplicationException("<b>From:</b> " + SPName + "<br/><br/><b>Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                }
            }
        }


        public new DataTable ExecuteSPAndReturnDataTable_WithDataTableInput(DataTable inputdataTable , string SPName = ""  )
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(SPName, CN))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300;
                        if (param != null)
                        {
                            foreach (SqlParameter item in param)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        this.Successful = true;
                        SqlDataReader rs = cmd.ExecuteReader();
                        dt.Load(rs);
                        rs.Close();
                        rs.Dispose();

                        return dt;
                    }
                }
                catch (Exception ex)
                {
                    CN.Close();
                    CN.Dispose();
                    this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;

                    Log_SP_ERROR(SPName, ex.Message, ex.StackTrace);
                    throw new ApplicationException("<b>From:</b> " + SPName + "<br/><br/><b>Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                }
            }
        }


        public new DataTable ExecuteSQLAndReturnDataTable(string sql)
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;

                        this.Successful = true;
                        SqlDataReader rs = cmd.ExecuteReader();
                        dt.Load(rs);
                        rs.Close();
                        rs.Dispose();
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                   this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(sql, ex.Message, ex.StackTrace);
                    throw new ApplicationException("<b>From:</b> " + sql + "<br/><br/><b>Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return null;
                }
            }
        }

        public DataTable ExecuteSQLAndReturnDataTable(string sql, string _constring)
        {
            using (SqlConnection CN = new SqlConnection(_constring))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;

                        this.Successful = true;
                        SqlDataReader rs = cmd.ExecuteReader();
                        dt.Load(rs);
                        rs.Close();
                        rs.Dispose();
                        return dt;
                    }
                }
                catch (Exception ex)
                {
                   this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(sql, ex.Message, ex.StackTrace);
                    throw new ApplicationException("<b>From:</b> " + sql + "<br/><br/><b>Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return null;
                }
            }
        }

        public new DataSet ExecuteSPAndReturnDataSet(string SPName = "")
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(SPName, CN))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 3000;
                        if (param != null)
                        {
                            foreach (SqlParameter item in param)
                            {
                                cmd.Parameters.Add(item);
                            }
                        }

                        this.Successful = true;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
                catch (Exception ex)
                {
                   this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    Log_SP_ERROR(SPName, ex.Message, ex.StackTrace);
                    throw new ApplicationException("<b>From:</b> " + SPName + "<br/><br/><b>Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                }
            }
        }



        public int ExecuteTransaction(string sql = "")
        {
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    this.Successful = true;
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        SqlTransaction transaction = CN.BeginTransaction() ;
                       
                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = transaction;

                        try
                        {
                            cmd.CommandTimeout = 300;
                            return cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            transaction.Commit();
                            CN.Close();
                            CN.Dispose();
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.Successful = false;
                    this.ErrorMessage = ex.Message;
                    this.ErrorStackTrace = ex.StackTrace;
                    CN.Close();
                    CN.Dispose();
                    Log_SP_ERROR(sql, ex.Message, ex.StackTrace);
                    throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return false;
                }
            }
        }

        public bool ValidateUser(string _username, string _password)
        {
            using (var cn = new SqlConnection(base.SqlConnectionString))
            {
                string _sql = @"SELECT [Username] FROM [Marketing].[App_Users] " +
                              @"WHERE [Username] = @u AND [Password] = @p";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = _username;
                cmd.Parameters
                    .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                    .Value = _password;

                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }
    
        public int Log_SP_ERROR (string QuerySP , string ErrorMessage ,  string stacktrace)
        {
            ErrorMessage = ErrorMessage.Replace("'", "");
            stacktrace = "";
            var sql = "INSERT INTO EServiceLog.dbo.API_LOG_SP_ERRORS (QuerySP,Error_msg,Stack_Trace , ProjectName) Values ('" + QuerySP +"','" + ErrorMessage + "','" + stacktrace +"','SGHMOBILEAPI_NEW')";
            using (SqlConnection CN = new SqlConnection(base.SqlConnectionString))
            {
                try
                {
                    CN.Open();

                    DataTable dt = new DataTable();
                    this.Successful = true;
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 300;
                        return cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    //this.Successful = false;
                    //this.ErrorMessage = ex.Message;
                    //this.ErrorStackTrace = ex.StackTrace;
                    //throw new ApplicationException("Error Message:</b> <br /> " + ex.Message + "<br /><br /><b>Stack Trace:</b><br /> " + ex.StackTrace);
                    //return false;
                    return 0;
                }
            }
        }
    }
}
