using FilesStorageApplication.Blob;
using FilesStorageApplication.StorageService;
using FilesStorageApplication.StorageServiceFactory;
using FilesStorageApplicationContract.Blob;
using FilesStorageDomain.Interfaces;
using FilesStorageEntityFramework;
using FilesStorageEntityFramework.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<FilesStorageEntityFrameworkDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
           .AddEntityFrameworkStores<FilesStorageEntityFrameworkDBContext>()
           .AddDefaultTokenProviders();


// Register storage services
builder.Services.AddScoped<IBlobRepository, BlobRepository>();
builder.Services.AddScoped<IBlobContentRepository, BlobContentRepository>();

builder.Services.AddScoped<IStorageService, S3StorageService>();

builder.Services.AddScoped<IStorageService, DatabaseStorageService>();

builder.Services.AddScoped<IStorageService, LocalFileSystemStorageService>();

builder.Services.AddScoped<IStorageService, FTPStorageService>();

builder.Services.AddScoped<S3StorageService>();
builder.Services.AddScoped<DatabaseStorageService>();
builder.Services.AddScoped<LocalFileSystemStorageService>();
builder.Services.AddScoped<FTPStorageService>();

builder.Services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();

builder.Services.AddScoped<IBlobService, BlobService>();



// Configure JWT authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();


// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the security scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Apply the security scheme globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                }
            });
});


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


