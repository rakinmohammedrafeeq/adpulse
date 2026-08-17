# AdPulse - Local Full Stack Shutdown Script

Write-Host "Shutting down AdPulse local infrastructure and containers..." -ForegroundColor Yellow
docker compose down

Write-Host "Stopping any running local dotnet or node processes for AdPulse..." -ForegroundColor Yellow
Get-Process -Name "dotnet", "node" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*AdPulse*" } | Stop-Process -Force -ErrorAction SilentlyContinue

Write-Host "AdPulse stopped cleanly." -ForegroundColor Green
