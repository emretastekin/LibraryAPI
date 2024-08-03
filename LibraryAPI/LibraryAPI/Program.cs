﻿using System.Text;
using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LibraryAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ApplicationContext _context;
            RoleManager<IdentityRole> _roleManager;
            UserManager<ApplicationUser> _userManager;
            IdentityRole identityRole;
            ApplicationUser applicationUser;
            Language language;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            builder.Services.AddAuthorization(options => options.AddPolicy("Makale", policy => policy.RequireClaim("Category", "Makale")));
            builder.Services.AddScoped<AuthorService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            

            var configuration = builder.Configuration;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>

            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero // Token ömrü bittiğinde hemen geçerliliği yitirir
                };
            });

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });

                // Güvenlik tanımlamaları
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
            { 
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
            }
            });

                
            });


            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }



            app.MapControllers();


            var scope=app.Services.CreateScope();
            _context=scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            _roleManager= scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager =scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            _context.Database.Migrate();

            if (_roleManager.FindByNameAsync("Admin").Result == null)
            {
                identityRole = new IdentityRole("Admin");
                _roleManager.CreateAsync(identityRole).Wait();
            }

            if (_userManager.FindByNameAsync("Admin").Result == null)
            {
                applicationUser = new ApplicationUser();
                applicationUser.UserName = "Admin";
                _userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                _userManager.AddToRoleAsync(applicationUser, "Admin").Wait();
            }

            if (_context.Languages!.Find("tur") == null)
            {
                language = new Language();
                language.Code = "tur";
                language.Name = "Türkçe";
                _context.Languages.Add(language);
                _context.SaveChanges();
            }

            

            app.Run();
        }
    }
}
