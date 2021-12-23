using FluentValidation.Results;
using FOS.DataLayer;
using FOS.Setup;
using FOS.Setup.Validation;
using FOS.Shared;
using FOS.Web.UI.Common;
using FOS.Web.UI.Common.CustomAttributes;
using FOS.Web.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FOS.Web.UI.Controllers
{
    public class ComplaintController : Controller
    {
        FOSDataModel db = new FOSDataModel();

        #region Complaint registration

        //Get Complaint Registration
        [CustomAuthorize]
        public ActionResult NewComplaint()
        {
            var objRetailer = new KSBComplaintData();
            objRetailer.Client = FOS.Setup.ManageRegionalHead.GetRegionalList();
            //Get Projects start
            var soid = Convert.ToInt32(Session["SORelationID"]);
            var ListOfProjects = db.SOProjects.Where(x => x.SaleOfficerID == soid).Select(x => x.ProjectID).Distinct().ToList();
            objRetailer.Projects = FOS.Setup.ManageCity.GetProjectsListForComplaintRegistration(ListOfProjects);
            //Get Projects END
            //Get zone start
            var ListOfZones = db.SOZoneAndTowns.Where(x => x.SOID == soid).Select(x => x.CityID).Distinct().ToList();
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityListForComplaintRegistration(ListOfZones);
            //Get zone END
            //Get Towns start
            var ListOfTowns = db.SOZoneAndTowns.Where(x => x.SOID == soid).Select(x => x.AreaID).Distinct().ToList();
            objRetailer.Areas = FOS.Setup.ManageCity.GetAreaListForComplaintRegistration(ListOfTowns);
            //Get Towns END
            objRetailer.Sites = FOS.Setup.ManageCity.GetSitesList();
            objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
            objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
            objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
            objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();
            return View(objRetailer);
        }

        //Post Complaint Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewComplaint([Bind(Exclude = "TID,SaleOfficers,Dealers")] KSBComplaintData newRetailer, HttpPostedFileBase Picture1, HttpPostedFileBase Picture2)
        {

            FOSDataModel dbContext = new FOSDataModel();
            newRetailer.StatusID = 2003;
            newRetailer.SaleOfficerID = (int)Session["SORelationID"];
            var TeamID = (int)Session["TeamID"];
            string path1 = "";
            string path2 = "";
            if (Picture1 != null)
            {
                var filename = Path.GetFileName(Picture1.FileName);
                path1 = Path.Combine(Server.MapPath("/Images/ComplaintImages/"), filename);
                Picture1.SaveAs(path1);
                path1 = "/Images/ComplaintImages/" + filename;
            }
            if (Picture2 != null)
            {
                var filename = Path.GetFileName(Picture2.FileName);
                path2 = Path.Combine(Server.MapPath("~/Images/ComplaintImages/"), filename);
                Picture2.SaveAs(path2);
                path2 = "/Images/ComplaintImages/" + filename;
            }
            Job retailerObj = new Job();
            var data = db.Retailers.Where(x => x.ID == newRetailer.SiteId).FirstOrDefault();
            var dateAndTime = DateTime.UtcNow.AddHours(5);
            int year = dateAndTime.Year;
            int month = dateAndTime.Month;
            string finalMonth = month.ToString().PadLeft(2, '0');
            int day = dateAndTime.Day;
            string finalday = day.ToString().PadLeft(2, '0');
            var datein = string.Format("{0}{1}{2}", year, finalMonth, finalday);
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);
            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            if (data.ZoneID == 7)
            {
                var Id1 = "O3";

                var counter = db.Jobs.Where(x => x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday && x.RegionID == data.RegionID && x.ZoneID == data.ZoneID).OrderByDescending(u => u.ID).Select(u => u.TicketNo).FirstOrDefault();


                if (counter == null)
                {
                    var ticketCount = 1;
                    string s = ticketCount.ToString().PadLeft(3, '0');
                    retailerObj.TicketNo = datein + "-" + Id1 + "-" + s;
                }
                else
                {
                    var splittedcounter = counter.Split('-');
                    var val = splittedcounter[2];
                    int value = Convert.ToInt32(val) + 1;
                    string s = value.ToString().PadLeft(3, '0');
                    retailerObj.TicketNo = datein + "-" + Id1 + "-" + s;
                }
            }
            else if (data.ZoneID == 8)
            {
                var Id2 = "O2";

                var counter = db.Jobs.Where(x => x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday && x.RegionID == data.RegionID && x.ZoneID == data.ZoneID).OrderByDescending(u => u.ID).Select(u => u.TicketNo).FirstOrDefault();


                if (counter == null)
                {
                    var ticketCount = 1;
                    string s = ticketCount.ToString().PadLeft(3, '0');
                    retailerObj.TicketNo = datein + "-" + Id2 + "-" + s;
                }
                else
                {
                    var splittedcounter = counter.Split('-');
                    var val = splittedcounter[2];
                    int value = Convert.ToInt32(val) + 1;
                    string s = value.ToString().PadLeft(3, '0');

                    retailerObj.TicketNo = datein + "-" + Id2 + "-" + s;
                }

            }
            else if (data.ZoneID == 9)
            {
                var Id3 = "O1";

                var counter = db.Jobs.Where(x => x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday && x.RegionID == data.RegionID && x.ZoneID == data.ZoneID).OrderByDescending(u => u.ID).Select(u => u.TicketNo).FirstOrDefault();


                if (counter == null)
                {
                    var ticketCount = 1;
                    string s = ticketCount.ToString().PadLeft(3, '0');
                    retailerObj.TicketNo = datein + "-" + Id3 + "-" + s;
                }
                else
                {
                    var splittedcounter = counter.Split('-');
                    var val = splittedcounter[2];
                    int value = Convert.ToInt32(val) + 1;
                    string s = value.ToString().PadLeft(3, '0');

                    retailerObj.TicketNo = datein + "-" + Id3 + "-" + s;
                }

            }
            //Add Data in Job Table
            retailerObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
            retailerObj.PersonName = newRetailer.Name;
            retailerObj.ResolvedAt = DateTime.UtcNow.AddHours(5);
            retailerObj.SiteID = newRetailer.SiteId;
            retailerObj.RegionID = data.RegionID;
            retailerObj.ZoneID = data.ZoneID;
            retailerObj.CityID = data.CityID;
            retailerObj.Areas = data.AreaID.ToString();
            retailerObj.SubDivisionID = data.SubDivisionID;
            retailerObj.ComplaintStatusId = 2003;
            retailerObj.FaultTypeId = newRetailer.FaulttypeId;
            retailerObj.LaunchedById = newRetailer.LaunchedByID;
            retailerObj.FaultTypeDetailID = newRetailer.FaulttypeDetailId;
            retailerObj.PriorityId = 0;
            retailerObj.IsActive = true;
            retailerObj.Status = true;
            retailerObj.InitialRemarks = newRetailer.Remarks;
            retailerObj.ComplainttypeID = 2;
            retailerObj.CreatedDate = DateTime.UtcNow.AddHours(5);
            retailerObj.SaleOfficerID = newRetailer.SaleOfficerID;
            db.Jobs.Add(retailerObj);
            //Add Data in JobDetail Table
            JobsDetail jobDetail = new JobsDetail();
            jobDetail.ID = db.JobsDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
            jobDetail.JobID = retailerObj.ID;
            jobDetail.ActivityType = newRetailer.FaultTypeDetailOtherRemarks;
            jobDetail.RetailerID = newRetailer.SiteId;
            jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
            jobDetail.SalesOficerID = newRetailer.SaleOfficerID;
            jobDetail.IsPublished =1;


            if (path1== "" || path1 == null)
            {
                jobDetail.Picture1 = null;
            }
            else
            {
                jobDetail.Picture1 = path1;
            }
            if (path2 == "" || path2 == null)
            {
                jobDetail.Picture2 = null;
            }
            else
            {
                jobDetail.Picture2 = path2;
            }
            db.JobsDetails.Add(jobDetail);
            //Add Data in Tbl_ComplaintHistory Table
            Tbl_ComplaintHistory history = new Tbl_ComplaintHistory();
            history.JobID = retailerObj.ID;
            history.JobDetailID = jobDetail.ID;
            history.FaultTypeDetailRemarks = newRetailer.FaultTypeDetailOtherRemarks;
            history.ProgressStatusRemarks = newRetailer.ProgressStatusOtherRemarks;
            history.FaultTypeId = newRetailer.FaulttypeId;
            history.FaultTypeDetailID = newRetailer.FaulttypeDetailId;
            history.TicketNo = retailerObj.TicketNo;
            history.ComplaintStatusId = 2003;
            history.InitialRemarks = newRetailer.Remarks;
            history.LaunchedById = newRetailer.SaleOfficerID;
            history.ComplainttypeID = 2;
            history.Picture1 = jobDetail.Picture1;
            history.Picture2 = jobDetail.Picture2;
            history.Picture3 = jobDetail.Picture3;
            history.SiteID = newRetailer.SiteId;
            history.PriorityId = 0;
            history.IsActive = true;
            history.IsPublished = 1;
            history.PersonName = retailerObj.PersonName;
            history.CreatedDate = DateTime.UtcNow.AddHours(5);
            db.Tbl_ComplaintHistory.Add(history);
            //Add Data in ComplaintNotification Table
            ComplaintNotification notify = new ComplaintNotification();
            notify.ID = db.ComplaintNotifications.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
            notify.JobID = retailerObj.ID;
            notify.JobDetailID = jobDetail.ID;
            notify.ComplaintHistoryID = history.ID;
            notify.IsSiteIDChanged = true;
            notify.IsSiteCodeChanged = true;
            notify.IsFaulttypeIDChanged = true;
            notify.IsFaulttypeDetailIDChanged = true;
            notify.IsPriorityIDChanged = true;
            notify.IsComplaintStatusIDChanged = true;
            notify.IsPersonNameChanged = true;
            notify.IsPicture1Changed = true;
            notify.IsPicture2Changed = true;
            notify.IsPicture3Changed = true;
            notify.IsProgressStatusIDChanged = false;
            notify.IsProgressStatusRemarksChanged = false;
            notify.IsFaulttypeDetailRemarksChanged = true;
            notify.IsAssignedSaleOfficerChanged = false;
            notify.IsUpdateRemarksChanged = false;
            notify.IsSeen = false;
            notify.CreatedDate = DateTime.UtcNow.AddHours(5);
            db.ComplaintNotifications.Add(notify);
            //Add Data in Notification Table
            var IDs = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == data.AreaID).Select(x => x.SOID).Distinct().ToList();
            foreach (var item in IDs)
            {

                NotificationSeen seen = new NotificationSeen();

                seen.JobID = retailerObj.ID;
                seen.JobDetailID = jobDetail.ID;
                seen.ComplainthistoryID = history.ID;
                seen.ComplaintNotificationID = notify.ID;
                seen.IsSeen = false;
                seen.SOID = item;
                db.NotificationSeens.Add(seen);
                db.SaveChanges();
            }
            string type = "Registraion";
            string message = "Complaint registered Successfully and ComplaintID is " + retailerObj.TicketNo + " Which is launched at:" + retailerObj.CreatedDate;
            if (retailerObj.ZoneID != 9)
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
                    var result = new ComplaintController().PushNotificationForRegistration(message, list, retailerObj.ID, type, data.ZoneID);
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
                    var result = new ComplaintController().PushNotificationForRegistration(message, list1, retailerObj.ID, type, data.ZoneID);
                }

                // Notification Send to Wasa

                var AreaID = Convert.ToInt32(data.AreaID);

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
                    var result2 = new ComplaintController().PushNotificationForWasa(message, list2, retailerObj.ID, type);
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
                        var result = new ComplaintController().PushNotificationForRegistration(message, list1, retailerObj.ID, type, data.ZoneID);
                    }
                }


                var AreaID = Convert.ToInt32(data.AreaID);

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
                    var result2 = new ComplaintController().PushNotificationForWasa(message, list2, retailerObj.ID, type);
                }
            }
            // Add Token Detail ...
            TokenDetail tokenDetail = new TokenDetail();
            tokenDetail.TokenName = newRetailer.Token;
            tokenDetail.Action = "Add New Retailer";
            tokenDetail.ProcessedDateTime = DateTime.Now;
            db.TokenDetails.Add(tokenDetail);
            db.SaveChanges();
            if (TeamID == 4)
            {
                return RedirectToAction("WASADashboard", "Home");
            }
            else
            {
                return RedirectToAction("Home", "Home");
            }
        }

        #endregion


        [CustomAuthorize]
        public ActionResult UpdateComplaints()
        {
            List<RegionData> regionalHeadData = new List<RegionData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetRegionalList();
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetProjectsData();
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            var objRetailer = new KSBComplaintData();
            objRetailer.Client = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
            objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
            objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
            objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
            objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();

            return View(objRetailer);
        }

        public JsonResult GetSitesList(int ClientID, int ProjectID, int CityID, int AreaID, int SubDivisionID)
        {
            var result = FOS.Setup.ManageCity.GetSiteList(ClientID, ProjectID, CityID, AreaID, SubDivisionID, "--Select Site--");
            return Json(result);
        }


        public JsonResult GetFaultTypeDetailList(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetFaulttypedetaileList(ClientID, "--Select Fault Type Detail--");
            return Json(result);
        }
        public JsonResult GetFaultTypeDetailListForFaulttypeID(int ClientID)
        {
            int? Faulttypeid =db.Tbl_ComplaintHistory.Where(x => x.JobID == ClientID && x.IsPublished==1).OrderByDescending(x => x.ID).Select(x => x.FaultTypeId).FirstOrDefault();
            var result = FOS.Setup.ManageCity.GetFaultTypeDetailListForFaulttypeID((int)Faulttypeid);
            return Json(result);
        }

        public JsonResult GetUpdateProgressStatusListForFaulttypeID(int ClientID)
        {
            int? Faulttypeid = db.Tbl_ComplaintHistory.Where(x => x.JobID == ClientID && x.IsPublished == 1).OrderByDescending(x => x.ID).Select(x => x.FaultTypeId).FirstOrDefault();
            var result = FOS.Setup.ManageCity.GetUpdateProgressStatusListForFaulttypeID((int)Faulttypeid);
            return Json(result);
        }
        public JsonResult GetWorkDoneStatusListForFaulttypeID(int ClientID)
        {
            int? Faulttypeid = db.Tbl_ComplaintHistory.Where(x => x.JobID == ClientID && x.IsPublished == 1).OrderByDescending(x => x.ID).Select(x => x.FaultTypeId).FirstOrDefault();
            var result = FOS.Setup.ManageCity.GetWorkDoneStatusListForFaulttypeID((int)Faulttypeid);
            return Json(result);
        }

        public JsonResult WorkDoneDetailList(int ClientID)
        {
            var result = FOS.Setup.ManageCity.WorkDoneDetailList(ClientID, "--Select Work Done Status--");
            return Json(result);
        }

        public JsonResult GetProgressDetailList(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetProgressDetailListForReport(ClientID, "--Select Progress Status--");
            return Json(result);
        }

        public JsonResult GetProgressDetailListForReport(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetProgressDetailListForReport(ClientID, "--Select Progress Status--");
            return Json(result);
        }

        public JsonResult GetWardListForReport(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetWardListByID(ClientID);
            return Json(result);
        }

        public JsonResult GetUpdateComplaint(int ComplaintID)
        {
            var Response = ManageJobs.GetUpdateComplaint(ComplaintID);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComplaintClientRemarks(int ComplaintId)
        {
            var Response = ManageJobs.GetComplaintClientRemarks(ComplaintId);
            return Json(Response);
        }


        public JsonResult GetSiteId(int ClientID)
        {
            var result = FOS.Setup.ManageCity.GetSiteIDList(ClientID);

            return Json(result);
        }

        public JsonResult GetComplaintChildData(DTParameters param, int ComplaintId)
        {
            var dtsource = new List<ComplaintProgress>();
            dtsource = ManageJobs.GetComplaintChildData(ComplaintId);
            foreach (var itm in dtsource)
            {
                if (itm.FaultTypeDetailName == "Others")
                {
                    itm.FaultTypeDetailName = "Others/" + itm.FaultTypeDetailRemarks;
                }
                if (itm.ComplaintStatus == "Resolved")
                {
                    itm.ProgressStatusName = db.WorkDones.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                else if (itm.ComplaintStatus == null)
                {
                    itm.ComplaintStatus = "New Complaint";
                }
                else
                {
                    itm.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                if (itm.ProgressStatusName == "Others")
                {
                    itm.ProgressStatusName = "Others/" + itm.ProgressStatusOtherRemarks;
                }
                if (itm.ProgressRemarks == null || itm.ProgressRemarks == "")
                {
                    itm.ProgressRemarks = "null";
                }
                if (itm.ProgressStatusName == null)
                {
                    itm.ProgressStatusName = "null";
                }
            }
            return Json(dtsource);

        }
        public JsonResult GetComplaintChildDataForKSB(DTParameters param, int ComplaintId)
        {
            var dtsource = new List<ComplaintProgress>();
            dtsource = ManageJobs.GetComplaintChildDataForKSB(ComplaintId);
            foreach (var itm in dtsource)
            {
                if (itm.FaultTypeDetailName == "Others")
                {
                    itm.FaultTypeDetailName = "Others/" + itm.FaultTypeDetailRemarks;
                }
                if (itm.ComplaintStatus == "Resolved")
                {
                    itm.ProgressStatusName = db.WorkDones.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                else if (itm.ComplaintStatus == null)
                {
                    itm.ComplaintStatus = "New Complaint";
                }
                else
                {
                    itm.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == itm.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
                }
                if (itm.ProgressStatusName == "Others")
                {
                    itm.ProgressStatusName = "Others/" + itm.ProgressStatusOtherRemarks;
                }
                if (itm.ProgressRemarks == null || itm.ProgressRemarks == "")
                {
                    itm.ProgressRemarks = "null";
                }
                if (itm.ProgressStatusName == null)
                {
                    itm.ProgressStatusName = "null";
                }
            }
            return Json(dtsource);

        }




        public JsonResult GetEditComplaint(int ProgressChildID)
        {
            var Response = ManageJobs.GetEditComplaint(ProgressChildID);          
             return Json(Response, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProgressIDData(int ProgressID)
        {
            var Response = ManageJobs.GetProgressIDData(ProgressID);
            if (Response.FaultTypeDetailName == "Others")
            {
                Response.FaultTypeDetailName = "Others/" + Response.FaultTypeDetailRemarks;
            }
            if (Response.ComplaintStatus == "Resolved")
            {
                Response.ProgressStatusName = db.WorkDones.Where(x => x.ID == Response.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
            }
            else if (Response.ComplaintStatus == null)
            {
                Response.ComplaintStatus = "New Complaint";
            }
            else
            {
                Response.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == Response.ProgressStatusID).Select(x => x.Name).FirstOrDefault();
            }
            if (Response.ProgressStatusName == "Others")
            {
                Response.ProgressStatusName = "Others/" + Response.ProgressStatusName;
            }
            if (Response.ProgressStatusName == null)
            {
                Response.ProgressStatusName = "null";
            }
            if (Response.ProgressRemarks == null || Response.ProgressRemarks == "")
            {
                Response.ProgressRemarks = "null";
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChatBoxData(int ComplaintID)
        {
            List<CityData> Response = null;
            using (FOSDataModel dbContext = new FOSDataModel())
            {

                var SiteID = dbContext.Jobs.Where(x => x.ID == ComplaintID).Select(x => x.SiteID).FirstOrDefault();
                var SiteName = dbContext.Retailers.Where(x => x.ID == SiteID).FirstOrDefault();

                Response = dbContext.ChatBoxes.Where(a => a.ComplaintID == ComplaintID).Select(a => new CityData
                {
                    ID = a.ID,
                    Remarks = a.Remarks,
                    ComplaintID = a.ComplaintID,
                    ShopName = a.DateInstall.ToString(),
                    SOID = a.SOID,
                    SaleOfficerName = dbContext.SaleOfficers.Where(x => x.ID == a.SOID).Select(x => x.Name).FirstOrDefault(),
                    SiteName = SiteName.Name/* + "(" + SiteName.RetailerCode + ")"*/,

                }).ToList();
            }
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostSMS(ChatBox SMSData)
        {
            int? Response =SMSData.ComplaintID;
            var job = db.Jobs.Where(x => x.ID == SMSData.ComplaintID).FirstOrDefault();
            ChatBox retailerObj = new ChatBox();
            retailerObj.ComplaintID = SMSData.ComplaintID;
            retailerObj.Remarks = SMSData.Remarks;
            retailerObj.DateInstall = DateTime.UtcNow.AddHours(5);
            retailerObj.SOID = SMSData.SOID;
            db.ChatBoxes.Add(retailerObj);
            db.SaveChanges();
            string type = "SMS";
            string message = "There is a new message in Complaint No  " + job.TicketNo + " Kindly Visit it ";

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
                            var result = new ComplaintController().PushNotificationForEdit(message, items, (int)SMSData.ComplaintID, type);
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
                            var result = new ComplaintController().PushNotificationForEdit(message, items, job.ID, type);
                        }
                    }
                    //if (list1 != null)
                    //{
                    //    var result = new CommonController().PushNotification(message, list1, rm.ComplaintID, type);
                    //}
                }
            }

            return Json(Response);
        }



        public JsonResult DeleteProgressIDData(int ProgressID)
        {

            int? JobDetailID = db.Tbl_ComplaintHistory.Where(x => x.ID == ProgressID).Select(x => x.JobDetailID).FirstOrDefault();
            var NotificationSeen = db.NotificationSeens.Where(x => x.JobDetailID == JobDetailID).ToList();
            foreach (var item in NotificationSeen)
            {
                db.NotificationSeens.Remove(item);
            }
            var ComplaintNotification = db.ComplaintNotifications.Where(x => x.JobDetailID == JobDetailID).FirstOrDefault();
            db.ComplaintNotifications.Remove(ComplaintNotification);
            var JobDetailRecord = db.JobsDetails.Where(x => x.ID == JobDetailID).FirstOrDefault();
            db.JobsDetails.Remove(JobDetailRecord);
            var Tbl_ComplaintHistoryRecord = db.Tbl_ComplaintHistory.Where(x => x.ID == ProgressID).FirstOrDefault();
            db.Tbl_ComplaintHistory.Remove(Tbl_ComplaintHistoryRecord);
            var Result = true;
            db.SaveChanges();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DeleteRemarksData(int ProgressID)
        {

            var JobDetailID = db.ClientRemarks.Where(x => x.ID == ProgressID).FirstOrDefault();
            db.ClientRemarks.Remove(JobDetailID);
            
            db.SaveChanges();
            var Result = true;
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ProgressPublish(int ProgressID)
        {
            var Result = true;
            var jobDetail = new JobsDetail();
            var job = new Job();
            var jobhistory = new Tbl_ComplaintHistory();

            int flag = 0;
            jobhistory = db.Tbl_ComplaintHistory.Where(u => u.ID == ProgressID).FirstOrDefault();
            if (jobhistory.IsPublished != 1)
            {
                jobhistory.IsPublished = 1;
                flag = 1;
            }
            jobDetail = db.JobsDetails.Where(u => u.ID == jobhistory.JobDetailID).FirstOrDefault();
            jobDetail.IsPublished = 1;
            jobDetail.ProgressStatusID = jobhistory.ProgressStatusID;
            jobDetail.AssignedToSaleOfficer = jobhistory.AssignedToSaleOfficer;
            jobDetail.ProgressStatusRemarks = jobhistory.ProgressStatusRemarks;
            jobDetail.ActivityType = jobhistory.FaultTypeDetailRemarks;
            jobDetail.DateComplete = DateTime.UtcNow.AddHours(5);
            job = db.Jobs.Where(u => u.ID == jobhistory.JobID).FirstOrDefault();
            job.FaultTypeId = jobhistory.FaultTypeId;
            job.FaultTypeDetailID = jobhistory.FaultTypeDetailID;
            job.ComplaintStatusId = jobhistory.ComplaintStatusId;
            job.ResolvedAt = DateTime.UtcNow.AddHours(5);
            job.LastUpdated = DateTime.UtcNow.AddHours(5);


            db.SaveChanges();
            if (flag == 1)

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
                        var result = new ComplaintController().PushNotification(message, list, job.ID, type);
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
                        var result = new ComplaintController().PushNotification(message, list1, job.ID, type);
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
                        var result2 = new ComplaintController().PushNotificationForWasa(message, list2, job.ID, type);
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
                            var result = new ComplaintController().PushNotification(message, list1, job.ID, type);
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
                        var result2 = new ComplaintController().PushNotificationForWasa(message, list2, job.ID, type);
                    }
                }



            }
            return Json(Result, JsonRequestBehavior.AllowGet);


        }
        public bool PushNotification(string Message, List<string> deviceIDs, int ID, string type)
        {
            var AppId = ConfigurationManager.AppSettings["OneSignalAppID"];

            var DevIDs = deviceIDs;
            foreach (var item in DevIDs)
            {
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    app_id = AppId,
                    contents = new { en = Message },
                    data = new { ComplaintID = ID, PushType = type },
                    include_player_ids = new string[] { item }
                };



                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;

                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                    return false;
                }

                System.Diagnostics.Debug.WriteLine(responseContent);



            }

            return true;

        }
        public bool PushNotificationForEdit(string Message, string deviceIDs, int ID, string type)
        {
            var AppId = ConfigurationManager.AppSettings["OneSignalAppID"];

            var DevIDs = deviceIDs;

            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = AppId,
                contents = new { en = Message },
                data = new { ComplaintID = ID, PushType = type },
                include_player_ids = new string[] { DevIDs }
            };



            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                return false;
            }

            System.Diagnostics.Debug.WriteLine(responseContent);





            return true;

        }
        public bool PushNotificationForWasa(string Message, List<string> deviceIDs, int ID, string type)
        {
            var AppId = ConfigurationManager.AppSettings["OneSignalAppIDForWasa"];

            var DevIDs = deviceIDs;
            foreach (var item in DevIDs)
            {
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    app_id = AppId,
                    contents = new { en = Message },
                    data = new { ComplaintID = ID, PushType = type },
                    include_player_ids = new string[] { item }
                };



                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;

                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                    return false;
                }

                System.Diagnostics.Debug.WriteLine(responseContent);



            }

            return true;

        }
        public bool PushNotificationForRegistration(string Message, List<string> deviceIDs, int ID, string type, int? ProjectID)
        {
            var AppId = ConfigurationManager.AppSettings["OneSignalAppID"];

            var DevIDs = deviceIDs;
            foreach (var item in DevIDs)
            {
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    app_id = AppId,
                    contents = new { en = Message },
                    data = new { ComplaintID = ID, PushType = type, ProjectID = ProjectID },
                    include_player_ids = new string[] { item }
                };



                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;

                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());

                    return false;
                }

                System.Diagnostics.Debug.WriteLine(responseContent);



            }

            return true;

        }



        public JsonResult GetCurrentComplaintDetail(int ComplaintId)
        {
            var Response = ManageRetailer.GetCurrentComplaintDetail(ComplaintId);
            if (Response.FaultTypesDetailName == "Others")
            {
                Response.FaultTypesDetailName = "Others/" + Response.FaultTypeDetailOtherRemarks;
            }
            Response.ProgressStatusId = db.JobsDetails.Where(x => x.JobID == Response.ID && x.IsPublished==1).OrderByDescending(x => x.ID).Select(x => x.ProgressStatusID).FirstOrDefault();
            Response.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == Response.ProgressStatusId).OrderByDescending(x => x.ID).Select(x => x.Name).FirstOrDefault();
            if (Response.StatusName == "Resolved")
            {
                Response.ProgressStatusName = db.WorkDones.Where(x => x.ID == Response.ProgressStatusId).Select(x => x.Name).FirstOrDefault();

            }
            else
            {
                Response.ProgressStatusName = db.ProgressStatus.Where(x => x.ID == Response.ProgressStatusId).Select(x => x.Name).FirstOrDefault();

            }
            if (Response.ProgressStatusName == "Others")
            {
                Response.ProgressStatusName = "Others/" + db.JobsDetails.Where(x => x.JobID == Response.ID).OrderByDescending(x => x.ID).Select(x => x.ProgressStatusRemarks).FirstOrDefault();
            }
            if (Response.ProgressStatusName == "" || Response.ProgressStatusName == null || Response.ProgressStatusId == null)
            {
                Response.ProgressStatusName = "null";
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetActualComplaintDetail(int ComplaintId)
        {
            var Response = ManageRetailer.GetActualComplaintDetail(ComplaintId);
            if (Response.FaultTypesDetailName == "Others")
            {
                Response.FaultTypesDetailName = "Others/" + Response.FaultTypeDetailOtherRemarks;
            }
            if (Response.Name == null || Response.Name == "")
            {
                Response.Name = "null";
            }
            if (Response.Remarks == null || Response.Remarks == "")
            {
                Response.Remarks = "null";
            }
            if (Response.FaultTypesDetailName == null || Response.FaultTypesDetailName == "")
            {
                Response.FaultTypesDetailName = "null";
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
      



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateComplaints(JobsData newRetailer)
        {
            Boolean boolFlag = true;
            ValidationResult results = new ValidationResult();
            try
            {
                if (newRetailer != null)
                {
                    if (newRetailer.ID == 0)
                    {
                        //ComplaintValidator validator = new ComplaintValidator();
                        //results = validator.Validate(newRetailer);
                        //boolFlag = results.IsValid;
                    }

                    //if (newRetailer.Phone1 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumber1Exist(newRetailer.ID, newRetailer.Phone1 == null ? "" : newRetailer.Phone1) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newRetailer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumber2Exist(newRetailer.ID, newRetailer.Phone2 == null ? "" : newRetailer.Phone2) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (newRetailer.Phone1 != null && newRetailer.Phone2 != null)
                    //{
                    //    if (FOS.Web.UI.Common.NumberCheck.CheckRetailerNumberExist(newRetailer.ID, newRetailer.Phone1 == null ? "" : newRetailer.Phone1, newRetailer.Phone2 == null ? "" : newRetailer.Phone2) == 1)
                    //    {
                    //        return Content("2");
                    //    }
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckCNICExist(newRetailer.CNIC, newRetailer.ID) == 1)
                    //{
                    //    return Content("3");
                    //}
                    //else
                    //{
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckAccountNoExist(newRetailer.AccountNo, newRetailer.ID) == 1)
                    //{
                    //    return Content("4");
                    //}
                    //else
                    //{
                    //}

                    //if (FOS.Web.UI.Common.RetailerChecks.CheckCardNoExist(newRetailer.CardNumber, newRetailer.ID) == 1)
                    //{
                    //    return Content("5");
                    //}
                    //else
                    //{
                    //}

                    if (boolFlag)
                    {
                        //try
                        //{

                        //    newRetailer.CreatedBy = SessionManager.Get<int>("UserID");
                        //    newRetailer.UpdatedBy = SessionManager.Get<int>("UserID");

                        //}
                        //catch { newRetailer.CreatedBy = 1; }

                        //  int Res = ManageRetailer.AddUpdateComplaint(newRetailer);
                        int Res = 1;
                        if (Res == 1)
                        {
                            return Content("1");
                        }
                        else if (Res == 3)
                        {
                            return Content("3");
                        }
                        else if (Res == 4)
                        {
                            return Content("4");
                        }
                        else if (Res == 5)
                        {
                            return Content("5");
                        }
                        else
                        {
                            return Content("0");
                        }
                    }
                    else
                    {
                        IList<ValidationFailure> failures = results.Errors;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("{0}:{1}", "*** Error ***", "<br/>"));
                        foreach (ValidationFailure failer in results.Errors)
                        {
                            sb.AppendLine(String.Format("{0}:{1}{2}", failer.PropertyName, failer.ErrorMessage, "<br/>"));
                            Response.StatusCode = 422;
                            return Json(new { errors = sb.ToString() });
                        }
                    }

                }
                return Content("0");
            }
            catch (Exception exp)
            {
                return Content("Exception : " + exp.Message);
            }
        }



        #region OpenComplaints

        public ActionResult OpenComplaints()
        {
            List<RegionData> regionalHeadData = new List<RegionData>();
            regionalHeadData = FOS.Setup.ManageRegionalHead.GetRegionalList();
            int regId = 0;
            if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
            {
                regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
            }
            else
            {
                regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
            }

            List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetProjectsData();
            var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

            var objRetailer = new KSBComplaintData();
            objRetailer.Client = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
            objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
            objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
            objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
            objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();

            return View(objRetailer);
        }


        #endregion



    }
}