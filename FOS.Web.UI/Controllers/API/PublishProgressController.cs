using FOS.DataLayer;
using FOS.Shared;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Services.Protocols;

namespace FOS.Web.UI.Controllers.API
{
    public class PublishProgressController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequests obj)
        {
            var jobDetail = new JobsDetail();
            var job = new Job();
            var jobhistory = new Tbl_ComplaintHistory();
            try
            {
                int flag = 0;
                jobhistory= db.Tbl_ComplaintHistory.Where(u => u.JobDetailID == obj.ID).FirstOrDefault();
                if (jobhistory.IsPublished == 0)
                {
                    jobhistory.IsPublished = 1;
                    flag = 1;
                }
                jobDetail = db.JobsDetails.Where(u => u.ID == obj.ID).FirstOrDefault();
                jobDetail.IsPublished = obj.IsPublished;
                jobDetail.ProgressStatusID = jobhistory.ProgressStatusID;
                jobDetail.AssignedToSaleOfficer = jobhistory.AssignedToSaleOfficer;
                jobDetail.ProgressStatusRemarks = jobhistory.ProgressStatusRemarks;
                jobDetail.ActivityType = jobhistory.FaultTypeDetailRemarks;
                jobDetail.DateComplete= DateTime.UtcNow.AddHours(5);
                job = db.Jobs.Where(u => u.ID == jobhistory.JobID).FirstOrDefault();
                job.FaultTypeId = jobhistory.FaultTypeId;
                job.FaultTypeDetailID = jobhistory.FaultTypeDetailID;
                job.ComplaintStatusId = jobhistory.ComplaintStatusId;
                job.ResolvedAt= DateTime.UtcNow.AddHours(5);
                job.LastUpdated= DateTime.UtcNow.AddHours(5);


                db.SaveChanges();
                if (flag==1)

                {
                    // Notification Send to KSB CC
                    string type = "Progress";
                    string message = "There is an Update Which is Published By CC in Complaint No" + job.TicketNo + " Kindly View it.";
                    if (job.ZoneID != 9)
                    {
                        var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                        List<string> list = new List<string>();
                        foreach (var item in SOIds)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list.Add(items);
                                }
                            }
                        }

                        if (list != null)
                        {
                            var result = new CommonController().PushNotification(message, list, job.ID, type);
                        }

                        // Notification For KSB Management
                        var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Select(x => x.ID).ToList();
                        List<string> list1 = new List<string>();
                        foreach (var item in SOIdss)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list1.Add(items);
                                }
                            }
                        }

                        if (list1 != null)
                        {
                            var result = new CommonController().PushNotification(message, list1, job.ID, type);
                        }

                        // Notification Send to Wasa

                        var AreaID = Convert.ToInt32(job.Areas);

                        var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == job.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        List<string> list2 = new List<string>();
                        foreach (var item in IdsforWasa)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list2.Add(items);
                                }
                            }
                        }
                        if (list2 != null)
                        {
                            var result2 = new CommonController().PushNotificationForWasa(message, list2, job.ID, type);
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
                                    list1.Add(items);
                                }
                            }
                            if (list1 != null)
                            {
                                var result = new CommonController().PushNotification(message, list1, job.ID, type);
                            }
                        }


                        var AreaID = Convert.ToInt32(job.Areas);

                        var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == job.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        List<string> list2 = new List<string>();
                        foreach (var item in IdsforWasa)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                            if (id != null)
                            {
                                foreach (var items in id)
                                {
                                    list2.Add(items);
                                }
                            }
                        }
                        if (list2 != null)
                        {
                            var result2 = new CommonController().PushNotificationForWasa(message, list2, job.ID, type);
                        }
                    }
                }

                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Publish Progress Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ProgressView Edit Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Complaint Edit Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }


        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }

        public string ConvertIntoByte1(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }
        public string ConvertIntoByte2(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

     
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }
        public class SuccessResponse
        {

        }
        public class DailyActivityRequests
        {

            public int ID { get; set; }


           
            public int? IsPublished { get; set; }

          




        }

       
    }
}