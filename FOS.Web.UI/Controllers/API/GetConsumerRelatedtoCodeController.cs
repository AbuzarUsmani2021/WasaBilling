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
    public class GetConsumerRelatedtoCodeController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int CityID,int AreaID,int SubDevisionID,int Code)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                var SubdivID = db.SubDivisions.Where(x => x.ID == SubDevisionID).Select(x => x.AreaIDRef).FirstOrDefault();
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (CityID > 0)
                {
                    object[] param = { CityID };

                    // RetailerData cty;

                    var result = dbContext.TBL_Consumers.Where(x => x.DDRID == CityID && x.WardID == AreaID  && x.ward_Digits == Code).Select(x => new
                    {
                        ID=x.ID,
                        ConsumerNo= x.ConnectionCode,
                        DDR= dbContext.Cities.Where(z => z.ID == x.DDRID).Select(z => z.Name).FirstOrDefault(),
                        Ward= dbContext.Areas.Where(z => z.ID == x.AreaID).Select(z => z.Name).FirstOrDefault(),
                        ConsumerName=x.ConsumerName,
                        Address=x.Address,
                        MobileNo=123,
                        AreaMarla=x.AreaMarla,
                        Lattitude=x.Latitude,
                        Longitude=x.Longitude,
                        MeterNo=x.MeterNo,
                        ConnectionType=dbContext.ConnectionTypes.Where(z=>z.ID==x.ConnectionCode).Select(z=>z.ConnectionTypeName).FirstOrDefault()

                    }).FirstOrDefault();


                    if (result != null)
                    {
                        return Ok(new
                        {
                            ConsumerInfo = result

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "VisitDetailController GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                ConsumerInfo = paramm
            });

        }


    }
}