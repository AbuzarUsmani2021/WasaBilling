using FOS.DataLayer;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.UI.WebControls;

namespace FOS.Setup
{
    public class Dashboard
    {

        public static RetailerGraphData RetailerGraph()
        {
            RetailerGraphData values = new RetailerGraphData();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                values.ApproveRetailers = dbContext.Retailers.Where(u => u.Status == true).ToList().Count();
                values.PendingRetailers = dbContext.Retailers.Where(u => u.Status == false).ToList().Count();
            }

            return values;
        }

        public static RetailerGraphData RetailerGraph(int RegionalHeadID)
        {
            RetailerGraphData values = new RetailerGraphData();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                values.ApproveRetailers = dbContext.Retailers.Where(u => u.Status == true && u.SaleOfficer.RegionalHeadID == RegionalHeadID).ToList().Count();
                values.PendingRetailers = dbContext.Retailers.Where(u => u.Status == false && u.SaleOfficer.RegionalHeadID == RegionalHeadID).ToList().Count();
            }

            return values;
        }

        //jobs Come From Jobs Table...
        //public static JobGraphData JobsGraph()
        //{
        //    JobGraphData values = new JobGraphData();

        //    using (FOSDataModel dbContext = new FOSDataModel())
        //    {
        //        values.Donejobs = dbContext.Jobs.Where(u => u.Status == true).ToList().Count();
        //        values.Pendingjobs = dbContext.Jobs.Where(u => u.Status == false).ToList().Count();
        //    }

        //    return values;
        //}

        public static JobGraphData JobsGraph()
        {
            JobGraphData values = new JobGraphData();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                values.Donejobs = dbContext.JobsDetails.Where(u => u.Status == true && u.Job.IsDeleted == false).ToList().Count();
                values.Pendingjobs = dbContext.JobsDetails.Where(u => u.Status == false && u.Job.IsDeleted == false).ToList().Count();
            }

            return values;
        }

        public static JobGraphData JobsGraph(int RegionalHeadID)
        {
            JobGraphData values = new JobGraphData();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                values.Donejobs = dbContext.JobsDetails.Where(u => u.Status == true && u.Retailer.SaleOfficer.RegionalHeadID == RegionalHeadID && u.Job.IsDeleted == false).ToList().Count();
                values.Pendingjobs = dbContext.JobsDetails.Where(u => u.Status == false && u.Retailer.SaleOfficer.RegionalHeadID == RegionalHeadID && u.Job.IsDeleted == false).ToList().Count();
            }

            return values;
        }



        public static List<CityGraphData> CityGraph()
        {
            List<CityGraphData> values = new List<CityGraphData>();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                values = dbContext.Regions.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new CityGraphData
                                {
                                    RegionName = u.Name,
                                    CityCount = u.Cities.Where(c=>c.RegionID == u.ID).Count()
                                }).ToList();
            }

            return values;
        }


        public static List<AreaGraphData> AreaGraph()
        {
            List<AreaGraphData> values = new List<AreaGraphData>();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                values = dbContext.Cities.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new AreaGraphData
                                {
                                    CityName = u.Name,
                                    AreaCount = u.Areas.Where(c => c.CityID == u.ID).Count()
                                }).ToList();
            }

            return values;
        }

        public static List<RegionalHeadGraphData> RegionalHeadGraph()
        {
            List<RegionalHeadGraphData> values = new List<RegionalHeadGraphData>();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                
                values = dbContext.RegionalHeads.Where(c => c.IsDeleted == false)
                            .Select
                            (
                                u => new RegionalHeadGraphData
                                {
                                    RegionalHeadName = u.Name,
                                    RegionCount = u.RegionalHeadRegions.Where(r=>r.RegionHeadID == u.ID).Select(r=>r.RegionID).Count()
                                }).ToList();
            }

            return values;
        }

        public static int SalesOfficerGraph()
        {
            int values;

            using (FOSDataModel dbContext = new FOSDataModel())
            {

                values = dbContext.SaleOfficers.Where(s => s.IsDeleted == false).Select(s=>s.ID).Count();
            }

            return values;
        }

        public static int SalesOfficerGraph(int RegionalHeadID)
        {
            int values;

            using (FOSDataModel dbContext = new FOSDataModel())
            {

                values = dbContext.SaleOfficers.Where(s => s.IsDeleted == false && s.RegionalHeadID == RegionalHeadID).Select(s => s.ID).Count();
            }

            return values;
        }

        public static int DealerGraph()
        {
            int values;

            using (FOSDataModel dbContext = new FOSDataModel())
            {

                values = dbContext.Dealers.Where(s => s.IsDeleted == false).Select(s => s.ID).Count();
            }

            return values;
        }


        public static List<SojobGraphData> SoJobGraph()
        {
            List<SojobGraphData> values = new List<SojobGraphData>();

            using (FOSDataModel dbContext = new FOSDataModel())
            {
                values = dbContext.Jobs
                            .Select
                            (
                                u => new SojobGraphData
                                {
                                    RegionName = u.SaleOfficer.City.Region.Name,
                                    JobsCount = u.JobsHistories.Where(c => c.Status == true && u.IsDeleted == false).Count()
                                }).ToList();
            }

            return values;
        }
        public static List<int?> SOPresenttoday()
        {
           
            FOSDataModel dbContext = new FOSDataModel();
            //var jobs = ManageJobs.GetJobsToExportInExcel();

            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);


            List<int?> lastMonth = (from lm in dbContext.Jobs

                                   where lm.CreatedDate >= dtFromToday && lm.CreatedDate <= dtToToday
                                   select lm.SaleOfficerID).ToList();



            return lastMonth;
        }
        public static List<int> SOAbsenttoday()
        {
          
            FOSDataModel dbContext = new FOSDataModel();
            //var jobs = ManageJobs.GetJobsToExportInExcel();

            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);


            List<int> lastMonth = (from lm in dbContext.AccessLogs

                                   where lm.LoginDate >= dtFromToday && lm.LoginDate <= dtToToday
                                   select lm.SaleOfficerID).ToList();



            return lastMonth;
        }

        public static List<int> FSPlannedtoday()
        {
        
            FOSDataModel dbContext = new FOSDataModel();
          

            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);


            List<int> lastMonth = (from lm in dbContext.Jobs

                                   where lm.CreatedDate >= dtFromToday && lm.CreatedDate <= dtToToday && lm.ComplaintStatusId==3
                                   select lm.ID).ToList();



            return lastMonth;
        }

        public static List<int> FSVisitedtoday()
        {
          
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            FOSDataModel dbContext = new FOSDataModel();
            List<int> lastMonth = (from lm in dbContext.Jobs

                                   where lm.CreatedDate >= dtFromToday && lm.CreatedDate <= dtToToday && lm.ComplaintStatusId == 4
                                   select lm.ID).ToList();
            return lastMonth;
        }

        public static List<int> RSPlannedToday()
        {
            
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            FOSDataModel dbContext = new FOSDataModel();

            List<int> lastMonth = (from lm in dbContext.Jobs

                                   where lm.CreatedDate >= dtFromToday && lm.CreatedDate <= dtToToday && lm.ComplaintStatusId == 1003 
                                   select lm.ID).ToList();
            return lastMonth;
        }


        public static List<int> OpenComplaints()
        {


            FOSDataModel dbContext = new FOSDataModel();

            List<int> lastMonth = (from lm in dbContext.Jobs

                                   where lm.ComplaintStatusId == 4
                                   select lm.ID).ToList();
            return lastMonth;
        }


        public static List<int> NewComplaints()
        {
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);

            FOSDataModel dbContext = new FOSDataModel();

            List<int> lastMonth = (from lm in dbContext.Jobs

                                   where lm.ComplaintStatusId == 2003 && lm.CreatedDate >= dtFromToday && lm.CreatedDate <= dtToToday 
                                   select lm.ID).ToList();
            return lastMonth;
        }


        public static List<string> RSVisitedToday()
        {
           
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
            FOSDataModel dbContext = new FOSDataModel();

            List<string> lastMonth = (from lm in dbContext.JobsDetails

                                      where lm.JobDate >= dtFromToday && lm.JobDate <= dtToToday && lm.VisitPurpose == "Follow Up" && lm.Status == true
                                      select lm.VisitPurpose).ToList();

            // lastMonth.RemoveAll(x => x == "");


            return lastMonth;
        }

        public static List<int> RSFollowUpToday()
        {
           
            FOSDataModel dbContext = new FOSDataModel();
            DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

            DateTime dtFromToday = dtFromTodayUtc.Date;
            DateTime dtToToday = dtFromToday.AddDays(1);
     

            List<int> lastMonth = (from lm in dbContext.Jobs

                                   where lm.CreatedDate >= dtFromToday && lm.CreatedDate <= dtToToday && lm.ComplaintStatusId ==6
                                   select lm.ID).ToList();

         


            return lastMonth;
        }




    }
}
