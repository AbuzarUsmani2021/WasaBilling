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
    public class GetSitesController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ProjectID, int ZoneID, int TownID, int SubDivisionID)
        {
            try
            {
                if (ZoneID > 0 && TownID > 0 && SubDivisionID > 0)
                {
                    var SubCat = ManageArea.GetSitesForAPIFinal(ProjectID ,ZoneID, TownID, SubDivisionID);
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            Sites = SubCat.Select(d => new
                            {
                                d.ID,
                                d.Name,
                                d.ShortCode
                            }).OrderBy(d => d.ID)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "SitesController GET API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                Sites = param
            });
        }


    }
}