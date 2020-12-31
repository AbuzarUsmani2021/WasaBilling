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
    public class ComplaintSummaryForWasaController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ProjectID,int SOID)
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

                var RoleID = db.SaleOfficers.Where(x => x.ID == SOID).Select(x => x.RoleID).FirstOrDefault();
                if (RoleID != 3)
                {


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
                                var open = db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                                var inprogress = db.Jobs.Where(x => x.ComplaintStatusId == 2003 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                                comlist.OpenComplaints = open + inprogress;
                                comlist.OpenComplaintsToday += comlist.InProgress + comlist.NewComplaints;
                                list.Add(comlist);
                            }

                        }
                        else
                        {
                            comlist = new MyComplaintSummary();
                            comlist.TotalComplaints += 0;
                            comlist.Resolved += 0;
                            comlist.InProgress += 0;
                            comlist.PassOn += 0;
                            comlist.NewComplaints += 0;
                            if (ProjectID == 0)
                            {
                                comlist.OpenComplaints += 0;
                            }
                            else
                            {
                                var open = db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                                var inprogress = db.Jobs.Where(x => x.ComplaintStatusId == 2003 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                                comlist.OpenComplaints = open + inprogress;
                            }
                            comlist.OpenComplaintsToday += comlist.InProgress + comlist.NewComplaints;
                            list.Add(comlist);
                        }

                    }



                }
                else
                {
                    var inprogress = 0;
                    var results = db.Sp_MyComplaintListForFS1_1(ProjectID, dtFromToday, dtToToday, SOID).ToList();

                    var query = results.GroupBy(d => d.ComplaintID)
                                    .SelectMany(g => g.OrderByDescending(d => d.LaunchDate)
                                  .Take(1));

                    foreach (var items in query)
                    {
                        comlist = new MyComplaintSummary();
                        comlist.TotalComplaints += 1;
                        comlist.Resolved += db.Jobs.Where(x => x.ComplaintStatusId == 3 && x.ID == items.ComplaintID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        comlist.InProgress += db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ID == items.ComplaintID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        comlist.PassOn += db.Jobs.Where(x => x.ComplaintStatusId == 1003 && x.ID == items.ComplaintID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        comlist.NewComplaints += db.Jobs.Where(x => x.ComplaintStatusId == 2003 && x.ID == items.ComplaintID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        // var open = db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                        if (items.StatusName == "InProgress")
                        {

                            inprogress += inprogress + 1;
                            comlist.OpenComplaints = inprogress;
                            comlist.OpenComplaintsToday += comlist.InProgress + comlist.NewComplaints;
                            list.Add(comlist);
                        }

                        //var IDS = dbContext.SOZoneAndTowns.Where(x => x.SOID == SOID).Distinct().ToList();

                        //foreach (var item in IDS)
                        //{

                        //    var result = db.Jobs.Where(x => x.ZoneID == ProjectID && x.CityID == item.CityID && x.Areas == item.AreaID.ToString() && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).ToList();

                        //    if (result.Count != 0)
                        //    {
                        //        foreach (var items in result)
                        //        {
                        //            comlist = new MyComplaintSummary();
                        //            comlist.TotalComplaints += 1;
                        //            comlist.Resolved += db.Jobs.Where(x => x.ComplaintStatusId == 3 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        //            comlist.InProgress += db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        //            comlist.PassOn += db.Jobs.Where(x => x.ComplaintStatusId == 1003 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        //            comlist.NewComplaints += db.Jobs.Where(x => x.ComplaintStatusId == 2003 && x.ID == items.ID && x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday).Count();
                        //            var open = db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                        //            var inprogress = db.Jobs.Where(x => x.ComplaintStatusId == 2003 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                        //            comlist.OpenComplaints = open + inprogress;
                        //            comlist.OpenComplaintsToday += comlist.InProgress + comlist.NewComplaints;
                        //            list.Add(comlist);
                        //        }

                        //    }
                        //    else
                        //    {
                        //        comlist = new MyComplaintSummary();
                        //        comlist.TotalComplaints += 0;
                        //        comlist.Resolved += 0;
                        //        comlist.InProgress += 0;
                        //        comlist.PassOn += 0;
                        //        comlist.NewComplaints += 0;
                        //        if (ProjectID == 0)
                        //        {
                        //            comlist.OpenComplaints += 0;
                        //        }
                        //        else
                        //        {
                        //            var open = db.Jobs.Where(x => x.ComplaintStatusId == 4 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                        //            var inprogress = db.Jobs.Where(x => x.ComplaintStatusId == 2003 && x.ZoneID == ProjectID && x.CreatedDate <= dtFromToday).Count();
                        //            comlist.OpenComplaints = open + inprogress;
                        //        }
                        //        comlist.OpenComplaintsToday += comlist.InProgress + comlist.NewComplaints;
                        //        list.Add(comlist);
                        //    }

                        //}
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
            object[] paramm = {};
            return Ok(new
            {
                MyComplaintSummary = paramm
            });

        }


    }



}