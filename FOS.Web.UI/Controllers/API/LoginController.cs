using FOS.DataLayer;
using FOS.Setup;
using FOS.Web.UI.Common;
using FOS.Web.UI.Controllers.API;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class LoginController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<LoginResponse> Post(LoginRequest inModel)
        {
            try
            {
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                if (inModel.UserName != null && inModel.Password != null)
                {

                    var SO = db.SaleOfficers.Where(s => s.UserName.ToLower().Equals(inModel.UserName.ToLower()) && s.Password.ToLower().Equals(inModel.Password.ToLower())).FirstOrDefault();

                    if (SO != null)
                    {

                        string Token = FOS.Web.UI.Common.Token.TokenAttribute.GenerateToken(inModel.UserName, inModel.Password);
                        Token tokenObj = new Token();

                        tokenObj.SalesOfficerID = SO.ID;
                        tokenObj.TokenName = Token;
                        tokenObj.TokenAssignDate = DateTime.Now;
                        db.Tokens.Add(tokenObj);
                        db.SaveChanges();

                        LoginResponse data = new LoginResponse
                        {
                            SOID = SO.ID,
                            Name = SO.Name,
                            SOType=SO.Type,
                            RoleID = SO.RoleID,
                            RegionalHeadID = SO.RegionalHeadID,
                            RegionID = SO.RegionID,
                            RegionalHeadType= new CommonController().GetRegionalHeadTypeID((int)SO.RegionalHeadID),
                           Token = Token,
                            Projects= new CommonController().GetProjects(SO.ID),
                            FaultTypes= new CommonController().GetFaultTypes(),
                            EquipmentCategory = new CommonController().GetEquipmentCategory(),
                            MeterReadingLovs = new CommonController().GetLovs(),
                            EquipmentBrand = new CommonController().GetEquipmentBrands(),
                            Priorities = new CommonController().GetPriorities(),
                            Status= new CommonController().GetComplaintStatus(),
                            Type= new CommonController().GetComplaintTypes(),
                            SiteStatuses = new CommonController().GetSiteStatuses(SO.RegionID),
                            LaunchedBy = new CommonController().GetLaunchedBy(),
                            EquipParent = new CommonController().GetEquipParent(),
                            EquipChild = new CommonController().GetEquipCild(),
                            MultiSelectlist = new CommonController().GetMultiselectList(),
                            Roles = new CommonController().GetRole(),
                            WorkDoneStatus= new CommonController().GetWorkDoneStatuses(),
                            VisitTypes = new CommonController().GetvisitTypesStatuses(),
                            VisitPersons = new CommonController().GetvisitPersons(),
                            ConnectionTypes = new CommonController().GetConnectiontypes(),
                            Staff = new CommonController().GetStaffList(SO.RegionalHeadID, SO.RegionID),
                            VisitPurposesTypes= new CommonController().GetvisitPurposeTypes(),
                            //RegionID=db.RegionalHeadRegions.Where(x=>x.RegionHeadID==SO.RegionalHeadID).Select(x=>x.RegionID).FirstOrDefault(),
                            //Region = new CommonController().GetCities(SO.RegionalHeadID),
                            Count = new CommonController().GetNotificationCount(SO.ID,SO.RoleID),
                            //RetailersRelatedtoSO = new CommonController().CustomersRrelatedToSoForCheckin(SO.ID),
                            //DistributorRelatedtoSO = new CommonController().DistributorRrelatedToSoForCheckin(SO.ID),
                            //MainCatg = new CommonController().MainCat(),
                            //RetailerClass = new CommonController().RetailerType(),
                            //RetailerType = new CommonController().RetailerType1(),
                            //SalesOfficer = new CommonController().SalesOfficers(SO.RegionalHeadID, SO.ID),
                            SalesOfficerNames = new CommonController().SalesOfficersNames(SO.ID),
                            AssignedSaleofficer= new CommonController().AssignedSalesOfficersNames((int)SO.RegionalHeadID),
                            //Followupreasons= new CommonController().FollowUp(),
                            //Retailers =db.Retailers.Where(x=>x.IsActive==true).Count(),
                            //Distributors= db.Dealers.Where(x => x.IsActive == true).Count(),
                            //RetailersOrders= (from lm in db.JobsDetails
                            //                  where lm.JobDate >= startDate
                            //                  && lm.JobDate <= endDate
                            //                  && lm.JobType == "Retailer Order"
                            //                  select lm).Count(),

                            //DistributorsOrders= (from lm in db.JobsDetails
                            //                     where lm.JobDate >= startDate
                            //                     && lm.JobDate <= endDate
                            //                  


                        };
                       

                        return new Result<LoginResponse>
                        {
                            Data = data,
                            Message = "Login successful",
                            ResultType = ResultType.Success,
                            Exception = null,
                            ValidationErrors = null
                        };
                    }

               

                    else
                    {
                        return new Result<LoginResponse>
                        {
                            Data = null,
                            Message = "Incorrect UserName/Password",
                            ResultType = ResultType.Failure,
                            Exception = null,
                            ValidationErrors = null
                        };
                    }
                    
                }

                return new Result<LoginResponse>
                {
                    Data = new LoginResponse(),
                    Message = "Please provide username/password",
                    ResultType = ResultType.Failure,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                return new Result<LoginResponse>
                {
                    Data = new LoginResponse(),
                    Message = "Something goes wrong!",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }
        }



    }


    public class LoginResponse
    {
        public string Name { get; set; }
        public string SOType { get; set; }
        public int SOID { get; set; }
        public int? RegionalHeadID { get; set; }
        public String Token { get; set; }
        public int? RoleID { get; set; }
        public int? RegionID { get; set; }
        public int Retailers { get; set; }
        public int Count { get; set; }
        public int RetailersOrders { get; set; }
        public int Distributors { get; set; }
        public int DistributorsOrders { get; set; }

        public List<City> Cities { get; set; }
        public List<Projects> Projects { get; set; }

        public List<Projects> RegionalHeadType { get; set; }
        public List<FaultType> FaultTypes { get; set; }
        public List<FaultType> EquipmentCategory { get; set; }
        public List<FaultType> MeterReadingLovs { get; set; }
        public List<FaultType> EquipmentBrand { get; set; }
        public List<Priority> Priorities { get; set; }
        public List<WorkDone> WorkDoneStatus { get; set; }
        public List<ComplaintType> VisitTypes { get; set; }

        public List<ComplaintType> EquipParent { get; set; }
        public List<EquipChild> EquipChild { get; set; }
        public List<ComplaintType> MultiSelectlist { get; set; }
        public List<ComplaintType> VisitPersons { get; set; }
        public List<ComplaintType> ConnectionTypes { get; set; }
        public List<StaffList> Staff { get; set; }
        public List<Purposes> VisitPurposesTypes { get; set; }
        public List<ComplaintStatus> Status { get; set; }
        public List<ComplaintType> Type { get; set; }
        public List<ComplaintType> SiteStatuses { get; set; }
        public List<ComplaintType> LaunchedBy { get; set; }

        public List<ComplaintType> Roles { get; set; }
        public List<Customers> Customers { get; set; }
        public List<CustomersForCheckin> RetailersRelatedtoSO { get; set; }

        public List<CustomersForCheckin> DistributorRelatedtoSO { get; set; }
        //public List<int> Retailers { get; set; }
        public List<MainCategories> MainCatg { get; set; }
        public List<RetailerType> RetailerClass { get; set; }

        public List<RetailerType> RetailerType { get; set; }
        public List<AllSaleOfficers> SalesOfficer { get; set; }
        public List<AllSaleOfficers> SalesOfficerNames { get; set; }
        public List<AllSaleOfficers> AssignedSaleofficer { get; set; }
        public List<City> Followupreasons { get; set; }
    }
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string OneSignalUserID { get; set; }
        public string IMEI { get; set; }
        public int AppUser { get; set; }
    }

    public class City
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Regions
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

public class Projects
{
    public int? ID { get; set; }
    public string Name { get; set; }
}


    public class ClientRemarks
    {
        public int? ID { get; set; }
        public int? ComplaintID { get; set; }
        public string Remarks { get; set; }
        public string SOName { get; set; }
        public DateTime? LaunchedAt { get; set; }
    }
    public class FaultType
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class Priority
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class WorkDone
    {
        public int? FaultTypeID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class ComplaintStatus
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class ComplaintType
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class Purposes
    {
        public int ID { get; set; }
        public int? PersonID { get; set; }
        public string Name { get; set; }
    }

    public class EquipChild
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
    }

    public class StaffList
    {
        public int ID { get; set; }
        public int? RegionID { get; set; }
        public int? HeadID { get; set; }
        public string Name { get; set; }
    }
    public class Customers
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class CustomersForCheckin
    {
        public int ID { get; set; }
        public string ShopName { get; set; }

        public bool ISActive { get; set; }

    }
    public class MainCategories
    {
        public int MainCategID { get; set; }
        public string MainCategDesc { get; set; }
    }

    public class RetailerType
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }


    public class AllSaleOfficers
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class AllRetailersRetailerToSO
    {
        public int? ID { get; set; }
       
        public string Name { get; set; }

    }
}