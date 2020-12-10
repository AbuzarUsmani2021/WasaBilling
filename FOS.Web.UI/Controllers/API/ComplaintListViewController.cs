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
    public class ComplaintListViewController : ApiController
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

                if (ComplaintID > 0)
                {
                    object[] param = { ComplaintID };
                    List<MyComplaintListView> list = new List<MyComplaintListView>();
                    MyComplaintListView comlist;
                    var result = dbContext.Sp_MyComplaintListViewFinal(ComplaintID, dtFromToday,dtToToday).ToList();


                   // var result1= dbContext.Sp_MyComplaintListViewPictures(ComplaintID, dtFromToday, dtToToday).ToList();
                    


                    foreach (var item in result)
                    {
                        
                          
                                comlist = new MyComplaintListView();


                                comlist.ComplaintID = item.ComplaintID;
                                comlist.SiteCode = item.SiteCode;
                                comlist.LaunchDate = item.LaunchDate;
                                comlist.SiteID = item.SiteID;
                                comlist.SiteName = item.SiteName;
                                comlist.FaultType = item.FaultType;
                                comlist.LaunchedByName = item.LaunchedByName;
                                comlist.FaultTypeDetail = item.FaultTypeDetail;
                                if (item.FaultTypeDetail == "Others")
                                {
                                    

                                    comlist.FaultTypeDetail = comlist.FaultTypeDetail + "/" + item.faulttypedetailremarks;
                                }
                                comlist.TicketNo = item.TicketNo;
                                comlist.InitialRemarks = item.InitialRemarks;
                                comlist.ComplaintStatus = item.StatusName;
                                comlist.Picture1 = item.Picture1;
                                comlist.Picture2 = item.Picture2;
                                comlist.Picture3 = item.Picture3;

                                comlist.ClientRemarks = new CommonController().GetClientRemarks((int)item.ComplaintID);


                                list.Add(comlist);

                           
                      
                       
                    }
                    
                    if (list != null && list.Count > 0)
                    {
                        return Ok(new
                        {
                            MyComplaintListView = list

                        });
                    }
                 
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "MyComplaintList GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                MyComplaintListView = paramm
            });

        }


    }


    public class MyComplaintListView
    {
        public int? ComplaintID { get; set; }
        public string TicketNo { get; set; }
        public DateTime? LaunchDate { get; set; }

        public string SiteName { get; set; }
        public string LaunchedByName { get; set; }
        public int? SiteID { get; set; }

        public string SiteCode { get; set; }

        public string ComplaintStatus { get; set; }

        public string FaultType { get; set; }
        public string FaultTypeDetail { get; set; }
        public string Picture1 { get; set; }
        public string Picture2 { get; set; }
        public string Picture3 { get; set; }

        public string InitialRemarks { get; set; }

        public List<ClientRemarks> ClientRemarks { get; set; }
    }
}