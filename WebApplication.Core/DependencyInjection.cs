using System;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication.Core.Common.Behaviours;
using WebApplication.Core.Common.CustomProblemDetails;
using WebApplication.Core.Common.Exceptions;
using WebApplication.Infrastructure;

namespace WebApplication.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddMediatR(new[] { typeof(DependencyInjection).Assembly }, cfg => cfg.AsScoped());
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            services.AddInfrastructureServices();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));

            services.AddProblemDetails(
                options =>
                {
                    options.IncludeExceptionDetails = (context, ex) => env.IsDevelopment();
                    options.Map<ValidationException>(ex => new BadRequestProblemDetails(ex));
                    options.Map<InvalidOperationException>(ex => new BadRequestProblemDetails(ex));
                    options.Map<ArgumentOutOfRangeException>(ex => new BadRequestProblemDetails(ex));
                    options.Map<NotFoundException>(ex => new NotFoundProblemDetails(ex));
                    options.Map<Exception>(ex => new UnhandledExceptionProblemDetails(ex));
                }
            );

            return services;
        }
    }
}
