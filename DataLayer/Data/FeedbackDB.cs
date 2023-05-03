using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace DataLayer.Data
{
    public class FeedbackDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public void SavePatientFeedback(string lang, int hospitalID, int patientID, int reservationID, string text, float rating, ref int Er_Status, ref string Msg)
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@PatientId", patientID),
                new SqlParameter("@VisitId", reservationID),
                new SqlParameter("@Feedback", text),
                new SqlParameter("@Rating", rating),
                new SqlParameter("@status", SqlDbType.Int),
                new SqlParameter("@msg", SqlDbType.NVarChar, 200)

            };

            DB.param[6].Direction = ParameterDirection.Output;
            DB.param[7].Direction = ParameterDirection.Output;

            var flag = DB.ExecuteSP("dbo.Save_PatientRating_SP");

            Er_Status = Convert.ToInt32(DB.param[6].Value);
            Msg = DB.param[7].Value.ToString();

        }
    }
}