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
    public class GetSMSController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ComplaintID)
        {
            try
            {
                if (ComplaintID > 0)
                {
                    var SubCat = ManageArea.GetSMSForAPI(ComplaintID);
                    if (SubCat != null && SubCat.Count > 0)
                    {
                        return Ok(new
                        {
                            SMS = SubCat.Select(d => new
                            {
                                d.ID,
                                d.Remarks,
                                d.LaunchAt,
                                d.ComplaintID,
                                d.SOID,
                                d.SaleOfficerName,
                                d.SiteName
                            }).OrderByDescending(d => d.ID)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "SMSController GET API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                SMS = param
            });
        }


    }
}