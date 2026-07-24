
using APIs.ConfigClasses;
using APIs.Responses;
using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers
{
    [Authorize]
    [RequiersdClaim("UseLeadsAndConverssionAnalysisTools", SharedDto_Enum.enPlaformRoles.User)]
    [Route("api/dashboard/business-analyses")]
    [ApiController]
    public class BusinessAnalyses: ControllerBase
    {
        private readonly IDiscoveryPlatformService _discoveryPlatformService;
        private readonly IClientSubscriptionService _clientSubscriptionService;
        public BusinessAnalyses(IDiscoveryPlatformService service
,                         IClientSubscriptionService clientSubscriptionService)
        {

            _discoveryPlatformService = service;
            _clientSubscriptionService = clientSubscriptionService;
        }

      
        [HttpGet("platform-leads-counts")]
        public async Task<ActionResult<ApiResult<List<DtoPlatformCount>>>> PlatformLeadCount()
        {

           var res = await _discoveryPlatformService.PlatformsDiscoveryContrubutionAsync();

            return Ok(ApiResult<List<DtoPlatformCount>>.Ok(res));

        }

        [HttpGet("platform-coverssion-counts")]
        public async Task<ActionResult<ApiResult<List<DtoPlatformCount>>>> GetConversionRatePerPlatform()
        {

            var res = await _clientSubscriptionService.GetConversionRatePerPlatform();

            return Ok(ApiResult<List<DtoPlatformCount>>.Ok(res));

        }



    }
}
