 param (
    [Parameter(Mandatory=$true)][string]$dir1,
    [Parameter(Mandatory=$true)][string]$dir2
 )

$source = Get-Content -Path "VerifyDirectories.cs"
Add-Type -TypeDefinition "$source" -Language CSharpVersion3

[VerifyDirectories]::Verify($dir1,$dir2)