namespace TestApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TicketModels", "Time", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TicketModels", "Time", c => c.DateTime(nullable: false));
        }
    }
}
