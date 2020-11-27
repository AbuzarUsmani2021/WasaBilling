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
    public class NotificationChildSeenController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ClientRemarksmodel rm)
        {
            ClientRemark retailerObj = new ClientRemark();
            try
            {
                var name = db.ComplaintNotifications.Where(x => x.JobID == rm.ComplaintID).ToList();

                foreach (var item in name)
                {
                    item.IsSeen = true;
                    db.SaveChanges();
                }
                 
                

                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Notification Seened Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                
              

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Notification Seened API Failed");
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
            public int ComplaintID { get; set; }
         


        }

    }
}
