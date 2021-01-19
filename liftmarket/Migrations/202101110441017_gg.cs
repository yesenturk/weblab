namespace liftmarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Siparislers", "OdendiMi", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Siparislers", "OdendiMi");
        }
    }
}
