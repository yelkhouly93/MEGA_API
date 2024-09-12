using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data;
using System;
using Oracle.ManagedDataAccess.Client;
using DataLayer.Model;

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


        public TestResultMain GetPatientLabResultsByApi_Dam( string testId , string ReportType  )
        {   
            TestResultMain testResult = new TestResultMain();

            var LABSQL = "";

            if (ReportType.ToLower() == "lab")
			{
                LABSQL = "SELECT Lpad(lcs.chart_description,Length(lcs.chart_description) + nvl(lcs.indentation,0),' ') section " +
                        " ,Lpad(lcsg.chart_description, Length(lcsg.chart_description) + nvl(lcsg.indentation, 0), ' ') testName " +
                        " ,lcg.group_id       ,lm.patient_id pat_id, om.episode_no      ,lm.lab_date collected_date " +
                        " , pm.pat_name_1 || ' ' || pm.pat_name_2 || ' ' || PM.PAT_NAME_3 || ' ' || pm.pat_name_family p_name " +
                        " ,Trunc(pm.date_of_birth) birth_date       ,pm.sex      ,pm.nationality_code      ,we.description we " +
                        " , om.attendance_type      ,li.result_type resultValueCategory, li.line_text interp " +
                        " , Lpad(li.chart_description, Length(li.chart_description) + nvl(lcsi.indentation, 0), ' ') parameter_name " +
                        " ,li.dual_code_type      ,lr.line_text      ,lr.result_num result, lr.result_code " +
                        " ,lr.lab_result      ,InitCap(sm.staff_name_family) || ' ' || InitCap(sm.staff_name_1) doctor " +
                        " ,lm.lab_id testCode, lr.start_numeric_value range_start, lr.end_numeric_value range_end " +
                        " , mu.description unit, cn.user_code severityID, Nvl(Pat.Get_Patient_File_Id(pm.patient_id), '*' || To_Char(pm.patient_id)) mrn " +
                        " ,lr.amend_by_user      ,lr.amend_last_date      ,lio.need_signature     , li.range_result " +
                        " ,lt.amend_by_user verified_by_user, lt.amend_last_date verification_date " +
                        " FROM lab_master lm      ,lab_test lt, lab_results lr      ,lab_ios li, lab_ios lio " +
                        " ,lab_chart_subgroup_ios lcsi, lab_chart_subgroups lcsg      ,lab_chart_groups lcg " +
                        " , lab_chart_sections lcs      ,orders_master om, order_lines ol " +
                        " ,patient_master pm, work_entities_data we      ,staff_master sm " +
                        " , codes_data mu      ,codes_data cn " +
                        " WHERE lt.order_line = lr.order_line AND lm.lab_id = lt.lab_id AND li.ios = lr.ios " +
                        " AND lio.ios = lt.ios AND lcsi.ios_grouping_main_ios = lt.ios AND lcsi.ios = lr.ios " +
                        " AND lcsg.subgroup_id = lcsi.subgroup_id AND lcg.group_id = lcsg.group_id AND lcs.section_id = lcg.section_id AND ol.order_line = lt.order_line " +
                        " AND om.master_order_no = ol.master_order_no AND pm.patient_id = om.patient_id " +
                        " AND sm.staff_id = om.orderer_staff_id AND we.work_entity = ol.order_work_entity " +
                        " AND li.unit_of_measure = mu.code(+) AND lr.conclusion = cn.code(+) AND lt.status || '' = 'F' AND OL.STATUS <> 'Q' " +
                        " AND li.chart_print_flag = 'Y' AND(lr.line_text  IS NOT NULL OR lr.result_num IS NOT NULL OR lr.result_code IS NOT NULL) " +
                        " AND    ol.order_line = "+ testId  + " ORDER BY        lcsi.seq_inside_subgroup,li.chart_description";

                var testOrders = EXECUTE_SQL_DT(LABSQL).ToList<TestResultMain_Dammam>();

                if (testOrders != null)
                {
                    testResult = MapTestResultModelToTestResultMain_DAM_LAB(testOrders);
                }
            }
            else
			{
                LABSQL = "SELECT Pat.Get_Patient_File_Id(patient_id) MRN,  A.Wrote_DateTime  as Collect_DATE " +
                            " ,Actual_Report_TEXT as Results " +
                            " FROM XY_REPORTS A ,XY_ATTENDANCE_IOS_DETAILS B " +
                            " WHERE B.REPORT_ID = A.REPORT_ID AND Order_Line = " + testId;

                var testOrders = EXECUTE_SQL_DT(LABSQL).ToList<TestResultMain_Dammam_XRAY>();
                if (testOrders != null && testOrders.Count > 0)                
                    testResult = MapTestResultModelToTestResultMain_DAM_XRAY(testOrders);                
                else
                    return null;
            }
            

            
            return testResult;
        }

        private TestResultMain MapTestResultModelToTestResultMain_DAM_LAB(List<TestResultMain_Dammam> testOrders)
        {
            TestResultMain testResultMain = new TestResultMain();
            List<TestResultParameter> testParameters = new List<TestResultParameter>();

            var icount = testOrders.Count();
            if (icount > 0)
			{
                testResultMain.testCode = testOrders[0].testCode;
                testResultMain.testName = testOrders[0].testName;
                testResultMain.section = testOrders[0].section;

                testResultMain.sample_name = testOrders[0].testCode;
                testResultMain.collected_date = testOrders[0].collected_date;


                foreach (var param in testOrders)
                {

                    TestResultParameter parameter = new TestResultParameter();
                    parameter.parameter_name = param.parameter_name ?? "";
                    parameter.result = param.result ?? "";
                    parameter.unit = param.unit ?? "";
                    parameter.range = (param.range_start  ?? "") + " - " + (param.range_end ?? "");

                    if (string.IsNullOrEmpty(param.range_start) && string.IsNullOrEmpty(param.range_end))
					{
                        parameter.range = param.interp;
                    }


                    parameter.ResultValueCategory = param.ResultValueCategory ?? "N";                    

                    parameter.rating = param.rating;
                    
                    parameter.severityID = "N";

                    if (param.severityID != null)
					{
                        //if (param.severityID.ToUpper() == "N" || param.severityID.ToUpper() == "H" || param.severityID.ToUpper() == "L" || param.severityID.ToUpper() == "P")
                        //    parameter.severityID = param.severityID.Trim();
                        if (param.severityID.ToUpper() == "N" || param.severityID.ToUpper() == "TH" )
                            parameter.severityID = "N";
                        else if (param.severityID.ToUpper() == "HC" || param.severityID.ToUpper() == "LC" || param.severityID.ToUpper() == "UN" || param.severityID.ToUpper() == "L" || param.severityID.ToUpper() == "H")
                            parameter.severityID = "H";
                        else
                            parameter.severityID = "N";
                    }

                    

                    parameter.Weightage = 0;

                    if (parameter.severityID.ToUpper() == "N")
                        parameter.Weightage = 0;
                    else if (parameter.severityID.ToUpper() == "H")
                        parameter.Weightage = 50;
                    else if (parameter.severityID.ToUpper() == "L")
                        parameter.Weightage = 50;
                    else if (parameter.severityID.ToUpper() == "P")
                        parameter.Weightage = 100;

                    parameter.parameter_name = parameter.parameter_name.Replace(":", "").Replace(".", "").Replace("%", "");
                    testParameters.Add(parameter);
                }
            }

            testParameters = testParameters.OrderByDescending(o => o.Weightage).ToList();

            testResultMain.parameters = testParameters;

            return testResultMain;

        }

        private TestResultMain MapTestResultModelToTestResultMain_DAM_XRAY(List<TestResultMain_Dammam_XRAY> testOrders)
        {
            TestResultMain testResultMain = new TestResultMain();
            List<TestResultParameter> testParameters = new List<TestResultParameter>();

            var icount = testOrders.Count();
            if (icount > 0)
            {
                testResultMain.testCode = "XRAY";
                testResultMain.testName = "XRAY";
                testResultMain.section = "RADIOLOGY";

                testResultMain.sample_name = "No Sample";
                testResultMain.collected_date = testOrders[0].Collect_DATE;


                foreach (var param in testOrders)
                {
                    TestResultParameter parameter = new TestResultParameter();
                    parameter.parameter_name = "RADIOLOGY REPORT";
                    parameter.result = param.Results;
                    parameter.unit = "";
                    parameter.range = "";
                    parameter.ResultValueCategory = "N";                    
                    parameter.severityID = "N";
                    parameter.Weightage = 0;
                    testParameters.Add(parameter);
                }
            }

            testParameters = testParameters.OrderByDescending(o => o.Weightage).ToList();

            testResultMain.parameters = testParameters;

            return testResultMain;

        }


    }
}
