using logging.Middlewares;
using logging.Middlewares.HttpHeader;
using logging.Middlewares.Logger;
using logging.Middlewares.Response;
using Masking.Serilog;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHeaderPropagation(options =>
                                      {
                                          options.Headers.Add("X-Correlation-ID");
                                          options.Headers.Add("X-Request-ID");
                                      });
var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.With(new RemovePropertiesEnricher())
            .Enrich.WithProperty("Application", "Payment")
            .Enrich.FromLogContext()
            .WriteTo.Console(new NsusJsonFormatter())
            .WriteTo.File(new NsusJsonFormatter(), "../logs/.log", rollingInterval: RollingInterval.Hour)
             // .Destructure.ByMaskingProperties("password", "Password", "PASSWORD", "token", "Token", "TOKEN")
            .CreateLogger();
builder.Logging.ClearProviders();
builder.Services.AddSerilog(logger);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHeaderPropagation();
app.UseHttpsRedirection();

app.UseAuthorization();
// app.UserSerilogRequestLogging();

app.MapControllers();
app.UseMiddleware<HeaderMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ResponseMiddleware>();

app.Run();
