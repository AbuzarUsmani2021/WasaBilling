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
    public class GetSubDivisionController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ZoneID, int TownID)
        {
            try
            {
                if (ZoneID > 0 && TownID > 0)
                {
                    var SubCat = ManageArea.GetSubDivisionForAPI(ZoneID, TownID);
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            SubDivision = SubCat.Select(d => new
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
                Log.Instance.Error(ex, "SubDivisionController GET API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                SubDivision = param
            });
        }


    }
}