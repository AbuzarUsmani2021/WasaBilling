using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class ReadallController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ClientRemarksmodel rm)
        {
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            NotificationSeen retailerObj = new NotificationSeen();
            try
            {
                var list = (from jd in db.JobsDetails
                            join ns in db.NotificationSeens
                            on jd.ID equals ns.JobDetailID
                            where jd.JobDate >= dtFromToday && jd.JobDate <= dtToToday && ns.SOID == rm.SOID
                            select ns
                  ).ToList();

                if (list != null)
                {
                    foreach (var item in list)
                    {
                        item.IsSeen = true;
                        db.SaveChanges();
                    }
                }

                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "All Notifications Seened Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                
              

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Client Remarks Added API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Client Remarks Added API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }



        }



        public class SuccessResponse
        {

        }

        public class ClientRemarksmodel
        {
   
            public int SOID { get; set; }
    


        }

    }
}
