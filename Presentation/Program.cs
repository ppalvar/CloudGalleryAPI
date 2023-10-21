using Application.ExternalServices.Interfaces;
using Application.Pipelines;
using Application.Services.Interfaces;
using Infraestructure.DbContexts;
using Infraestructure.ExternalServices.Local;
using Infraestructure.ExternalServices.Mocks;
using Infraestructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CloudGalleryDbContext>(
    opt => opt.UseMySQL(builder.Configuration.GetConnectionString("MySql")!,
    opt => opt.MigrationsAssembly(nameof(Presentation)))
);

builder.Services.AddAutoMapper(
    AppDomain.CurrentDomain.GetAssemblies()
);

{
    builder.Services.AddScoped<IVirusScannerService, VirusScannerServiceMock>();
    builder.Services.AddScoped<IThumbnailGenerationService, ThumbnailGeneratorServiceMock>();
    builder.Services.AddScoped<IFileEncryptService, FileEncryptServiceMock>();
    builder.Services.AddScoped<IPhotoStorageService, PhotoStorageServiceLocal>();
}

builder.Services.AddScoped<PhotoProcessingPipeline>();

builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
