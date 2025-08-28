using Core.Models.Identity;
using Core.Services;
using Core.Specification.Main;
using lawyer.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using Repo.Data;
using Repo.Repos;
using Serv.Services;

namespace lawyer.Api.Extensions
{
    public static class AppService
    {
        public static async Task<IServiceCollection> AddStoreServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));


            services.AddScoped<ContractFileUrlResolver>();




            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(ISpecification<>), typeof(BaseSpecification<>));
            //services.AddScoped<IBasketRepository, BasketRepository>();

            services.Configure<EmailSetting>(config.GetSection("EmailSetting"));

            services.AddScoped<IEmailSender, EmailSender>();

            services.AddScoped<IAuthService, AuthService>();

            //services.AddScoped<IPasswordResetService, PasswordResetService>();

            services.AddScoped<ITemporaryUserRepository, TemporaryUserRepository>();
            services.AddScoped<IResetPasswordTempRepository, ResetPasswordTempRepository>();


            return services;

        }
    }
}
