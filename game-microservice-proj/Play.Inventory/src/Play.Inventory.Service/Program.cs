using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddMongo().AddMongoRepository<InventoryItem>("inventoryitem");

        //Adding consumer for rabbitmq
        builder.Services.AddMongoRepository<CatalogItem>("catalogitem")
            .AddMassTransitWithRabbitMq();

        // AddCatalogClient(builder);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

        app.UseAuthorization();

        app.MapControllers();

        app.Run();


    }
    private static void AddCatalogClient(WebApplicationBuilder builder)
    {
        var jitterer = new Random();
        var services = builder.Services;
        builder.Services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5270");
        })
        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync
        (
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            + TimeSpan.FromMilliseconds(jitterer.NextInt64(0, 1000))
        )) //order of these policy matters
        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3,
            TimeSpan.FromSeconds(15),
            onBreak: (outcome, timespan) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning($"Opening the cirucit for {timespan.TotalSeconds} seconds...");
            },
            onReset: () =>
            {
                var serviceProvider = services.BuildServiceProvider();
                serviceProvider.GetService<ILogger<CatalogClient>>()?
                    .LogWarning($"Closing the circuit...");
            }
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1)); //handle different transient errors
    }
}