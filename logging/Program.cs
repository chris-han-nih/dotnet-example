using logging.Middlewares;
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
             // https://github.com/serilog-contrib/Serilog.Enrichers.Sensitive
             // https://github.com/serilog-contrib/Serilog.Enrichers.Sensitive/blob/master/README.md
             // https://benfoster.io/blog/serilog-best-practices/
             // https://nblumhardt.com/2021/06/customize-serilog-json-output/
            // .Enrich.WithSensitiveDataMasking()
            .Enrich.FromLogContext()
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

app.MapControllers();
app.UseMiddleware<LoggingMiddleware>();

app.Run();
