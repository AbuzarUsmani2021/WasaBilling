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
    public class ChatBoxController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ChatBoxmodel rm)
        {
            ChatBox retailerObj = new ChatBox();
            try
            {
                var job = db.Jobs.Where(x => x.ID == rm.ComplaintID).FirstOrDefault();
               

                retailerObj.ComplaintID = rm.ComplaintID;
                retailerObj.Remarks = rm.Remarks;
               
                retailerObj.DateInstall = DateTime.UtcNow.AddHours(5);
                retailerObj.SOID = rm.SOID;
                
                db.ChatBoxes.Add(retailerObj);
                 
                db.SaveChanges();
                string type = "SMS";
                string message = "There is a new message in Complaint No  " + job.TicketNo + " Kindly Visit it ";

                if (job.ZoneID != 9)
                {

                    var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID ==2).Select(x => x.ID).ToList();
                    List<string> list = new List<string>();

                    foreach (var item in SOIds)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id != null)
                        {
                            foreach (var items in id)
                            {
                                var result = new CommonController().PushNotificationForEdit(message, items, rm.ComplaintID, type);
                            }
                        }
                    }

                   

                }
                else
                {
                    // Notification For Progressive Management
                    var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 6 && x.RoleID == 2).Select(x => x.ID).ToList();
                    List<string> list1 = new List<string>();
                    foreach (var item in SOIdss)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id != null)
                        {
                            foreach (var items in id)
                            {
                                var result = new CommonController().PushNotificationForEdit(message, items, rm.ComplaintID, type);
                            }
                        }
                        //if (list1 != null)
                        //{
                        //    var result = new CommonController().PushNotification(message, list1, rm.ComplaintID, type);
                        //}
                    }
                }

                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Remarks Added Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                
              

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "SMS  Added API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "SMS Remarks Added API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }



        }



        public class SuccessResponse
        {

        }
        public class ChatBoxmodel
        {
            public int ComplaintID { get; set; }
            public int SOID { get; set; }
            public string Remarks { get; set; }
           


        }

    }
}
