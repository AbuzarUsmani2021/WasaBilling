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
    public class GetTownsRelatedToZonesController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ZoneID,int SOID)
        {
            try
            {
                if (ZoneID > 0)
                {
                    var Item = ManageArea.GetAreasForAPI(ZoneID, SOID);
                    if (Item != null && Item.Count > 0)
                    {
                        return Ok(new
                        {
                            Towns = Item.Select(d => new
                            {
                                d.ID,
                                d.Name
                            }).OrderBy(d => d.ID)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "TownsController Get API Failed");
            }

            return Ok(new
            {
                Towns = new { }
            });
        }
    }
}