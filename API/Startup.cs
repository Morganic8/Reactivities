using System.Text;
using Application.Activities;
using Application.Interfaces;
using API.Middleware;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<DataContext>(opt => {
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy => {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
                });
            });
            services.AddMediatR(typeof(List.Handler).Assembly);
            services.AddControllers(opt => {
                    //Every request requires a authticated user
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                    opt.Filters.Add(new AuthorizeFilter(policy));
                })
                //cfg = config
                .AddFluentValidation(cfg => {
                    //look for any validators in any specified project like Create.cs
                    //this will now look for any validators in the Application project
                    cfg.RegisterValidatorsFromAssemblyContaining<Create>();
                });

            //*** Start: Identity Service Config ***
            var builder = services.AddIdentityCore<AppUser>(); //set up user type identity
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services); //new instance
            identityBuilder.AddEntityFrameworkStores<DataContext>(); //Creates user stores
            identityBuilder.AddSignInManager<SignInManager<AppUser>>(); //creates and manages users from a service from identity core, helps with username and password

            //This key needs to be hidden better and a random string
            //in terminal: dotnet user-secrets to set/list/clear/remove/init secrets - use enviroment vairables
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
            //Identity will not run without running AddAuth()
            //JwtBearerDefaults needs nuget package above in using statements
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => {
                    //set JWT option defaults
                    opt.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                    };
                });

            //**END: Identity Service Config**

            //Inject IJwt where we need it in application within certain classes. 
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            //Get username out of token and use the class anywhere we need it inside the app
            services.AddScoped<IUserAccessor, UserAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            if (env.IsDevelopment()) {
                //app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            //**** ORDER MATTERS ****** Microsoft Standard

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}