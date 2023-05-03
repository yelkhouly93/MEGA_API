using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;


namespace DataLayer.Data
{
    public class LoginDB
    {
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");

        public UserInfo ValidateLoginUser(string lang, int hospitalID, string userName, string Password, ref int Er_Status, ref string Msg)
        {

            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Lang", lang),
                new SqlParameter("@BranchId", hospitalID),
                new SqlParameter("@UserName", userName),
                new SqlParameter("@Password", Password),
                new SqlParameter("@Er_Status", SqlDbType.Int),
                new SqlParameter("@Msg", SqlDbType.NVarChar, 200)
            };
            DB.param[4].Direction = ParameterDirection.Output;
            DB.param[5].Direction = ParameterDirection.Output;

            var _userInfoModel = new List<UserInfoModel>();
            UserInfo _userInfo = new UserInfo();
            _userInfoModel = DB.ExecuteSPAndReturnDataTable("DBO.[Validate_User_SP]").ToListObject<UserInfoModel>();

            _userInfo = MapUserInfoModelToUserInf(_userInfoModel);
            Er_Status = Convert.ToInt32(DB.param[4].Value);
            Msg = DB.param[5].Value.ToString();

            return _userInfo;

        }

        private UserInfo MapUserInfoModelToUserInf(List<UserInfoModel> _userInfoModel)
        {
            UserInfo _userInfo = new UserInfo();
            if (_userInfoModel != null && _userInfoModel.Count > 0)
            {
                _userInfo.address = _userInfoModel[0].PAddress;
                _userInfo.birthday = _userInfoModel[0].DOB;
                _userInfo.email = _userInfoModel[0].PEmail;
                _userInfo.family_name = _userInfoModel[0].FamilyName;
                _userInfo.first_name = _userInfoModel[0].FirstName;
                _userInfo.gender = _userInfoModel[0].GenderId;
                _userInfo.hospital_id = _userInfoModel[0].Branch_Id;
                _userInfo.id = _userInfoModel[0].PatientId;
                _userInfo.last_name = _userInfoModel[0].LastName;
                _userInfo.marital_status_id = _userInfoModel[0].MaritalStatusId;
                _userInfo.middle_name = _userInfoModel[0].MiddleName;
                _userInfo.name = _userInfoModel[0].Name;
                _userInfo.national_id = _userInfoModel[0].NationalId;
                _userInfo.phone = _userInfoModel[0].PCellno;
                _userInfo.registration_no = null;
                _userInfo.title_id = _userInfoModel[0].TitleId;
            }

            return _userInfo;
        }
    }
}
