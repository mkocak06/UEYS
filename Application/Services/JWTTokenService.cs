using Amazon.S3.Model;
using Application.Interfaces;
using Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class JWTTokenService : IJWTTokenService
    {
        private readonly TokenOptions _options;

        public JWTTokenService(TokenOptions options)
        {
            _options = options;
        }

        public TokenResponse GenerateToken(long adminId, string email)
        {
            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, adminId.ToString()),
                new Claim(ClaimTypes.Name, email),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddMinutes(_options.AccessExpiration);
            var jwtToken = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(_options.AccessExpiration),
                signingCredentials: credentials
            );
            //return new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Expiration = expiration
            };
        }
    }
}
