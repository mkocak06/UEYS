# Modules
* API: Rest api project
* Core: Contains classes and interfaces
* Application: Contains sevices
* Infrastructure: Contains database and external service codes

# Database
PostgreSQL database is used in the project. Database connection information is stored in appsettings.

# Authentication
Project is using JWT based authentication scheme. There is JWTTokenService class that generates token.
When you enabled JWT token you should also enable authentication, also if you have any problem on authentication or any other middleware, you can see detailed logs by enabling trace log level in the development configuration json file.

# Authorization
We are using permission based authorization system. Koru project provides necessary infrastructure for authorization



# Publish
First run following command to get a release

`dotnet publish -c Release`

* Create a migration sql file by following command and run on postgre

`dotnet ef migrations script --idempotent --output "script.sql"`

* Create specified port publish

`ExecStart=/usr/bin/dotnet /var/www/boilerplate/API.dll --urls http://localhost:${portNumber}`
       
or set the program.cs file like down
```
webBuilder.UseUrls("http://localhost:${portNumber}");
webBuilder.UseKestrel();
```
* In Ubuntu 16.04
	
`vi /etc/systemd/system/kestrel-boilerplate.service`

Put 

```
[Unit]
Description=Boilerplate App

[Service]
WorkingDirectory=/var/www/boilerplate
ExecStart=/usr/bin/dotnet /var/www/boilerplate/API.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-boilerplate
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```
Run the following command

`systemctl enable kestrel-boilerplate.service`


# Migrations
adding migrations in multiple project solutions, run following command on solution directory
`dotnet ef migrations add InitialCreate -s API -p Infrastructure --context ApplicationDbContext`

To update database run the following command on solution directory
 `dotnet ef database update -s API -p Infrastructure --context ApplicationDbContext`


 To undo migration run the following command on the solution directory
`dotnet ef migrations remove -s API -p Infrastructure --context ApplicationDbContext`



To undo db update run the following command on the solution directory, with the last migration intended to be active
`dotnet ef database update MigrationClassNameToBeActive -s API -p Infrastructure --context ApplicationDbContext`


# Various Problems and Solutions
NETSDK 1004 dependency error
## Solution:
run dotnet restore