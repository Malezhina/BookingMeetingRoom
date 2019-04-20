namespace Atos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_in_Event_field_NameUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "NameUser", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "NameUser");
        }
    }
}
