using FOS.DataLayer;
using FOS.Shared;
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
    public class SiteEquipmentRegistrationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(SiteEquipmentDetailData obj)
        {
            SiteEquipmentDetail retailerObj = new SiteEquipmentDetail();
            try
            {
                retailerObj.ID = db.SiteEquipmentDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                retailerObj.SiteID = obj.SiteID;
                // Client Id is region id in retailers table
                retailerObj.BrandID = obj.BrandID;

                // Zone Id is Project id in retailers table
                retailerObj.EquipmentCategoryID = obj.EquipmentCatID;

                retailerObj.EquipmentTypeID = obj.EquipmentTypeID;
                retailerObj.MaterialNo = obj.MaterialNo;
                retailerObj.Condition = obj.Condition;
                retailerObj.Capacity = obj.Capacity;
                retailerObj.Size = obj.Size;
                retailerObj.Color = obj.Color;
                
                retailerObj.YearOfManufacture = obj.YearOfManufacture;
                // retailerObj.CardNumber = obj.CardNumber;
                retailerObj.YearOfInstall = obj.YearOfInstall;
                retailerObj.Guarantee = obj.Guarantee;
                retailerObj.GuaranteeDetail = obj.GuaranteeDetail;
                retailerObj.ExpiryDate = obj.ExpiryDate;

                retailerObj.MaintainedBy = obj.MaintainedByKSB;
                retailerObj.MaintainedByWhome = obj.MaintaineByWhome;

                retailerObj.Weight = obj.Weight;
                retailerObj.MediumInUse = obj.MediumInUse;
                retailerObj.OperatingTemperature = obj.OperatingTemperature;
                retailerObj.OperatingPressure = obj.OperatingPressure;
                retailerObj.Remarks = obj.Remarks;

                db.SiteEquipmentDetails.Add(retailerObj);

                db.SaveChanges();



                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Site Equipment Registration Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               
            

            }
            catch (Exception ex)
            {
               
                Log.Instance.Error(ex, "Add Site Equipment API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Site Equipment Registration API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex, 
                    ValidationErrors = null
              
            };
               

                

            }

         

        }



    }
}