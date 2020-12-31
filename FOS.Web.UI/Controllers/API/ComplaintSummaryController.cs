using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class ComplaintSummaryController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ProjectID, int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                List<MyComplaintSummary> list = new List<MyComplaintSummary>();
                List<MyComplaintSummary> list2 = new List<MyComplaintSummary>();
                MyComplaintSummary comlist;
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);

                var IDS = dbContext.SOZoneAndTowns.Where(x => x.SOID == SOID).Distinct().ToList();

                foreach (var item in IDS)
                {

                    var result = db.Jobs.Where(x => x.ZoneID == ProjectID && x.CityID == item.CityID && x.Areas == item.AreaID.ToString() && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).ToList();

                    if (result.Count != 0)
                    {
                        foreach (var items in result)
                        {
                            comlist = new MyComplaintSummary();
                            comlist.TotalComplaints += 1;
                            comlist.Resolved += db.Jobs.Where(x => x.ComplaintStatusId == 3 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                            comlist.InProgress += db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                            comlist.PassOn += db.Jobs.Where(x => x.ComplaintStatusId == 1003 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                            comlist.NewComplaints += db.Jobs.Where(x => x.ComplaintStatusId == 2003 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                            comlist.OpenComplaints += db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                            comlist.OpenComplaintsToday += comlist.InProgress + comlist.NewComplaints;
                            list.Add(comlist);
                        }

                    }

                }









                if (list != null)
                {
                    return Ok(new
                    {
                        MyComplaintSummary = list

                    });
                }


            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "MyComplaintSummary GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                MyComplaintSummary = paramm
            });

        }


    }


    public class MyComplaintSummary
    {
        public int TotalComplaints { get; set; }


        public int NewComplaints { get; set; }

        public int Resolved { get; set; }

        public int InProgress { get; set; }

        public int PassOn { get; set; }
        public int OpenComplaintsToday { get; set; }
        public int OpenComplaints { get; set; }

    }
}