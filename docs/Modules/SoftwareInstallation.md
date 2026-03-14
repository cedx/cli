# Software Installation

## Microsoft Build of OpenJDK
Downloads and installs the [Microsoft Build of OpenJDK](https://www.microsoft.com/openjdk) into the specified folder:

```powershell
# Install OpenJDK to the default location.
Install-Jdk

# Install OpenJDK to a custom location.
Install-Jdk -Path "C:\Program Files\OpenJDK"

# Installs a specific OpenJDK release instead of the latest version.
Install-Jdk -Version 25
```

## Node.js
Downloads and installs the latest [Node.js](https://nodejs.org) release into the specified folder:

```powershell
# Install Node.js to the default location.
Install-Node

# Install Node.js to a custom location.
Install-Node -Path "C:\Program Files\Node.js"
```

## PHP
Downloads and installs the latest [PHP](https://www.php.net) release into the specified folder:

```powershell
# Install PHP to the default location.
Install-Php

# Install PHP to a custom location.
Install-Php -Path "C:\Program Files\PHP"
```

> [!NOTE]
> This command only supports the Windows platform.
