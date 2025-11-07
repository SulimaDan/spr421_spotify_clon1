using spr421_spotify_clone.DAL.Entities;

namespace spr421_spotify_clone.DAL.Repositories.Track
{
    public interface ITrackRepository
        : IGenericRepository<TrackEntity>
    {
        IQueryable<TrackEntity> Tracks { get; }
    }
}
