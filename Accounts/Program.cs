using Accounts.Common;
using Accounts.Configuration;
using Accounts.DataAccess;
using Accounts.DataAccess.Contracts;
using Accounts.Service;
using Accounts.Service.Contracts;
using AutoMapper;
using GreenPipes;
using Invocation.MassTransit.Consumer;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var environmentName = builder.Configuration.GetSection("ASPNETCORE_ENVIRONMENT").Value;

var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
var queueSettings = configuration.GetSection(Constants.QUEUE_SETTINGS);
var options = queueSettings.Get<QueueSettings>();

Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
builder.Services.AddLogging(configure => configure
    .AddSerilog()
    .AddConfiguration(configuration.GetSection("Logging")));

builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddSingleton<IQueueMessageService, QueueMessageService>();
builder.Services.AddSingleton<IAppConfigurationService, AppConfigurationService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
               .WithExposedHeaders("Accountid");
    });
});

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<TransactionCreationResponseQueue>();

    config.AddBus((sp) =>
    {
        return Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(new Uri(options.Address), h =>
            {
                h.Username(options.Username);
                h.Password(options.Password);

                //if (options.UseSSL == true)
                //{
                //    h.UseSsl(ssl =>
                //    {
                //        ssl.Protocol = System.Security.Authentication.SslProtocols.Tls12;
                //        ssl.CertificatePath = options.CertificatePath;
                //        ssl.CertificatePassphrase = options.CertificatePassphrase;
                //        ssl.ServerName = options.ServerName;
                //    });
                //}
            });
            // define retry policy
            cfg.UseMessageRetry(r =>
            {
                r.Interval(1, TimeSpan.FromSeconds(3));
            });

            cfg.ConfigureJsonDeserializer(settings =>
            {
                settings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
                return settings;
            });

            //define the receive endpoint
            cfg.ReceiveEndpoint(options.TransactionInsertQueueSettings.ResponseQueue, ep =>
            {
                //link the queue to the Consumer Class
                ep.Consumer<TransactionCreationResponseQueue>(sp);
            });
        });
    });
});

builder.Services.AddMassTransitHostedService();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new AutoMapperConfig());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
