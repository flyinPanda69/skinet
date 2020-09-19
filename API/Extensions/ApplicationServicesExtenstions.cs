using System.Linq;
using API.Errors;
using Core.Interfaces;
using InfraStructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServicesExtenstions
    {
        public static IServiceCollection AddsApplicationService(this IServiceCollection services) 
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext => 
                {
                    //Inside the action context, we can get the model state errors, that is what api attribute is using to identify any error
                    //and adding it to the modelstate dictionary. 
                    var errors = actionContext.ModelState
                        .Where(e=>e.Value.Errors.Count() >0)
                        .SelectMany(x=>x.Value.Errors)
                        .Select(z=>z.ErrorMessage).ToArray();

                        var errorResponse = new ApiValidationErrorResponse{
                            Errors = errors
                        };
                        return new BadRequestObjectResult(errorResponse);
                };
            });
            return services;
        }
    }

}