# Service Management
Install and remove Windows services for [.NET](https://dotnet.microsoft.com) and [Node.js](https://nodejs.org) web applications, using [NSSM](https://nssm.cc):

```powershell
# Create a new NSSM service for the application located in the current working directory.
New-NssmService

# Create a new NSSM service for the application located in the specified directory.
New-NssmService -Path "C:\Projects\MyWebApplication"

# Use the specified credentials as the logon account.
New-NssmService -Credential "DOMAIN\UserName"

# Start the newly created service immediately.
New-NssmService -Start

# Remove the NSSM service of the application located in the current working directory.
Remove-NssmService

# Remove the NSSM service of the application located in the specified directory.
Remove-NssmService -Path "C:\Projects\MyWebApplication"
```
