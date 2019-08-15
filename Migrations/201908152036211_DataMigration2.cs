namespace TestApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TicketModels", "OwnerID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TicketModels", "OwnerID", c => c.Int());
        }
    }
}
