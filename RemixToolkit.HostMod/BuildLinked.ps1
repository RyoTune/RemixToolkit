# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/RemixToolkit.HostMod/*" -Force -Recurse
dotnet publish "./RemixToolkit.HostMod.csproj" -c Release -o "$env:RELOADEDIIMODS/RemixToolkit.HostMod" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location