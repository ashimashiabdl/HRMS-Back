//using AutoMapper;
//using Hr.SystemSetting.Core.DTOs;
//using Hr.SystemSetting.Infrastructure.Services;
//using HR.BaseInfo.Core.DTOs;
//using HR.Order.Core.DTOs;
//using HR.Order.Infrastructure.Services;
//using HR.SharedKernel.API;
//using HR.SharedKernel.Dapper;
//using HR.SharedKernel.DTOs;
//using Microsoft.AspNetCore.Mvc;

//namespace HRMS.API.Controllers.Order
//{
//    [Route("api/BatchRequestDetailReference")]
//[ControllerGroup("Order", " احکام")]
//    public class BatchRequestDetailReferenceController : AppBaseController
//    {
//        private readonly BatchRequestDetailReferenceService _batchRequestService;
//        public BatchRequestDetailReferenceController(BatchRequestDetailReferenceService BatchRequestDetailReferenceService, ILogger<OrderController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper) : base(logger, accessor, mapper, dapper)
//        {
//            _batchRequestService = BatchRequestDetailReferenceService;
//            _batchRequestService._IP = _Ip;
//            _batchRequestService._UserName = CurrentUserName;
//            _batchRequestService._currentUserDefaultOrganId = currentUserDefaultOrganId;
//        }
//        [HttpGet, Route("Get/{id}")]
//        
//        public IActionResult Get(int id)
//        {
//            return this.AppOk(_batchRequestService.Get(id));
//        }

//        [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
//        
//        public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true)
//        {
//            return this.AppOk(_batchRequestService.GetPagedData(currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired));
//        }
//        [HttpPost("Post")]
//        
//        public async Task<IActionResult> Post([FromBody] BatchRequestDetailReferenceDTO body)
//        {
//            if (ModelState.IsValid)
//            {
//                
//                return Ok(await _batchRequestService.CreateForAsync(body));
//            }
//            return this.AppBadRequest(ModelState);
//        }
//        [HttpPut("Put")]
//        
//        public async Task<IActionResult> Put([FromBody] BatchRequestDetailReferenceDTO body)
//        {
//            var result = await _batchRequestService.UpdateForAsync(body);
//            return this.AppOk(result);
//        }


//    }
//}
