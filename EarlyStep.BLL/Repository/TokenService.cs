using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Responses.AccountResponses;
using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EarlyStep.BLL.Repository;


public class TokenRepository : ITokenService
{
    private readonly IConfiguration configuration;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly string jwtKey;


    public TokenRepository(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager)
    {
        this.configuration = configuration;
        this.userManager = userManager;

        jwtKey = configuration["JWT:Key"]
            ?? throw new InvalidOperationException("JWT Key is missing");
    }


    public async Task<AuthResponse> CreateTokenAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);


        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),

            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)

        };


        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }


        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );


        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );


        var token = new JwtSecurityToken(
          issuer: configuration["JWT:Issuer"],
          audience: configuration["JWT:Audience"],
          claims: claims,
          expires: DateTime.UtcNow.AddDays(
              Convert.ToDouble(configuration["JWT:DurationInDays"])
          ),
          signingCredentials: credentials
      );


        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);


        return new AuthResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            AccessToken = accessToken
        };
    }
}