using DataLayer.Common;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Data.FCM
{
	public class fcmDB
	{
        CustomDBHelper DB = new CustomDBHelper("RECEPTION");
        public DataTable Get_IncompleteReminder_DT()
        {
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@Entry_Purpose", "Missed Appointments Tracking")                
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("[SmartFeature].[Get_IncompleteBookingJourneyReminders_SP]");
            
            return dataTable;
        }

        // For Video call Later Change the Class
        public ZoomInfo_UAE VC_GetZoomCallInfo(string ID)
		{
            DB.param = new SqlParameter[]
            {
                new SqlParameter("@DataID", ID)
            };
            var dataTable = DB.ExecuteSPAndReturnDataTable("[dbo].[Get_VideoCall_FORAPI_Zoom_SP]").ToModelObject<ZoomInfo_UAE>();
            return dataTable;
            
        }
    }
}
