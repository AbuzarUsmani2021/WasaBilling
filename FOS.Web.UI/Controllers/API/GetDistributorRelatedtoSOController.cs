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
    public class GetDistributorRelatedtoSOController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);


                DateTime dtFromToday = dtFromTodayUtc.Date;
                DateTime todate = dtFromToday.AddDays(1);
                DateTime fromdate = todate.AddDays(-30);

                if (SOID > 0)
                {
                    object[] param = { SOID };




                    var result = dbContext.sp_GetDistributorListInDSR(SOID,fromdate,todate).ToList();

                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            DistributorRelatedToSO = result

                        });
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "DistributorRelatedToSO GET API Failed");
            }
            object[] paramm = {  };
            return Ok(new
            {
                DistributorRelatedToSO = paramm
            });

        }
    }
}