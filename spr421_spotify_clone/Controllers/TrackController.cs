using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using spr421_spotify_clone.BLL.Dtos.Track;
using spr421_spotify_clone.BLL.Services.Track;
using spr421_spotify_clone.DAL.Settings;
using spr421_spotify_clone.Extensions;

namespace spr421_spotify_clone.Controllers
{
    [ApiController]
    [Route("api/track")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleSettings.RoleAdmin)]
    public class TrackController : ControllerBase
    {
        private readonly ITrackService _trackService;
        private readonly IWebHostEnvironment _environment;

        public TrackController(ITrackService trackService, IWebHostEnvironment environment)
        {
            _trackService = trackService;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateTrackDto dto)
        {
            var rootPath = _environment.ContentRootPath;
            var audioPath = Path.Combine(rootPath, "storage", "audio");
            var imagesPath = Path.Combine(rootPath, "storage", "images");

            var response = await _trackService.CreateAsync(dto, audioPath, imagesPath);
            return this.ToActionResult(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsync()
        {
            var response = await _trackService.GetAllAsync();
            return this.ToActionResult(response);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveAsync([FromQuery] string id)
        {
            var rootPath = _environment.ContentRootPath;
            var audioPath = Path.Combine(rootPath, "storage", "audio");
            var response = await _trackService.RemoveAsync(id, audioPath);
            return this.ToActionResult(response);
        }
    }
}
