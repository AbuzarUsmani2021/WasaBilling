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
    public class EquipmentTypeController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int EquipmentCatID)
        {
            try
            {
                if (EquipmentCatID > 0 )
                {
                    var Item = FOS.Setup.ManageCity.GetEquipmenttypedetaileList(EquipmentCatID);
                    if (Item != null && Item.Count > 0)
                    {
                        return Ok(new
                        {
                            EquipmentTypeDetail = Item.Where(s => s.IsDeleted==false).Select(d => new
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
                Log.Instance.Error(ex, "FaultTypeDetailController Get API Failed");
            }
            object[] param = { };
            return Ok(new
            {
                EquipmentTypeDetail = param
            });
        }
    }
}