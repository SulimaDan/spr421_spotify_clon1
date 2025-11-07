using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using spr421_spotify_clone.BLL.Dtos.Auth;
using spr421_spotify_clone.BLL.Settings;
using spr421_spotify_clone.DAL.Entities.Identity;
using spr421_spotify_clone.DAL.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace spr421_spotify_clone.BLL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<ServiceResponse> LoginAsync(LoginDto dto)
        {
            var user = await GetByLoginAsync(dto.Login);

            if(user == null)
            {
                return new ServiceResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Логін вказано невірно"
                };
            }

            bool passwordResult = await _userManager.CheckPasswordAsync(user, dto.Password);

            if(!passwordResult)
            {
                return new ServiceResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Пароль вказано невірно"
                };
            }

            string token = await GenerateJwtTokenAsync(user);

            return new ServiceResponse
            {
                Message = "Успішний вхід",
                Payload = token
            };
        }

        private async Task<ApplicationUser?> GetByLoginAsync(string login)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == login.ToUpper()
                || u.NormalizedUserName == login.ToUpper());
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("id", user.Id!),
                new Claim("userName", user.UserName!),
                new Claim("email", user.Email!),
                new Claim("firstName", user.FirstName ?? string.Empty),
                new Claim("lastName", user.LastName ?? string.Empty),
                new Claim("avatar", user.AvatarUrl ?? string.Empty)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            
            if(userRoles.Count == 0)
            {
                userRoles.Add(RoleSettings.RoleUser);
            }

            var roleClaims = userRoles.Select(r => new Claim("roles", r));
            claims.AddRange(roleClaims);

            var secretKey = _jwtSettings.SecretKey;

            if(string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException(nameof(secretKey));
            }

            var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                audience: _jwtSettings.Audience,
                issuer: _jwtSettings.Issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpireHours),
                signingCredentials: credentials
                );

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(token);
        }
    }
}
