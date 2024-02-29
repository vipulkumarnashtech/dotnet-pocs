using Play.Catalogue.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDB;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddMongo()
            .AddMongoRepository<Item>("item")
            .AddMassTransitWithRabbitMq();

        builder.Services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });
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
}