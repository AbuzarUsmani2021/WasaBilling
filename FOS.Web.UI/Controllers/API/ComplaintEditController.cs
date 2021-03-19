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
    public class ComplaintEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequest obj)
        {
            var JobObj = new Job();
            var JobDet = new JobsDetail();

            if (obj.RoleID == 2)
            {
                try
                {
                    JobObj = db.Jobs.Where(u => u.ID == obj.ID).FirstOrDefault();
                    JobObj.PersonName = obj.Name;
                    JobObj.ComplaintStatusId = obj.StatusID;
                    JobObj.PriorityId = obj.PriorityId;
                    JobObj.FaultTypeId = obj.FaulttypeId;
                    JobObj.ResolvedHours = obj.ResolvedHour;
                    // JobObj.InitialRemarks = obj.Remarks;
                    JobObj.FaultTypeDetailID = obj.FaulttypeDetailId;

                    if (obj.StatusID == 3 || obj.StatusID==1003)
                    {
                        JobObj.ResolvedAt = DateTime.UtcNow.AddHours(5);
                    }

                    JobObj.ComplainttypeID = obj.ComplaintTypeID;
                    JobObj.LastUpdated = DateTime.UtcNow.AddHours(5);
                    db.SaveChanges();

                    JobsDetail jobDetail = new JobsDetail();
                    jobDetail.ID = db.JobsDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                    jobDetail.JobID = JobObj.ID;
                    jobDetail.PRemarks = obj.Remarks;
                    jobDetail.AssignedToSaleOfficer = obj.AssignedToID;
                    jobDetail.RetailerID = JobObj.SiteID;
                    jobDetail.IsPublished = 1;
                    jobDetail.ProgressStatusID = obj.ProgressStatusId;
                    jobDetail.ProgressStatusRemarks = obj.ProgressStatusOtherRemarks;
                    jobDetail.WorkDoneID = obj.WorkDoneID;
                    jobDetail.ActivityType = obj.FaultTypeDetailOtherRemarks;
                    jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
                    jobDetail.DateComplete = DateTime.UtcNow.AddHours(5);
                    jobDetail.SalesOficerID = obj.SaleOfficerID;
                    jobDetail.ChildFaultTypeDetailID = obj.FaulttypeDetailId;
                    jobDetail.ChildFaultTypeID = obj.FaulttypeId;
                    jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                    jobDetail.ChildStatusID = obj.StatusID;
                    jobDetail.ChildAssignedSaleOfficerID = obj.AssignedToID;
                    if (obj.Picture1 == "" || obj.Picture1 == null)
                    {
                        jobDetail.Picture1 = null;
                    }
                    else
                    {
                        jobDetail.Picture1 = ConvertIntoByte(obj.Picture1, "Complaint", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }
                    if (obj.Picture2 == "" || obj.Picture2 == null)
                    {
                        jobDetail.Picture2 = null;
                    }
                    else
                    {
                        jobDetail.Picture2 = ConvertIntoByte1(obj.Picture2, "Complaint1", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }
                    if (obj.Picture3 == "" || obj.Picture3 == null)
                    {
                        jobDetail.Picture3 = null;
                    }
                    else
                    {
                        jobDetail.Picture3 = ConvertIntoByte2(obj.Picture3, "Complaint2", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }

                    db.JobsDetails.Add(jobDetail);

                    Tbl_ComplaintHistory history = new Tbl_ComplaintHistory();
                    history.ID = db.Tbl_ComplaintHistory.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                    history.JobID = JobObj.ID;
                    history.JobDetailID = jobDetail.ID;
                    history.FaultTypeDetailRemarks = obj.FaultTypeDetailOtherRemarks;
                    history.ProgressStatusRemarks = jobDetail.ProgressStatusRemarks;
                    history.FaultTypeId = obj.FaulttypeId;
                    history.FaultTypeDetailID = obj.FaulttypeDetailId;
                    history.ProgressStatusID = obj.ProgressStatusId;
                    history.TicketNo = JobObj.TicketNo;
                    history.InitialRemarks = JobObj.InitialRemarks;
                    history.ComplaintStatusId = obj.StatusID;
                    history.Picture1 = jobDetail.Picture1;
                    history.Picture2 = jobDetail.Picture2;
                    history.Picture3 = jobDetail.Picture3;
                    history.SiteID = jobDetail.RetailerID;
                    history.LaunchedById = JobObj.SaleOfficerID;
                    history.UpdateRemarks = jobDetail.PRemarks;
                    history.PriorityId = obj.PriorityId;
                    history.AssignedToSaleOfficer = obj.AssignedToID;
                    history.FirstAssignedSO = obj.AssignedToID;
                    history.IsActive = true;
                    history.IsPublished = 1;


                    history.CreatedDate = DateTime.UtcNow.AddHours(5);

                    db.Tbl_ComplaintHistory.Add(history);

                    var secondLastdata = db.Tbl_ComplaintHistory.OrderByDescending(s => s.ID).FirstOrDefault();

                    if (secondLastdata == null)
                    {
                        var data = db.Tbl_ComplaintHistory.FirstOrDefault();

                        ComplaintNotification notify = new ComplaintNotification();
                        notify.ID = db.ComplaintNotifications.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        notify.JobID = JobObj.ID;
                        notify.JobDetailID = jobDetail.ID;
                        notify.ComplaintHistoryID = history.ID;

                        if (data.SiteID == history.SiteID)
                        {
                            notify.IsSiteIDChanged = false;
                        }
                        else
                        {
                            notify.IsSiteIDChanged = true;
                        }
                        if (data.FaultTypeId == history.FaultTypeId)
                        {
                            notify.IsFaulttypeIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeIDChanged = true;
                        }

                        notify.IsSiteCodeChanged = false;
                        if (data.FaultTypeDetailID == history.FaultTypeDetailID)
                        {
                            notify.IsFaulttypeDetailIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailIDChanged = true;
                        }
                        if (data.PriorityId == history.PriorityId)
                        {
                            notify.IsPriorityIDChanged = false;
                        }
                        else
                        {
                            notify.IsPriorityIDChanged = true;
                        }
                        if (data.ComplaintStatusId == history.ComplaintStatusId)
                        {
                            notify.IsComplaintStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsComplaintStatusIDChanged = true;
                        }
                        if (data.PersonName == history.PersonName)
                        {
                            notify.IsPersonNameChanged = false;
                        }
                        else
                        {
                            notify.IsPersonNameChanged = true;
                        }


                        notify.IsPicture1Changed = false;
                        notify.IsPicture2Changed = false;
                        notify.IsPicture3Changed = false;

                        if (data.ProgressStatusID == history.ProgressStatusID)
                        {
                            notify.IsProgressStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusIDChanged = true;
                        }

                        if (data.ProgressStatusRemarks == history.ProgressStatusRemarks)
                        {
                            notify.IsProgressStatusRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusRemarksChanged = true;
                        }

                        if (data.FaultTypeDetailRemarks == history.FaultTypeDetailRemarks)
                        {
                            notify.IsFaulttypeDetailRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailRemarksChanged = true;
                        }
                        if (data.AssignedToSaleOfficer == history.AssignedToSaleOfficer)
                        {
                            notify.IsAssignedSaleOfficerChanged = false;
                        }
                        else
                        {
                            notify.IsAssignedSaleOfficerChanged = true;
                        }
                        if (data.UpdateRemarks == history.UpdateRemarks)
                        {
                            notify.IsUpdateRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsUpdateRemarksChanged = true;
                        }

                        notify.CreatedDate = DateTime.UtcNow.AddHours(5);
                        db.ComplaintNotifications.Add(notify);
                        var UID = int.Parse(JobObj.Areas);
                        var IDs = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == UID).Select(x => x.SOID).Distinct().ToList();
                        foreach (var item in IDs)
                        {

                            NotificationSeen seen = new NotificationSeen();

                            seen.JobID = JobObj.ID;
                            seen.JobDetailID = jobDetail.ID;
                            seen.ComplainthistoryID = history.ID;
                            seen.ComplaintNotificationID = notify.ID;
                            seen.IsSeen = false;
                            seen.SOID = item;

                            db.NotificationSeens.Add(seen);
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        ComplaintNotification notify = new ComplaintNotification();
                        notify.ID = db.ComplaintNotifications.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        notify.JobID = JobObj.ID;
                        notify.JobDetailID = jobDetail.ID;
                        notify.ComplaintHistoryID = history.ID;

                        if (secondLastdata.SiteID == history.SiteID)
                        {
                            notify.IsSiteIDChanged = false;
                        }
                        else
                        {
                            notify.IsSiteIDChanged = true;
                        }
                        if (secondLastdata.FaultTypeId == history.FaultTypeId)
                        {
                            notify.IsFaulttypeIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeIDChanged = true;
                        }

                        notify.IsSiteCodeChanged = false;
                        if (secondLastdata.FaultTypeDetailID == history.FaultTypeDetailID)
                        {
                            notify.IsFaulttypeDetailIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailIDChanged = true;
                        }
                        if (secondLastdata.PriorityId == history.PriorityId)
                        {
                            notify.IsPriorityIDChanged = false;
                        }
                        else
                        {
                            notify.IsPriorityIDChanged = true;
                        }
                        if (secondLastdata.ComplaintStatusId == history.ComplaintStatusId)
                        {
                            notify.IsComplaintStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsComplaintStatusIDChanged = true;
                        }
                        if (secondLastdata.PersonName == history.PersonName)
                        {
                            notify.IsPersonNameChanged = false;
                        }
                        else
                        {
                            notify.IsPersonNameChanged = true;
                        }


                        notify.IsPicture1Changed = false;
                        notify.IsPicture2Changed = false;
                        notify.IsPicture3Changed = false;

                        if (secondLastdata.ProgressStatusID == history.ProgressStatusID)
                        {
                            notify.IsProgressStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusIDChanged = true;
                        }

                        if (secondLastdata.ProgressStatusRemarks == history.ProgressStatusRemarks)
                        {
                            notify.IsProgressStatusRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusRemarksChanged = true;
                        }

                        if (secondLastdata.FaultTypeDetailRemarks == history.FaultTypeDetailRemarks)
                        {
                            notify.IsFaulttypeDetailRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailRemarksChanged = true;
                        }
                        if (secondLastdata.AssignedToSaleOfficer == history.AssignedToSaleOfficer)
                        {
                            notify.IsAssignedSaleOfficerChanged = false;
                        }
                        else
                        {
                            notify.IsAssignedSaleOfficerChanged = true;
                        }
                        if (secondLastdata.UpdateRemarks == history.UpdateRemarks)
                        {
                            notify.IsUpdateRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsUpdateRemarksChanged = true;
                        }
                        notify.IsSeen = false;
                        notify.CreatedDate = DateTime.UtcNow.AddHours(5);
                        db.ComplaintNotifications.Add(notify);

                        var UID = int.Parse(JobObj.Areas);
                        var IDs = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == UID).Select(x => x.SOID).Distinct().ToList();

                        foreach (var item in IDs)
                        {

                            NotificationSeen seen = new NotificationSeen();

                            seen.JobID = JobObj.ID;
                            seen.JobDetailID = jobDetail.ID;
                            seen.ComplainthistoryID = history.ID;
                            seen.ComplaintNotificationID = notify.ID;
                            seen.IsSeen = false;
                            seen.SOID = item;

                            db.NotificationSeens.Add(seen);
                            db.SaveChanges();
                        }
                    }


                    db.SaveChanges();




                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Complaint Updated Failed");
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Complaint Edit Failed",
                        ResultType = ResultType.Exception,
                        Exception = ex,
                        ValidationErrors = null
                    };
                }

                if (obj.AssignedToID != 0)
                {
                    // Notification To Assigned FS
                    string message = "Complaint Is Assigned and Complaint No is " + JobObj.TicketNo;
                    string messages = "There is an Update in Complaint No" + JobObj.TicketNo + " Kindly View it.";
                    string type = "Progress";
                    List<string> list = new List<string>();

                    if (JobObj.ZoneID != 9)
                    {
                        var SOIDS = db.OneSignalUsers.Where(x => x.UserID == obj.AssignedToID).Select(x => x.OneSidnalUserID).FirstOrDefault();
                       

                        if (SOIDS!=null)
                        {

                            var result = new CommonController().PushNotificationForEdit(message, SOIDS, JobObj.ID, type);
                        }
                            

                            // Notification For KSB MGT
                            var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Distinct().Select(x => x.ID).ToList();
                            List<string> lists = new List<string>();
                            foreach (var item in SOIds)
                            {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id.Count > 0)
                            {
                                foreach (var items in id)
                                {
                                    var result = new CommonController().PushNotificationForEdit(messages, items, JobObj.ID, type);
                                }
                            }

                            
                        }
                            //if (lists != null)
                            //{
                            //    var result1 = new CommonController().PushNotification(messages, lists, JobObj.ID, type);
                            //}


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
                                    var result1 = new CommonController().PushNotificationForEdit(messages, items, JobObj.ID, type);
                                }
                                }
                            }
                            //if (listss.Count > 0)
                            //{
                                
                            //}


                      
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
                                    var result1 = new CommonController().PushNotificationForEdit(messages, items, JobObj.ID, type);
                                }
                            }
                        }
                        //if (list2 != null)
                        //{
                        //    var result2 = new CommonController().PushNotificationForWasa(message, list2, JobObj.ID, type);
                        //}
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
                                    var result1 = new CommonController().PushNotificationForEdit(messages, items, JobObj.ID, type);
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
                                    var result1 = new CommonController().PushNotificationForEdit(messages, items, JobObj.ID, type);
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
                    string type = "Progress";
                    List<string> list = new List<string>();
                    string message = "There is an Update in Complaint No" + JobObj.TicketNo + " Kindly View it.";
                    if (JobObj.ZoneID != 9)
                    {

                        var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Select(x => x.ID).ToList();
                        List<string> lists = new List<string>();
                        foreach (var item in SOIds)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id.Count > 0)
                            {
                                foreach (var items in id)
                                {
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, JobObj.ID, type);
                                }
                            }
                        }
                        //if (lists != null)
                        //{
                        //    var result1 = new CommonController().PushNotification(message, lists, JobObj.ID, type);
                        //}

                        var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                        List<string> listss = new List<string>();
                        foreach (var item in SOIds)
                        {
                            var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                            if (id.Count > 0)
                            {
                                foreach (var items in id)
                                {
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, JobObj.ID, type);
                                }
                            }
                        }
                        //if (listss != null)
                        //{
                        //    var result1 = new CommonController().PushNotification(message, listss, JobObj.ID, type);
                        //}



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
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, JobObj.ID, type);
                                }
                            }
                        }
                        //if (list2 != null)
                        //{
                        //    var result2 = new CommonController().PushNotificationForWasa(message, list2, JobObj.ID, type);
                        //}
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
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, JobObj.ID, type);
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
                                    var result1 = new CommonController().PushNotificationForEdit(message, items, JobObj.ID, type);
                                }
                            }
                        }
                        //if (list2 != null)
                        //{
                        //    var result2 = new CommonController().PushNotificationForWasa(message, list2, JobObj.ID, type);
                        //}
                    }
                }
            }

            else
            {
                try
                {
                    JobObj = db.Jobs.Where(u => u.ID == obj.ID).FirstOrDefault();
                    JobDet = db.JobsDetails.Where(u => u.JobID == JobObj.ID && u.IsPublished == 1).OrderByDescending(u => u.ID).FirstOrDefault();

                    JobsDetail jobDetail = new JobsDetail();
                    jobDetail.ID = db.JobsDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                    jobDetail.JobID = JobObj.ID;
                    jobDetail.PRemarks = obj.Remarks;
                    jobDetail.AssignedToSaleOfficer = obj.AssignedToID;
                    jobDetail.RetailerID = JobObj.SiteID;
                    jobDetail.IsPublished = 0;
                    if (obj.StatusID == 3 || obj.StatusID == 1003)
                    {
                        JobObj.ResolvedAt = DateTime.UtcNow.AddHours(5);
                    }
                    jobDetail.ProgressStatusID = JobDet.ProgressStatusID;
                    jobDetail.ProgressStatusRemarks = obj.ProgressStatusOtherRemarks;
                    jobDetail.WorkDoneID = obj.WorkDoneID;
                    jobDetail.ActivityType = obj.FaultTypeDetailOtherRemarks;
                    jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                    jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
                    jobDetail.SalesOficerID = obj.SaleOfficerID;
                    jobDetail.ChildFaultTypeDetailID = obj.FaulttypeDetailId;
                    jobDetail.ChildFaultTypeID = obj.FaulttypeId;
                    jobDetail.ChildStatusID = obj.StatusID;
                    jobDetail.ChildAssignedSaleOfficerID = obj.AssignedToID;
                    if (obj.Picture1 == "" || obj.Picture1 == null)
                    {
                        jobDetail.Picture1 = null;
                    }
                    else
                    {
                        jobDetail.Picture1 = ConvertIntoByte(obj.Picture1, "Complaint", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }
                    if (obj.Picture2 == "" || obj.Picture2 == null)
                    {
                        jobDetail.Picture2 = null;
                    }
                    else
                    {
                        jobDetail.Picture2 = ConvertIntoByte1(obj.Picture2, "Complaint1", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }
                    if (obj.Picture3 == "" || obj.Picture3 == null)
                    {
                        jobDetail.Picture3 = null;
                    }
                    else
                    {
                        jobDetail.Picture3 = ConvertIntoByte2(obj.Picture3, "Complaint2", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }

                    db.JobsDetails.Add(jobDetail);

                    if (JobDet.AssignedToSaleOfficer != obj.AssignedToID)
                    {
                        var data = db.JobsDetails.Where(u => u.JobID == JobObj.ID && u.IsPublished == 1).ToList();
                        foreach (var item in data)
                        {
                            item.AssignedToSaleOfficer = obj.AssignedToID;
                            item.ChildAssignedSaleOfficerID = obj.AssignedToID;
                            db.SaveChanges();
                        }


                        var data2 = db.Tbl_ComplaintHistory.Where(x => x.JobID == JobObj.ID).ToList();

                        foreach (var item in data2)
                        {
                            item.AssignedToSaleOfficer = obj.AssignedToID;
                            // item.FirstAssignedSO = item.AssignedToSaleOfficer;
                            db.SaveChanges();
                        }
                    }


                    db.SaveChanges();
                    Tbl_ComplaintHistory history = new Tbl_ComplaintHistory();

                    history.JobID = JobObj.ID;
                    history.JobDetailID = jobDetail.ID;
                    history.FaultTypeDetailRemarks = obj.FaultTypeDetailOtherRemarks;
                    history.ProgressStatusRemarks = jobDetail.ProgressStatusRemarks;
                    history.FaultTypeId = obj.FaulttypeId;
                    history.FaultTypeDetailID = obj.FaulttypeDetailId;
                    history.ComplaintStatusId = obj.StatusID;
                    history.ProgressStatusID = obj.ProgressStatusId;
                    history.AssignedToSaleOfficer = obj.AssignedToID;
                    history.LaunchedById = JobObj.SaleOfficerID;
                    history.Picture1 = jobDetail.Picture1;
                    history.Picture2 = jobDetail.Picture2;
                    history.Picture3 = jobDetail.Picture3;
                    history.SiteID = jobDetail.RetailerID;
                    history.TicketNo = JobObj.TicketNo;
                    history.InitialRemarks = JobObj.InitialRemarks;
                    history.PriorityId = obj.PriorityId;
                    history.IsActive = true;
                    history.IsPublished = 0;
                    history.UpdateRemarks = jobDetail.PRemarks;

                    history.CreatedDate = DateTime.UtcNow.AddHours(5);

                    db.Tbl_ComplaintHistory.Add(history);


                    var secondLastdata = db.Tbl_ComplaintHistory.OrderByDescending(s => s.ID).FirstOrDefault();

                    if (secondLastdata == null)
                    {
                        var data = db.Tbl_ComplaintHistory.FirstOrDefault();

                        ComplaintNotification notify = new ComplaintNotification();
                        notify.ID = db.ComplaintNotifications.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        notify.JobID = JobObj.ID;
                        notify.JobDetailID = jobDetail.ID;
                        notify.ComplaintHistoryID = history.ID;

                        if (data.SiteID == history.SiteID)
                        {
                            notify.IsSiteIDChanged = false;
                        }
                        else
                        {
                            notify.IsSiteIDChanged = true;
                        }
                        if (data.FaultTypeId == history.FaultTypeId)
                        {
                            notify.IsFaulttypeIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeIDChanged = true;
                        }

                        notify.IsSiteCodeChanged = false;
                        if (data.FaultTypeDetailID == history.FaultTypeDetailID)
                        {
                            notify.IsFaulttypeDetailIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailIDChanged = true;
                        }
                        if (data.PriorityId == history.PriorityId)
                        {
                            notify.IsPriorityIDChanged = false;
                        }
                        else
                        {
                            notify.IsPriorityIDChanged = true;
                        }
                        if (data.ComplaintStatusId == history.ComplaintStatusId)
                        {
                            notify.IsComplaintStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsComplaintStatusIDChanged = true;
                        }
                        if (data.PersonName == history.PersonName)
                        {
                            notify.IsPersonNameChanged = false;
                        }
                        else
                        {
                            notify.IsPersonNameChanged = true;
                        }

                        if (secondLastdata.Picture1 == history.Picture1)
                        {
                            notify.IsPicture1Changed = false;
                        }
                        else
                        {
                            notify.IsPicture1Changed = true;
                        }

                        if (secondLastdata.Picture2 == history.Picture2)
                        {
                            notify.IsPicture2Changed = false;
                        }
                        else
                        {
                            notify.IsPicture2Changed = true;
                        }

                        if (secondLastdata.Picture3 == history.Picture3)
                        {
                            notify.IsPicture3Changed = false;
                        }
                        else
                        {
                            notify.IsPicture3Changed = true;
                        }

                        if (data.ProgressStatusID == history.ProgressStatusID)
                        {
                            notify.IsProgressStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusIDChanged = true;
                        }

                        if (data.ProgressStatusRemarks == history.ProgressStatusRemarks)
                        {
                            notify.IsProgressStatusRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusRemarksChanged = true;
                        }

                        if (data.FaultTypeDetailRemarks == history.FaultTypeDetailRemarks)
                        {
                            notify.IsFaulttypeDetailRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailRemarksChanged = true;
                        }
                        if (data.AssignedToSaleOfficer == history.AssignedToSaleOfficer)
                        {
                            notify.IsAssignedSaleOfficerChanged = false;
                        }
                        else
                        {
                            notify.IsAssignedSaleOfficerChanged = true;
                        }
                        if (data.UpdateRemarks == history.UpdateRemarks)
                        {
                            notify.IsUpdateRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsUpdateRemarksChanged = true;
                        }

                        notify.CreatedDate = DateTime.UtcNow.AddHours(5);
                        db.ComplaintNotifications.Add(notify);
                        var UID = int.Parse(JobObj.Areas);
                        var IDs = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == UID).Select(x => x.SOID).Distinct().ToList();
                        foreach (var item in IDs)
                        {

                            NotificationSeen seen = new NotificationSeen();

                            seen.JobID = JobObj.ID;
                            seen.JobDetailID = jobDetail.ID;
                            seen.ComplainthistoryID = history.ID;
                            seen.ComplaintNotificationID = notify.ID;
                            seen.IsSeen = false;
                            seen.SOID = item;

                            db.NotificationSeens.Add(seen);
                            db.SaveChanges();
                        }
                    }

                    else
                    {
                        ComplaintNotification notify = new ComplaintNotification();
                        notify.ID = db.ComplaintNotifications.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                        notify.JobID = JobObj.ID;
                        notify.JobDetailID = jobDetail.ID;
                        notify.ComplaintHistoryID = history.ID;

                        if (secondLastdata.SiteID == history.SiteID)
                        {
                            notify.IsSiteIDChanged = false;
                        }
                        else
                        {
                            notify.IsSiteIDChanged = true;
                        }
                        if (secondLastdata.FaultTypeId == history.FaultTypeId)
                        {
                            notify.IsFaulttypeIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeIDChanged = true;
                        }

                        notify.IsSiteCodeChanged = false;
                        if (secondLastdata.FaultTypeDetailID == history.FaultTypeDetailID)
                        {
                            notify.IsFaulttypeDetailIDChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailIDChanged = true;
                        }
                        if (secondLastdata.PriorityId == history.PriorityId)
                        {
                            notify.IsPriorityIDChanged = false;
                        }
                        else
                        {
                            notify.IsPriorityIDChanged = true;
                        }
                        if (secondLastdata.ComplaintStatusId == history.ComplaintStatusId)
                        {
                            notify.IsComplaintStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsComplaintStatusIDChanged = true;
                        }
                        if (secondLastdata.PersonName == history.PersonName)
                        {
                            notify.IsPersonNameChanged = false;
                        }
                        else
                        {
                            notify.IsPersonNameChanged = true;
                        }
                        if (secondLastdata.Picture1 == history.Picture1)
                        {
                            notify.IsPicture1Changed = false;
                        }
                        else
                        {
                            notify.IsPicture1Changed = true;
                        }

                        if (secondLastdata.Picture2 == history.Picture2)
                        {
                            notify.IsPicture2Changed = false;
                        }
                        else
                        {
                            notify.IsPicture2Changed = true;
                        }

                        if (secondLastdata.Picture3 == history.Picture3)
                        {
                            notify.IsPicture3Changed = false;
                        }
                        else
                        {
                            notify.IsPicture3Changed = true;
                        }



                        if (secondLastdata.ProgressStatusID == history.ProgressStatusID)
                        {
                            notify.IsProgressStatusIDChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusIDChanged = true;
                        }

                        if (secondLastdata.ProgressStatusRemarks == history.ProgressStatusRemarks)
                        {
                            notify.IsProgressStatusRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsProgressStatusRemarksChanged = true;
                        }

                        if (secondLastdata.FaultTypeDetailRemarks == history.FaultTypeDetailRemarks)
                        {
                            notify.IsFaulttypeDetailRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsFaulttypeDetailRemarksChanged = true;
                        }
                        if (secondLastdata.AssignedToSaleOfficer == history.AssignedToSaleOfficer)
                        {
                            notify.IsAssignedSaleOfficerChanged = false;
                        }
                        else
                        {
                            notify.IsAssignedSaleOfficerChanged = true;
                        }
                        if (secondLastdata.UpdateRemarks == history.UpdateRemarks)
                        {
                            notify.IsUpdateRemarksChanged = false;
                        }
                        else
                        {
                            notify.IsUpdateRemarksChanged = true;
                        }
                        notify.IsSeen = false;
                        notify.CreatedDate = DateTime.UtcNow.AddHours(5);
                        db.ComplaintNotifications.Add(notify);
                        var UID = int.Parse(JobObj.Areas);
                        var IDs = db.SOZoneAndTowns.Where(x => x.CityID == JobObj.CityID && x.AreaID == UID).Select(x => x.SOID).Distinct().ToList();
                        foreach (var item in IDs)
                        {

                            NotificationSeen seen = new NotificationSeen();

                            seen.JobID = JobObj.ID;
                            seen.JobDetailID = jobDetail.ID;
                            seen.ComplainthistoryID = history.ID;
                            seen.ComplaintNotificationID = notify.ID;
                            seen.IsSeen = false;
                            seen.SOID = item;

                            db.NotificationSeens.Add(seen);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Instance.Error(ex, "Complaint Updated Failed");
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Complaint Edit Failed",
                        ResultType = ResultType.Exception,
                        Exception = ex,
                        ValidationErrors = null
                    };
                }

                string type = "Progress";
                string message = "There Is An Update in Complaint No" + JobObj.TicketNo + " Which Is Performed By Field Staff. Kindly Publish it.";

                if (JobObj.ZoneID != 9)
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
                                var result1 = new CommonController().PushNotificationForEdit(message, items, JobObj.ID, type);
                            }
                        }
                    }

                    //if (list != null)
                    //{

                    //    var result = new CommonController().PushNotification(message, list, JobObj.ID, type);
                    //}
                }
                else
                {  // Notification For Progressive Management
                    var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 6 && x.RoleID == 2).Select(x => x.ID).ToList();
                    List<string> list1 = new List<string>();
                    foreach (var item in SOIdss)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                var result1 = new CommonController().PushNotificationForEdit(message, items, JobObj.ID, type);
                            }
                        }
                        //if (list1 != null)
                        //{
                        //    var result = new CommonController().PushNotification(message, list1, JobObj.ID, type);
                        //}
                    }

                }
            }
            return new Result<SuccessResponse>
            {
                Data = null,
                Message = "Complaint Updated Successfully",
                ResultType = ResultType.Success,
                Exception = null,
                ValidationErrors = null
            };
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
        public class DailyActivityRequest
        {
           
            public int ID { get; set; }


            public int SaleOfficerID { get; set; }

            public int ResolvedHour { get; set; }
            public int AssignedToID { get; set; }


            public int? ComplaintTypeID { get; set; }


            public string FaultTypeDetailOtherRemarks { get; set; }
            public string ProgressStatusOtherRemarks { get; set; }




            public Nullable<int> StatusID { get; set; }

            public Nullable<int> RetailerID { get; set; }

            public int RoleID { get; set; }
            public int SiteId { get; set; }
            public int? FaulttypeId { get; set; }

            public int? PriorityId { get; set; }

            public int? ProgressStatusId { get; set; }

            public int? WorkDoneID { get; set; }
            public string ProgressStatusName { get; set; }
            public int? FaulttypeDetailId { get; set; }

            public string Picture1 { get; set; }
            public string Picture2 { get; set; }
            public string Picture3 { get; set; }
            public string Picture4 { get; set; }

            public string Picture5 { get; set; }
            public string Name { get; set; }

            public string Token { get; set; }
            public string Remarks { get; set; }

            public List<CityData> Cities { get; set; }
            public List<RegionData> Regions { get; set; }
            public List<AreaData> Areas { get; set; }
            public List<RegionData> Client { get; set; }
            public List<FaultTypeData> faultTypes { get; set; }
            public List<PriorityData> priorityDatas { get; set; }
            public List<ComplaintStatus> complaintStatuses { get; set; }
            public List<ComplaintStatus> ProgressStatus { get; set; }
            public List<ComplaintStatus> ComplaintTypes { get; set; }
            public List<FaultTypeData> faultTypesDetail { get; set; }
            public List<ComplaintLaunchedBy> LaunchedBy { get; set; }
            public List<SubDivisionData> SubDivisions { get; set; }
            public List<RetailerData> Sites { get; set; }
            public List<SaleOfficerData> SaleOfficers { get; set; }

        }

        public class StockItemModel
        {
     
            public int ItemID { get; set; }
            public decimal Quantity { get; set; }
            public decimal Price { get; set; }

        }
    }
}