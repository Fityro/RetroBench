using System.Linq;
using System.Management;

namespace Aida64Clone.Models
{
    public static class HardwareMonitor
    {
        public static string GetCpuUsage()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM Win32_Processor"))
                {
                    var firstItem = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault();
                    if (firstItem != null)
                        return firstItem["LoadPercentage"].ToString();
                }
            }
            catch { }
            return "N/A";
        }

        public static string GetCpuTemperature()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT CurrentTemperature FROM Win32_TemperatureProbe"))
                {
                    var firstItem = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault();
                    if (firstItem != null)
                    {
                        var tempValue = firstItem["CurrentTemperature"] as uint?;
                        if (tempValue.HasValue)
                        {
                            // CurrentTemperature в десятых долях градусов Цельсия
                            return (tempValue.Value / 10.0).ToString();
                        }
                    }
                }
            }
            catch { }
            return "N/A";
        }
    }
}