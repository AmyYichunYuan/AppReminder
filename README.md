# AppReminder

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
    Register-WmiEvent -Query "SELECT * FROM Win32_ProcessStartTrace" -Action {
        if ($Event.SourceEventArgs.NewEvent.ProcessName -eq "EXCEL.EXE") {
            Write-Host "Excel started!"
        }
    }
