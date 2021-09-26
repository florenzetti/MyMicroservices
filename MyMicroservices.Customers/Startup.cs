using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MyMicroservices.Customers.Dto;
using MyMicroservices.Customers.Infrastructure;
using MyMicroservices.Customers.Models;
using MyMicroservices.Customers.Services;
using System.Linq;
using System.Text.Json.Serialization;

namespace MyMicroservices.Customers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddDbContext<CustomersContext>(options =>
            {
                options.UseSqlServer(Configuration.GetValue<string>("ConnectionString"));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Customers", Version = "v1" });
            });

            //services
            services.AddSingleton<IHashService, HashService>();
            services.AddSingleton(o =>
            {
                var configuration = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Customer, CustomerDto>()
                    .ForMember(dest => dest.CreditCardIds, options => options.MapFrom(source => source.CreditCards.Select(o => o.Id)));
                    cfg.CreateMap<CustomerDto, Customer>()
                    .ForMember(dest => dest.Id, options => options.Ignore());
                });

                return configuration.CreateMapper();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //ensure that database is created
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider
                .GetRequiredService<CustomersContext>()
                .Database.EnsureCreated();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyMicroservices.Customers v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
