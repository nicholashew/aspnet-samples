# ASP.NET Core Middleware IP Restriction

Web application to demonstrate limit access to Web App by IP address by Middleware

## Setup

Configure the whitelisted IP addresses in the appsettings.json by adding a new section called “IpSecuritySettings”:

```
"IpSecuritySettings": {
  "AllowedIPs": "::1,0.0.0.0,127.0.0.1" // comma-delimited list of whitelisted IP addresses, seperator ',' or ';'
}
```

