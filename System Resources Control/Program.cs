using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace System.Resources.Control
{
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
        private readonly ISystemInfoService _systemInfoService;
        public Dictionary<int, Action> OptionActions { get; }
         private Dictionary<int, Action> _cpuOptionActions;


        public Controller(IModel model, IView view)
        {
            _model = model;
            _view = view;
            _systemInfoService = new SystemInfoService();
            OptionActions = new Dictionary<int, Action>
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

        public void ShowMainScreen() => _view.ShowMainScreen();
        public void ShowInvalidOptionMessage() => _view.ShowInvalidOptionMessage();
        public void ShowErrorMessage(string message) => _view.ShowErrorMessage(message);
        public void ShowProgramTerminationMessage() => _view.ShowProgramTerminationMessage();
        public void ShowRestartingMessage() => _view.ShowRestartingMessage();
        public void AskToContinue() => _view.AskToContinue();

        private void ShowCpuInformation()
        {
           _cpuOptionActions = new Dictionary<int, Action>
            {
              { 1, DisplayDetailedCpuInfo },
              { 2, DisplayCoreFrequency },
              { 3, RunCpuBenchmark }
            };

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
                         if (_cpuOptionActions.TryGetValue(option, out var action))
                             action();
                         else if(option == 4)
                                 isStopped = true;
                         else
                               _view.ShowInvalidOptionMessage();
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
            var threadCounters = Enumerable.Range(0, Environment.ProcessorCount)
                .Select(i => new PerformanceCounter("Processor", "% Processor Time", i.ToString())).ToArray();

            Console.Clear();
            _view.DisplayMessage("--- CPU core usage monitoring ---");
            _view.DisplayMessage("Press 'q' to stop monitoring.\n");
                
            while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Q)
            {
               Console.Clear();
                _view.DisplayMessage("--- CPU core usage monitoring ---");
               _view.DisplayMessage("Press 'q' to stop monitoring.\n");

                 foreach (var counter in threadCounters)
                    _view.DisplayMessage($"Thread {Array.IndexOf(threadCounters, counter)}: {counter.NextValue():F2}%");
                 
                 Thread.Sleep(500);
            }
                
            foreach (var counter in threadCounters)
                counter.Dispose();
        }
        private void RunCpuBenchmark()
        {
            _view.DisplayMessage("\n--- CPU Benchmark ---");
            _view.DisplayMessage("Simulating heavy workload on all cores and threads...\n");
                
            int threadCount = Environment.ProcessorCount;
            double[] threadScores = new double[threadCount];
                Stopwatch stopwatch = Stopwatch.StartNew();

            Parallel.For(0, threadCount, i =>
            {
                threadScores[i] = _model.SimulateHeavyWorkload(i);
                    _view.DisplayMessage($"Thread {i} completed with score {threadScores[i]:F2}");
            });
                stopwatch.Stop();

            double totalScore = threadScores.Sum(score => _model.CalculateScore(score, stopwatch.Elapsed));

             _view.DisplayMessage($"\nBenchmark completed in {stopwatch.ElapsedMilliseconds} ms.");
             _view.DisplayMessage($"Total Score: {totalScore:F2} points");
        }

         private void ShowGpuInformation()
        {
            try
            {
                foreach (var gpuInfo in _systemInfoService.GetGpuInfo())
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
                _view.DisplayMessage("\n--- Disk Information ---");
               foreach(var diskInfo in _systemInfoService.GetDiskInfo())
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
                _view.DisplayMessage("\n--- Network Information ---");
                foreach (var networkInfo in _systemInfoService.GetNetworkInfo())
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
                _view.DisplayMessage(temperatureInfo.HasTemperatureData
                    ? $"Temperature: {temperatureInfo.TemperatureCelsius:F2}°C"
                    : "No temperature information available.");
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
        public class SystemInfoService : ISystemInfoService
    {
         public CpuInfo GetDetailedCpuInfo() => GetManagementObject("Win32_Processor", obj => new CpuInfo
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
        });
         public List<GpuInfo> GetGpuInfo() => GetManagementObjects("Win32_VideoController", obj =>
         {
             long.TryParse(obj["AdapterRAM"]?.ToString(), out long adapterRam);
             return new GpuInfo
             {
                Name = obj["Name"]?.ToString(),
                AdapterRAM = adapterRam == 0 ? "Not Available" : $"{adapterRam / (1024 * 1024)} MB",
                DriverVersion = obj["DriverVersion"]?.ToString(),
                VideoProcessor = obj["VideoProcessor"]?.ToString()
            };
         });

         public RamInfo GetRamInfo()
         {
            return GetManagementObject("Win32_PhysicalMemory", objCollection =>
           {
               long totalCapacity = 0;
               var modules = new List<RamModuleInfo>();
               foreach (var obj in objCollection)
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
           });
         }

        public List<DiskInfo> GetDiskInfo() => GetManagementObjects("Win32_DiskDrive", obj => new DiskInfo
        {
            Model = obj["Model"]?.ToString(),
            InterfaceType = obj["InterfaceType"]?.ToString(),
            Size = Convert.ToInt64(obj["Size"]) / (1024 * 1024 * 1024),
            MediaType = obj["MediaType"]?.ToString()
        });
        public List<NetworkInfo> GetNetworkInfo() => GetManagementObjects("Win32_NetworkAdapter where NetEnabled = true", obj => new NetworkInfo
        {
            Name = obj["Name"]?.ToString(),
            MACAddress = obj["MACAddress"]?.ToString(),
            Speed = Convert.ToInt64(obj["Speed"])
        });
        public SystemSummaryCpuInfo GetSystemSummaryCpuInfo() => GetDetailedCpuInfo().ToSystemSummaryCpuInfo();
        public SystemSummaryRamInfo GetSystemSummaryRamInfo() => GetRamInfo().ToSystemSummaryRamInfo();
        public SystemSummaryGpuInfo GetSystemSummaryGpuInfo() => GetGpuInfo().FirstOrDefault()?.ToSystemSummaryGpuInfo() ?? new SystemSummaryGpuInfo();
        public SystemSummaryDiskInfo GetSystemSummaryDiskInfo() => GetDiskInfo().FirstOrDefault()?.ToSystemSummaryDiskInfo() ?? new SystemSummaryDiskInfo();
       public TemperatureInfo GetTemperatureInfo() => GetManagementObject("MSAcpi_ThermalZoneTemperature", obj =>
        {
            if (obj != null && double.TryParse(obj["CurrentTemperature"]?.ToString(), out double tempKelvin))
            {
                return new TemperatureInfo
                {
                   HasTemperatureData = true,
                    TemperatureCelsius = (tempKelvin - 2732) / 10.0
                };
            }
            return new TemperatureInfo { HasTemperatureData = false };
        }, "root\\WMI");
        
       private T GetManagementObject<T>(string query, Func<ManagementObject, T> mapper, string scope = null)
        {
            try
            {
                using var searcher = new ManagementObjectSearcher(scope ?? string.Empty, $"SELECT * FROM {query}");
               return searcher.Get().Cast<ManagementObject>().FirstOrDefault() is { } obj ? mapper(obj) : default;

            }
           catch (Exception )
            {
                return default;
            }
        }
       
        private T GetManagementObject<T>(string query, Func<ManagementObjectCollection, T> mapper, string scope = null)
       {
            try
           {
                using var searcher = new ManagementObjectSearcher(scope ?? string.Empty, $"SELECT * FROM {query}");
               return mapper(searcher.Get());

           }
            catch (Exception)
           {
              return default;
           }
       }
        private List<T> GetManagementObjects<T>(string query, Func<ManagementObject, T> mapper,string scope = null)
        {
            try
            {
               using var searcher = new ManagementObjectSearcher(scope ?? string.Empty, $"SELECT * FROM {query}");
               return searcher.Get().Cast<ManagementObject>().Select(mapper).ToList();
            }
           catch (Exception)
           {
               return new List<T>();
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
        
        public SystemSummaryCpuInfo ToSystemSummaryCpuInfo() => new SystemSummaryCpuInfo
        {
            Name = Name,
            NumberOfCores = NumberOfCores,
            NumberOfLogicalProcessors = NumberOfLogicalProcessors,
        };
    }


    public class GpuInfo
    {
        public string Name { get; set; }
        public string AdapterRAM { get; set; }
        public string DriverVersion { get; set; }
        public string VideoProcessor { get; set; }
        
        public SystemSummaryGpuInfo ToSystemSummaryGpuInfo() => new SystemSummaryGpuInfo
        {
            Name = Name,
            AdapterRAM = AdapterRAM,
        };
    }
    
    public class RamInfo
    {
        public long TotalCapacity { get; set; }
        public List<RamModuleInfo> Modules { get; set; }
        
        public SystemSummaryRamInfo ToSystemSummaryRamInfo() => new SystemSummaryRamInfo
        {
            TotalRam = TotalCapacity,
        };
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
        public SystemSummaryDiskInfo ToSystemSummaryDiskInfo() => new SystemSummaryDiskInfo
        {
            Model = Model,
            Size = Size,
        };
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