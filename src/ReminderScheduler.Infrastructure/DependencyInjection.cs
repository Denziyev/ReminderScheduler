using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReminderScheduler.Application.Services.Abstract;
using ReminderScheduler.Application.Services.Concrete;
using ReminderScheduler.Application.Validators;
using ReminderScheduler.Domain.Repositories;
using ReminderScheduler.Infrastructure.DataAccess.Contexts;
using ReminderScheduler.Infrastructure.Mappings;
using ReminderScheduler.Infrastructure.Repositories;
using ReminderScheduler.Infrastructure.Services.Concrete;
using Telegram.Bot;
using ReminderScheduler.Domain.Entities;

namespace ReminderScheduler.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure DbContext with PostgreSQL
            services.AddDbContext<ReminderSchedulerApiDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IReminderRepository, ReminderRepository>();

            services.AddScoped<IEmailSender, EmailSender>();
            // Register ITelegramBotClient with DI
            services.AddSingleton<ITelegramBotClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var botToken = configuration["Telegram:BotToken"];
                return new TelegramBotClient(botToken);
            });

            // Register TelegramSender with DI
            services.AddScoped<ITelegramSender, TelegramSender>();

            //services.AddScoped<IUserService, UserService>();

            services.AddHostedService<ReminderBackgroundService>();



            // Register AutoMapper and add profiles
            services.AddAutoMapper(typeof(MappingProfile));


            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));

            // Inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            // Configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Add the processing strategy
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            // the clientId/clientIp resolvers use it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMemoryCache();

            return services;
        }
    }
}
