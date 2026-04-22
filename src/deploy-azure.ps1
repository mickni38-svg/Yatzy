# =====================================================
# Deploy script – bygger Angular + .NET og deployer til Azure
# Kør fra: C:\Users\Michael\Documents\MINEPROJEKTER\Yatzy\src\
# Brug:    .\deploy-azure.ps1
# =====================================================

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

Write-Host "Trin 1/4 - Bygger Angular (production)..." -ForegroundColor Cyan
Set-Location "$root\yatzy-web"
npx ng build --configuration production
if ($LASTEXITCODE -ne 0) { Write-Host "Angular build fejlede!" -ForegroundColor Red; exit 1 }

Write-Host "Trin 2/4 - Kopierer Angular til wwwroot..." -ForegroundColor Cyan
$src = "$root\yatzy-web\dist\yatzy-web\browser"
$dst = "$root\Yatzy.Api\wwwroot"
if (Test-Path $dst) { Remove-Item $dst -Recurse -Force }
Copy-Item $src $dst -Recurse

Write-Host "Trin 3/4 - Bygger og publisher .NET API..." -ForegroundColor Cyan
Set-Location "$root\Yatzy.Api"
dotnet publish -c Release -r win-x86 --self-contained true -o "./publish-output"
if ($LASTEXITCODE -ne 0) { Write-Host ".NET build fejlede!" -ForegroundColor Red; exit 1 }

Write-Host "Trin 4/4 - Pakker og deployer til Azure..." -ForegroundColor Cyan
Compress-Archive -Path "./publish-output/*" -DestinationPath "./deploy.zip" -Force
az webapp deploy --resource-group yatzy-rg --name yatzy --src-path "./deploy.zip" --type zip

Write-Host ""
Write-Host "Deploy faerdig!" -ForegroundColor Green
Write-Host "https://yatzy-hzdhdjhmbeg7c3gj.denmarkeast-01.azurewebsites.net" -ForegroundColor Green
