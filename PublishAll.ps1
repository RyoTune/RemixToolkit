# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

./Publish.ps1 -ProjectPath "RemixToolkit.Reloaded/RemixToolkit.Reloaded.csproj" `
              -PackageName "RemixToolkit.Reloaded" `
              -PublishOutputDir "Publish/ToUpload/RemixToolkit" `

./Publish.ps1 -ProjectPath "RemixToolkit.HostMod/RemixToolkit.HostMod.csproj" `
              -PackageName "RemixToolkit.HostMod" `
              -PublishOutputDir "Publish/ToUpload/HostMod" `

Pop-Location