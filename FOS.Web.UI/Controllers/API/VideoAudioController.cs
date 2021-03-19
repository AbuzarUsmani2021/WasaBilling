using FOS.DataLayer;
using FOS.Shared;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class VideoAudioController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public async Task<string> Upload()
        { 
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(5, 99999999);
                var filename = "";
                var ctx = HttpContext.Current;
                var uploadPath = System.Web.HttpContext.Current.Server.MapPath(@"~/ComplaintVideos/");
                Directory.CreateDirectory(uploadPath);
                var provider = new MultipartFormDataStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(provider);

                var Type = HttpContext.Current.Request.Params["Type"];
                var ids = HttpContext.Current.Request.Params["ID"];
                var roleID = HttpContext.Current.Request.Params["RoleID"];
                var DetailID = Convert.ToInt32(ids);
                var roleid = Convert.ToInt32(roleID);


                if (Type == "Video")
                {

                    foreach (var file in provider.FileData)
                    {
                         filename = (file.Headers.ContentDisposition.FileName);
                        filename = filename.Trim('"');
                        filename = randomNumber + filename;
                        var loc = file.LocalFileName;

                        var filepath = Path.Combine(uploadPath, filename);
                        File.Move(loc, filepath);
                    }

                    var jobDetailID = db.JobsDetails.Where(x => x.ID == DetailID).FirstOrDefault();
                    var JobObj = db.Jobs.Where(x => x.ID == jobDetailID.JobID).FirstOrDefault();
                    jobDetailID.Video = "/ComplaintVideos/" + filename;
                    jobDetailID.VideoDate= DateTime.UtcNow.AddHours(5);
                  
                    var HistoryID = db.Tbl_ComplaintHistory.Where(x => x.JobDetailID == DetailID).FirstOrDefault();
                    HistoryID.Video= "/ComplaintVideos/" + filename;
                    HistoryID.VideoDate= DateTime.UtcNow.AddHours(5);
                    if (HistoryID.ComplaintStatusId != 2003)
                    {
                        if (roleid == 2)
                        {
                            jobDetailID.IsPublished =1;
                            HistoryID.IsPublished = 1;
                        }
                        else
                        {
                            jobDetailID.IsPublished = 0;
                            HistoryID.IsPublished = 0;
                        }
                   
                        }
                    db.SaveChanges();
                    string message = "Video Is Attached on Complaint No " + JobObj.TicketNo + ". Kindly View it";
                    string type = "ProgressView";

                    if (JobObj.ZoneID != 9)
                    {
                        //var SOIDS = db.OneSignalUsers.Where(x => x.UserID == obj.AssignedToID).Select(x => x.OneSidnalUserID).FirstOrDefault();


                        //if (SOIDS != null)
                        //{

                        //    var result = new CommonController().PushNotificationForEdit(message, SOIDS, JobObj.ID, type);
                        //}


                        //// Notification For KSB MGT
                        //var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Distinct().Select(x => x.ID).ToList();
                        //List<string> lists = new List<string>();
                        //foreach (var item in SOIds)
                        //{
                        //    var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        //    if (id.Count > 0)
                        //    {
                        //        foreach (var items in id)
                        //        {
                        //            var result = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                        //        }
                        //    }


                        //}
                        ////if (lists != null)
                        ////{
                        ////    var result1 = new CommonController().PushNotification(messages, lists, JobObj.ID, type);
                        ////}


                        // Notification For KSB CC
                        var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                        List<string> listss = new List<string>();
                        foreach (var item in SOIdss)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id.Count > 0)
                            {
                                foreach (var items in id)
                                {
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                                }
                            }
                        }
                        //if (listss.Count > 0)
                        //{

                        //}



                        //var AreaID = Convert.ToInt32(JobObj.Areas);

                        //var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        //List<string> list2 = new List<string>();
                        //foreach (var item in IdsforWasa)
                        //{
                        //    var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                        //    if (id.Count > 0)
                        //    {
                        //        foreach (var items in id)
                        //        {
                        //            var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                        //        }
                        //    }
                        //}
                        ////if (list2 != null)
                        ////{
                        ////    var result2 = new CommonController().PushNotificationForWasa(message, list2, JobObj.ID, type);
                        ////}
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
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                                }
                            }
                            //if (list1 != null)
                            //{
                            //    var result = new CommonController().PushNotification(message, list1, JobObj.ID, type);
                            //}
                        }


                        var AreaID = Convert.ToInt32(JobObj.Areas);

                        var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        List<string> list2 = new List<string>();
                        foreach (var item in IdsforWasa)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                            if (id.Count > 0)
                            {
                                foreach (var items in id)
                                {
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                                }
                            }
                        }
                        //if (list2 != null)
                        //{
                        //    var result2 = new CommonController().PushNotificationForWasa(message, list2, JobObj.ID, type);
                        //}
                    }

                }
                else
                {
                    foreach (var file in provider.FileData)
                    {
                        filename = (file.Headers.ContentDisposition.FileName);
                        filename = filename.Trim('"');
                        filename = randomNumber + filename;
                        var loc = file.LocalFileName;

                        var filepath = Path.Combine(uploadPath, filename);
                        File.Move(loc, filepath);
                    }

                    var jobDetailID = db.JobsDetails.Where(x => x.ID == DetailID).FirstOrDefault();
                    var JobObj = db.Jobs.Where(x => x.ID == jobDetailID.JobID).FirstOrDefault();
                    jobDetailID.Audio = "/ComplaintVideos/" + filename;
                    jobDetailID.AudioDate= DateTime.UtcNow.AddHours(5);
                    
                    var HistoryID = db.Tbl_ComplaintHistory.Where(x => x.JobDetailID == DetailID).FirstOrDefault();
                    HistoryID.AudioDate= DateTime.UtcNow.AddHours(5);
                    HistoryID.Audio = "/ComplaintVideos/" + filename;
                    if (HistoryID.ComplaintStatusId != 2003)
                    {
                        if (roleid == 2)
                        {
                            jobDetailID.IsPublished = 1;
                            HistoryID.IsPublished = 1;
                        }
                        else
                        {
                            jobDetailID.IsPublished = 0;
                            HistoryID.IsPublished = 0;
                        }
                    }
                  
                    db.SaveChanges();
                    string message = "Audio Is Attached on Complaint No " + JobObj.TicketNo + ". Kindly View it";
                    string type = "ProgressView";

                    if (JobObj.ZoneID != 9)
                    {
                        //var SOIDS = db.OneSignalUsers.Where(x => x.UserID == obj.AssignedToID).Select(x => x.OneSidnalUserID).FirstOrDefault();


                        //if (SOIDS != null)
                        //{

                        //    var result = new CommonController().PushNotificationForEdit(message, SOIDS, JobObj.ID, type);
                        //}


                        //// Notification For KSB MGT
                        //var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Distinct().Select(x => x.ID).ToList();
                        //List<string> lists = new List<string>();
                        //foreach (var item in SOIds)
                        //{
                        //    var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        //    if (id.Count > 0)
                        //    {
                        //        foreach (var items in id)
                        //        {
                        //            var result = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                        //        }
                        //    }


                        //}
                        ////if (lists != null)
                        ////{
                        ////    var result1 = new CommonController().PushNotification(messages, lists, JobObj.ID, type);
                        ////}


                        // Notification For KSB CC
                        var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                        List<string> listss = new List<string>();
                        foreach (var item in SOIdss)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id.Count > 0)
                            {
                                foreach (var items in id)
                                {
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                                }
                            }
                        }
                        //if (listss.Count > 0)
                        //{

                        //}



                        //var AreaID = Convert.ToInt32(JobObj.Areas);

                        //var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        //List<string> list2 = new List<string>();
                        //foreach (var item in IdsforWasa)
                        //{
                        //    var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                        //    if (id.Count > 0)
                        //    {
                        //        foreach (var items in id)
                        //        {
                        //            var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                        //        }
                        //    }
                        //}
                        ////if (list2 != null)
                        ////{
                        ////    var result2 = new CommonController().PushNotificationForWasa(message, list2, JobObj.ID, type);
                        ////}
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
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                                }
                            }
                            //if (list1 != null)
                            //{
                            //    var result = new CommonController().PushNotification(message, list1, JobObj.ID, type);
                            //}
                        }


                        //var AreaID = Convert.ToInt32(JobObj.Areas);

                        //var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                        //List<string> list2 = new List<string>();
                        //foreach (var item in IdsforWasa)
                        //{
                        //    var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                        //    if (id.Count > 0)
                        //    {
                        //        foreach (var items in id)
                        //        {
                        //            var result1 = new CommonController().PushNotificationForEdit(message, items, DetailID, type);
                        //        }
                        //    }
                        //}
                        ////if (list2 != null)
                        ////{
                        ////    var result2 = new CommonController().PushNotificationForWasa(message, list2, JobObj.ID, type);
                        ////}
                    }
                }
               

                return "Uploaded";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }




    }
}