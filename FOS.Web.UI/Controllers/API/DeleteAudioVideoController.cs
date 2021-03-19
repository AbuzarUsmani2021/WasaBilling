using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class DeleteAudioVideoController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ClientRemarksmodel rm)
        {
            ClientRemark retailerObj = new ClientRemark();
            try
            {
                var name = db.JobsDetails.Where(x => x.ID == rm.ProgressID).FirstOrDefault();
                var name2 = db.Tbl_ComplaintHistory.Where(x => x.JobDetailID == rm.ProgressID).FirstOrDefault();
                if (rm.Type=="Video")
                {
                    name.Video = null;
                    name.VideoDate = null;
                    name2.Video = null;
                    name2.VideoDate = null;
                }
                else
                {
                    name.Audio = null;
                    name.AudioDate = null;
                    name2.Audio = null;
                    name2.AudioDate = null;
                }
                 
                db.SaveChanges();

          
               //// var AreaID = Convert.ToInt32(data.Areas);

                //var IDs = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();

            
               


              



                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Audio/Video Deleted Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                
              

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Client Remarks Added API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Client Remarks Added API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }



        }



        public class SuccessResponse
        {

        }
        public class ClientRemarksmodel
        {
            public int ProgressID { get; set; }
           
            public string Type { get; set; }
          


        }

    }
}
