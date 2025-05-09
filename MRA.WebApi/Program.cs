using MRA.WebApi.Startup;
using MRA.DependencyInjection;
using MRA.DependencyInjection.Startup;
using MRA.DTO.Enums.Drawing;
using MRA.DTO.Enums;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddCustomLogging(builder.Configuration, builder.Environment);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddCustomAppSettingsFiles(builder.Environment.EnvironmentName, builder.Environment.IsDevelopment())
    .AddEnvironmentVariables();

if (builder.Environment.IsProduction())
{
    builder.Configuration.ConfigureKeyVault(builder.Configuration);
    builder.Logging.AddAzureWebAppDiagnostics();
}

builder.Services.AddLogging();
builder.Services.AddDependencyInjectionServices(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCorsPolicies();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new EnumStringJsonConverter<DrawingFilterTypes>());
        options.JsonSerializerOptions.Converters.Add(new EnumStringJsonConverter<DrawingPaperSizes>());
        options.JsonSerializerOptions.Converters.Add(new EnumStringJsonConverter<DrawingProductTypes>());
        options.JsonSerializerOptions.Converters.Add(new EnumStringJsonConverter<DrawingSoftwares>());
        options.JsonSerializerOptions.Converters.Add(new EnumStringJsonConverter<DrawingTypes>());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
