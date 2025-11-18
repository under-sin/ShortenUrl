using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace ShortenUrl.Infra.Migrations;

public static class DatabaseMigration
{
    public static void Migrate(string connectionString, IServiceCollection services)
    {
        EnsureDatabaseExists(connectionString);
        MigrateDatabase(services);
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is null or empty.", nameof(connectionString));

        var builder = new MySqlConnectionStringBuilder(connectionString);
        var database = builder.Database;

        builder.Database = database;
        builder.Remove("Database");


        using var dbConnection = new MySqlConnection(builder.ConnectionString);

        var parameters = new DynamicParameters();
        parameters.Add("dbName", database);

        var records = dbConnection.Query("SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @dbname", parameters);

        if (!records.Any())
        {
            dbConnection.Execute($"CREATE DATABASE {database}");
        }
    }

    private static void MigrateDatabase(IServiceCollection services)
    {
        var runner = services.BuildServiceProvider()
            .GetRequiredService<FluentMigrator.Runner.IMigrationRunner>();
        
        runner.ListMigrations();
        runner.MigrateUp();
    }
}