namespace liftmarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jj : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Rcts", "UrunId_Id", "dbo.Uruns");
            DropForeignKey("dbo.Rcts", "Urun_Id", "dbo.Uruns");
            DropIndex("dbo.Rcts", new[] { "Urun_Id" });
            DropIndex("dbo.Rcts", new[] { "UrunId_Id" });
            RenameColumn(table: "dbo.Rcts", name: "Urun_Id", newName: "UrunId");
            AlterColumn("dbo.Rcts", "UrunId", c => c.Int(nullable: false));
            CreateIndex("dbo.Rcts", "UrunId");
            AddForeignKey("dbo.Rcts", "UrunId", "dbo.Uruns", "Id", cascadeDelete: true);
            DropColumn("dbo.Rcts", "UrunId_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rcts", "UrunId_Id", c => c.Int());
            DropForeignKey("dbo.Rcts", "UrunId", "dbo.Uruns");
            DropIndex("dbo.Rcts", new[] { "UrunId" });
            AlterColumn("dbo.Rcts", "UrunId", c => c.Int());
            RenameColumn(table: "dbo.Rcts", name: "UrunId", newName: "Urun_Id");
            CreateIndex("dbo.Rcts", "UrunId_Id");
            CreateIndex("dbo.Rcts", "Urun_Id");
            AddForeignKey("dbo.Rcts", "Urun_Id", "dbo.Uruns", "Id");
            AddForeignKey("dbo.Rcts", "UrunId_Id", "dbo.Uruns", "Id");
        }
    }
}
