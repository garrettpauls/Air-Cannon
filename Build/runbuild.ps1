cls

Import-Module '.\Tools\PSake\psake.psm1'
Invoke-psake '.\build.ps1' Test -framework 4.0
Remove-Module psake