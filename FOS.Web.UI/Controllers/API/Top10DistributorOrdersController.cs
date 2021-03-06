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
    public class Top10DistributorOrdersController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {

                var SoHeirarchy = dbContext.Sp_PresentSOHeirarchy1_1(SOID).ToList();

                int Saleofficerid = 0;
                foreach (var item in SoHeirarchy)
                {
                    Saleofficerid = item.SaleOfficerID;
                }
                // List<Tbl_Access> list = null;
                var list = dbContext.Tbl_Access.Where(x => x.RepotedUP == Saleofficerid).ToList();

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                List<Top10Dis> obj = new List<Top10Dis>();
                foreach(var itm in list)
                {
                   List<Sp_Top10DistributorOrdersCityWiseGraph1_3_Result> data = dbContext.Sp_Top10DistributorOrdersCityWiseGraph1_3(localTime, itm.SaleOfficerID).ToList();

                    if (data.Count != 0)
                    {
                        foreach (var ele in data)

                            if (obj.Count >= 1)
                            {
                                var result = obj.Find(x => x.CityName == ele.City);
                                if (result != null)
                                {
                                    result.Top10DistributorOrdersCityWise = result.Top10DistributorOrdersCityWise + ele.Orders;

                                }
                                else
                                {
                                    obj.Add(new Top10Dis()
                                    {
                                        //the name of the db column
                                        CityName = ele.City,
                                        Top10DistributorOrdersCityWise = ele.Orders,

                                    });

                                }


                            }
                            else
                            {
                                obj.Add(new Top10Dis()
                                {
                                    //the name of the db column
                                    CityName = ele.City,
                                    Top10DistributorOrdersCityWise = ele.Orders,

                                });
                            }
                    }

                  }

                        var DisData = obj.ToList();
                        if (obj.Count!= 0)
                    {
                        return Ok(new
                        {
                            Top10DistributorOrdersCityWise = DisData
                            
                        });
                    }
                
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = {};
            return Ok(new
            {
                Top10DistributorOrdersCityWise = paramm
            });

        }


    }
}
public class Top10Dis
{
    public string CityName { get; set; }
    public int? Top10DistributorOrdersCityWise { get; set; }
}