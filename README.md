# AppReminder

**Publish Notes**
```bash
dotnet publish AppWatcherService/AppWatcherService.csproj -c Release -r win-x64 --self-contained true --source "https://api.nuget.org/v3/index.json" -o ./publish/AppWatcherService
dotnet publish ShowMessageApp/ShowMessageApp.csproj -c Release -r win-x64 --self-contained true --source "https://api.nuget.org/v3/index.json" -o ./publish/ShowMessageApp
```

**Installation Notes**
1. Download installer.zip.
2. Open Command Prompt as Administrator.
3. Navigate to the AppWatcherService folder.
4. Install the Windows Service:
```bash
sc create AppWatcherService binPath= "C:\full\path\to\AppWatcherService\AppWatcherService.exe"
```

5. Update appsettings.json. Ensure The path to ShowMessageApp.exe is correct. Update nlog.config if needed.
6.Start the service:
```bash
sc start AppWatcherService
```

**Way to test WMI**

Open a command prompt as Administrator and run:
```bash
sc query winmgmt
```

If it's not running, start it with
```bash
net start winmgmt
```

Test WMI with WBEMTest:

    Press Win + R, type wbemtest, and press Enter.
    Click Connect, use root\cimv2, and click Connect.
    Click Query, and try a simple query like: SELECT * FROM Win32_Process

To test event-based WMI queries, you can use PowerShell (Admin) instead:
```bash
Register-WmiEvent -Query "SELECT * FROM Win32_ProcessStartTrace" -Action {
    if ($Event.SourceEventArgs.NewEvent.ProcessName -eq "EXCEL.EXE") {
        Write-Host "Excel started!"
    }
}
```


**Uninstallation Notes**
```bash
sc stop AppWatcherService
sc delete AppWatcherService
```
