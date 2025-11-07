using spr421_spotify_clone.DAL.Entities;

namespace spr421_spotify_clone.DAL.Repositories.Track
{
    public class TrackRepository 
        : GenericRepository<TrackEntity>, ITrackRepository
    {
        public TrackRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<TrackEntity> Tracks => GetAll();
    }
}
