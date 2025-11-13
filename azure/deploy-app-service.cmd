@echo on
SETLOCAL ENABLEDELAYEDEXPANSION

set RESOURCE_GROUP=%1
set ENVIRONMENT=%2
set LOCATION=%3
set PARAM_FILE=app-service.%ENVIRONMENT%.parameters.json


call az group create --name "%RESOURCE_GROUP%" --location "%LOCATION%"
if %ERRORLEVEL% NEQ 0 (
    echo Error creating resource group.
    pause
    exit /b 1
)

call az deployment group create --resource-group "%RESOURCE_GROUP%"  --template-file app-service.bicep --parameters @"%PARAM_FILE%" --verbose

ENDLOCAL
