using System;
using System.Text;
using System.Threading.Tasks;
using Application.Activities;
using Application.Interfaces;
using Application.Profiles;
using API.Middleware;
using API.SignalR;
using AutoMapper;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
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

        public void ConfigureDevelopmentServices(IServiceCollection services) {
            services.AddDbContext<DataContext>(opt => {
                opt.UseLazyLoadingProxies();
                opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });

            ConfigureServices(services);

        }

        public void ConfigureProductionServices(IServiceCollection services) {
            services.AddDbContext<DataContext>(opt => {
                opt.UseLazyLoadingProxies();
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            });

            ConfigureServices(services);

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy => {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("WWW-Authenticate")
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials();
                });
            });
            services.AddMediatR(typeof(List.Handler).Assembly);
            //inject AutoMapper into Application project and tell it where the assemblies are located
            services.AddAutoMapper(typeof(List.Handler));
            services.AddSignalR();
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

            services.AddAuthorization(opt => {
                opt.AddPolicy("IsActivityHost", policy => {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });

            //Add AuthHandler for Lifetime of operation and not the entirety of request
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

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
                    ValidateIssuer = false,
                    //Check to see if Token is expired
                    ValidateLifetime = true,
                    //If token expires this will trigger after one min and give user 401 unauthorized.
                    ClockSkew = TimeSpan.Zero
                    };
                    //Do something with the Token when we receive it
                    opt.Events = new JwtBearerEvents {

                        //Specific Event OnMessagedReceived
                        OnMessageReceived = context => {
                            //get accessToken
                            var accessToken = context.Request.Query["access_token"];
                            //get path
                            var path = context.HttpContext.Request.Path;

                            //Make sure we have access token and the path starts with /chat
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat"))) {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            //**END: Identity Service Config**

            //Inject IJwt where we need it in application within certain classes. 
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            //Get username out of token and use the class anywhere we need it inside the app
            services.AddScoped<IUserAccessor, UserAccessor>();
            //Inject this into the application where we need to
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();

            services.AddScoped<IProfileReader, ProfileReader>();

            //Configuration is being injected into the Startup constructor
            //We can use Config to get the user-secrets - set the type in the class
            //specify the section of where in the user-secrets Cloudinary:ApiKey
            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            if (env.IsDevelopment()) {
                //app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            //**** ORDER MATTERS ****** Microsoft Standard

            //NWebsec.AspNetCore.Middleware - SECURITY STUFF!
            //prevent content sniffing
            app.UseXContentTypeOptions();
            //restrict the amount of information to other sites when referring othrer sites
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            //Protects from cross scripting attacks
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            //blocks iframes and click jacking attacks
            app.UseXfo(opt => opt.Deny());
            app.UseCsp(opt => opt
                .BlockAllMixedContent()
                .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com", "sha256-F4GpCPyRepgP5znjMD8sc7PEjzet5Eef4r09dEGPpTs="))
                .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com").CustomSources("blob:", "data:"))
                .ScriptSources(s => s.Self().CustomSources("sha256-ma5XxS1EBgt17N22Qq31rOxxRWRfzUTQS1KOtfYwuNo="))
            );

            //looks for any files called index.html
            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                //letting our application know that when we reach this /chat endpoint
                // it will map to the SignalR hub
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}