
using Core.Models.Identity;
using lawyer.Api.Extensions;
using lawyer.Api.Helpers;
using lawyer.Api.Helpers.Mapping;
using lawyer.Extensions;
using Mashal.Helpers.Errors;
using Mashal.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repo.Data;
using Repo.DataSeedind;
using Repo.DataSeedind.Identity;

namespace Mashal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpContextAccessor();



            builder.Services.AddIdentityServices(builder.Configuration);
            await builder.Services.AddStoreServices(builder.Configuration);


            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfiles>();
            });

            // update it with the actual frontend URL 
            // after deployment i will update it
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });






            //to read from environment variables not appsettings
            //builder.Configuration.Sources.Clear();
            //builder.Configuration
            //       .AddEnvironmentVariables();

            //DotNetEnv.Env.Load();

            //builder.Configuration
            //       .SetBasePath(Directory.GetCurrentDirectory())
            //       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) 
            //       .AddEnvironmentVariables();               









            //for validation error
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(e => e.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });





            var app = builder.Build();


            //seed data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                var seeder = new DataSeeder(context, env.WebRootPath);
                await seeder.SeedAsync();
            }


            //seeding admin and roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityDbContextSeed.SeedUserAsync(userManager, roleManager);
            }


            //migrate database
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }





            // Configure the HTTP request pipeline.


            app.UseCors("AllowAnyOrigin");
           // not sure if this is needed here or in the middleware section below


            app.UseAuthentication();
            app.UseAuthorization();




            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseStaticFiles();
            app.UseRouting();

            app.Urls.Add("http://0.0.0.0:5000");
            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
