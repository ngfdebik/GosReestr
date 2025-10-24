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
    TokenResponse GenerateTokens(string userId, string username, List<string> roles);
    ClaimsPrincipal ValidateToken(string token);
    //string GenerateRefreshToken();
    RefreshTokenData ValidateRefreshToken(string refreshToken);
    TokenResponse RefreshTokens(string accessToken, string refreshToken, List<string> roles);
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

    public TokenResponse GenerateTokens(string userId, string username, List<string> roles)
    {
        var accessToken = GenerateAccessToken(userId, username, roles);
        var refreshToken = GenerateRefreshToken(userId);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            TokenType = "Bearer"
        };
    }

    private string GenerateAccessToken(string userId, string username, List<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(_jwtSecret);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Добавляем роли в claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
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
        var expiresAt = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiryDays);

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

    public TokenResponse RefreshTokens(string accessToken, string refreshToken, List<string> roles)
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

        var accessTokenUserId = GetUserIdFromClaims(accessTokenPrincipal.Claims);
        var username = GetUsernameFromClaims(accessTokenPrincipal.Claims);

        if (accessTokenUserId != refreshTokenData.UserId)
        {
            throw new SecurityTokenException("Token mismatch");
        }

        if (string.IsNullOrEmpty(username))
        {
            throw new SecurityTokenException("Invalid access token");
        }

        // Теперь передаем roles в GenerateTokens
        return GenerateTokens(refreshTokenData.UserId, username, roles);
    }

    private string GetUserIdFromClaims(IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
            ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
            ?? claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }

    private string GetUsernameFromClaims(IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value
            ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
            ?? claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value
            ?? claims.FirstOrDefault(c => c.Type == "username")?.Value
            ?? claims.FirstOrDefault(c => c.Type == "user")?.Value
            ?? claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
    }
}