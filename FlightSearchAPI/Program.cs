using FlightSearchAPI.Services;
using FlightSearchAPI.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddSingleton<FlightMappingService>();
builder.Services.AddSingleton<GolFlightService>();
builder.Services.AddSingleton<LatamFlightService>();
builder.Services.AddSingleton<IFlightService, FlightAggregatorService>(provider =>
{
    var golService = provider.GetRequiredService<GolFlightService>();
    var latamService = provider.GetRequiredService<LatamFlightService>();
    return new FlightAggregatorService(new IFlightService[] { golService, latamService });
});

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
