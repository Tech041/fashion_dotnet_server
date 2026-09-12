using EcommerceServer.Dtos;
using EcommerceServer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceServer.Controllers
{
    [Route("api/visit")]
    [ApiController]
    public class VisitorTracker(IVisitorTracker visitorService): ControllerBase
    {
        [HttpPost("track/{visitorId}")]

        public async Task<ActionResult> TrackVisitor([FromRoute] string visitorId)
        {
            try
            {
                await visitorService.TrackVisitorAsync(visitorId);
                
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<VisitorStat>> GetVisitorStats([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                var stats = await visitorService.GetVisitorStatsAsync(page, limit);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
