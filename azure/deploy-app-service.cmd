@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

if "%~3"=="" (
    echo Usage: %~nx0 ^<resource-group-name^> ^<environment^> ^<location^>
    echo.
    echo    resource-group-name   Azure resource group where the template will be deployed.
    echo    environment           Deployment slot identifier \(dev, prod, etc.\).
    echo    location              Azure region for the resource group \(e.g. westus3\).
    echo.
    echo Example:
    echo    %~nx0 tl-rg-dev dev westus3
    EXIT /B 1
)

set RESOURCE_GROUP=%1
set ENVIRONMENT=%2
set LOCATION=%3
set PARAM_FILE=azure\app-service.%ENVIRONMENT%.parameters.json

if not exist "%PARAM_FILE%" (
    echo Parameter file "%PARAM_FILE%" was not found.
    EXIT /B 1
)

echo Creating or updating resource group "%RESOURCE_GROUP%" in "%LOCATION%"...
az group create --name "%RESOURCE_GROUP%" --location "%LOCATION%"

if ERRORLEVEL 1 (
    echo Failed to ensure resource group exists.
    EXIT /B 1
)

echo Deploying App Service infrastructure using %PARAM_FILE%...
az deployment group create ^
    --resource-group "%RESOURCE_GROUP%" ^
    --template-file azure\app-service.bicep ^
    --parameters @"%PARAM_FILE%"

ENDLOCAL
