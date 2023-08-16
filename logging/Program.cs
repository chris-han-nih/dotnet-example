using logging.Middlewares.Logger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.With(new RemovePropertiesEnricher())
            .Enrich.WithProperty("Application", "Payment")
            .WriteTo.Console(new NsusJsonFormatter())
            .WriteTo.File(new NsusJsonFormatter(), "../logs/.log", rollingInterval: RollingInterval.Hour)
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

// app.UseHeaderPropagation();
app.UseHttpsRedirection();

app.UseAuthorization();
// app.UserSerilogRequestLogging();

app.MapControllers();
app.UseMiddleware<LoggingMiddleware>();

app.Run();
