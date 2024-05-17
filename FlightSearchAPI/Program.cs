using FlightSearchAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the HTTP clients for GOL and LATAM services
builder.Services.AddHttpClient<GolFlightService>();
builder.Services.AddHttpClient<LatamFlightService>();

// Register flight services explicitly
builder.Services.AddTransient<GolFlightService>();
builder.Services.AddTransient<LatamFlightService>();

// Register FlightAggregatorService with specific implementation
builder.Services.AddTransient<IFlightService>(serviceProvider =>
{
    var golService = serviceProvider.GetRequiredService<GolFlightService>();
    var latamService = serviceProvider.GetRequiredService<LatamFlightService>();
    return new FlightAggregatorService(new IFlightService[] { golService, latamService });
});

// Register logging services
builder.Services.AddLogging();

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
