namespace spr421_spotify_clone.BLL.Settings
{
    public class JwtSettings
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? SecretKey { get; set; }
        public int ExpireHours { get; set; }
    }
}
