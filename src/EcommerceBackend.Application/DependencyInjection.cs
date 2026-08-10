using FluentValidation;
using FluentValidation.AspNetCore;
using EcommerceBackend.Application.Services;
using EcommerceBackend.Application.Validators.Auth;
using EcommerceBackend.Application.Interfaces.Services;
using EcommerceBackend.Application.Services.Auth;
using EcommerceBackend.Application.Interfaces.Email;
using EcommerceBackend.Application.Services.Email;

namespace EcommerceBackend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // AutoMapper + FluentValidation
        services.AddAutoMapper(cfg => { }, typeof(DependencyInjection).Assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<ITokenService, TokenService>();

        // Email
        services.AddScoped<ILoginNotificationService, LoginNotificationService>();
        return services;
    }
}