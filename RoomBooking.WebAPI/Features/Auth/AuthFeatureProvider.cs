using System.Text;
using RoomBooking.Data;
using RoomBooking.ServiceDefaults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RoomBooking.Data.Entities.Users;
using RoomBooking.Data.Entities.Roles;
using RoomBooking.WebAPI.Features.Auth.Options;
using RoomBooking.WebAPI.Features.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;
using RoomBooking.WebAPI.Features.Auth.Handlers.RegisterUser;
using RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;
using RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;

namespace RoomBooking.WebAPI.Features.Auth;

public sealed class AuthFeatureProvider : FeatureProvider
{
    #region Base
    public override void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddOptionsWithValidateOnStart<JwtOptions>().BindConfiguration(JwtOptions.SectionName);
        builder.Services.AddOptionsWithValidateOnStart<RefreshTokenOptions>().BindConfiguration(RefreshTokenOptions.SectionName);
        IConfigurationSection jwt = builder.Configuration.GetRequiredSection(JwtOptions.SectionName);

        builder.Services.AddIdentityCore<UserEntity>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = AuthConstraints.PasswordMinLength;
        }).AddRoles<RoleEntity>().AddSignInManager().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            string jwtKey = jwt[nameof(JwtOptions.Key)]!;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt[nameof(JwtOptions.Issuer)],
                ValidateAudience = true,
                ValidAudience = jwt[nameof(JwtOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        builder.Services.AddAuthorization();

        builder.Services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
    }
    public override void UseMiddleware(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
    public override void MapEndpoints(WebApplication app)
    {
        app.MapRegisterUserEndpoint();
        app.MapLoginUserEndpoint();
        app.MapRefreshAccessTokenEndpoint();
        if(app.Environment.IsEnvironment(Environments2.Testing))
        {
            app.MapGenerateTokensEndpoint();
        }
    }
    #endregion
}