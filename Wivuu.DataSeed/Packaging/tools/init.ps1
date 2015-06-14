# Ref: http://haacked.com/archive/2011/04/19/writing-a-nuget-package-that-adds-a-command-to-the.aspx/

param($installPath, $toolsPath, $package)

Import-Module (Join-Path $toolsPath DataSeed.psm1) -DisableNameChecking