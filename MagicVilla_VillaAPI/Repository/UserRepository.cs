using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MagicVilla_VillaAPI;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MappingConfig _mapper;
        private string secretKey;

        public UserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager, MappingConfig mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u =>
                       u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user == null || !isValid)
            {
                return new TokenDTO()
                {
                    AccessToken = ""
                };
            }

            //if user was found generate JWT Token
            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await getAccessToken(user, jwtTokenId);
            var refreshToken = await createNewRefreshToken(user.Id, jwtTokenId);

            TokenDTO tokenDto = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return tokenDto;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.UserName,
                Email = registerationRequestDTO.UserName,
                NormalizedEmail = registerationRequestDTO.UserName.ToUpper(),
                Name = registerationRequestDTO.Name
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    const string customerRole = "customer";
                    if (!await _roleManager.RoleExistsAsync(customerRole))
                    {
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole(customerRole));
                        if (!roleResult.Succeeded)
                        {
                            await _userManager.DeleteAsync(user);
                            return null;
                        }
                    }
                    var assignmentResult = await _userManager.AddToRoleAsync(user, customerRole);
                    if (!assignmentResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);
                        return null;
                    }
                    return _mapper.ToUserDTO(user);
                }
            }
            catch (Exception e)
            {

            }

            return new UserDTO();
        }

        public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO)
        {
            if (tokenDTO == null || string.IsNullOrWhiteSpace(tokenDTO.AccessToken) || string.IsNullOrWhiteSpace(tokenDTO.RefreshToken)) return new TokenDTO();

            // Find an existing refresh token
            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var existingRefreshToken = await _db.RefreshTokens.FromSqlInterpolated(
                $"SELECT * FROM [RefreshTokens] WITH (UPDLOCK, HOLDLOCK) WHERE [Refresh_Token] = {tokenDTO.RefreshToken}")
                .SingleOrDefaultAsync();
            if (existingRefreshToken == null)
            {
                return new TokenDTO();
            }

            // Compare data from existing refresh and access token provided and if there is any missmatch then consider it as a fraud
            var isTokenValid = getAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid) return new TokenDTO();

            // When someone tries to use not valid refresh token, fraud possible
            if (!existingRefreshToken.IsValid)
            {
                //var chainRecords = _db.RefreshTokens.Where(u => u.UserId == existingRefreshToken.UserId
                //&& u.JwtTokenId == existingRefreshToken.JwtTokenId);

                //foreach (var item in chainRecords)
                //{
                //    item.IsValid = false;
                //}
                //_db.UpdateRange(chainRecords);
                //_db.SaveChanges();

                await markAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
                await transaction.CommitAsync();

                return new TokenDTO();
            }

            // If just expired then mark as invalid and return empty
            if (existingRefreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                await markTokenAsInvalid(existingRefreshToken);
                await transaction.CommitAsync();
                return new TokenDTO();
            }

            var applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == existingRefreshToken.UserId);
            if (applicationUser == null) return new TokenDTO();
            // Consume once in the database before issuing a replacement. A crash fails closed and requires login.
            var now = DateTime.UtcNow;
            var consumed = await _db.RefreshTokens.Where(t => t.Id == existingRefreshToken.Id && t.IsValid && t.ExpiresAt > now)
                .ExecuteUpdateAsync(update => update.SetProperty(t => t.IsValid, false));
            if (consumed != 1)
            {
                await markAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
                await transaction.CommitAsync();
                return new TokenDTO();
            }
            var access = await getAccessToken(applicationUser, existingRefreshToken.JwtTokenId);
            var refresh = await createNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            await transaction.CommitAsync();
            return new TokenDTO { AccessToken = access, RefreshToken = refresh };
        }

        public async Task RevokeRefreshToken(TokenDTO tokenDTO)
        {
            if (tokenDTO == null || string.IsNullOrWhiteSpace(tokenDTO.AccessToken) || string.IsNullOrWhiteSpace(tokenDTO.RefreshToken)) return;

            await using var transaction = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var existingRefreshToken = await _db.RefreshTokens.FromSqlInterpolated(
                $"SELECT * FROM [RefreshTokens] WITH (UPDLOCK, HOLDLOCK) WHERE [Refresh_Token] = {tokenDTO.RefreshToken}")
                .SingleOrDefaultAsync();
            if (existingRefreshToken == null)
                return;

            // Compare data from existing refresh and access token provided and
            // if there is any missmatch then we should do nothing with refresh token
            var isTokenValid = getAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                return;
            }

            await markAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            await transaction.CommitAsync();
        }


        private async Task<string> getAccessToken(ApplicationUser user, string jwtTokenId)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Aud, "hesamshoushtarian.com")
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = "https://magicvilla-api.com",
                Audience = "https://test-magic-api.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }

        private bool getAccessTokenData(string accessToken, string expectedUserId, string expectedTokenId)
        {
            return MagicVilla_VillaAPI.Security.TokenPairValidator.Matches(accessToken, expectedUserId, expectedTokenId, secretKey);
        }

        private async Task<string> createNewRefreshToken(string userId, string tokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = tokenId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }

        private async Task markAllTokenInChainAsInvalid(string userId, string tokenId)
        {
            await _db.RefreshTokens.Where(u => u.UserId == userId && u.JwtTokenId == tokenId)
                  .ExecuteUpdateAsync(u => u.SetProperty(refreshToken => refreshToken.IsValid, false));
        }

        private Task markTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            return _db.SaveChangesAsync();
        }
    }
}
