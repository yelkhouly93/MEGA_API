using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;
using System;
using Oracle.ManagedDataAccess.Client;

namespace DataLayer.Data.ORACLE
{
    public class ORACLECS_DB
	{
        //// Connection string for Oracle using OleDb
        //string connectionString = "Provider=OraOLEDB.Oracle;DataSource=(DESCRIPTION=(CID=GTU_APP)(ADDRESS_LIST=(ADDRESS=" +
        //                            "(PROTOCOL = TCP)(HOST = 130.11.2.101)(PORT = 1521)))(CONNECT_DATA=(SID=PROD)" +
        //                            "(SERVER=DEDICATED)));User Id = SGHD_INT; Password=SGHD_PROD_int#123#123;";

        // Connection string for Oracle database
        string connectionString = "User Id=SGHD_INT;Password=SGHD_PROD_int#123#123;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=130.11.2.101)(PORT=1521))(CONNECT_DATA=(SID=PROD)));";


        public void TESTEXECUTE ()
		{

            DataTable dt = new DataTable();
            // SQL query to execute (replace with your query)
            string query = "SELECT * FROM XY_REPORTS WHERE REPORT_ID < 246";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    // Open connection
                    connection.Open();
                    Console.WriteLine("Connection successful.");

                    // Example query
                    //string query = "SELECT * FROM YourTableName";

                    // Create Oracle command
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        // Execute query and retrieve data
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Example: Retrieve data from columns
                                int id = reader.GetInt32(reader.GetOrdinal("ID"));
                                string name = reader.GetString(reader.GetOrdinal("Name"));

                                Console.WriteLine($"ID: {id}, Name: {name}");
                            }
                        }
                    }
                }
                catch (OracleException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        public DataTable EXECUTE_SQL_DT(string SQL)
        {

            DataTable dt = new DataTable();
            // SQL query to execute (replace with your query)
            string query = "SELECT * FROM XY_REPORTS WHERE REPORT_ID < 246";
            query = SQL;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    // Open connection
                    connection.Open();
                    Console.WriteLine("Connection successful.");

                    // Example query
                    //string query = "SELECT * FROM YourTableName";

                    // Create Oracle command
                    using (OracleCommand command = new OracleCommand(query, connection))
                    {
                        // Execute query and retrieve data
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                            reader.Close();
                            reader.Dispose();

                            return dt;

                            //while (reader.Read())
                            //{
                            //    // Example: Retrieve data from columns
                            //    int id = reader.GetInt32(reader.GetOrdinal("ID"));
                            //    string name = reader.GetString(reader.GetOrdinal("Name"));

                            //    Console.WriteLine($"ID: {id}, Name: {name}");
                            //}
                        }
                    }
                }
                catch (OracleException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                return null;
            }
        }
    
    
        public void GetLabResults ()
		{

		}
    }
}
