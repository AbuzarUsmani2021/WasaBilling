﻿using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using FOS.Web.UI.Common;

namespace FOS.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private FOSDataModel db = new FOSDataModel();
        private static int _regionalHeadID = 0;

        private static int RegionalheadID
        {
            get
            {
                if (_regionalHeadID == 0)
                {
                    _regionalHeadID = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
                }

                return _regionalHeadID;
            }
        }

        [CustomAuthorize]
        public ActionResult Home()
        {
            var objRetailer = new KSBComplaintData();
            List<GetDataRelatedToFaultType_Result> faulttypes=null;
            List<GetTotalByDate_Result> PresentSO = null;

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

              
                objRetailer.Client = regionalHeadData;
                objRetailer.SaleOfficers = SaleOfficerObj;
                objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
                objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
                objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
                objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
                objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.FieldOfficers = FOS.Setup.ManageCity.GetFieldOfficersList();
            objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
                objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
                objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
                objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
                objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
                 objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();


            // ViewBag.rptid = "";
            ViewBag.retailers = ManageRetailer.GetRetailerForGrid().Count();
                ViewBag.Towns = db.Areas.Where(x => x.IsActive == true).Count();
                ViewBag.SubDivisions = db.SubDivisions.Count();

                var jobs = ManageJobs.GetJobsToExportInExcel();

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var today = DateTime.Today;
                var month = new DateTime(today.Year, today.Month, 1);
                var first = month.AddMonths(-1);
                var last = month.AddDays(-1);

                // Last Month Sales
                var CurrentMonthRetailerSale = (from lm in db.Jobs
                                                where lm.CreatedDate >= first
                                                && lm.CreatedDate <= last

                                                select lm).ToList();

                // New Customers Today

                var CurrentMonthDistributorrSale = (from lm in db.Jobs
                                                    where lm.CreatedDate >= startDate && lm.CreatedDate <= endDate
                                                    select lm).ToList();

                //// Current Month Order Delievered
                var PreviousMonthRetailerDelievered = (from lm in db.JobsDetails
                                                       where lm.JobDate >= first
                                                       && lm.JobDate <= last
                                                       && lm.JobType == "Retailer Order"
                                                       select lm).ToList();



                ViewBag.Lastmonthsale = CurrentMonthRetailerSale.Count();
                ViewBag.ThisMonthSale = CurrentMonthDistributorrSale.Count();
                ViewBag.ThisMonthSaleDone = PreviousMonthRetailerDelievered.Count();
                //ViewBag.PreviousMonthSaleDone = PreviousMonthDistributorDelievered.Count();
                //ViewBag.TodaySaleDone = ThisMonthSampleDelievered.Count();
                ViewBag.SOPresentToday = Dashboard.SOPresenttoday().Count();
                ViewBag.SOAbsentToday = Dashboard.SOAbsenttoday().Count();
                ViewBag.FSPlanndeToday = Dashboard.FSPlannedtoday().Count();
                ViewBag.FSVisitedToday = Dashboard.FSVisitedtoday().Count();
                ViewBag.RSPlannedToday = Dashboard.RSPlannedToday().Count();
                ViewBag.OpenComplaints = Dashboard.OpenComplaints().Count();
                ViewBag.RSFollowUpToday = Dashboard.RSFollowUpToday().Count();
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);
              
               
                List<GetTComplaintsStatusWise_Result> SOVisits = objRetailers.SOVisitsToday(dtFromToday, dtToToday,0);

                 faulttypes = objRetailers.FaultTypeGraph(dtFromToday, dtToToday,0);
                PresentSO = objRetailers.TotalPresentSO();
          
         

               // ViewBag.DataPoints = JsonConvert.SerializeObject(result1);
                ViewBag.DataPoints1 = JsonConvert.SerializeObject(faulttypes);
                ViewBag.DataPoints2 = JsonConvert.SerializeObject(PresentSO);
                ViewBag.DataPoints3 = JsonConvert.SerializeObject(SOVisits);
             
             
          
            return View(objRetailer);
        }

        [CustomAuthorize]
        public ActionResult UserHome()
        {
            return View();
        }

        public JsonResult RetailerGraph()
        {
            RetailerGraphData result;
            if (RegionalheadID == 0)
            {
                result = FOS.Setup.Dashboard.RetailerGraph();
            }
            else
            {
                result = FOS.Setup.Dashboard.RetailerGraph(RegionalheadID);
            }
            return Json(result);
        }

        public JsonResult SaveClientRemarks(ClientRemark model)
        {
            ClientRemark CR = new ClientRemark();
            CR.ComplaintID = model.ComplaintID;
            CR.RemarksDate = DateTime.Now;
            CR.IsActive = true;
            CR.Isdeleted = false;
            CR.ClientRemarks = model.ClientRemarks;
            CR.RemarksByName = "HammadWASATestUser(Mgt)";
            db.ClientRemarks.Add(CR);
            db.SaveChanges();
            var result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JobsGraph()
        {
            JobGraphData result;
            if (RegionalheadID == 0)
            {
                result = FOS.Setup.Dashboard.JobsGraph();
            }
            else
            {
                result = FOS.Setup.Dashboard.JobsGraph(RegionalheadID);
            }
            return Json(result);
        }

        public JsonResult CityGraph()
        {
            List<CityGraphData> result = FOS.Setup.Dashboard.CityGraph();
            return Json(result);
        }

        public JsonResult AreaGraph()
        {
            List<AreaGraphData> result = FOS.Setup.Dashboard.AreaGraph();
            return Json(result);
        }

        public JsonResult RegionalHeadGraph()
        {
            List<RegionalHeadGraphData> result = FOS.Setup.Dashboard.RegionalHeadGraph();
            return Json(result);
        }

        public int SalesOfficerGraph()
        {
            if (RegionalheadID == 0)
            {
                return FOS.Setup.Dashboard.SalesOfficerGraph();
            }
            else
            {
                return FOS.Setup.Dashboard.SalesOfficerGraph(RegionalheadID);
            }
        }

        public int DealerGraph()
        {
            return FOS.Setup.Dashboard.DealerGraph();
        }

        //public int GetCount()
        //{
        //    int count;
        //    var objRetailer = new RetailerData();
        //    if (RegionalheadID == 0)
        //    {
        //        count = FOS.Setup.ManageRetailer.GetPendingRetailerCountApproval();
        //    }
        //    else
        //    {
        //        count = FOS.Setup.ManageRetailer.GetPendingRetailerCountApproval(RegionalheadID);
        //    }
        //    return count;
        //}

        public int GetTotalRetailer()
        {
            int count;
            var objRetailer = new RetailerData();

            if (RegionalheadID == 0)
            {
                count = db.Retailers.Count();
            }
            else
            {
                count = db.Retailers.Where(r => r.SaleOfficer.RegionalHeadID == RegionalheadID).Count();
            }
            return count;
        }

        public int GetTotalJobs()
        {
            var objJobs = new JobsDetailData();
            int count;

            if (RegionalheadID == 0)
            {
                count = db.JobsDetails.Where(jd => jd.Job.IsDeleted == false).Count();
            }
            else
            {
                count = db.JobsDetails.Where(j => j.RegionalHeadID == RegionalheadID && j.Job.IsDeleted == false).Count();
            }
            return count;
        }

        public int GetTotalSalesOfficer()
        {
            int count;
            var objSalesOfficer = new SaleOfficerData();

            if (RegionalheadID == 0)
            {
                count = db.SaleOfficers.Count();
            }
            else
            {
                count = db.SaleOfficers.Where(s => s.RegionalHeadID == RegionalheadID).Count();
            }

            return count;
        }

        public JsonResult SoJobGraph()
        {
            List<SojobGraphData> result = FOS.Setup.Dashboard.SoJobGraph();
            return Json(result);
        }

        //public int GetCount()
        //{
        //    int count;
        //    var objRetailer = new RetailerData();

        //    count = FOS.Setup.ManageRetailer.GetDeletedRetailerCountApproval();

        //    return count;
        //}

        public ActionResult GraphicalDashboard()
        {
            var objRetailer = new KSBComplaintData();
            List<GetDataRelatedToFaultType_Result> faulttypes = null;
            List<GetTotalByDate_Result> PresentSO = null;

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



            // ViewBag.rptid = "";
            ViewBag.retailers = ManageRetailer.GetRetailerForGrid().Count();
            ViewBag.Towns = db.Areas.Where(x => x.IsActive == true).Count();
            ViewBag.SubDivisions = db.SubDivisions.Count();

            var jobs = ManageJobs.GetJobsToExportInExcel();

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);

            // Last Month Sales
            var CurrentMonthRetailerSale = (from lm in db.Jobs
                                            where lm.CreatedDate >= first
                                            && lm.CreatedDate <= last

                                            select lm).ToList();

            // New Customers Today

            var CurrentMonthDistributorrSale = (from lm in db.Jobs
                                                where lm.CreatedDate >= startDate && lm.CreatedDate <= endDate
                                                select lm).ToList();

            //// Current Month Order Delievered
            var PreviousMonthRetailerDelievered = (from lm in db.JobsDetails
                                                   where lm.JobDate >= first
                                                   && lm.JobDate <= last
                                                   && lm.JobType == "Retailer Order"
                                                   select lm).ToList();



            ViewBag.Lastmonthsale = CurrentMonthRetailerSale.Count();
            ViewBag.ThisMonthSale = CurrentMonthDistributorrSale.Count();
            ViewBag.ThisMonthSaleDone = PreviousMonthRetailerDelievered.Count();
            //ViewBag.PreviousMonthSaleDone = PreviousMonthDistributorDelievered.Count();
            //ViewBag.TodaySaleDone = ThisMonthSampleDelievered.Count();
            ViewBag.SOPresentToday = Dashboard.SOPresenttoday().Count();
            ViewBag.SOAbsentToday = Dashboard.SOAbsenttoday().Count();
            ViewBag.FSPlanndeToday = Dashboard.FSPlannedtoday().Count();
            ViewBag.FSVisitedToday = Dashboard.FSVisitedtoday().Count();
            ViewBag.RSPlannedToday = Dashboard.RSPlannedToday().Count();
            ViewBag.OpenComplaints = Dashboard.OpenComplaints().Count();
            ViewBag.RSFollowUpToday = Dashboard.RSFollowUpToday().Count();
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);


            List<GetTComplaintsStatusWise_Result> SOVisits = objRetailers.SOVisitsToday(dtFromToday, dtToToday, 0);

            faulttypes = objRetailers.FaultTypeGraph(dtFromToday, dtToToday, 0);
            PresentSO = objRetailers.TotalPresentSO();



            // ViewBag.DataPoints = JsonConvert.SerializeObject(result1);
            ViewBag.DataPoints1 = JsonConvert.SerializeObject(faulttypes);
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(PresentSO);
            ViewBag.DataPoints3 = JsonConvert.SerializeObject(SOVisits);



            return View(objRetailer);
        }


        public JsonResult PostUpdateComplaint(KSBComplaintData model)
        {
            var JobObj = new Job();
            var JobDet = new JobsDetail();


            JobObj = db.Jobs.Where(u => u.ID == model.UpdateComplaintID).FirstOrDefault();
            JobObj.LastUpdated = DateTime.UtcNow.AddHours(5); ;
            JobObj.FaultTypeId = model.UpdateFaulttypeId;
            JobObj.PriorityId = model.UpdatePriorityId;
            JobObj.ComplaintStatusId = model.UpdateStatusID;
            JobObj.PersonName = model.UpdatePerson;
            JobObj.FaultTypeDetailID = model.UpdateFaulttypeDetailId;
            JobObj.ComplainttypeID = model.UpdateComplaintTypeID;
            if (JobObj.ComplaintStatusId == 3)
            {
                JobObj.ResolvedAt = DateTime.UtcNow.AddHours(5);
            }
            JobObj.ResolvedHours = model.UpdateTime;
            db.SaveChanges();

            JobsDetail jobDetail = new JobsDetail();
            jobDetail.ID = db.JobsDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
            jobDetail.JobID = model.UpdateComplaintID;
            jobDetail.SalesOficerID = 49;
            jobDetail.RetailerID = JobObj.SiteID;
            jobDetail.PRemarks = model.UpdateProgressRemarks;
            jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
            jobDetail.DateComplete = DateTime.UtcNow.AddHours(5);
            if (Request.Files["UpdatePicture1"] != null)
            {
                var file = Request.Files["UpdatePicture1"];
                if (file.FileName != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    var extension = System.IO.Path.GetExtension(filename).ToLower();
                    var path = HostingEnvironment.MapPath(Path.Combine("/Images/ComplaintImages/", filename));
                    file.SaveAs(path);
                    model.UpdatePicture1 = "/Images/ComplaintImages/" + filename;

                }
            }
            if (Request.Files["UpdatePicture2"] != null)
            {
                var file = Request.Files["UpdatePicture2"];
                if (file.FileName != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    var extension = System.IO.Path.GetExtension(filename).ToLower();
                    var path = HostingEnvironment.MapPath(Path.Combine("/Images/ComplaintImages/", filename));
                    file.SaveAs(path);
                    model.UpdatePicture2 = "/Images/ComplaintImages/" + filename;

                }
            }
            if (model.UpdatePicture1 == "" || model.UpdatePicture1 == null)
            {
                jobDetail.Picture1 = null;
            }
            else
            {
                jobDetail.Picture1 = model.UpdatePicture1;
            }
            if (model.UpdatePicture2 == "" || model.UpdatePicture2 == null)
            {
                jobDetail.Picture2 = null;
            }
            else
            {
                jobDetail.Picture2 = model.UpdatePicture2;
            }
            jobDetail.ProgressStatusID = model.UpdateProgressStatusId;
            //jobDetail.FaultTypeDetailRemarks = model.UpdateFaultTypeDetailOtherRemarks; discussed by abuzar bhai
            jobDetail.ProgressStatusRemarks = model.UpdateProgressStatusOtherRemarks;
            jobDetail.AssignedToSaleOfficer = model.UpdateSalesOficerID;
            if (model.UpdateStatusID == 3)
            {
                jobDetail.WorkDoneID = model.UpdateProgressStatusId;
            }
            jobDetail.IsPublished = 1;
            jobDetail.ChildFaultTypeID = model.UpdateFaulttypeId;
            jobDetail.ChildFaultTypeDetailID = model.UpdateFaulttypeDetailId;
            jobDetail.ChildStatusID = model.UpdateStatusID;
            jobDetail.ChildAssignedSaleOfficerID = model.UpdateSalesOficerID;
            db.JobsDetails.Add(jobDetail);

            Tbl_ComplaintHistory history = new Tbl_ComplaintHistory();
            history.ID = db.Tbl_ComplaintHistory.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
            history.JobID = JobObj.ID;
            history.JobDetailID = jobDetail.ID;
            history.CreatedDate = DateTime.UtcNow.AddHours(5);
            history.IsActive = true;
            history.SiteID = jobDetail.RetailerID;
            history.TicketNo = JobObj.TicketNo;
            history.FaultTypeId = model.UpdateFaulttypeId;
            history.PriorityId = model.UpdatePriorityId;
            history.ComplaintStatusId = model.UpdateStatusID;
            history.LaunchedById = 49;
            history.PersonName = model.UpdatePerson;
            history.FaultTypeDetailID = model.UpdateFaulttypeDetailId;
            history.ComplainttypeID = model.UpdateComplaintTypeID;
            history.Picture1 = jobDetail.Picture1;
            history.Picture2 = jobDetail.Picture2;
            history.ProgressStatusID = model.UpdateProgressStatusId;
            history.FaultTypeDetailRemarks = model.UpdateFaultTypeDetailOtherRemarks;
            history.ProgressStatusRemarks = model.UpdateProgressStatusOtherRemarks;
            history.AssignedToSaleOfficer = model.UpdateSalesOficerID;
            history.IsPublished = 1;
            history.UpdateRemarks = model.UpdateProgressRemarks;
            history.InitialRemarks = JobObj.InitialRemarks;
            history.FirstAssignedSO = model.UpdateSalesOficerID;
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


            var result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilteredDashboard(DateTime? sdate , DateTime? edate, int? TID)
        {
            var objRetailer = new KSBComplaintData();
            List<GetDataRelatedToFaultType_Result> faulttypes = null;
            List<GetTotalByDate_Result> PresentSO = null;

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



            // ViewBag.rptid = "";
            ViewBag.retailers = ManageRetailer.GetRetailerForGrid().Count();
            ViewBag.Towns = db.Areas.Where(x => x.IsActive == true).Count();
            ViewBag.SubDivisions = db.SubDivisions.Count();

            var jobs = ManageJobs.GetJobsToExportInExcel();

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);

            // Last Month Sales
            var CurrentMonthRetailerSale = (from lm in db.Jobs
                                            where lm.CreatedDate >= first
                                            && lm.CreatedDate <= last

                                            select lm).ToList();

            // New Customers Today

            var CurrentMonthDistributorrSale = (from lm in db.Jobs
                                                where lm.CreatedDate >= startDate && lm.CreatedDate <= endDate
                                                select lm).ToList();

            //// Current Month Order Delievered
            var PreviousMonthRetailerDelievered = (from lm in db.JobsDetails
                                                   where lm.JobDate >= first
                                                   && lm.JobDate <= last
                                                   && lm.JobType == "Retailer Order"
                                                   select lm).ToList();



            ViewBag.Lastmonthsale = CurrentMonthRetailerSale.Count();
            ViewBag.ThisMonthSale = CurrentMonthDistributorrSale.Count();
            ViewBag.ThisMonthSaleDone = PreviousMonthRetailerDelievered.Count();
            //ViewBag.PreviousMonthSaleDone = PreviousMonthDistributorDelievered.Count();
            //ViewBag.TodaySaleDone = ThisMonthSampleDelievered.Count();
            ViewBag.SOPresentToday = Dashboard.SOPresenttoday().Count();
            ViewBag.SOAbsentToday = Dashboard.SOAbsenttoday().Count();
            ViewBag.FSPlanndeToday = Dashboard.FSPlannedtoday().Count();
            ViewBag.FSVisitedToday = Dashboard.FSVisitedtoday().Count();
            ViewBag.RSPlannedToday = Dashboard.RSPlannedToday().Count();
            ViewBag.OpenComplaints = Dashboard.OpenComplaints().Count();
            ViewBag.RSFollowUpToday = Dashboard.RSFollowUpToday().Count();
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);



           

         
                DateTime start = (DateTime)sdate;
                DateTime end = (DateTime)edate;
                int ProjectID = (int)TID;
                faulttypes = objRetailers.FaultTypeGraph(start, end, ProjectID);
            PresentSO = objRetailers.TotalPresentSO();
            List<GetTComplaintsStatusWise_Result> SOVisits = objRetailers.SOVisitsToday(start, end, ProjectID);
            // ViewBag.DataPoints = JsonConvert.SerializeObject(result1);
            ViewBag.DataPoints6 = JsonConvert.SerializeObject(faulttypes);
            ViewBag.DataPoints5 = JsonConvert.SerializeObject(PresentSO);
            ViewBag.DataPoints4 = JsonConvert.SerializeObject(SOVisits);



            return View(objRetailer);
        }


        [CustomAuthorize]
        public ActionResult WasaDashboard()
        {
            var objRetailer = new KSBComplaintData();
            List<GetDataRelatedToFaultType_Result> faulttypes = null;
            List<GetTotalByDate_Result> PresentSO = null;

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


            objRetailer.Client = regionalHeadData;
            objRetailer.SaleOfficers = SaleOfficerObj;
            objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
            objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
            objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
            objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
            objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
            objRetailer.FieldOfficers = FOS.Setup.ManageCity.GetFieldOfficersList();
            objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
            objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
            objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
            objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
            objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
            objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();


            var userID = Convert.ToInt32(Session["UserID"]);

            if (userID == 1025)
            {
                objRetailer.Projects = FOS.Setup.ManageCity.GetProjectsList();
            }
            else
            {
                var soid = db.Users.Where(x => x.ID == userID).Select(x => x.SOIDRelation).FirstOrDefault();

                var list = db.SOProjects.Where(x => x.SaleOfficerID == soid).Select(x => x.ProjectID).Distinct().ToList();

                var Projectlist= FOS.Setup.ManageCity.GetProjectsListForUsers(list);
                objRetailer.Projects = Projectlist;
            }


            // ViewBag.rptid = "";
            ViewBag.retailers = ManageRetailer.GetRetailerForGrid().Count();
            ViewBag.Towns = db.Areas.Where(x => x.IsActive == true).Count();
            ViewBag.SubDivisions = db.SubDivisions.Count();

            var jobs = ManageJobs.GetJobsToExportInExcel();

            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);

            // Last Month Sales
            var CurrentMonthRetailerSale = (from lm in db.Jobs
                                            where lm.CreatedDate >= first
                                            && lm.CreatedDate <= last

                                            select lm).ToList();

            // New Customers Today

            var CurrentMonthDistributorrSale = (from lm in db.Jobs
                                                where lm.CreatedDate >= startDate && lm.CreatedDate <= endDate
                                                select lm).ToList();

            //// Current Month Order Delievered
            var PreviousMonthRetailerDelievered = (from lm in db.JobsDetails
                                                   where lm.JobDate >= first
                                                   && lm.JobDate <= last
                                                   && lm.JobType == "Retailer Order"
                                                   select lm).ToList();



            ViewBag.Lastmonthsale = CurrentMonthRetailerSale.Count();
            ViewBag.ThisMonthSale = CurrentMonthDistributorrSale.Count();
            ViewBag.ThisMonthSaleDone = PreviousMonthRetailerDelievered.Count();
            //ViewBag.PreviousMonthSaleDone = PreviousMonthDistributorDelievered.Count();
            //ViewBag.TodaySaleDone = ThisMonthSampleDelievered.Count();
            ViewBag.SOPresentToday = Dashboard.SOPresenttoday().Count();
            ViewBag.SOAbsentToday = Dashboard.SOAbsenttoday().Count();
            ViewBag.FSPlanndeToday = Dashboard.FSPlannedtoday().Count();
            ViewBag.FSVisitedToday = Dashboard.FSVisitedtoday().Count();
            ViewBag.RSPlannedToday = Dashboard.RSPlannedToday().Count();
            ViewBag.OpenComplaints = Dashboard.OpenComplaints().Count();
            ViewBag.RSFollowUpToday = Dashboard.RSFollowUpToday().Count();
            ManageRetailer objRetailers = new ManageRetailer();
            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);


            List<GetTComplaintsStatusWise_Result> SOVisits = objRetailers.SOVisitsToday(dtFromToday, dtToToday, 0);

            faulttypes = objRetailers.FaultTypeGraph(dtFromToday, dtToToday, 0);
            PresentSO = objRetailers.TotalPresentSO();



            // ViewBag.DataPoints = JsonConvert.SerializeObject(result1);
            ViewBag.DataPoints1 = JsonConvert.SerializeObject(faulttypes);
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(PresentSO);
            ViewBag.DataPoints3 = JsonConvert.SerializeObject(SOVisits);



            return View(objRetailer);
        }


    }
}