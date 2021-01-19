namespace liftmarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class v : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hammaddes", "EsikStokSayisi", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hammaddes", "EsikStokSayisi");
        }
    }
}
