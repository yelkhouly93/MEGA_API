using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Hosting;


namespace DataLayer.Data
{
    public class LABResultDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        //public PateintTestsModel GetPatientLABnRADResults(string lang, int hospitalId, int resgistrationNo, ref int Er_Status, ref string Msg)
        //{
        //    PateintTestsModel _patientLABResults = new PateintTestsModel();
        //    DB.param = new SqlParameter[]
        //    {
        //        new SqlParameter("@BranchId", hospitalId),
        //        new SqlParameter("@RegistrationNo ", resgistrationNo),
        //        new SqlParameter("@status", SqlDbType.Int),
        //        new SqlParameter("@msg", SqlDbType.VarChar, 100)
        //    };
        //    DB.param[2].Direction = ParameterDirection.Output;
        //    DB.param[3].Direction = ParameterDirection.Output;

        //    var patientResultDataset = DB.ExecuteSPAndReturnDataTable("DBO.[Get_PatientLABnRADResults_SP]").ToListObject<PateintTestsModel>();
            
            
        //    //_patientData = MapPatinetDataModelToPatientData(patientDataset);
        //    Er_Status = Convert.ToInt32(DB.param[2].Value);
        //    Msg = DB.param[3].Value.ToString();

        //    //if (Er_Status == 0 && patientResultDataset != null && patientResultDataset.Tables.Count > 0)
        //    //{
        //    //    _patientLABResults = MapDataSetToPatientResultData(patientResultDataset);
        //    //}
        //    //else
        //    //{
        //    //    _patientLABResults = null;
        //    //}

        //    return _patientLABResults;

        //}

        //private PateintTestsModel MapDataSetToPatientResultData(List<PateintTestsModel> _prepModel)
        //{
        //    PateintTestsModel _prep = new PateintTestsModel();
        //    if (_prep != null && _prepModel.Count > 0)
        //    {
        //        _prep.RegistrationNo = _prepModel[0].RegistrationNo;
        //        _prep.ReportType = _prepModel[0].ReportType;
        //        _prep.ReportDate = _prepModel[0].ReportDate;
        //        _prep.OPIP = _prepModel[0].OPIP;
        //        _prep.TestName = _prepModel[0].TestName;
        //        _prep.ReportFileName = _prepModel[0].ReportFileName;
                
        //    }

        //    return _prep;
        //}
    }
}
