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
    public class GetNotificationCountController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int SOID,int RoleID)
        {
            try
            {
                if (SOID > 0)
                {
                    var SubCat = new CommonController().GetNotificationCount(SOID,RoleID);
                    if (SubCat!=0)
                    {
                        return Ok(new
                        {
                            Count = SubCat
                           
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            Count = 0

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
                Count = 0
            });
        }


    }
}