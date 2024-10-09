using CompanyEmployees;
using CompanyEmployees.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;

var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog/config"));

NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
    .Services.BuildServiceProvider()
    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
    .OfType<NewtonsoftJsonPatchInputFormatter>().First();

// NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
// {
//     var services = new ServiceCollection()
//         .AddLogging()
//         .AddMvc()
//         .AddNewtonsoftJson();

//     using var serviceProvider = services.Services.BuildServiceProvider();
//     var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
//     return mvcOptions.InputFormatters
//         .OfType<NewtonsoftJsonPatchInputFormatter>()
//         .First();
// }

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers(config => {
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
}).AddXmlDataContractSerializerFormatters()
.AddCustomCSVFormatter()
.AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// var logger = app.Services.GetRequiredService<ILoggerManager>();
// app.ConfigureExceptionHandler(logger);
app.UseExceptionHandler(opt => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
    app.UseHsts();

// if (app.Environment.IsDevelopment())
// {
//     // app.UseSwagger();
//     // app.UseSwaggerUI();
//     app.UseDeveloperExceptionPage();
// }
// else
// {
//     app.UseHsts();
// }

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
