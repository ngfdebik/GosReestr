// Services/JwtService.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using AuthTest;
public interface IJwtService
{
    TokenResponse GenerateTokens(string userId, string username);
    ClaimsPrincipal ValidateToken(string token);
    //string GenerateRefreshToken();
    RefreshTokenData ValidateRefreshToken(string refreshToken);
    TokenResponse RefreshTokens(string accessToken, string refreshToken);
}

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly byte[] _jwtSecret;
    private readonly byte[] _refreshSecret;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;

        // Инициализация ключей
        _jwtSecret = GetValidKey(_jwtSettings.Key);
        _refreshSecret = GetValidKey(_jwtSettings.RefreshKey);
    }

    private byte[] GetValidKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            // Генерация случайного ключа
            var randomKey = new byte[32];
            RandomNumberGenerator.Fill(randomKey);
            return randomKey;
        }

        var keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length < 32)
        {
            // Дополнение ключа до 32 байт
            var newKey = new byte[32];
            Array.Copy(keyBytes, newKey, Math.Min(keyBytes.Length, 32));
            return newKey;
        }

        return keyBytes;
    }

    public TokenResponse GenerateTokens(string userId, string username)
    {
        var accessToken = GenerateAccessToken(userId, username);
        var refreshToken = GenerateRefreshToken(userId);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            TokenType = "Bearer"
        };
    }

    private string GenerateAccessToken(string userId, string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(_jwtSecret);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken(string userId)
    {
        var tokenId = Guid.NewGuid().ToString();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

        var claims = new[]
        {
            new Claim("user_id", userId),
            new Claim("token_id", tokenId),
            new Claim("expires_at", expiresAt.ToString("O")),
            new Claim("type", "refresh")
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(_refreshSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // Остальные методы остаются без изменений...
    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(_jwtSecret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    public RefreshTokenData ValidateRefreshToken(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(_refreshSecret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out _);

            var userId = principal.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            var tokenId = principal.Claims.FirstOrDefault(c => c.Type == "token_id")?.Value;
            var expiresAtStr = principal.Claims.FirstOrDefault(c => c.Type == "expires_at")?.Value;
            var type = principal.Claims.FirstOrDefault(c => c.Type == "type")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenId) ||
                type != "refresh" || !DateTime.TryParse(expiresAtStr, out var expiresAt))
            {
                return null;
            }

            return new RefreshTokenData
            {
                UserId = userId,
                TokenId = tokenId,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch
        {
            return null;
        }
    }

    public TokenResponse RefreshTokens(string accessToken, string refreshToken)
    {
        var refreshTokenData = ValidateRefreshToken(refreshToken);
        if (refreshTokenData == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        var accessTokenPrincipal = ValidateToken(accessToken);
        if (accessTokenPrincipal == null)
        {
            throw new SecurityTokenException("Invalid access token");
        }

        // Используем утилитарные методы
        var accessTokenUserId = ClaimUtils.GetUserId(accessTokenPrincipal.Claims);
        var username = ClaimUtils.GetUsername(accessTokenPrincipal.Claims);

        if (accessTokenUserId != refreshTokenData.UserId)
        {
            throw new SecurityTokenException("Token mismatch");
        }

        if (string.IsNullOrEmpty(username))
        {
            throw new SecurityTokenException("Invalid access token");
        }

        return GenerateTokens(refreshTokenData.UserId, username);
    }
}