using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace System.Resources.Control
{
    public class Program
    {
        private readonly IController _controller;

        public Program(IController controller)
        {
            _controller = controller;
        }

        static void Main()
        {
            
            var view = new ConsoleView();
            var systemInfoService = new SystemInfoService();
            var model = new Model(systemInfoService);
            var controller = new Controller(model, view);

            var program = new Program(controller);
            program.Run();
        }

        public void Run()
        {
            bool isStopped = false;

            while (!isStopped)
            {
                try
                {
                    _controller.ShowMainScreen();
                    if (int.TryParse(Console.ReadLine(), out int option))
                    {
                        if (_controller.OptionActions.ContainsKey(option))
                        {
                            _controller.OptionActions[option]();
                        }
                        else
                        {
                            _controller.ShowInvalidOptionMessage();
                        }
                    }
                    else
                    {
                        _controller.ShowInvalidOptionMessage();
                    }

                }
                catch (Exception ex)
                {
                    _controller.ShowErrorMessage(ex.Message);
                }

                if (!isStopped)
                {
                    _controller.AskToContinue();
                    var key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.N)
                    {
                        isStopped = true;
                        _controller.ShowProgramTerminationMessage();
                    }
                    else
                    {
                        _controller.ShowRestartingMessage();
                    }
                }
            }
        }
    }

    public class Model : IModel
    {
        private readonly ISystemInfoService _systemInfoService;
        public List<PerformanceCounter> ThreadCounters { get; set; }

        public Model(ISystemInfoService systemInfoService)
        {
            _systemInfoService = systemInfoService;
           InitializeThreadCounters();
        }


        public void InitializeThreadCounters()
        {
            int threadCount = Environment.ProcessorCount;
            ThreadCounters = new List<PerformanceCounter>();

            for (int i = 0; i < threadCount; i++)
            {
                ThreadCounters.Add(new PerformanceCounter("Processor", "% Processor Time", i.ToString()));
            }
        }


        public double SimulateHeavyWorkload(int threadIndex)
        {
            double result = 0;
            const int iterations = 100_000_000;

            for (int i = 1; i <= iterations; i++)
            {
                result += Math.Sqrt(i) * Math.Sin(i);
            }

            double threadScore = result / 1_000_000;

            return threadScore;
        }


        public int CalculateScore(double threadScore, TimeSpan elapsedTime)
        {
            double baseScore = 10000.0 / (1 + threadScore / 1000.0);

            double timeBonus = 10000.0 / (elapsedTime.TotalMilliseconds + 1);

            double totalScore = baseScore + timeBonus;

            return Math.Max(0, (int)totalScore);
        }
    }
    
    public interface IModel
    {
        List<PerformanceCounter> ThreadCounters { get; set; }
        void InitializeThreadCounters();
        double SimulateHeavyWorkload(int threadIndex);
        int CalculateScore(double threadScore, TimeSpan elapsedTime);

    }


    public interface IView
    {
        void ShowMainScreen();
        void ShowInvalidOptionMessage();
        void ShowErrorMessage(string message);
        void ShowProgramTerminationMessage();
        void ShowRestartingMessage();
        void AskToContinue();
        void DisplayMessage(string message);
    }

    public class ConsoleView : IView
    {
        public void ShowMainScreen()
        {
            Console.WriteLine("------- System Resources Control -------");
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. CPU Information");
            Console.WriteLine("2. GPU Information");
            Console.WriteLine("3. RAM Information");
            Console.WriteLine("4. Disk Information");
            Console.WriteLine("5. Network Information");
            Console.WriteLine("6. System Summary");
            Console.WriteLine("7. Temperature Information");
            Console.WriteLine("8. Exit");
            Console.WriteLine("Enter your choice (1-8):");
        }

        public void ShowInvalidOptionMessage()
        {
            Console.WriteLine("Invalid option. Please choose again.");
        }

        public void ShowErrorMessage(string message)
        {
            Console.WriteLine($"Error: {message}");
        }

        public void ShowProgramTerminationMessage()
        {
            Console.WriteLine("Program terminated.");
        }

        public void ShowRestartingMessage()
        {
            Console.WriteLine("\nRestarting monitoring...\n");
        }

        public void AskToContinue()
        {
            Console.WriteLine("\nDo you want to continue? (y/n)");
        }
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }


    public class Controller : IController
    {
        private readonly IModel _model;
        private readonly IView _view;
        private readonly Dictionary<int, Action> _optionActions;
        private readonly ISystemInfoService _systemInfoService;

        public Dictionary<int, Action> OptionActions => _optionActions;

        public Controller(IModel model, IView view)
        {
            _model = model;
            _view = view;
            _systemInfoService = new SystemInfoService();
            _optionActions = new Dictionary<int, Action>
        {
            { 1, ShowCpuInformation },
            { 2, ShowGpuInformation },
            { 3, ShowRamInformation },
            { 4, ShowDiskInformation },
            { 5, ShowNetworkInformation },
            { 6, ShowSystemSummary },
            { 7, ShowTemperatureInformation },
            { 8, TerminateProgram }
        };
        }

        public void ShowMainScreen()
        {
            _view.ShowMainScreen();
        }

        public void ShowInvalidOptionMessage()
        {
            _view.ShowInvalidOptionMessage();
        }

        public void ShowErrorMessage(string message)
        {
            _view.ShowErrorMessage(message);
        }

        public void ShowProgramTerminationMessage()
        {
            _view.ShowProgramTerminationMessage();
        }

        public void ShowRestartingMessage()
        {
            _view.ShowRestartingMessage();
        }

        public void AskToContinue()
        {
            _view.AskToContinue();
        }


        private void ShowCpuInformation()
        {
            bool isStopped = false;

            while (!isStopped)
            {
                try
                {
                    _view.DisplayMessage("----- CPU Information -----");
                    _view.DisplayMessage("Select an option:");
                    _view.DisplayMessage("1. Detailed Processor Info");
                    _view.DisplayMessage("2. Monitor CPU Core Usage");
                    _view.DisplayMessage("3. Run CPU Benchmark");
                    _view.DisplayMessage("4. Go Back");


                    if (int.TryParse(Console.ReadLine(), out int option))
                    {
                        switch (option)
                        {
                            case 1:
                                DisplayDetailedCpuInfo();
                                break;
                            case 2:
                                DisplayCoreFrequency();
                                break;
                            case 3:
                                RunCpuBenchmark();
                                break;
                            case 4:
                                isStopped = true;
                                break;
                            default:
                                _view.ShowInvalidOptionMessage();
                                break;
                        }
                    }
                    else
                    {
                        _view.ShowInvalidOptionMessage();
                    }
                }
                catch (Exception ex)
                {
                    _view.ShowErrorMessage(ex.Message);
                }
            }
        }
        private void DisplayDetailedCpuInfo()
        {
            try
            {
                var cpuInfo = _systemInfoService.GetDetailedCpuInfo();
                _view.DisplayMessage("\n--- Processor Information ---");
                _view.DisplayMessage($"Processor Name: {cpuInfo.Name}");
                _view.DisplayMessage($"Manufacturer: {cpuInfo.Manufacturer}");
                _view.DisplayMessage($"Number of Cores: {cpuInfo.NumberOfCores}");
                _view.DisplayMessage($"Number of Logical Processors: {cpuInfo.NumberOfLogicalProcessors}");
                _view.DisplayMessage($"Max Clock Speed: {cpuInfo.MaxClockSpeed} MHz");
                _view.DisplayMessage($"Current Clock Speed: {cpuInfo.CurrentClockSpeed} MHz");
                _view.DisplayMessage($"Processor ID: {cpuInfo.ProcessorId}");
                _view.DisplayMessage($"L2 Cache Size: {cpuInfo.L2CacheSize} KB");
                _view.DisplayMessage($"L3 Cache Size: {cpuInfo.L3CacheSize} KB");
                _view.DisplayMessage($"Architecture: {cpuInfo.Architecture}");
                _view.DisplayMessage($"Processor Type: {cpuInfo.ProcessorType}");
                _view.DisplayMessage($"Status: {cpuInfo.Status}");
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Error fetching CPU info: {ex.Message}");
            }
        }
        
        private void DisplayCoreFrequency()
        {
            int threadCount = Environment.ProcessorCount;
            PerformanceCounter[] threadCounters = new PerformanceCounter[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                threadCounters[i] = new PerformanceCounter("Processor", "% Processor Time", i.ToString());
            }

            Console.Clear();
            _view.DisplayMessage("--- CPU core usage monitoring ---");
            _view.DisplayMessage("Press 'q' to stop monitoring.\n");

            while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Q)
            {
                Console.Clear();
                _view.DisplayMessage("--- CPU core usage monitoring ---");
                _view.DisplayMessage("Press 'q' to stop monitoring.\n");

                for (int i = 0; i < threadCount; i++)
                {
                    _view.DisplayMessage($"Thread {i}: {threadCounters[i].NextValue():F2}%");
                }

                Thread.Sleep(500);
            }

            foreach (var counter in threadCounters)
            {
                counter.Dispose();
            }
        }

        private void RunCpuBenchmark()
        {
            _view.DisplayMessage("\n--- CPU Benchmark ---");
            _view.DisplayMessage("Simulating heavy workload on all cores and threads...\n");

            int threadCount = Environment.ProcessorCount;
            double[] threadScores = new double[threadCount];
            object lockObject = new object();

            Stopwatch stopwatch = Stopwatch.StartNew();

            Parallel.For(0, threadCount, i =>
            {
                threadScores[i] = _model.SimulateHeavyWorkload(i);
                  _view.DisplayMessage($"Thread {i} completed with score {threadScores[i]:F2}");
            });

            stopwatch.Stop();

            double totalScore = 0;
            foreach (var score in threadScores)
            {
                lock (lockObject)
                {
                    totalScore += _model.CalculateScore(score, stopwatch.Elapsed);
                }
            }
            
            _view.DisplayMessage($"\nBenchmark completed in {stopwatch.ElapsedMilliseconds} ms.");
            _view.DisplayMessage($"Total Score: {totalScore:F2} points");
        }
    
        private void ShowGpuInformation()
        {
            try
            {
                var gpuInfos = _systemInfoService.GetGpuInfo();

                foreach (var gpuInfo in gpuInfos)
                {
                    _view.DisplayMessage("\n--- GPU Information ---");
                    _view.DisplayMessage($"GPU Name: {gpuInfo.Name}");
                    _view.DisplayMessage($"Adapter RAM: {gpuInfo.AdapterRAM}");
                    _view.DisplayMessage($"Driver Version: {gpuInfo.DriverVersion}");
                    _view.DisplayMessage($"Video Processor: {gpuInfo.VideoProcessor}");
                }
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Error fetching GPU info: {ex.Message}");
            }
        }
        
        private void ShowRamInformation()
        {
            try
            {
                var ramInfos = _systemInfoService.GetRamInfo();

                _view.DisplayMessage("\n--- RAM Information ---");
                _view.DisplayMessage($"Total Installed RAM: {ramInfos.TotalCapacity} GB\n");

                int moduleNumber = 1;
                foreach (var ramInfo in ramInfos.Modules)
                {
                    _view.DisplayMessage($"Module {moduleNumber++}:");
                    _view.DisplayMessage($"  Capacity: {ramInfo.Capacity} GB");
                    _view.DisplayMessage($"  Manufacturer: {ramInfo.Manufacturer}");
                    _view.DisplayMessage($"  Speed: {ramInfo.Speed} MHz");
                    _view.DisplayMessage($"  Part Number: {ramInfo.PartNumber}\n");
                }
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Error fetching RAM info: {ex.Message}");
            }
        }


        public void ShowDiskInformation()
        {
            try
            {
                var diskInfos = _systemInfoService.GetDiskInfo();
                _view.DisplayMessage("\n--- Disk Information ---");

                foreach (var diskInfo in diskInfos)
                {
                   _view.DisplayMessage($"Model: {diskInfo.Model}");
                   _view.DisplayMessage($"Interface Type: {diskInfo.InterfaceType}");
                    _view.DisplayMessage($"Size: {diskInfo.Size} GB");
                   _view.DisplayMessage($"Media Type: {diskInfo.MediaType}");
                }
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Error fetching Disk info: {ex.Message}");
            }
        }

        public void ShowNetworkInformation()
        {
             try
            {
                var networkInfos = _systemInfoService.GetNetworkInfo();
                 _view.DisplayMessage("\n--- Network Information ---");

                foreach (var networkInfo in networkInfos)
                {
                    _view.DisplayMessage($"Name: {networkInfo.Name}");
                    _view.DisplayMessage($"MAC Address: {networkInfo.MACAddress}");
                    _view.DisplayMessage($"Speed: {networkInfo.Speed} bps");
                }
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Error fetching Network info: {ex.Message}");
            }
        }

        public void ShowSystemSummary()
        {
            try
            {
                _view.DisplayMessage("\n--- System Summary ---");
                
                var cpuInfo = _systemInfoService.GetSystemSummaryCpuInfo();
                _view.DisplayMessage($"CPU: {cpuInfo.Name} - {cpuInfo.NumberOfCores} Cores, {cpuInfo.NumberOfLogicalProcessors} Threads");


                var ramInfo = _systemInfoService.GetSystemSummaryRamInfo();
                _view.DisplayMessage($"RAM: {ramInfo.TotalRam} GB Total");


                var gpuInfo = _systemInfoService.GetSystemSummaryGpuInfo();
                _view.DisplayMessage($"GPU: {gpuInfo.Name} - {gpuInfo.AdapterRAM} bytes of Memory");
                
                var diskInfo = _systemInfoService.GetSystemSummaryDiskInfo();
                _view.DisplayMessage($"Disk: {diskInfo.Model} - {diskInfo.Size} bytes Capacity");
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Error fetching system summary: {ex.Message}");
            }
        }

        public void ShowTemperatureInformation()
        {
            try
            {
                var temperatureInfo = _systemInfoService.GetTemperatureInfo();

                _view.DisplayMessage("\n--- Temperature Information ---");
                if (temperatureInfo.HasTemperatureData)
                {
                    _view.DisplayMessage($"Temperature: {temperatureInfo.TemperatureCelsius:F2}°C");
                }
                else
                {
                   _view.DisplayMessage("No temperature information available.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Error fetching Temperature info: {ex.Message}");
            }
        }


        public void TerminateProgram()
        {
            _view.DisplayMessage("Exiting program...");
            Environment.Exit(0);
        }
    }

    public interface IController
    {
        void ShowMainScreen();
        void ShowInvalidOptionMessage();
        void ShowErrorMessage(string message);
        void ShowProgramTerminationMessage();
        void ShowRestartingMessage();
        void AskToContinue();
        Dictionary<int, Action> OptionActions { get; }
    }
    
    public interface ISystemInfoService
    {
        CpuInfo GetDetailedCpuInfo();
        List<GpuInfo> GetGpuInfo();
        RamInfo GetRamInfo();
        List<DiskInfo> GetDiskInfo();
        List<NetworkInfo> GetNetworkInfo();
        SystemSummaryCpuInfo GetSystemSummaryCpuInfo();
        SystemSummaryRamInfo GetSystemSummaryRamInfo();
        SystemSummaryGpuInfo GetSystemSummaryGpuInfo();
        SystemSummaryDiskInfo GetSystemSummaryDiskInfo();
         TemperatureInfo GetTemperatureInfo();

    }
    
    public class SystemInfoService : ISystemInfoService
    {
        public CpuInfo GetDetailedCpuInfo()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
                var obj = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

                return obj == null
                    ? new CpuInfo()
                    : new CpuInfo
                    {
                        Name = obj["Name"]?.ToString(),
                        Manufacturer = obj["Manufacturer"]?.ToString(),
                        NumberOfCores = Convert.ToInt32(obj["NumberOfCores"]),
                        NumberOfLogicalProcessors = Convert.ToInt32(obj["NumberOfLogicalProcessors"]),
                        MaxClockSpeed = Convert.ToInt32(obj["MaxClockSpeed"]),
                        CurrentClockSpeed = Convert.ToInt32(obj["CurrentClockSpeed"]),
                        ProcessorId = obj["ProcessorId"]?.ToString(),
                        L2CacheSize = Convert.ToInt32(obj["L2CacheSize"]),
                        L3CacheSize = Convert.ToInt32(obj["L3CacheSize"]),
                        Architecture = obj["Architecture"]?.ToString(),
                        ProcessorType = obj["ProcessorType"]?.ToString(),
                        Status = obj["Status"]?.ToString()
                    };
            }
            catch (Exception)
            {
                return new CpuInfo();
            }
        }


        public List<GpuInfo> GetGpuInfo()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
                var gpuInfos = new List<GpuInfo>();
                 
                foreach (ManagementObject obj in searcher.Get())
                {
                    long adapterRam = 0;
                    if (obj["AdapterRAM"] != null && long.TryParse(obj["AdapterRAM"].ToString(), out long ram))
                    {
                         adapterRam = ram / (1024 * 1024);
                    }

                  gpuInfos.Add(new GpuInfo
                  {
                      Name = obj["Name"]?.ToString(),
                      AdapterRAM = adapterRam == 0 ? "Not Available" : $"{adapterRam} MB",
                      DriverVersion = obj["DriverVersion"]?.ToString(),
                      VideoProcessor = obj["VideoProcessor"]?.ToString()
                  });
                 }
                return gpuInfos;
            }
            catch (Exception)
            {
              return new List<GpuInfo>();
            }
        }


        public RamInfo GetRamInfo()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
                long totalCapacity = 0;
                var modules = new List<RamModuleInfo>();

                foreach (ManagementObject obj in searcher.Get())
                {
                    totalCapacity += Convert.ToInt64(obj["Capacity"]);
                    modules.Add(new RamModuleInfo
                    {
                       Capacity = Convert.ToInt64(obj["Capacity"]) / (1024 * 1024 * 1024),
                       Manufacturer = obj["Manufacturer"]?.ToString(),
                       Speed = Convert.ToInt32(obj["Speed"]),
                       PartNumber = obj["PartNumber"]?.ToString()
                    });
                }

                return new RamInfo
                {
                  TotalCapacity = totalCapacity / (1024 * 1024 * 1024),
                    Modules = modules
                };

            }
            catch (Exception)
            {
                return new RamInfo();
            }
        }

        public List<DiskInfo> GetDiskInfo()
        {
           try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
                var diskInfos = new List<DiskInfo>();
                
                foreach (ManagementObject obj in searcher.Get())
                {
                    diskInfos.Add(new DiskInfo
                    {
                        Model = obj["Model"]?.ToString(),
                        InterfaceType = obj["InterfaceType"]?.ToString(),
                        Size = Convert.ToInt64(obj["Size"]) / (1024 * 1024 * 1024),
                        MediaType = obj["MediaType"]?.ToString()
                    });
                }
               return diskInfos;
            }
            catch (Exception)
            {
               return new List<DiskInfo>();
            }
        }

        public List<NetworkInfo> GetNetworkInfo()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_NetworkAdapter where NetEnabled = true");
                var networkInfos = new List<NetworkInfo>();

                foreach (ManagementObject obj in searcher.Get())
                {
                    networkInfos.Add(new NetworkInfo
                    {
                        Name = obj["Name"]?.ToString(),
                        MACAddress = obj["MACAddress"]?.ToString(),
                        Speed = Convert.ToInt64(obj["Speed"])
                    });
                }
                return networkInfos;
            }
            catch (Exception)
            {
               return new List<NetworkInfo>();
            }
        }

        public SystemSummaryCpuInfo GetSystemSummaryCpuInfo()
        {
             var cpuInfo = GetDetailedCpuInfo();
            return new SystemSummaryCpuInfo
            {
               Name = cpuInfo.Name,
               NumberOfCores = cpuInfo.NumberOfCores,
               NumberOfLogicalProcessors = cpuInfo.NumberOfLogicalProcessors
            };
        }

        public SystemSummaryRamInfo GetSystemSummaryRamInfo()
        {
            var ramInfo = GetRamInfo();
           return new SystemSummaryRamInfo
           {
               TotalRam = ramInfo.TotalCapacity
           };
        }

        public SystemSummaryGpuInfo GetSystemSummaryGpuInfo()
        {
            var gpuInfo = GetGpuInfo().FirstOrDefault();
            return gpuInfo == null ? new SystemSummaryGpuInfo() : new SystemSummaryGpuInfo
            {
                Name = gpuInfo.Name,
                AdapterRAM = gpuInfo.AdapterRAM
            };
        }

        public SystemSummaryDiskInfo GetSystemSummaryDiskInfo()
        {
           var diskInfo = GetDiskInfo().FirstOrDefault();
           return diskInfo == null ? new SystemSummaryDiskInfo() : new SystemSummaryDiskInfo
           {
               Model = diskInfo.Model,
                Size = diskInfo.Size
           };
        }


        public TemperatureInfo GetTemperatureInfo()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                var obj = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                
                if (obj != null)
                {
                    double tempKelvin = Convert.ToDouble(obj["CurrentTemperature"]);
                    double tempCelsius = (tempKelvin - 2732) / 10.0;
                    return new TemperatureInfo
                    {
                        HasTemperatureData = true,
                        TemperatureCelsius = tempCelsius
                    };
                }

                return new TemperatureInfo { HasTemperatureData = false };
            }
            catch (Exception)
            {
               return new TemperatureInfo {HasTemperatureData = false};
            }
        }
    }
    
    
   public class CpuInfo
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public int NumberOfCores { get; set; }
        public int NumberOfLogicalProcessors { get; set; }
        public int MaxClockSpeed { get; set; }
        public int CurrentClockSpeed { get; set; }
        public string ProcessorId { get; set; }
        public int L2CacheSize { get; set; }
        public int L3CacheSize { get; set; }
        public string Architecture { get; set; }
        public string ProcessorType { get; set; }
        public string Status { get; set; }
    }


    public class GpuInfo
    {
        public string Name { get; set; }
        public string AdapterRAM { get; set; }
        public string DriverVersion { get; set; }
        public string VideoProcessor { get; set; }
    }
    
    public class RamInfo
    {
        public long TotalCapacity { get; set; }
        public List<RamModuleInfo> Modules { get; set; }
    }

    public class RamModuleInfo
    {
        public long Capacity { get; set; }
        public string Manufacturer { get; set; }
        public int Speed { get; set; }
        public string PartNumber { get; set; }
    }

    public class DiskInfo
    {
        public string Model { get; set; }
        public string InterfaceType { get; set; }
        public long Size { get; set; }
        public string MediaType { get; set; }
    }

    public class NetworkInfo
    {
        public string Name { get; set; }
        public string MACAddress { get; set; }
        public long Speed { get; set; }
    }
    
     public class SystemSummaryCpuInfo
    {
        public string Name { get; set; }
        public int NumberOfCores { get; set; }
        public int NumberOfLogicalProcessors { get; set; }
    }
    
     public class SystemSummaryRamInfo
    {
        public long TotalRam { get; set; }
    }
    
     public class SystemSummaryGpuInfo
    {
        public string Name { get; set; }
        public string AdapterRAM { get; set; }
    }
    
    public class SystemSummaryDiskInfo
    {
        public string Model { get; set; }
        public long Size { get; set; }
    }
    
    public class TemperatureInfo
    {
        public bool HasTemperatureData { get; set; }
        public double TemperatureCelsius { get; set; }
    }
}