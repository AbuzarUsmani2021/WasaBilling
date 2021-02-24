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
    public class ClientRemarksController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ClientRemarksmodel rm)
        {
            ClientRemark retailerObj = new ClientRemark();
            try
            {
                var name = db.SaleOfficers.Where(x => x.UserName == rm.SOName).Select(x => x.Name).FirstOrDefault();

                retailerObj.ComplaintID = rm.ComplaintID;
                retailerObj.ClientRemarks = rm.Remarks;
                retailerObj.IsActive = true;
                retailerObj.Isdeleted = true;
                retailerObj.RemarksDate = DateTime.UtcNow.AddHours(5);
                retailerObj.RemarksBy = rm.SOID;
                retailerObj.RemarksByName = name;
                db.ClientRemarks.Add(retailerObj);
                 
                db.SaveChanges();

                var data = db.Jobs.Where(x => x.ID == rm.ComplaintID).FirstOrDefault();
               //// var AreaID = Convert.ToInt32(data.Areas);

                //var IDs = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();

            
                string type = "Remarks";
                string message = "Complaint Remarks against ComplaintNo " + data.TicketNo + " is punched by client. Kindly Visit it:";
                //var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                //List<string> list = new List<string>();
                //foreach (var item in SOIds)
                //{
                //    var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                //    if (id != null)
                //    {
                //        foreach (var items in id)
                //        {
                //            list.Add(items);
                //        }
                //    }
                //}

                //if (list != null)
                //{
                //    var result = new CommonController().PushNotificationForRegistration(message, list, rm.ComplaintID, type, data.ZoneID);
                //}

                //// Notification Send to Wasa

                //var AreaID = Convert.ToInt32(data.Areas);

                //var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                //List<string> list2 = new List<string>();
                //foreach (var item in IdsforWasa)
                //{
                //    var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                //    if (id != null)
                //    {
                //        foreach (var items in id)
                //        {
                //            list2.Add(items);
                //        }
                //    }
                //}
                //if (list2 != null)
                //{
                //    var result2 = new CommonController().PushNotificationForWasa(message, list2, rm.ComplaintID, type);
                //}


                if (data.ZoneID != 9)
                {
                    var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                    List<string> list = new List<string>();
                    foreach (var item in SOIds)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list.Add(items);
                            }
                        }
                    }

                    if (list != null)
                    {
                        var result = new CommonController().PushNotificationForRegistration(message, list, rm.ComplaintID, type, data.ZoneID);
                    }

                    // Notification For KSB Management
                    var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Select(x => x.ID).ToList();
                    List<string> list1 = new List<string>();
                    foreach (var item in SOIdss)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list1.Add(items);
                            }
                        }
                    }

                    if (list1 != null)
                    {
                        var result = new CommonController().PushNotificationForRegistration(message, list1, rm.ComplaintID, type, data.ZoneID);
                    }

                    // Notification Send to Wasa

                    var AreaID = Convert.ToInt32(data.Areas);

                    var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                    List<string> list2 = new List<string>();
                    foreach (var item in IdsforWasa)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list2.Add(items);
                            }
                        }
                    }
                    if (list2 != null)
                    {
                        var result2 = new CommonController().PushNotificationForWasa(message, list2, rm.ComplaintID, type);
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
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list1.Add(items);
                            }
                        }
                        if (list1 != null)
                        {
                            var result = new CommonController().PushNotificationForRegistration(message, list1, rm.ComplaintID, type, data.ZoneID);
                        }
                    }


                    var AreaID = Convert.ToInt32(data.Areas);

                    var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                    List<string> list2 = new List<string>();
                    foreach (var item in IdsforWasa)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list2.Add(items);
                            }
                        }
                    }
                    if (list2 != null)
                    {
                        var result2 = new CommonController().PushNotificationForWasa(message, list2, rm.ComplaintID, type);
                    }
                }



                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Client Remarks Added Successfully",
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
            public int ComplaintID { get; set; }
            public int SOID { get; set; }
            public string Remarks { get; set; }
            public string SOName { get; set; }


        }

    }
}
