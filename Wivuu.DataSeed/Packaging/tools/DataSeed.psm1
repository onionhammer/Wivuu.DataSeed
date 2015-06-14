<# 
.MODULE FUNCTIONS

.EXAMPLE
	Enable-Data-Migrations
	# Creates a new empty migration, extending from InitialDataMigration class

.EXAMPLE
	Add-Data-Migration(name)
	# Creates a stub migration class, extending from DataMigration<T>

#>
function Enable-Data-Migrations() {
	Write-Host "Enabling data-migrations..."
	Add-Migration "dataseed-migration"
}

function Add-Data-Migration() {
	param (
        [parameter(Position = 0, Mandatory = $true)]
        [string] $Name
	)

	Write-Host "Adding data-migration", $Name, "..."
}

Export-ModuleMember Enable-Data-Migrations, Add-Data-Migration