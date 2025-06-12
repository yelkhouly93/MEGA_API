using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace DataLayer.Data
{
	public class TrackingLogsDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");



        public DataTable GetSurveyQuestions(string lang, int SurveyID, int ServiceID = 0)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@SurveyId", SurveyID),
                new SqlParameter("@ServiceID", ServiceID)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("Survey.Get_Survey_Questions_SP");
            return dataTable;
        }

        public void SaveTrackingLogs(string Entry_Purpose, string Entry_RefId1_Branch, string Entry_RefId2_MRN, string Identifier_Names, string Identifier_Values ,  string App_Device_Id ,string Lang, bool isUpdate = true)
        {
            DB.param = new SqlParameter[]
            {   
                new SqlParameter("@Entry_Purpose", Entry_Purpose),
                new SqlParameter("@Entry_RefId1", Entry_RefId1_Branch),
                new SqlParameter("@Entry_RefId2", Entry_RefId2_MRN),
                new SqlParameter("@Identifier_Names", Identifier_Names),
                new SqlParameter("@Identifier_Values", Identifier_Values),
                new SqlParameter("@App_Device_Id", App_Device_Id),
                new SqlParameter("@Lang", Lang)
            };

            if (isUpdate)
                DB.ExecuteNonQuerySP("SmartFeature.Update_AppJourneyIdentifiers_SP ");
            else
                DB.ExecuteNonQuerySP("SmartFeature.Add_AppJourneyIdentifiers_SP");
        }


    }
}
