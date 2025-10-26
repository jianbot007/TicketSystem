using Application.Contracts.Interfaces;
using Application.Services;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Persistence;
using Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<SearchService>(); 
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await SeedData.InitializeAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
