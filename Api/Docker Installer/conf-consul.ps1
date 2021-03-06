﻿Write-Output "Criando parâmetros de configuração no Consul..."
$Url = "http://localhost:8500/v1/kv/"
if($Args[0]) {
	$Url = $Args[0]
}
Write-Output "Usando API do consul em $Url" 
Write-Output ""

$Global = @'
{
  "loggingLevel": "Warning",
  "databaseLogging": "User ID=postgres;Password=W01fM0n1t0r;Host=10.0.75.1;Port=5432;Database=logs;",
  "healthCheckIntervalSegs": "10",
  "deregisterCriticalServiceAfterSegs": "10",
  "cors": "localhost:4200",
  "broker": {
    "hostname": "10.0.75.1",
    "exchangeName": "tottem"
  },
  "identityServerAddress": "http://10.0.75.1:16000"
}
'@

$Jobs = @'
{
  "tags": "Recurring jobs",
  "connectionString": "User ID=postgres;Password=W01fM0n1t0r;Host=10.0.75.1;Port=5433;Database=hangfire;",
  "jobs": [
    {
      "intervalMinutes": "15",      
      "targetServiceName": "Sample2",
      "messageType": "InfoMessage"
    }
  ]
}
'@

$Gateway = @'
{
  "Tags": "Gateway,Hub,Services",
    "ReRoutes": [],
    "Aggregates": [],
    "GlobalConfiguration": {
        "RequestIdKey": null,
        "ServiceDiscoveryProvider": {
            "Host": "10.0.75.1",
            "Port": 8500,
            "Type": "Consul",
            "Token": null,
            "ConfigurationKey": null
        },
        "RateLimitOptions": {
            "ClientIdHeader": "ClientId",
            "QuotaExceededMessage": null,
            "RateLimitCounterPrefix": "ocelot",
            "DisableRateLimitHeaders": false,
            "HttpStatusCode": 429
        },
        "QoSOptions": {
            "ExceptionsAllowedBeforeBreaking": 0,
            "DurationOfBreak": 0,
            "TimeoutValue": 0
        },
        "BaseUrl": null,
        "LoadBalancerOptions": {
            "Type": "LeastConnection",
            "Key": null,
            "Expiry": 0
        },
        "DownstreamScheme": "http",
        "HttpHandlerOptions": {
            "AllowAutoRedirect": false,
            "UseCookieContainer": false,
            "UseTracing": false
        }
    }
}
'@

$IdentityServer = @'
{
  "tags": "Authtentication,JWT,Token",
  "connectionString": "Data Source=localhost;Initial Catalog=AuthContext;Persist Security Info=True;Integrated Security=True;",
  "wolfMonitorConnectionString":"Data Source=localhost;Initial Catalog=WolfMonitorContext;Persist Security Info=True;Integrated Security=True;",
  "issuerUri": "http://localhost",
  "agentsApiSecret":"agentSuperSecret",
  "clients": [
    {
      "id": "postman",
      "secret": "postmanSecret",
      "name": "Postman Client",
      "scopes": [
      		"Agents", "Monitoring"
      ]
    }
  ]
}
'@

$Register = @'
{
	"Tags": "Register",
	"connectionString":"Data Source=localhost;Initial Catalog=WolfMonitorContext;Persist Security Info=True;Integrated Security=True;",
	"authConnectionString":"Data Source=localhost;Initial Catalog=AuthContext;Persist Security Info=True;Integrated Security=True;",
	"authSettings": {
		"secret": "IxrAjDoa2FqElO7IhrSrUJELhUckePEPVpaePlS_Xaw",
		"issuer": "https://localhost:9001/",
		"clientId": "099153c2625149bc8ecb3e85e03f0022"
	}
}
'@
$Agents = @'
{
  "Tags": "Agents",
  "connectionString":"Data Source=localhost;Initial Catalog=WolfMonitorContext;Persist Security Info=True;Integrated Security=True;",
  "authConnectionString":"Data Source=localhost;Initial Catalog=AuthContext;Persist Security Info=True;Integrated Security=True;",
  "monitoringConnectionString":"Data Source=localhost;Initial Catalog=WolfMonitoringContext;Persist Security Info=True;Integrated Security=True;",
  "apiName":"Agents",
  "apiSecret":"agentsSuperSecret"
}
'@

$Users = @'
{
  "Tags": "Users",
  "connectionString":"Data Source=localhost;Initial Catalog=WolfMonitorContext;Persist Security Info=True;Integrated Security=True;",
  "authConnectionString":"Data Source=localhost;Initial Catalog=AuthContext;Persist Security Info=True;Integrated Security=True;",
  "apiName":"Users",
  "apiSecret":"usersSuperSecret"
}
'@
$Monitoring = @'
{
  "Tags": "Monitoring",
  "connectionString":"Data Source=localhost;Initial Catalog=WolfMonitorContext;Persist Security Info=True;Integrated Security=True;",
  "monitoringConnectionString":"Data Source=localhost;Initial Catalog=WolfMonitoringContext;Persist Security Info=True;Integrated Security=True;",
  "authConnectionString":"Data Source=localhost;Initial Catalog=AuthContext;Persist Security Info=True;Integrated Security=True;",
  "apiName":"Monitoring",
  "apiSecret":"monitoringSuperSecret"
}
'@

$response = Invoke-RestMethod -Method 'Put' -Uri $url"Global" -Body $Global
if($response -eq 'true') {
	Write-Output "Configuração Global criada com sucesso!"
}

$response = Invoke-RestMethod -Method 'Put' -Uri $url"Jobs" -Body $Jobs
if($response -eq 'true') {
	Write-Output "Configuração Jobs criada com sucesso!"
}

$response = Invoke-RestMethod -Method 'Put' -Uri $url"Gateway" -Body $Gateway
if($response -eq 'true') {
	Write-Output "Configuração Gateway criada com sucesso!"
}

$response = Invoke-RestMethod -Method 'Put' -Uri $url"IdentityServer" -Body $IdentityServer
if($response -eq 'true') {
	Write-Output "Configuração IdentityServer criada com sucesso!"
}


$response = Invoke-RestMethod -Method 'Put' -Uri $url"Register" -Body $Register
if($response -eq 'true') {
	Write-Output "Configuração Register criada com sucesso!"
}
$response = Invoke-RestMethod -Method 'Put' -Uri $url"Agents" -Body $Agents
if($response -eq 'true') {
	Write-Output "Configuração Agents criada com sucesso!"
}
$response = Invoke-RestMethod -Method 'Put' -Uri $url"Monitoring" -Body $Monitoring
if($response -eq 'true') {
	Write-Output "Configuração Monitoring criada com sucesso!"
}
$response = Invoke-RestMethod -Method 'Put' -Uri $url"Monitoring" -Body $Users
if($response -eq 'true') {
	Write-Output "Configuração Users criada com sucesso!"
}
Write-Output ""
Write-Output "Configuração concluída com sucesso!"
Write-Output ""