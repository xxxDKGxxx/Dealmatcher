namespace Dealmatcher.Backend.API.Configurations;

public static class AuthenticationConfigs
{
    public static IServiceCollection AddAuthenticationConfigs(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["Authentication:Jwt:SecretKey"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = config["Authentication:Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Authentication:Jwt:Audience"],
                    ValidateLifetime = true,
                };
            });
        services.AddAuthorization();
        return services;
    }
}
