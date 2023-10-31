using Application.ExternalServices.Interfaces;
using Application.Pipelines;
using Application.Pipelines.Interfaces;
using Application.Services.Interfaces;
using FluentValidation;
using Infraestructure.DbContexts;
using Infraestructure.ExternalServices.Local;
using Infraestructure.ExternalServices.Mocks;
using Infraestructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CloudGalleryDbContext>(
    opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDB")!,
    opt => opt.MigrationsAssembly(nameof(Presentation)))
);

builder.Services.AddAutoMapper(
    AppDomain.CurrentDomain.GetAssemblies()
);

builder.Services.AddValidatorsFromAssemblyContaining<IValidatorMarker>();
builder.Services.AddScoped<PhotoUploadValidator>();
builder.Services.AddScoped<UserRegisterValidator>();

//Add Photo processing pipeline
{
    builder.Services.AddScoped<IVirusScannerService, VirusScannerServiceMock>();
    builder.Services.AddScoped<IThumbnailGenerationService, ThumbnailGeneratorServiceMock>();
    builder.Services.AddScoped<IFileEncryptService, FileEncryptServiceMock>();
    builder.Services.AddScoped<IPhotoStorageService, PhotoStorageServiceLocal>();

    builder.Services.AddScoped<IPhotoProcessingPipeline, PhotoProcessingPipeline>();
}

//Add services
{
    builder.Services.AddScoped<IPhotoService, PhotoService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    builder.Services.AddSingleton(builder.Configuration);
}

//Add JWT auth
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = true;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});

//Add swagger
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });
}

var app = builder.Build();

//this allows to use swagger in prod environment, use wisely :3
bool keepSwagger = true;
if (app.Environment.IsDevelopment() || keepSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction()) {
    try {
        using var serviceScope = app.Services.CreateScope();
        var services = serviceScope.ServiceProvider;
        var dbContext = services.GetRequiredService<CloudGalleryDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception e){
        app.Logger.LogError(e.Message);
    }
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
