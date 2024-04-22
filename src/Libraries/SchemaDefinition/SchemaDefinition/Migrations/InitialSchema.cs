using FluentMigrator;
using System.Data;

namespace SchemaDefinition.Migrations
{
    /// <summary>
    /// The Initial Schema for the Database
    /// </summary>
    [GemstoneMigration(author: "C. Lackner", branchNumber: 0, year: 2024, month: 04, day: 22)]
    public class InitialSchema : Migration
    {
        public override void Down()
        {
            Delete.Table("Device");
        }

        public override void Up()
        {
            Create.Table("Device")
                .WithColumn("ID").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("ParentID").AsInt32().Nullable().ForeignKey("Device", "ID")
                .WithColumn("UniqueID").AsString().Nullable()
                .WithColumn("Acronym").AsString(200).NotNullable()
                .WithColumn("Name").AsString(200).Nullable()
                .WithColumn("OriginalSource").AsString(200).Nullable()
                .WithColumn("IsConcentrator").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("CompanyID").AsInt32().Nullable()
                .WithColumn("HistorianID").AsInt32().Nullable()
                .WithColumn("AccessID").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("VendorDeviceID").AsInt32().Nullable()
                .WithColumn("ProtocolID").AsInt32().Nullable()
                .WithColumn("Longitude").AsDecimal(9, 6).Nullable()
                .WithColumn("Latitude").AsDecimal(9, 6).Nullable()
                .WithColumn("InterconnectionID").AsInt32().Nullable()
                .WithColumn("ConnectionString").AsString().Nullable()
                .WithColumn("TimeZone").AsString(200).Nullable()
                .WithColumn("LoadOrder").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("Enabled").AsBoolean().NotNullable().WithDefaultValue(false)
                .WithColumn("CreatedOn").AsDateTime().NotNullable().WithDefaultValue(System.DateTime.Now)
                .WithColumn("CreatedBy").AsString(200).NotNullable().WithDefaultValue("")
                .WithColumn("UpdatedOn").AsDateTime().NotNullable().WithDefaultValue(System.DateTime.Now)
                .WithColumn("UpdatedBy").AsString(200).NotNullable().WithDefaultValue("");
                 
            Create.UniqueConstraint("IX_Device_UniqueID").OnTable("Device").Column("UniqueID");
            Create.UniqueConstraint("IX_Device_NodeID_Acronym").OnTable("Device").Columns("Acronym");
        }
    }
}