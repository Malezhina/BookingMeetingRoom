namespace Atos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_NumberOfSeats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MeetingRooms", "NumberOfSeats", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MeetingRooms", "NumberOfSeats");
        }
    }
}
