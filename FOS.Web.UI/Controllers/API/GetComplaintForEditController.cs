using FOS.DataLayer;
using FOS.Setup;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace FOS.Web.UI.Controllers.API
{
    public class GetComplaintForEditController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ComplaintID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);

                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime dtToToday = dtFromToday.AddDays(1);
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (ComplaintID > 0)
                {
                    List<MyComplaintListForEdit> list = new List<MyComplaintListForEdit>();
                    MyComplaintListForEdit comlist;
                    object[] param = { ComplaintID };

                    // RetailerData cty;

                    var result = dbContext.Sp_GetComplaintForEdit(ComplaintID).ToList();
                    var result1 = dbContext.Sp_MyComplaintListRemarksGet1_1(ComplaintID, dtFromToday, dtToToday).ToList();



                    foreach (var item in result)
                    {
                        foreach (var items in result1)
                        {
                            if (item.ComplaintID == items.ComplaintID)
                            {
                                comlist = new MyComplaintListForEdit();


                                comlist.ComplaintID = item.ComplaintID;
                                comlist.SiteCode = item.SiteCode;
                                comlist.LaunchDate = item.LaunchDate;
                                comlist.SiteID = item.SiteID;
                                comlist.SiteName = item.SiteName;
                                comlist.PriorityId = item.PriorityId;
                                comlist.PriorityName = item.PriorityName;
                                comlist.PersonName = item.PersonName;
                                comlist.FaultTypeDetailID = item.FaultTypeDetailID;
                                comlist.SaleOfficerID = item.SaleOfficerID;
                                comlist.ProgressRemarks = items.ProgressStatusName;
                                comlist.InitialRemarks = item.InitialRemarks;
                                comlist.FaultTypeDetailOtherRemarks = items.Activitytype;
                                comlist.FaultTypeDetailName = item.FaultTypeDetailName;
                                comlist.ComplaintStatusID = item.ComplaintStatusId;
                                comlist.ComplaintStatusName = item.ComplaintStatusName;
                                comlist.ComplainttypeID = item.ComplainttypeID;
                                comlist.ComplainttypeName = item.ComplainttypeName;
                                comlist.ResolutionHour = new CommonController().GetResolvedHour(item.ComplaintID);
                                comlist.FaultTypeId = item.FaultTypeId;
                                comlist.FaulttypeName = item.FaulttypeName;
                                comlist.ProgressStatusName = items.ProgressStatusName;
                                comlist.ProgressStatusRemarks = items.ProgressStatusRemarks;
                                comlist.ProgressStatusID = items.ProgressStatusID;
                                comlist.AssignedSaleOfficerID = items.AssignedSaleOfficerID;
                                comlist.AssignedSaleOfficerName = new CommonController().GetAssignedSaleOfficerNAme(items.AssignedSaleOfficerID);

                                list.Add(comlist);

                            }
                        }

                    }
                    if (list != null && list.Count > 0)
                    {
                        return Ok(new
                        {
                            ComplaintDetail = list

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "StockDetail GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                ComplaintDetail = paramm
            });

        }


    }

    public class MyComplaintListForEdit
    {
        public int ComplaintID { get; set; }
        public int? ResolutionHour { get; set; }
        public int? AssignedSaleOfficerID   { get; set; }
        public string TicketNo { get; set; }
        public string AssignedSaleOfficerName { get; set; }
        public DateTime? LaunchDate { get; set; }

        public string SiteName { get; set; }
        public string ProgressStatusName { get; set; }
        public string ProgressStatusRemarks { get; set; }
        public string LaunchedByName { get; set; }
        public int? SiteID { get; set; }
        public int? ProgressStatusID { get; set; }
        public int? SaleOfficerID { get; set; }
        public string SiteCode { get; set; }
        public int? FaultTypeId { get; set; }
        public int? PriorityId { get; set; }
        public int? ComplaintStatusID { get; set; }
        public int? FaultTypeDetailID { get; set; }
        public int? ComplainttypeID { get; set; }
        public string PriorityName { get; set; }
        public string ComplainttypeName { get; set; }
        public string PersonName { get; set; }
        public string ComplaintStatusName { get; set; }
        public string FaulttypeName { get; set; }
        public string FaultTypeDetailName { get; set; }
        public string FaultTypeDetailOtherRemarks { get; set; }
        public string SaleOfficerName { get; set; }

        public string InitialRemarks { get; set; }

        public string ProgressRemarks { get; set; }
    }
}