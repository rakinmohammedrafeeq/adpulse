# AdPulse - Local Full Stack Startup Script
# Technical Interview Demonstration Launcher

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "   AdPulse - Full-Stack Advertising Intelligence        " -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host ""

# Ensure .NET 9 is on PATH
if (Test-Path "$HOME\.dotnet") {
    $env:PATH = "$HOME\.dotnet;$env:PATH"
} elseif (Test-Path "C:\Users\ASUS\.dotnet") {
    $env:PATH = "C:\Users\ASUS\.dotnet;$env:PATH"
}

# 1. Start Docker Compose Infrastructure
Write-Host "[1/4] Starting Docker infrastructure (SQL Server, Redis, Elasticsearch, Kibana, Logstash)..." -ForegroundColor Yellow
docker compose up -d

Write-Host "Waiting for database and Redis services to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# 2. Start ASP.NET Core REST API
Write-Host "[2/4] Starting ASP.NET Core API on http://localhost:5000 ..." -ForegroundColor Yellow
$apiProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "`$env:PATH = 'C:\Users\ASUS\.dotnet;`$env:PATH'; cd '$PSScriptRoot\..\Services\AdPulse.API'; dotnet run" -PassThru
Write-Host "  -> API started (PID: $($apiProcess.Id)). Swagger at http://localhost:5000/swagger" -ForegroundColor Green

# 3. Start Node.js Event Ingestion Service
Write-Host "[3/4] Starting Node.js Event Ingestion Service on http://localhost:3001 ..." -ForegroundColor Yellow
$ingestProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\..\Services\EventIngestion'; npm start" -PassThru
Write-Host "  -> Event Ingestion Service started (PID: $($ingestProcess.Id))" -ForegroundColor Green

# 4. Start Vue.js Dashboard
Write-Host "[4/4] Starting Vue 3 Dashboard on http://localhost:5173 ..." -ForegroundColor Yellow
$frontendProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\..\Frontend\adpulse-dashboard'; npm run dev" -PassThru
Write-Host "  -> Frontend Dashboard started (PID: $($frontendProcess.Id))" -ForegroundColor Green

Write-Host ""
Write-Host "========================================================" -ForegroundColor Green
Write-Host " AdPulse Platform is Running!" -ForegroundColor Green
Write-Host "========================================================" -ForegroundColor Green
Write-Host " Dashboard:       http://localhost:5173" -ForegroundColor White
Write-Host " REST API / Doc:  http://localhost:5000/swagger" -ForegroundColor White
Write-Host " Event Ingestion: http://localhost:3001" -ForegroundColor White
Write-Host " Kibana:          http://localhost:5601" -ForegroundColor White
Write-Host " Elasticsearch:   http://localhost:9200" -ForegroundColor White
Write-Host ""
Write-Host " Demo Account:    admin@acme.com / Password123!" -ForegroundColor Cyan
Write-Host " Default Tenant:  Acme Corporation" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Green
