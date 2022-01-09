using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.http;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.EnvironmentName == "Development")
{
    Console.WriteLine("--> Using InMemoryDb");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDatabase"));
}
else if (builder.Environment.EnvironmentName == "Production")
{
    Console.WriteLine("--> Using SqlServerDb");
    var connectionString = builder.Configuration.GetConnectionString("PlatFormsConn");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
}



builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSwaggerGen();

Console.WriteLine($"--> CommandService Endpoint: {builder.Configuration["CommandService"]}");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.IsProduction())
{
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.SeedSampleData(app.Environment.IsProduction());

app.Run();