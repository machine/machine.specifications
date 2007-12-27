import System
import Machine.Migrations

class ThirdMigration(SimpleMigration):
  def Up():
    Log.Info("Up")
	
  def Down():
    Log.Info("Down")
