using Microsoft.AspNetCore.Mvc;
using CloudNativeApp.Models;
using CloudNativeApp.Services;

namespace CloudNativeApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThingsController : ControllerBase
    {
        private readonly S3StorageService s3Service;

        public ThingsController(S3StorageService s3Service)
        {
            this.s3Service = s3Service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateThing([FromBody] Thing thing)
        {
            await s3Service.SaveThingAsync(thing);
            return Ok(new { message = "Thing saved", timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetThing(string id)
        {
            var thing = await s3Service.GetThingAsync(id);
            if (thing == null)
                return NotFound();

            return Ok(new
            {
                things_stored = new Dictionary<string, object>
                {
                    { id, new { thing_attribute = thing.ThingAttribute } }
                },
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThing(string id)
        {
            await s3Service.DeleteThingAsync(id);
            return Ok(new { message = "Thing deleted", timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
        }
    }
}
