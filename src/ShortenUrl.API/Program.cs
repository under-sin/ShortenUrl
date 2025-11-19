using ShortenUrl.Application;
using ShortenUrl.Infra;
using ShortenUrl.Infra.Migrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Não executa migrations em ambiente de testes
if (!app.Environment.IsEnvironment("Testing"))
{
    MigrateDatabase();
}

app.Run();

void MigrateDatabase()
{   
    var connectionString = builder.Configuration.GetConnectionString("Connection");
    
    DatabaseMigration.Migrate(connectionString!, builder.Services);
}

// Torna a classe Program acessível para testes de integração
public partial class Program { }
