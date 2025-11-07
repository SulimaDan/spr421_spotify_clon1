using spr421_spotify_clone.BLL.Dtos.Genre;
using spr421_spotify_clone.DAL.Entities;

namespace spr421_spotify_clone.BLL.Dtos.Track
{
    public class TrackDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? AudioUrl { get; set; }
        public string? PosterUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public GenreDto? Genre { get; set; }
    }
}
