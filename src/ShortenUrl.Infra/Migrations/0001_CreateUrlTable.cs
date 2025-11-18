using FluentMigrator;

namespace ShortenUrl.Infra.Migrations;

[Migration(202511170001, "Create Urls Table")]
public class CreateUrlTable_20251117_0001 : Migration
{
    private const string TableName = "Urls";

    public override void Up()
    {
        if (!Schema.Table(TableName).Exists())
        {
            Create.Table(TableName)
                .WithColumn("ShortCode").AsString(7).PrimaryKey().NotNullable()
                .WithColumn("OriginalUrl").AsString().NotNullable()
                .WithColumn("CreatedAt").AsDateTime().NotNullable();
        }
    }

    public override void Down()
    {
        if (Schema.Table(TableName).Exists())
        {
            Delete.Table(TableName);
        }
    }
}
