using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Smart_Learning_App.Extensions
{
    public static class CustomJwtAuth
    {
        public static void AddCustomJwtAuth ( this IServiceCollection services , ConfigurationManager configuration)
        {
            services.AddAuthentication(
                o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }
                ).AddJwtBearer(
                 o =>
                 {
                     o.RequireHttpsMetadata = false;
                     o.SaveToken = true;
                     o.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidateIssuer = true,
                         ValidIssuer = configuration["JWT:Issuer"],
                         ValidateAudience = false ,
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))

                     };
                 }
                );
        }
    }
}
