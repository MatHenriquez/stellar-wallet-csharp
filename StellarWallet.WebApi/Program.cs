using StellarWallet.Application.Interfaces;
using StellarWallet.Application.Mappers;
using StellarWallet.Application.Services;
using StellarWallet.Domain.Repositories;
using StellarWallet.Infrastructure.DatabaseConnection;
using StellarWallet.Infrastructure.Repositories;
using StellarWallet.Infrastructure.Stellar;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContext>(); // Add DatabaseContext
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Add UserRepository
builder.Services.AddScoped<IUserService, UserService>(); // Add UserService
builder.Services.AddScoped<IBlockchainService, Stellar>(); // Add BlockchainService
builder.Services.AddScoped<ITransactionService, TransactionService>(); // Add TransactionService
builder.Services.AddScoped<IAuthService, AuthService>(); // Add AuthService
builder.Services.AddAutoMapper(typeof(AutoMapperProfile)); // Add AutoMapper

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowBlazor");

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
