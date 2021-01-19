namespace liftmarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hh : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Hammaddes", "Recete_Id", "dbo.Recetes");
            DropForeignKey("dbo.Recetes", "UrunId", "dbo.Uruns");
            DropIndex("dbo.Hammaddes", new[] { "Recete_Id" });
            DropIndex("dbo.Recetes", new[] { "UrunId" });
            CreateTable(
                "dbo.Rcts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HammaddeId = c.Int(nullable: false),
                        NeKadar = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleteDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        Urun_Id = c.Int(),
                        UrunId_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hammaddes", t => t.HammaddeId, cascadeDelete: true)
                .ForeignKey("dbo.Uruns", t => t.Urun_Id)
                .ForeignKey("dbo.Uruns", t => t.UrunId_Id)
                .Index(t => t.HammaddeId)
                .Index(t => t.Urun_Id)
                .Index(t => t.UrunId_Id);
            
            DropColumn("dbo.Hammaddes", "Recete_Id");
            DropTable("dbo.Recetes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Recetes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UrunId = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeleteDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Hammaddes", "Recete_Id", c => c.Int());
            DropForeignKey("dbo.Rcts", "UrunId_Id", "dbo.Uruns");
            DropForeignKey("dbo.Rcts", "Urun_Id", "dbo.Uruns");
            DropForeignKey("dbo.Rcts", "HammaddeId", "dbo.Hammaddes");
            DropIndex("dbo.Rcts", new[] { "UrunId_Id" });
            DropIndex("dbo.Rcts", new[] { "Urun_Id" });
            DropIndex("dbo.Rcts", new[] { "HammaddeId" });
            DropTable("dbo.Rcts");
            CreateIndex("dbo.Recetes", "UrunId");
            CreateIndex("dbo.Hammaddes", "Recete_Id");
            AddForeignKey("dbo.Recetes", "UrunId", "dbo.Uruns", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Hammaddes", "Recete_Id", "dbo.Recetes", "Id");
        }
    }
}
