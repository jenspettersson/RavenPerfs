Powershell script to run until it throws:

```
$statusCode = 200;
$elapsed = [System.Diagnostics.Stopwatch]::StartNew()
while($statusCode -eq 200){
    $appPool = "RAVENBUGS"
    Write-Host $elapsed.Elapsed
    Write-Host "Restarting application pool $appPool"
    Restart-WebAppPool $appPool
    Write-Host "Restarted!"
    Start-Sleep -m 200

    Try
    {
        $uri = "http://localhost/Raven.Metrics.Bug/api/values";
        Write-Host "Invoking: $uri"
        $r = Invoke-WebRequest -URI $uri  
        $statusCode = $r.StatusCode
    }
    Catch{
        Write-Host $_.Exception -ForegroundColor Red
        Break;
    }
}
```