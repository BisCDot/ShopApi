using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Helpers;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.Model.Repository;
using Shopping_Cart_Api.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

namespace Shopping_Cart_Api
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
            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["ConnectionStrings:Redis"];
                options.InstanceName = "SampleInstance";
            });
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<ICartService, CartService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IProductService, ProductService>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration["JwtIssuerOptions:Issuer"],

                ValidateAudience = true,
                ValidAudience = Configuration["JwtIssuerOptions:Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtIssuerOptions:Key"])),
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            //add jwt bearer token autentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.ClaimsIssuer = Configuration["JwtIssuerOptions:Issuer"];
                options.TokenValidationParameters = tokenValidationParameters;
                options.SaveToken = true;
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(Constants.AdministratorRole), policy => policy.RequireClaim(JwtRegisteredClaimNames.Nonce, Constants.AdministratorRole));
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(nameof(Constants.SimpleUser), policy => policy.RequireClaim(JwtRegisteredClaimNames.Nonce, Constants.SimpleUser));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop Api", Version = "v1" });
            });
            services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:5001").AllowAnyMethod().AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            //create roles needed for application

            EnsureRolesAsync(roleManager).Wait();

            //Create an account and make it administrator
            AssignAdminRole(userManager).Wait();

            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
            app.UseCors("ApiCorsPolicy");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var alreadyExistsAdmin = await roleManager.RoleExistsAsync(Constants.AdministratorRole);
            var alradyExistsUser = await roleManager.RoleExistsAsync(Constants.SimpleUser);
            if (alreadyExistsAdmin) return;
            else await roleManager.CreateAsync(new IdentityRole(Constants.AdministratorRole));
            if (alradyExistsUser)
            {
                return;
            }
            else
            {
                await roleManager.CreateAsync(new IdentityRole(Constants.SimpleUser));
            }
        }

        public static async Task AssignAdminRole(UserManager<ApplicationUser> userManager)
        {
            var testAdmin = await userManager.Users.Where(x => x.UserName == "Admin").SingleOrDefaultAsync();
            if (testAdmin == null)
            {
                testAdmin = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com"
                };

                await userManager.CreateAsync(testAdmin, "Admin@123");
            }
            else
            {
                var isAdmin = await userManager.IsInRoleAsync(testAdmin, Constants.AdministratorRole);
                if (!isAdmin) await userManager.AddToRoleAsync(testAdmin, Constants.AdministratorRole);
            }
        }
    }
}