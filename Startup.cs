using DotNetCoreBoilerPlate.Interfaces;
using DotNetCoreBoilerPlate.Middleware;
using DotNetCoreBoilerPlate.Models;
using DotNetCoreBoilerPlate.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreBoilerPlate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var DataSource = Configuration.GetConnectionString("DataSource");
            var Database = Configuration.GetConnectionString("DatabaseName");
            var UserID = Configuration.GetConnectionString("UserName");
            var Password = Configuration.GetConnectionString("PWD");
            string cnstr = "Server=" + DataSource + ";Database=" + Database + ";UID=" + UserID + ";PWD=" + Password + ";";
            services.AddDbContext<NetCoreBoilerPlateContext>(item => item.UseSqlServer(cnstr));

            services.AddScoped<ILogin, LoginRepository>();
            services.AddScoped<IUser, UserRepository>();

            services.AddMvc();
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
                });

                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
                c.DocInclusionPredicate((name, api) => true);
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                // hide swagger default header and custom options
                app.UseSwaggerUI(options =>
                {
                    options.DefaultModelsExpandDepth(-1);
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    options.InjectStylesheet("/css/swagger-custom.css");
                });
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<JWTMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    // Dynamic sorting extension class
    public static class DynamicSortingExtensions
    {
        static readonly MethodInfo s_OrderBy = typeof(Queryable).GetMethods()
            .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        static readonly MethodInfo s_OrderByDescending = typeof(Queryable).GetMethods()
            .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        static readonly MethodInfo s_ThenBy = typeof(Queryable).GetMethods()
            .Where(m => m.Name == "ThenBy" && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        static readonly MethodInfo s_ThenByDescending = typeof(Queryable).GetMethods()
            .Where(m => m.Name == "ThenByDescending" && m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName)
        {
            return BuildQuery(s_OrderBy, query, propertyName);
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName, bool isDescending)
        {
            if (isDescending)
                return BuildQuery(s_OrderByDescending, query, propertyName);
            else
                return BuildQuery(s_OrderBy, query, propertyName);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IQueryable<TSource> query, string propertyName, bool isDescending)
        {
            if (isDescending)
                return BuildQuery(s_ThenByDescending, query, propertyName);
            else
                return BuildQuery(s_ThenBy, query, propertyName);
        }

        public static IOrderedQueryable<TSource> OrderByDescending<TSource>(this IQueryable<TSource> query, string propertyName)
        {
            return BuildQuery(s_OrderByDescending, query, propertyName);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IQueryable<TSource> query, string propertyName)
        {
            return BuildQuery(s_ThenBy, query, propertyName);
        }

        public static IOrderedQueryable<TSource> ThenByDescending<TSource>(this IQueryable<TSource> query, string propertyName)
        {
            return BuildQuery(s_ThenByDescending, query, propertyName);
        }

        static IOrderedQueryable<TSource> BuildQuery<TSource>(MethodInfo method, IQueryable<TSource> query, string propertyName)
        {
            var entityType = typeof(TSource);

            var propertyInfo = entityType.GetProperty(propertyName);
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException(nameof(propertyName), "Unknown column " + propertyName);

            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            var genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

            return (IOrderedQueryable<TSource>)genericMethod.Invoke(genericMethod, new object[] { query, selector })!;
        }
    }
}
