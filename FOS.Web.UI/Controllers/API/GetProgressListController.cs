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
    public class GetProgressListController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ComplaintID,int RoleID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (ComplaintID > 0)
                {
                    object[] param = { ComplaintID };

                    if (RoleID == 2)
                    {

                        var result = dbContext.Sp_GetProgressListFinal(ComplaintID).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ProgressList = result

                            });
                        }
                    }
                    else
                    {
                        var result = dbContext.Sp_GetProgressList1_4Final(ComplaintID).ToList();
                        if (result != null && result.Count > 0)
                        {
                            return Ok(new
                            {
                                ProgressList = result

                            });
                        }

                    }

                   

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ProgressList GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                ProgressList = paramm
            });

        }


    }
}