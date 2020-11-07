using FOS.DataLayer;
using FOS.Setup;
using FOS.Shared;
using FOS.Web.UI.Common.CustomAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

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
    }
}