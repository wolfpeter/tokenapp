using DeviceId;

namespace TokenApp.Services;

public class MainService
{
    public string GenerateDeviceId()
    {
        return new DeviceIdBuilder()
            .AddMachineName()
            .AddOsVersion()
            .OnWindows(windows => windows.AddWindowsDeviceId().AddMachineGuid())
            .ToString();
    }
}