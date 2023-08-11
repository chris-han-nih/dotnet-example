using logging.Middlewares;
using Masking.Serilog;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddHttpLogging(options =>
//                                 {
//                                     options.LoggingFields =
//                                         HttpLoggingFields.RequestHeaders
//                                       | HttpLoggingFields.RequestBody
//                                       | HttpLoggingFields.ResponseHeaders
//                                       | HttpLoggingFields.ResponseBody;
//                                 });

builder.Services.AddHttpContextAccessor();
builder.Services.AddHeaderPropagation(options =>
                                      {
                                          options.Headers.Add("X-Correlation-ID");
                                          options.Headers.Add("X-Request-ID");
                                      });
var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("Application", "Payment")
            .Enrich.With(new RemovePropertiesEnricher())
             // https://github.com/serilog-contrib/Serilog.Enrichers.Sensitive
             // https://github.com/serilog-contrib/Serilog.Enrichers.Sensitive/blob/master/README.md
             // https://benfoster.io/blog/serilog-best-practices/
             // https://nblumhardt.com/2021/06/customize-serilog-json-output/
             // https://www.c-sharpcorner.com/article/logging-and-tracing-in-multiple-microservice-with-correlation-using-net-core/
             // correlationId https://mderriey.com/2016/11/18/correlation-id-with-asp-net-web-api/
            // .Enrich.WithSensitiveDataMasking()
            .Enrich.FromLogContext()
            .Destructure.ByMaskingProperties("password", "Password", "PASSWORD", "token", "Token", "TOKEN")
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
app.UseMiddleware<LoggingMiddleware>();

app.Run();
