namespace Atos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_in_Event_field_IsConfirmed : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "IsConfirmed", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "IsConfirmed", c => c.Boolean(nullable: false));
        }
    }
}
