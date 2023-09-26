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
	public class SurveyDB
	{
		CustomDBHelper DB = new CustomDBHelper("RECEPTION");


        public DataTable GetSurveyQuestions(string lang, int SurveyID)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@SurveyId", SurveyID)                
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("Survey.Get_Survey_Questions_SP");
            return dataTable;
        }

        public void SaveSurveyQuestionAnswers(int BranchID, int MRN, string AnswerQuery, string Source,int SurveyId,int ServiceId, ref int errStatus, ref string errMessage)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@AnswerQuery", AnswerQuery),
                new SqlParameter("@SurveyId", SurveyId),                
                new SqlParameter("@MRN", MRN),
                new SqlParameter("@BranchId", BranchID),
                new SqlParameter("@ServiceId", ServiceId),
                new SqlParameter("@Sources", Source),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 1000)
            };

            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            DB.ExecuteNonQuerySP("Survey.Save_Survey_QuestionAnswers_SP");

            errStatus = Convert.ToInt32(DB.param[6].Value);
            errMessage = DB.param[7].Value.ToString();
        }


    }
}
