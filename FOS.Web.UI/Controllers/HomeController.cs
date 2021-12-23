using FOS.DataLayer;
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
using FOS.Web.UI.Controllers.API;

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



            int TeamID = (int)Session["TeamID"];
            //    List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetProjects(TeamID);
            //    var objSaleOff = SaleOfficerObj.FirstOrDefault();

            List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);

              
                //objRetailer.Client = regionalHeadData;
                ////Get Projects start
                //var soid = Convert.ToInt32(Session["SORelationID"]);
                var ListOfProjects = db.Cities.Select(x => x.ID).Distinct().ToList();
                objRetailer.Projects = FOS.Setup.ManageSaleOffice.GetProjectsListForDashboard(ListOfProjects);
                //////Get Projects END
                //objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
                //objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
                //objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
                //objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
                //objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
                //objRetailer.FieldOfficers = FOS.Setup.ManageCity.GetFieldOfficersList(TeamID);
                //objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
                //objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
                //objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
                //objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
                //objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
                // objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();


            // ViewBag.rptid = "";
                //ViewBag.retailers = ManageRetailer.GetRetailerForGrid().Count();
                //ViewBag.Towns = db.Areas.Where(x => x.IsActive == true).Count();
                //ViewBag.SubDivisions = db.SubDivisions.Count();

                //var jobs = ManageJobs.GetJobsToExportInExcel();

                //DateTime now = DateTime.Now;
                //var startDate = new DateTime(now.Year, now.Month, 1);
                //var endDate = startDate.AddMonths(1).AddDays(-1);
                //var today = DateTime.Today;
                //var month = new DateTime(today.Year, today.Month, 1);
                //var first = month.AddMonths(-1);
                //var last = month.AddDays(-1);

                //// Last Month Sales
                //var CurrentMonthRetailerSale = (from lm in db.Jobs
                //                                where lm.CreatedDate >= first
                //                                && lm.CreatedDate <= last

                //                                select lm).ToList();

                //// New Customers Today

                //var CurrentMonthDistributorrSale = (from lm in db.Jobs
                //                                    where lm.CreatedDate >= startDate && lm.CreatedDate <= endDate
                //                                    select lm).ToList();

                ////// Current Month Order Delievered
                //var PreviousMonthRetailerDelievered = (from lm in db.JobsDetails
                //                                       where lm.JobDate >= first
                //                                       && lm.JobDate <= last
                //                                       && lm.JobType == "Retailer Order"
                //                                       select lm).ToList();



                //ViewBag.Lastmonthsale = CurrentMonthRetailerSale.Count();
                //ViewBag.ThisMonthSale = CurrentMonthDistributorrSale.Count();
                //ViewBag.ThisMonthSaleDone = PreviousMonthRetailerDelievered.Count();
                ////ViewBag.PreviousMonthSaleDone = PreviousMonthDistributorDelievered.Count();
                ////ViewBag.TodaySaleDone = ThisMonthSampleDelievered.Count();
                //ViewBag.SOPresentToday = Dashboard.SOPresenttoday().Count();
                //ViewBag.SOAbsentToday = Dashboard.SOAbsenttoday().Count();
                //ViewBag.FSPlanndeToday = Dashboard.FSPlannedtoday().Count();
                //ViewBag.FSVisitedToday = Dashboard.FSVisitedtoday().Count();
                //ViewBag.RSPlannedToday = Dashboard.RSPlannedToday().Count();
                //ViewBag.OpenComplaints = Dashboard.OpenComplaints().Count();
                //ViewBag.RSFollowUpToday = Dashboard.RSFollowUpToday().Count();
                ManageRetailer objRetailers = new ManageRetailer();
                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);
              
               
                List<GetTComplaintsStatusWise_Result> SOVisits = objRetailers.SOVisitsToday(dtFromToday, dtToToday,0);

                // faulttypes = objRetailers.FaultTypeGraph(dtFromToday, dtToToday,0);
                //PresentSO = objRetailers.TotalPresentSO();
          
         

               // ViewBag.DataPoints = JsonConvert.SerializeObject(result1);
                //ViewBag.DataPoints1 = JsonConvert.SerializeObject(faulttypes);
                //ViewBag.DataPoints2 = JsonConvert.SerializeObject(PresentSO);
                //ViewBag.DataPoints3 = JsonConvert.SerializeObject(SOVisits);
             
             
          
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
            var result = false;
            if (model.ClientRemarks!=null)
            {
                ClientRemark CR = new ClientRemark();
                CR.ComplaintID = model.ComplaintID;
                CR.RemarksDate = DateTime.UtcNow.AddHours(5);
                CR.IsActive = true;
                CR.Isdeleted = false;
                CR.RemarksBy = (int?)Session["SORelationID"];
                CR.ClientRemarks = model.ClientRemarks;
                CR.RemarksByName = db.SaleOfficers.Where(x => x.ID == CR.RemarksBy).Select(x => x.Name).FirstOrDefault();
                db.ClientRemarks.Add(CR);
                db.SaveChanges();
                 result = true;
            }
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCompaintDateTime(DateTime UpdateDateTime, int UpdateDateTimeComplaintID)
        {
            var result = UpdateDateTimeComplaintID;
            if (UpdateDateTime != null)
            {
                Tbl_ComplaintHistory ComplaintHistorydata = db.Tbl_ComplaintHistory.Where(x => x.JobID == UpdateDateTimeComplaintID).OrderByDescending(x => x.ID).FirstOrDefault();
                ComplaintHistorydata.CreatedDate = UpdateDateTime;

                JobsDetail JobDetailData = db.JobsDetails.Where(x => x.JobID == UpdateDateTimeComplaintID).OrderByDescending(x => x.ID).FirstOrDefault();
                JobDetailData.DateComplete = UpdateDateTime;
                JobDetailData.JobDate = UpdateDateTime;

                Job JobData = db.Jobs.Where(x => x.ID == UpdateDateTimeComplaintID).OrderByDescending(x => x.ID).FirstOrDefault();
                JobData.LastUpdated = UpdateDateTime;
                JobData.ResolvedAt = UpdateDateTime;
                db.SaveChanges();

                result = UpdateDateTimeComplaintID;
            }

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
        [CustomAuthorize]
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
            var result = false;
            if(model.RoleID==2)
            {
                JobObj = db.Jobs.Where(u => u.ID == model.UpdateComplaintID).FirstOrDefault();
                JobObj.PersonName = model.UpdatePerson;
                JobObj.ComplaintStatusId = model.UpdateStatusID;
                JobObj.PriorityId = model.UpdatePriorityId;
                JobObj.FaultTypeId = model.UpdateFaulttypeId;
                JobObj.ResolvedHours = model.UpdateTime;
                JobObj.FaultTypeDetailID = model.UpdateFaulttypeDetailId;
                if (JobObj.ComplaintStatusId == 3)
                {
                    JobObj.ResolvedAt = DateTime.UtcNow.AddHours(5);
                }
                JobObj.ComplainttypeID = model.UpdateComplaintTypeID;
                JobObj.LastUpdated = DateTime.UtcNow.AddHours(5);
                db.SaveChanges();

                JobsDetail jobDetail = new JobsDetail();
                jobDetail.ID = db.JobsDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                jobDetail.JobID = model.UpdateComplaintID;
                jobDetail.PRemarks = model.UpdateProgressRemarks;
                jobDetail.AssignedToSaleOfficer = model.UpdateSalesOficerID;
                jobDetail.RetailerID = JobObj.SiteID;
                jobDetail.IsPublished = 1;
                jobDetail.ProgressStatusID = model.UpdateProgressStatusId;
                jobDetail.ProgressStatusRemarks = model.UpdateProgressStatusOtherRemarks;
                if (model.UpdateStatusID == 3)
                {
                    jobDetail.WorkDoneID = model.UpdateProgressStatusId;
                }
                jobDetail.ActivityType = model.UpdateFaultTypeDetailOtherRemarks;
                jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
                jobDetail.DateComplete = DateTime.UtcNow.AddHours(5);
                jobDetail.SalesOficerID = (int?)Session["SORelationID"];
                jobDetail.ChildFaultTypeDetailID = model.UpdateFaulttypeDetailId;
                jobDetail.ChildFaultTypeID = model.UpdateFaulttypeId;
                jobDetail.ChildStatusID = model.UpdateStatusID;
                jobDetail.ChildAssignedSaleOfficerID = model.UpdateSalesOficerID;
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
                 db.JobsDetails.Add(jobDetail);

                Tbl_ComplaintHistory history = new Tbl_ComplaintHistory();
                history.ID = db.Tbl_ComplaintHistory.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                history.JobID = JobObj.ID;
                history.JobDetailID = jobDetail.ID;
                history.FaultTypeDetailRemarks = model.UpdateFaultTypeDetailOtherRemarks;
                history.ProgressStatusRemarks = model.UpdateProgressStatusOtherRemarks;
                history.FaultTypeId = model.UpdateFaulttypeId;
                history.FaultTypeDetailID = model.UpdateFaulttypeDetailId;
                history.ProgressStatusID = model.UpdateProgressStatusId;
                history.TicketNo = JobObj.TicketNo;
                history.InitialRemarks = JobObj.InitialRemarks;
                history.ComplaintStatusId = model.UpdateStatusID;
                history.Picture1 = jobDetail.Picture1;
                history.Picture2 = jobDetail.Picture2;
                history.SiteID = jobDetail.RetailerID;
                history.LaunchedById = db.Jobs.Where(x => x.ID == model.UpdateComplaintID).Select(x => x.LaunchedById).FirstOrDefault();
                history.UpdateRemarks = model.UpdateProgressRemarks;
                history.PriorityId = model.UpdatePriorityId;
                history.AssignedToSaleOfficer = model.UpdateSalesOficerID;
                history.FirstAssignedSO = model.UpdateSalesOficerID;
                history.IsActive = true;
                history.IsPublished = 1;
                history.CreatedDate = DateTime.UtcNow.AddHours(5);
                history.PersonName = model.UpdatePerson;
                history.ComplainttypeID = model.UpdateComplaintTypeID;
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
                if (model.UpdateSalesOficerID != 0)
                {
                    // Notification To Assigned FS
                    string message = "Complaint Is Assigned and Complaint No is " + JobObj.TicketNo;
                    string messages = "There is an Update in Complaint No" + JobObj.TicketNo + " Kindly View it.";
                    string type = "Progress";
                    List<string> list = new List<string>();

                    if (JobObj.ZoneID != 9)
                    {
                        var SOIDS = db.OneSignalUsers.Where(x => x.UserID == model.UpdateSalesOficerID).Select(x => x.OneSidnalUserID).FirstOrDefault();

                        if (SOIDS != null)
                        {

                            var result2 = new CommonController().PushNotificationForEdit(message, SOIDS, JobObj.ID, type);
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
                                    var result2 = new CommonController().PushNotificationForEdit(messages, items, JobObj.ID, type);
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
                result = true;
            }
            else
            {
                    JobObj = db.Jobs.Where(u => u.ID == model.UpdateComplaintID).FirstOrDefault();
                    JobDet = db.JobsDetails.Where(u => u.JobID == JobObj.ID && u.IsPublished == 1).OrderByDescending(u => u.ID).FirstOrDefault();

                    JobsDetail jobDetail = new JobsDetail();
                    jobDetail.ID = db.JobsDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                    jobDetail.JobID = JobObj.ID;
                    jobDetail.PRemarks = model.UpdateProgressRemarks;
                    jobDetail.AssignedToSaleOfficer = model.UpdateSalesOficerID;
                    jobDetail.RetailerID = JobObj.SiteID;
                    jobDetail.IsPublished = 0;
                if (model.UpdateStatusID == 3)
                {
                    JobObj.ResolvedAt = DateTime.UtcNow.AddHours(5);
                }
                jobDetail.ActivityType = model.UpdateFaultTypeDetailOtherRemarks;
                    jobDetail.ProgressStatusID = model.UpdateProgressStatusId;
                    jobDetail.ProgressStatusRemarks = model.UpdateProgressStatusOtherRemarks;
                    jobDetail.WorkDoneID = model.UpdateProgressStatusId;
                    jobDetail.SalesOficerID = JobObj.SaleOfficerID;
                    jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
                    jobDetail.ChildFaultTypeDetailID = model.UpdateFaulttypeDetailId;
                    jobDetail.ChildFaultTypeID = model.UpdateFaulttypeId;
                    jobDetail.ChildStatusID = model.UpdateStatusID;
                    jobDetail.ChildAssignedSaleOfficerID = model.UpdateSalesOficerID;
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

                db.JobsDetails.Add(jobDetail);

                    if (JobDet.AssignedToSaleOfficer != model.UpdateSalesOficerID)
                    {
                        var data = db.JobsDetails.Where(u => u.JobID == JobObj.ID && u.IsPublished == 1).ToList();
                        foreach (var item in data)
                        {
                            item.AssignedToSaleOfficer = model.UpdateSalesOficerID;
                            item.ChildAssignedSaleOfficerID = model.UpdateSalesOficerID;
                            db.SaveChanges();
                        }


                        var data2 = db.Tbl_ComplaintHistory.Where(x => x.JobID == JobObj.ID).ToList();

                        foreach (var item in data2)
                        {
                            item.AssignedToSaleOfficer = model.UpdateSalesOficerID;
                            // item.FirstAssignedSO = item.AssignedToSaleOfficer;
                            db.SaveChanges();
                        }
                    }


                    db.SaveChanges();
                    Tbl_ComplaintHistory history = new Tbl_ComplaintHistory();
                    history.JobID = JobObj.ID;
                    history.JobDetailID = jobDetail.ID;
                    history.FaultTypeDetailRemarks = model.UpdateFaultTypeDetailOtherRemarks;
                    history.ProgressStatusRemarks = model.UpdateProgressStatusOtherRemarks;
                    history.FaultTypeId = model.UpdateFaulttypeId;
                    history.FaultTypeDetailID = model.UpdateFaulttypeDetailId;
                    history.ComplaintStatusId = model.UpdateStatusID;
                    history.ProgressStatusID = model.UpdateProgressStatusId;
                    history.AssignedToSaleOfficer = model.UpdateSalesOficerID;
                    history.LaunchedById = JobObj.SaleOfficerID;
                    history.Picture1 = jobDetail.Picture1;
                    history.Picture2 = jobDetail.Picture2;
                    history.SiteID = jobDetail.RetailerID;
                    history.TicketNo = JobObj.TicketNo;
                    history.InitialRemarks = JobObj.InitialRemarks;
                    history.PriorityId = model.UpdatePriorityId;
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
                result = true;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PostEditComplaint(KSBComplaintData obj)
        {
            int result = 1;
            var jobDetail = new JobsDetail();
            var job = new Job();
            var comHis = new Tbl_ComplaintHistory();

            jobDetail = db.JobsDetails.Where(u => u.ID == obj.JobDetailID).FirstOrDefault();
            jobDetail.PRemarks = obj.EditRemarks;
            jobDetail.AssignedToSaleOfficer = obj.EditSalesOficerID;
            jobDetail.ProgressStatusID = obj.EditProgressStatusId;
            jobDetail.ProgressStatusRemarks = obj.EditProgressStatusOtherRemarks;
            //jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
            jobDetail.IsPublished = 1;
            jobDetail.ChildFaultTypeDetailID = obj.EditFaulttypeDetailId;
            jobDetail.ChildFaultTypeID = obj.EditFaulttypeId;
            jobDetail.ChildStatusID = obj.EditStatusID;
            jobDetail.ChildAssignedSaleOfficerID = obj.EditSalesOficerID;
            if (Request.Files["ChildEditPicture1"] != null)
            {
                var file = Request.Files["ChildEditPicture1"];
                if (file.FileName != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    var extension = System.IO.Path.GetExtension(filename).ToLower();
                    var path = HostingEnvironment.MapPath(Path.Combine("/Images/ComplaintImages/", filename));
                    file.SaveAs(path);
                    obj.ChildEditPicture1 = "/Images/ComplaintImages/" + filename;

                }
            }
            if (Request.Files["ChildEditPicture2"] != null)
            {
                var file = Request.Files["ChildEditPicture2"];
                if (file.FileName != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    var extension = System.IO.Path.GetExtension(filename).ToLower();
                    var path = HostingEnvironment.MapPath(Path.Combine("/Images/ComplaintImages/", filename));
                    file.SaveAs(path);
                    obj.ChildEditPicture2 = "/Images/ComplaintImages/" + filename;

                }
            }
            if (obj.ChildEditPicture1 == "" || obj.ChildEditPicture1 == null)
            {
                jobDetail.Picture1 = null;
            }
            else
            {
                jobDetail.Picture1 = obj.ChildEditPicture1;
            }
            if (obj.ChildEditPicture2 == "" || obj.ChildEditPicture2 == null)
            {
                jobDetail.Picture2 = null;
            }
            else
            {
                jobDetail.Picture2 = obj.ChildEditPicture2;
            }

            job = db.Jobs.Where(x => x.ID == jobDetail.JobID).FirstOrDefault();
            job.FaultTypeId = obj.EditFaulttypeId;
            job.FaultTypeDetailID = obj.EditFaulttypeDetailId;
            job.PriorityId = obj.EditPriorityId;
            job.ResolvedHours = obj.EditTime;
            job.PersonName = obj.EditName;
            job.ComplaintStatusId = obj.EditStatusID;

            comHis = db.Tbl_ComplaintHistory.Where(x => x.ID ==obj.ProgressID).FirstOrDefault();
            comHis.SiteID = job.SiteID;
            comHis.FaultTypeId = obj.EditFaulttypeId;
            comHis.PriorityId = obj.EditPriorityId;
            comHis.ComplaintStatusId = obj.EditStatusID;
            comHis.FaultTypeDetailID = obj.EditFaulttypeDetailId;
            comHis.Picture1 = obj.ChildEditPicture1;
            comHis.Picture2 = obj.ChildEditPicture2;
            comHis.ProgressStatusID = obj.EditProgressStatusId;
            comHis.IsPublished = 1;
            comHis.FaultTypeDetailRemarks = obj.EditFaultTypeDetailOtherRemarks;
            comHis.ProgressStatusRemarks = obj.EditProgressStatusOtherRemarks;
            comHis.AssignedToSaleOfficer = obj.EditSalesOficerID;
            comHis.UpdateRemarks = obj.EditRemarks;
            db.SaveChanges();

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


        //[CustomAuthorize]
        //public ActionResult WasaDashboard()
        //{
        //    var objRetailer = new KSBComplaintData();
        //    List<GetDataRelatedToFaultType_Result> faulttypes = null;
        //    List<GetTotalByDate_Result> PresentSO = null;

        //    List<RegionData> regionalHeadData = new List<RegionData>();
        //    regionalHeadData = FOS.Setup.ManageRegionalHead.GetRegionalList();
        //    int regId = 0;
        //    if (FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser() == 0)
        //    {
        //        regId = regionalHeadData.Select(r => r.ID).FirstOrDefault();
        //    }
        //    else
        //    {
        //        regId = FOS.Web.UI.Controllers.AdminPanelController.GetRegionalHeadIDRelatedToUser();
        //    }

        //    //List<SaleOfficerData> SaleOfficerObj = ManageSaleOffice.GetProjectsData();
        //    //var objSaleOff = SaleOfficerObj.FirstOrDefault();

        //    List<DealerData> DealerObj = ManageDealer.GetAllDealersListRelatedToRegionalHead(regId);


        //    objRetailer.Client = regionalHeadData;
        //    //objRetailer.SaleOfficers = SaleOfficerObj;
        //    //Get Projects start
        //    var soid = Convert.ToInt32(Session["SORelationID"]);
        //    var ListOfProjects = db.SOProjects.Where(x => x.SaleOfficerID == soid).Select(x => x.ProjectID).Distinct().ToList();
        //    objRetailer.Projects = FOS.Setup.ManageSaleOffice.GetProjectsListForDashboard(ListOfProjects);
        //    //Get Projects END
        //    objRetailer.Cities = FOS.Setup.ManageCity.GetCityList();
        //    objRetailer.priorityDatas = FOS.Setup.ManageCity.GetPrioritiesList();
        //    objRetailer.faultTypes = FOS.Setup.ManageCity.GetFaultTypesList();
        //    objRetailer.faultTypesDetail = FOS.Setup.ManageCity.GetFaultTypesDetailList();
        //    objRetailer.complaintStatuses = FOS.Setup.ManageCity.GetComplaintStatusList();
        //    objRetailer.FieldOfficers = FOS.Setup.ManageCity.GetFieldOfficersList();
        //    objRetailer.ProgressStatus = FOS.Setup.ManageCity.GetProgressStatusList();
        //    objRetailer.LaunchedBy = FOS.Setup.ManageCity.GetLaunchedByList();
        //    objRetailer.Areas = FOS.Setup.ManageArea.GetAreaList();
        //    objRetailer.SubDivisions = ManageRetailer.GetSubDivisionsList();
        //    objRetailer.Sites = FOS.Setup.ManageArea.GetSitesList();
        //    objRetailer.ComplaintTypes = FOS.Setup.ManageCity.GetComplaintTypeList();


        //    //var userID = Convert.ToInt32(Session["UserID"]);

        //    //if (userID == 1025)
        //    //{
        //    //    objRetailer.Projects = FOS.Setup.ManageCity.GetProjectsList();
        //    //}
        //    //else if(userID == 1026|| userID == 1027)
        //    //{
        //    //    var soid = db.Users.Where(x => x.ID == userID).Select(x => x.SOIDRelation).FirstOrDefault();

        //    //    var list = db.SOProjects.Where(x => x.SaleOfficerID == soid).Select(x => x.ProjectID).Distinct().ToList();

        //    //    var Projectlist= FOS.Setup.ManageCity.GetProjectsListForUsers(list);
        //    //    objRetailer.Projects = Projectlist;
        //    //}
        //    //else
        //    //{
        //    //    var soid = db.Users.Where(x => x.ID == userID).Select(x => x.SOIDRelation).FirstOrDefault();

        //    //    var list = db.SOProjects.Where(x => x.SaleOfficerID == soid).Select(x => x.ProjectID).Distinct().ToList();

        //    //    var Projectlist = FOS.Setup.ManageCity.GetProjectsListForSDOS(list);
        //    //    objRetailer.Projects = Projectlist;
        //    //}


        //    // ViewBag.rptid = "";
        //    ViewBag.retailers = ManageRetailer.GetRetailerForGrid().Count();
        //    ViewBag.Towns = db.Areas.Where(x => x.IsActive == true).Count();
        //    ViewBag.SubDivisions = db.SubDivisions.Count();

        //    var jobs = ManageJobs.GetJobsToExportInExcel();

        //    DateTime now = DateTime.Now;
        //    var startDate = new DateTime(now.Year, now.Month, 1);
        //    var endDate = startDate.AddMonths(1).AddDays(-1);
        //    var today = DateTime.Today;
        //    var month = new DateTime(today.Year, today.Month, 1);
        //    var first = month.AddMonths(-1);
        //    var last = month.AddDays(-1);

        //    // Last Month Sales
        //    var CurrentMonthRetailerSale = (from lm in db.Jobs
        //                                    where lm.CreatedDate >= first
        //                                    && lm.CreatedDate <= last

        //                                    select lm).ToList();

        //    // New Customers Today

        //    var CurrentMonthDistributorrSale = (from lm in db.Jobs
        //                                        where lm.CreatedDate >= startDate && lm.CreatedDate <= endDate
        //                                        select lm).ToList();

        //    //// Current Month Order Delievered
        //    var PreviousMonthRetailerDelievered = (from lm in db.JobsDetails
        //                                           where lm.JobDate >= first
        //                                           && lm.JobDate <= last
        //                                           && lm.JobType == "Retailer Order"
        //                                           select lm).ToList();



        //    ViewBag.Lastmonthsale = CurrentMonthRetailerSale.Count();
        //    ViewBag.ThisMonthSale = CurrentMonthDistributorrSale.Count();
        //    ViewBag.ThisMonthSaleDone = PreviousMonthRetailerDelievered.Count();
        //    //ViewBag.PreviousMonthSaleDone = PreviousMonthDistributorDelievered.Count();
        //    //ViewBag.TodaySaleDone = ThisMonthSampleDelievered.Count();
        //    ViewBag.SOPresentToday = Dashboard.SOPresenttoday().Count();
        //    ViewBag.SOAbsentToday = Dashboard.SOAbsenttoday().Count();
        //    ViewBag.FSPlanndeToday = Dashboard.FSPlannedtoday().Count();
        //    ViewBag.FSVisitedToday = Dashboard.FSVisitedtoday().Count();
        //    ViewBag.RSPlannedToday = Dashboard.RSPlannedToday().Count();
        //    ViewBag.OpenComplaints = Dashboard.OpenComplaints().Count();
        //    ViewBag.RSFollowUpToday = Dashboard.RSFollowUpToday().Count();
        //    ManageRetailer objRetailers = new ManageRetailer();
        //    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
        //    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

        //    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        //    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
        //    DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

        //    DateTime dtFromToday = dtFromTodayUtc.Date;
        //    DateTime dtToToday = dtFromToday.AddDays(1);


        //    List<GetTComplaintsStatusWise_Result> SOVisits = objRetailers.SOVisitsToday(dtFromToday, dtToToday, 0);

        //    faulttypes = objRetailers.FaultTypeGraph(dtFromToday, dtToToday, 0);
        //    PresentSO = objRetailers.TotalPresentSO();



        //    // ViewBag.DataPoints = JsonConvert.SerializeObject(result1);
        //    ViewBag.DataPoints1 = JsonConvert.SerializeObject(faulttypes);
        //    ViewBag.DataPoints2 = JsonConvert.SerializeObject(PresentSO);
        //    ViewBag.DataPoints3 = JsonConvert.SerializeObject(SOVisits);



        //    return View(objRetailer);
        //}


    }
}