using Microsoft.EntityFrameworkCore;
using EcommerceBackend.Application.Interfaces;
using EcommerceBackend.Infrastructure.Persistence;
using EcommerceBackend.Infrastructure.Repository;
using EcommerceBackend.Infrastructure.Email;
using EcommerceBackend.Application.Interfaces.Email;

namespace EcommerceBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext and ConnectionString
        services.AddDbContext<EcommerceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Seeding initial data.
        services.AddTransient<Seed>();

        //Email
        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.AddScoped<IEmailTemplateRenderer, MjmlHandlebarsEmailTemplateRenderer>();
        var emailProvider = configuration["Email:Provider"];
        if (string.Equals(emailProvider, "Azure", StringComparison.OrdinalIgnoreCase))
            services.AddScoped<IEmailSender, AzureEmailSender>();
        else
            services.AddScoped<IEmailSender, FilePreviewEmailSender>();

        return services;
    }
}