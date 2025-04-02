# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/RemixToolkit.Reloaded/*" -Force -Recurse
dotnet publish "./RemixToolkit.Reloaded.csproj" -c Release -o "$env:RELOADEDIIMODS/RemixToolkit.Reloaded" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location