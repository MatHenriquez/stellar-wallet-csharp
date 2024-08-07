﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StellarWallet.Domain.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StellarWallet.Application.Services
{
    public class JwtService(IConfiguration config) : IJwtService
    {
        private readonly string secretKey = config.GetSection("Jwt").GetSection("Key").Value ?? throw new Exception("Secret key not found");
        private readonly string issuer = config.GetSection("Jwt").GetSection("Issuer").Value ?? throw new Exception("Issuer not found");
        private readonly string audience = config.GetSection("Jwt").GetSection("Audience").Value ?? throw new Exception("Audience not found");

        public string CreateToken(string email, string role)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            claims.AddClaim(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddYears(100),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokenConfig);
        }

        public string DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (secretKey == null)
                throw new Exception("Secret key not found");
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var allClaims = claimsPrincipal.Claims.ToList();
                if (allClaims.Count == 0)
                    throw new Exception("No claims found");

                var emailClaim = allClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var roleClaim = allClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                if (emailClaim == null || roleClaim == null)
                    throw new Exception("Email or role claim not found");

                return emailClaim.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error decoding JWT: {ex.Message}");
            }
        }
    }
}
