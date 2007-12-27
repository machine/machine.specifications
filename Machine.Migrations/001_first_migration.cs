using System;
using Machine.Migrations;

public class FirstMigration : SimpleMigration
{
  public override void Up()
  {
    Console.WriteLine(this.Schema.HasTable("Booger"));
  }

  public override void Down()
  {
  }
}