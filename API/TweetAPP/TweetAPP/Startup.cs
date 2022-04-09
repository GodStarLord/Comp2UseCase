using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TweetAPP.Exceptions;
using TweetAPP.Models;
using TweetAPP.Repositories;
using TweetAPP.Service;

namespace TweetAPP
{
    public class Startup
    { 
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure DB Connection
            services.AddDbContext<TweetDBContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("ConnectionKey"));
            }, ServiceLifetime.Transient);

            services.AddTransient<ITweetAppService, TweetAppService>();
            services.AddTransient<ITweetRepository, TweetRepository>();

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options =>
               options.AllowAnyOrigin()
                      .AllowAnyMethod()
                     .AllowAnyHeader());
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Title = "Tweet App",
                    Version = "V1",
                    Description = ".NET Core API for Tweet App"
                });
            });

            services.AddMvc(
                config =>
                {
                    config.Filters.Add(typeof(CustomFilter));
                });

            // Add/Configure JWT Bearer in middleware
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                var key = Configuration["AppSettings:Token"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowOrigin");
            app.UseAuthorization();

            // Custom Middleware
            // Add Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "Tweet App API");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
