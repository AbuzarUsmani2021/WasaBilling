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
    public class GetPicturesForEditController : ApiController
    {

        FOSDataModel db = new FOSDataModel();

        public IHttpActionResult Get(int ComplaintID)
        {
            FOSDataModel dbContext = new FOSDataModel();
            try
            {
                //List<RetailerData> MAinCat = new List<RetailerData>();
                if (ComplaintID > 0)
                {
                    object[] param = { ComplaintID };

                    // RetailerData cty;

                    var result = dbContext.Sp_GetPicturesForEdit1_1(ComplaintID).ToList();


                    if (result != null && result.Count > 0)
                    {
                        return Ok(new
                        {
                            PictureDetail = result

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Complaint Detail GET API Failed");
            }
            object[] paramm = { };
            return Ok(new
            {
                PictureDetail = paramm
            });

        }


    }
}