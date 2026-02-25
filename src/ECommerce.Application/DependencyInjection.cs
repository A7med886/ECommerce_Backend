using ECommerce.Application.Behaviors;
using ECommerce.Application.Mappings;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddAutoMapper(typeof(DependencyInjection).Assembly);
        //builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); });

        //builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));
        ////builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderCommandValidator).Assembly);
        //builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
        //builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Register FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        //services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();

        // Register Behaviors (ORDER MATTERS)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

        return services;
    }
}