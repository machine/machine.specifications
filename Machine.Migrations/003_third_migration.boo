import System
import Machine.Migrations

class ThirdMigration(SimpleMigration):
  def Up():
    Log.Info("Up")
    print Schema.Tables()
	
  def Down():
    Log.Info("Down")
    print Schema.Tables()
