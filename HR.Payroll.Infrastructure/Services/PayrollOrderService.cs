using AutoMapper;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Data;
using HR.Order.Infrastructure.Helpers;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.SharedKernel;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LinqKit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;

namespace HR.Payroll.Infrastructure.Services;

public class PayrollOrderService(
IMapper mapper, IdentityContext IdentityContext, CustomIdentityContext CustomIdentityContext , UserResolverService UserResolverService, UserOrganizationUnitService UserOrganizationUnitService, UserWorkPlaceService UserWorkPlaceService, UserCostCenterService UserCostCenterService, IUnitOfWork<OrderContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<InterdictOrder, OrderContext, InterdictOrderDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly UserCostCenterService _userCostCenterService = UserCostCenterService;
    private readonly UserWorkPlaceService _userWorkPlaceService = UserWorkPlaceService;
    private readonly UserOrganizationUnitService _userOrganizationUnitService = UserOrganizationUnitService;
    //private readonly AspNetUsersService _aspNetUsersService = AspNetUsersService;
    private UserResolverService _userService = UserResolverService;
    IdentityContext _identityContext = IdentityContext;
    CustomIdentityContext _customIdentityContext = CustomIdentityContext;
    /// <summary>
    /// گرفتن فهرست احکام بر اساس شناسه کارمندی
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public OperationResult GetOrderListByEmployeeID(GetOrderListByEmployeeIdRequest req, bool isFromProfile = false)
    {

        try
        {
            var ret = GetOrderList(req, isFromProfile);
            List<SqlParameter> list = new List<SqlParameter>();
            list.Add(new SqlParameter("@EmployeeID", req.EmployeeId));
            var orderCount = HR.SharedKernel.Data.DbFunction.CallDBFunction(_connectionString, "Order", "Fn_GetOrderCount", list.ToArray());
            return OperationResult.Succeeded(payload: ret, rowCount: Convert.ToInt32(orderCount));
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    /// <summary>
    /// فهرست احکام در انتظار تایید حقوق دستمزد
    /// </summary>
    /// <returns></returns>
    public  OperationResult getCurrentOrganPayRollPendigOrders(PayRollOrderPagerDTO req)
    {
        try
        {



            List<long> validStatusId =
            [
                (long)Enums.OrderStatus.FinalOrder,
                (long)Enums.OrderStatus.LastOrder,
            ];


            var all =  unitOfWork.Context.InterdictOrders
                .Include(i => i.RecruitOrder)
                .Include(i => i.RecruitOrder.Employee)
                .Include(i => i.RecruitOrder.PayLocation)
                .Include(i => i.RecruitOrder.CostCenter)
                .Include(i => i.RecruitOrder.OrganizationUnit)
                .Include(i => i.RecruitOrder.WorkPlace)
                .Include(i => i.RecruitOrder.Project)
                .Include(i => i.OrderType)
                .Include(i => i.RecruitOrder.EmployeeStatus)
                .Include(i => i.RecruitOrder.EmployeeType)
                .Include(i => i.RecruitOrder.OrganizationJob)
                .Include(i => i.RecruitOrder.OrganisationPosition)
                .Include(i => i.Status)
                .Where(i => i.RecruitOrder.PayLocationId == _currentUserDefaultOrganId && i.PayRollAprove != true
                && validStatusId.Contains(i.StatusId)
                && (req.StartDate.HasValue ? (i.StartDate.HasValue && i.StartDate.Value.Date == req.StartDate.Value) : true)
                //&& (req.EndDate.HasValue ? (i.EndDate.HasValue && i.EndDate.Value.Date == req.EndDate.Value) : true)
                && ((req.OrderTypeId == 0 || req.OrderTypeId == null) || (i.OrderTypeId == req.OrderTypeId && req.OrderTypeId > 0))
                && ((req.WorkPlaceId == 0 || req.WorkPlaceId == null) || (i.RecruitOrder.WorkPlaceId == req.WorkPlaceId && req.WorkPlaceId > 0))
                && ((req.OrganizationUnitId == 0 || req.OrganizationUnitId == null) || (i.RecruitOrder.OrganizationUnitId == req.OrganizationUnitId && req.OrganizationUnitId > 0))
                && ((req.CostCenterId == 0 || req.CostCenterId == null) || (i.RecruitOrder.CostCenterId == req.CostCenterId && req.CostCenterId > 0))

                )
                ;

            var currentUser =  _identityContext.AspNetUsers.Find(_userService.GetUserId());
            var userCostCenterSetting = _customIdentityContext.UserCostCenters.Where(i => i.UserId == currentUser.Id).Select(i => i.CostCenterId).ToList();
            var userWorkPlaceSetting = _customIdentityContext.UserWorkPlaces.Where(i => i.UserId == currentUser.Id).Select(i => i.WorkPlaceId).ToList();
            var userOrganizationUnitSetting = _customIdentityContext.UserOrganizationUnits.Where(i => i.UserId == currentUser.Id).Select(i => i.OrganizationUnitId).ToList();

            all = all.Where(i => userCostCenterSetting.Contains(i.RecruitOrder.CostCenterId) || userWorkPlaceSetting.Contains(i.RecruitOrder.WorkPlaceId.HasValue ? i.RecruitOrder.WorkPlaceId.Value : 0) || userOrganizationUnitSetting.Contains(i.RecruitOrder.OrganizationUnitId.HasValue ? i.RecruitOrder.OrganizationUnitId.Value : 0));

            all = all.OrderByDescending(i => i.LastModifiedDate);
            int rowCount = 0;

            int currentPage = req.currentPage >= 0 ? req.currentPage : 0;
            var startIndex = req.pageSize * (req.currentPage);

            if (string.IsNullOrEmpty(req.activeSortColumn))
            {
                req.activeSortColumn = "CreateDate";
                req.Sortdirection = "desc";
            }
            if (req.activeSortColumn != "")
            {
                var desc = req.Sortdirection == "asc" ? false : true;
                string command = desc ? "OrderByDescending" : "OrderBy";
                var type = typeof(InterdictOrder);
                var property = type.GetProperty(req.activeSortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                              all.Expression, Expression.Quote(orderByExpression));
                all.Provider.CreateQuery<InterdictOrder>(resultExpression);
            }

            if (!string.IsNullOrEmpty(req.filter))
            {
                var type = typeof(PayRollOrderCartableDTO);
                var predicate = PredicateBuilder.New<InterdictOrder>();
                foreach (var property in type.GetProperties())
                {
                    switch (property.Name.ToLower().Trim())
                    {
                        case "firstname":
                            predicate = predicate.Or(s => Helper.GetPropertyAsString(s.RecruitOrder.Employee, property.Name) != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.RecruitOrder.Employee, property.Name).Contains(req.filter));
                            break;
                        case "lastname":
                            predicate = predicate.Or(s => Helper.GetPropertyAsString(s.RecruitOrder.Employee, property.Name) != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.RecruitOrder.Employee, property.Name).Contains(req.filter));
                            break;
                        case "nationalno":
                            predicate = predicate.Or(s => Helper.GetPropertyAsString(s.RecruitOrder.Employee, property.Name) != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.RecruitOrder.Employee, property.Name).Contains(req.filter));
                            break;
                        case "ordertype":
                            predicate = predicate.Or(s => Helper.GetPropertyAsString(s.OrderType, "title") != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.OrderType, "title").Contains(req.filter));
                            break;
                        case "costcenter":
                            predicate = predicate.Or(s => Helper.GetPropertyAsString(s.RecruitOrder.CostCenter, "title") != null && HR.SharedKernel.Share.Helper.GetPropertyAsString(s.RecruitOrder.CostCenter, "title").Contains(req.filter));
                            break;
                    }

                }
                var AsEnumerable = all.AsEnumerable().Where(predicate);
                all = AsEnumerable.AsQueryable();
            }
            var pagedData = all
                .Skip(startIndex)
                .Take(req.pageSize);

            if (all == null)
            {
                rowCount = 0;
            }
            else
            {
                if (all.Any())
                {
                    rowCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(all.Count())));
                }
                else
                {
                    rowCount = 0;
                }
            }
            var FlatList = _mapper.Map<List<PayRollOrderCartableDTO>>(pagedData);
            return OperationResult.Succeeded(payload: FlatList, rowCount: rowCount);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطایی رخ داده");
        }
    }
    /// <summary>
    /// تایید حکم در کارتابل حقوق و دستمزد
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public OperationResult PayRollApprove(PayRollApproveDTO req)
    {
        try
        {
            if (_currentUserDefaultPaymentPeriod > 0)
            {

            }
            else
            {
                return OperationResult.Failed("دوره پیش فرض یافت نشد");
            }

            PayrollPaymentPeriodDTO payrollPaymentPeriodDTO = _unitOfWork.Context.Database.SqlQuery<PayrollPaymentPeriodDTO>($"SELECT *\r\n  FROM [Payroll].[Payment_Period]\r\n  where Id ={_currentUserDefaultPaymentPeriod}").Single();
            if (payrollPaymentPeriodDTO.IsClosed == true)
            {
                return OperationResult.Failed("دوره پیش فرض شما بسته می باشد");
            }

            if (Convert.ToDateTime(req.RealExecuteDate) == DateTime.MinValue)
            {
                return OperationResult.Failed("تاریخ اجرای ارسالی معتبر نمی باشد");
            }

            _unitOfWork.CreateTransaction();
            var interdict = _unitOfWork.Context.InterdictOrders.Include(i => i.RecruitOrder).Single(i => i.Id == req.InterdictId);
            interdict.ArearsStatusId = (long)Enums.ArearsStatus.Normal;
            List<long> validOrderStatus = new() {
            (long)Enums.OrderStatus.LastOrder,
            (long)Enums.OrderStatus.FinalOrder
            };

            var allOrders = _unitOfWork.Context.InterdictOrders.Include(i => i.RecruitOrder).Where(i => i.RecruitOrder.EmployeeId == interdict.RecruitOrder.EmployeeId && i.PayRollAprove != true && validOrderStatus.Contains(i.StatusId));

            if (allOrders.Any(i => i.StartDate <= interdict.StartDate && i.Serial < interdict.Serial))
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("لطفا احکام را به ترتیب سریال از کوچک به بزرگ تایید بفرمایید");
            }

            //PersianCalendar pc = new PersianCalendar();
            //var currentShamsiYear = pc.GetYear(DateTime.Now);

            var ExistingFiche = _unitOfWork.Context.Database.SqlQuery<long?>($"SELECT Id\r\n  FROM [Payroll].[Fiche] where [EmployeeId] = {interdict.RecruitOrder.EmployeeId} and [PaymentPeriodId] = {_currentUserDefaultPaymentPeriod}");

            if (ExistingFiche == null)
            {

            }
            else
            {
                if (ExistingFiche.Any())
                {
                    if (interdict.StartDate > payrollPaymentPeriodDTO.EndDate)
                    {

                    }
                    else
                    {
                        _unitOfWork.Rollback();
                        return OperationResult.Failed("برای کارمند انتخابی در دوره جاری فیش وجود دارد");
                    }
                }
            }

            if (Convert.ToDateTime(req.RealExecuteDate) >= interdict.StartDate)
            {
                if (Convert.ToDateTime(req.RealExecuteDate) >= payrollPaymentPeriodDTO.StartDate)
                {

                }
                else
                {
                    /// اگر تاریخ اجرای حکم و اعمال در حقوق و دستمزد هر دو قبل از تاریخ شروع دوره جاری هستند حکم مورد نظر ایجاد معوقه کرده است 
                    interdict.ArearsStatusId = (long)Enums.ArearsStatus.NeedToCalculate;
                    interdict.IsArrears = true;
                    interdict.ApproveTimePaymentPeriod = _currentUserDefaultPaymentPeriod;
                }
            }
            else
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("تاریخ اجرای ارسالی معتبر نمی باشد");
            }
            var recruit = interdict.RecruitOrder;

            List<long> validStates =
            [
                (long)Enums.OrderStatus.LastOrder,
                (long)Enums.OrderStatus.FinalOrder,
            ];

            if (validStates.Contains(interdict.StatusId))
            {

            }
            else
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("وضعیت حکم معتبر نمی باشد");
            }

            if (recruit != null)
            {
                if (recruit.PayLocationId == _currentUserDefaultOrganId)
                {
                    if (_unitOfWork.Context.Entry(interdict).GetDatabaseValues().GetValue<bool>("PayRollAprove") != true)
                    {
                        if (Convert.ToDateTime(req.RealExecuteDate) >= interdict.StartDate)
                        {
                            interdict.PayRollAprove = true;
                            interdict.PayRollAproveDate = DateTime.Now;
                            interdict.PayRollRealExecuteDate =Convert.ToDateTime(req.RealExecuteDate);
                            interdict.PayRollApproveUser = "";
                            _unitOfWork.Context.Update(interdict);
                            _unitOfWork.Context.SaveChanges();
                            _unitOfWork.Commit();
                            return OperationResult.Succeeded();
                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("تاریخ اجرای ارسالی معتبر نمی باشد");
                        }
                    }
                }
            }
            _unitOfWork.Rollback();
            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed("خطای بحرانی ره داده است");
        }
    }

    /// <summary>
    /// گرفتن فهرست احکام کارکنان
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public List<SP_GetOrderListByEmployeeID_Result> GetOrderList(GetOrderListByEmployeeIdRequest req, bool isFromProfile = false)
    {
        List<SP_GetOrderListByEmployeeID_Result> ret = new List<SP_GetOrderListByEmployeeID_Result>();
        using (SqlConnection con = new SqlConnection(_connectionString))
        {
            req.PageNo = req.PageNo + 1;
            SqlCommand cmd = new SqlCommand("[Order].[SP_GetOrderListByEmployeeID]", con);
            cmd.Parameters.AddWithValue("@employeeID", req.EmployeeId);
            cmd.Parameters.AddWithValue("@CurrentUserId", req.CurrentUserId);
            cmd.Parameters.AddWithValue("@PageNo", req.PageNo);
            cmd.Parameters.AddWithValue("@PageSize", req.PageSize);
            cmd.Parameters.AddWithValue("@PaylocationList", "");
            cmd.Parameters.AddWithValue("@SortColumn", req.SortColumn);
            cmd.Parameters.AddWithValue("@SortOrder", req.SortOrder);
            cmd.Parameters.AddWithValue("@isFromProfile", isFromProfile);

            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                ret.Add(rdr.ConvertToObject<SP_GetOrderListByEmployeeID_Result>());

            }
            con.Close();
        }
        return ret;
    }
    /// <summary>
    /// تایید حکم بدون گردش کار از فرم مدیریتی
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public OperationResult FinalApproveOrder(long Id)
    {
        try
        {
            var InterdictOrder = Get(Id).Payload;
            if (InterdictOrder.StatusId != (long)Enums.OrderStatus.Pending)
            {
                return OperationResult.Failed("وضعیت حکم مورد نظر در حال بررسی کارگزینی نیست");
            }

            if (OrderSerialSequenceHelper.HasSmallerPendingOrder(
                    InterdictOrder.EmployeeId,
                    (short)(InterdictOrder.Serial ?? 0),
                    Id,
                    _unitOfWork.Context))
            {
                return OperationResult.Failed(OrderSerialSequenceHelper.ApproveOutOfOrderMessage);
            }

            List<SP_GetOrderListByEmployeeID_Result> orderList = GetOrderList(new GetOrderListByEmployeeIdRequest()
            {
                EmployeeId = InterdictOrder.EmployeeId,
                CurrentUserId = _userService.GetUserId(),
                PageNo = 0,
                PageSize = 1000,
                SortColumn = "",
                SortOrder = ""
            });




            if (orderList.Count == 1)
            {
                if (orderList.SingleOrDefault().StatusId == (long)Enums.OrderStatus.Pending)
                {
                    try
                    {
                        _unitOfWork.CreateTransaction();
                        var toApproveinterdict = _db.Set<InterdictOrder>().Find(Id);
                        toApproveinterdict.StatusId = (long)Enums.OrderStatus.FinalOrder;

                        _db.Set<InterdictOrder>().Update(toApproveinterdict);
                        _unitOfWork.Context.SaveChanges();
                        _unitOfWork.Commit();
                    }
                    catch (Exception)
                    {
                        _unitOfWork.Rollback();
                        throw;
                    }
                }
            }
            else
            {
                if (orderList.Count() > 1)
                {


                    if (orderList.Any(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder))
                    {
                        _unitOfWork.CreateTransaction();
                        var toApproveinterdict = _db.Set<InterdictOrder>().Find(Id);
                        if (orderList.Any(i => i.StatusId == (long)Enums.OrderStatus.Draft))
                        {
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("فرد مورد نظر حکم پیش نویس دارد ابتدا آن را تعیین وضعیت بفرمایید");
                        }
                        if (orderList.Any(i => i.OrderSerial < toApproveinterdict.Serial && i.StartDate.Value.Date > toApproveinterdict.StartDate.Value.Date))
                        {
                            toApproveinterdict.StatusId = (long)Enums.OrderStatus.FinalAprove;
                        }
                        else
                        {
                            toApproveinterdict.StatusId = (long)Enums.OrderStatus.FinalOrder;

                        }
                        var previewsFinalOrder = _db.Set<InterdictOrder>().Find(orderList.Single(i => i.StatusId == (long)Enums.OrderStatus.FinalOrder).Id);
                        if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Cancelation)
                        {
                            var toCancelInterdict = _db.Set<InterdictOrder>().Find(toApproveinterdict.CorrectedInterdictOrderId);
                            if (toCancelInterdict.StatusId == (long)Enums.OrderStatus.FinalOrder)
                            {
                                if (orderList.Any(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                                {
                                    foreach (var finalApproveOrder in orderList.Where(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                                    {
                                        InterdictOrder finalAppOrder = _db.Set<InterdictOrder>().Find(finalApproveOrder.Id);
                                        finalAppOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                                        _db.Set<InterdictOrder>().Update(finalAppOrder);
                                    }
                                }
                            }
                            toCancelInterdict.StatusId = (long)Enums.OrderStatus.CanceledOrder;
                            _db.Set<InterdictOrder>().Update(toCancelInterdict);
                        }
                        if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Correction)
                        {
                            var toCorrectInterdict = _db.Set<InterdictOrder>().Find(toApproveinterdict.CorrectedInterdictOrderId);
                            if (toCorrectInterdict.StatusId == (long)Enums.OrderStatus.FinalOrder)
                            {
                                if (orderList.Any(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                                {
                                    foreach (var finalApproveOrder in orderList.Where(i => i.StartDate.Value.Date <= toApproveinterdict.StartDate.Value.Date && i.StatusId == (long)Enums.OrderStatus.FinalAprove))
                                    {
                                        InterdictOrder finalAppOrder = _db.Set<InterdictOrder>().Find(finalApproveOrder.Id);
                                        finalAppOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                                        _db.Set<InterdictOrder>().Update(finalAppOrder);
                                    }
                                }
                            }

                            toCorrectInterdict.StatusId = (long)Enums.OrderStatus.CorrectedOrder;
                            _db.Set<InterdictOrder>().Update(toCorrectInterdict);



                        }
                        if (toApproveinterdict.CorrectedInterdictOrderId == previewsFinalOrder.Id)
                        {

                        }
                        else
                        {
                            if (orderList.Any(i => i.OrderSerial < toApproveinterdict.Serial && i.StartDate > toApproveinterdict.StartDate))
                            {


                            }
                            else
                            {
                                previewsFinalOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                                _db.Set<InterdictOrder>().Update(previewsFinalOrder);
                            }

                        }

                        if (toApproveinterdict.IssueTypeId == (long)Enums.IssueType.Normal)
                        {
                            if (toApproveinterdict.StatusId == (long)Enums.OrderStatus.FinalOrder)
                            {
                                previewsFinalOrder.StatusId = (long)Enums.OrderStatus.LastOrder;
                                _db.Set<InterdictOrder>().Update(previewsFinalOrder);
                            }
                        }

                        _db.Set<InterdictOrder>().Update(toApproveinterdict);

                        _unitOfWork.Context.SaveChanges();
                        _unitOfWork.Commit();
                    }
                }

            }
        }
        catch (Exception ex)
        {
            return OperationResult.Failed();
        }

        return OperationResult.Succeeded();

    }
    /// <summary>
    /// اعتبار سنجی سمت سرور و محاسبه مجدد و دخیره حکم
    /// </summary>
    /// <returns></returns>



    public OperationResult FinalApproveOrderAll()
    {
        try
        {
            IList<InterdictOrder> allPendingOrders = All()
                .Include(i => i.RecruitOrder)
                .Where(i => i.StatusId == (long)Enums.OrderStatus.Pending)
                .OrderBy(i => i.RecruitOrder!.EmployeeId)
                .ThenBy(i => i.Serial)
                .ToList();
            foreach (var item in allPendingOrders)
            {
                try
                {
                    var resp = FinalApproveOrder(item.Id);
                }
                catch (Exception ex)
                {
                }
            }
            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed("خطای سیستمی");
        }
    }
    public bool Validate(InterdictOrder entity, object etc = null)
    {
        return true;
    }
}
