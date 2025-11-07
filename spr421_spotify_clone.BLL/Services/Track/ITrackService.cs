using spr421_spotify_clone.BLL.Dtos.Track;

namespace spr421_spotify_clone.BLL.Services.Track
{
    public interface ITrackService
    {
        Task<ServiceResponse> CreateAsync(CreateTrackDto dto, string audioFilePath, string posterFilePath);
        Task<ServiceResponse> RemoveAsync(string id, string audioFilePath);
        Task<ServiceResponse> GetAllAsync();
    }
}
