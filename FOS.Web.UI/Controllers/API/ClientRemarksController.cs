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
    public class ClientRemarksController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ClientRemarksmodel rm)
        {
            ClientRemark retailerObj = new ClientRemark();
            try
            {

                retailerObj.ComplaintID = rm.ComplaintID;
                retailerObj.ClientRemarks = rm.Remarks;
                retailerObj.IsActive = true;
                retailerObj.Isdeleted = true;
                retailerObj.RemarksDate = DateTime.UtcNow.AddHours(5);
                retailerObj.RemarksBy = rm.SOID;
                retailerObj.RemarksByName = rm.SOName;
                db.ClientRemarks.Add(retailerObj);
                 
                db.SaveChanges();

                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Client Remarks Added Successfully",
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
            public int ComplaintID { get; set; }
            public int SOID { get; set; }
            public string Remarks { get; set; }
            public string SOName { get; set; }


        }

    }
}
