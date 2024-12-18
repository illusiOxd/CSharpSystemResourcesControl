namespace System_Resources_Control
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Threading;
    using static System.Console;

    class Program
    {
        static void GetCpuInformation()
        {
            bool isStopped = false;

            while (!isStopped)
            {
                try
                {
                    WriteLine("----- CPU Information -----");
                    WriteLine("Select an option:");
                    WriteLine("1. Detailed Processor Info");
                    WriteLine("2. Monitor CPU Core Usage");
                    WriteLine("3. Run CPU Benchmark");
                    WriteLine("4. Go Back");
                    int option = int.Parse(ReadLine());

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
                            WriteLine("Invalid option. Please choose again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static void DisplayDetailedCpuInfo()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_Processor");

                foreach (ManagementObject obj in searcher.Get())
                {
                    WriteLine("\n--- Processor Information ---");
                    WriteLine($"Processor Name: {obj["Name"]}");
                    WriteLine($"Manufacturer: {obj["Manufacturer"]}");
                    WriteLine($"Number of Cores: {obj["NumberOfCores"]}");
                    WriteLine($"Number of Logical Processors: {obj["NumberOfLogicalProcessors"]}");
                    WriteLine($"Max Clock Speed: {obj["MaxClockSpeed"]} MHz");
                    WriteLine($"Current Clock Speed: {obj["CurrentClockSpeed"]} MHz");
                    WriteLine($"Processor ID: {obj["ProcessorId"]}");
                    WriteLine($"L2 Cache Size: {obj["L2CacheSize"]} KB");
                    WriteLine($"L3 Cache Size: {obj["L3CacheSize"]} KB");
                    WriteLine($"Architecture: {obj["Architecture"]}");
                    WriteLine($"Processor Type: {obj["ProcessorType"]}");
                    WriteLine($"Status: {obj["Status"]}");
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Error fetching CPU info: {ex.Message}");
            }
        }

        static void DisplayCoreFrequency()
        {
            int threadCount = Environment.ProcessorCount;
            PerformanceCounter[] threadCounters = new PerformanceCounter[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                threadCounters[i] = new PerformanceCounter("Processor", "% Processor Time", i.ToString());
            }

            Clear();
            WriteLine("--- CPU core usage monitoring ---");
            WriteLine("Press 'q' to stop monitoring.\n");

            while (!KeyAvailable || ReadKey(true).Key != ConsoleKey.Q)
            {
                Clear();
                WriteLine("--- CPU core usage monitoring ---");
                WriteLine("Press 'q' to stop monitoring.\n");

                for (int i = 0; i < threadCount; i++)
                {
                    WriteLine($"Thread {i}: {threadCounters[i].NextValue():F2}%");
                }

                Thread.Sleep(500);
            }

            foreach (var counter in threadCounters)
            {
                counter.Dispose();
            }
        }

        static void RunCpuBenchmark()
        {
            WriteLine("\n--- CPU Benchmark ---");
            WriteLine("Simulating heavy workload on all cores and threads...\n");

            int threadCount = Environment.ProcessorCount;
            double[] threadScores = new double[threadCount];
            object lockObject = new object(); 

            Stopwatch stopwatch = Stopwatch.StartNew();

            Parallel.For(0, threadCount, i =>
            {
                threadScores[i] = SimulateHeavyWorkload(i); 
            });

            stopwatch.Stop();

            double totalScore = 0;
            foreach (var score in threadScores)
            {
                lock (lockObject)
                {
                    totalScore += CalculateScore(score, stopwatch.Elapsed); 
                }
            }

            WriteLine($"\nBenchmark completed in {stopwatch.ElapsedMilliseconds} ms.");
            WriteLine($"Total Score: {totalScore:F2} points");
        }


        static double SimulateHeavyWorkload(int threadIndex)
        {
            double result = 0;
            const int iterations = 100_000_000;

            for (int i = 1; i <= iterations; i++)
            {
                result += Math.Sqrt(i) * Math.Sin(i);
            }

            double threadScore = result / 1_000_000;
            WriteLine($"Thread {threadIndex} completed with score {threadScore:F2}");
            return threadScore;
        }

        static int CalculateScore(double threadScore, TimeSpan elapsedTime)
        {
            double baseScore = 10000.0 / (1 + threadScore / 1000.0); 
            
            double timeBonus = 10000.0 / (elapsedTime.TotalMilliseconds + 1); 
            
            double totalScore = baseScore + timeBonus;
            
            return Math.Max(0, (int)totalScore); 
        }


        static void GetGpuInformation()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_VideoController");

                foreach (ManagementObject obj in searcher.Get())
                {
                    WriteLine("\n--- GPU Information ---");
                    WriteLine($"GPU Name: {obj["Name"]}");

                    if (obj["AdapterRAM"] != null && long.TryParse(obj["AdapterRAM"].ToString(), out long adapterRam))
                    {
                        WriteLine($"Adapter RAM: {adapterRam / (1024 * 1024)} MB");
                    }
                    else
                    {
                        WriteLine("Adapter RAM: Not available");
                    }

                    WriteLine($"Driver Version: {obj["DriverVersion"]}");
                    WriteLine($"Video Processor: {obj["VideoProcessor"]}");
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Error fetching GPU info: {ex.Message}");
            }
        }
        
        static void GetRamInformation()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
                
                long totalCapacity = 0;
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalCapacity += Convert.ToInt64(obj["Capacity"]);
                }

                WriteLine("\n--- RAM Information ---");
                WriteLine($"Total Installed RAM: {totalCapacity / (1024 * 1024 * 1024)} GB\n");
                
                int moduleNumber = 1;
                foreach (ManagementObject obj in searcher.Get())
                {
                    WriteLine($"Module {moduleNumber++}:");
                    WriteLine($"  Capacity: {Convert.ToInt64(obj["Capacity"]) / (1024 * 1024 * 1024)} GB");
                    WriteLine($"  Manufacturer: {obj["Manufacturer"]}");
                    WriteLine($"  Speed: {obj["Speed"]} MHz");
                    WriteLine($"  Part Number: {obj["PartNumber"]}\n");
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Error fetching RAM info: {ex.Message}");
            }
        }


        static void GetDiskInformation()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
                WriteLine("\n--- Disk Information ---");

                foreach (ManagementObject obj in searcher.Get())
                {
                    WriteLine($"Model: {obj["Model"]}");
                    WriteLine($"Interface Type: {obj["InterfaceType"]}");
                    WriteLine($"Size: {Convert.ToInt64(obj["Size"]) / (1024 * 1024 * 1024)} GB");
                    WriteLine($"Media Type: {obj["MediaType"]}");
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Error fetching Disk info: {ex.Message}");
            }
        }
        
        static void GetNetworkInformation()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("select * from Win32_NetworkAdapter where NetEnabled = true");
                WriteLine("\n--- Network Information ---");

                foreach (ManagementObject obj in searcher.Get())
                {
                    WriteLine($"Name: {obj["Name"]}");
                    WriteLine($"MAC Address: {obj["MACAddress"]}");
                    WriteLine($"Speed: {obj["Speed"]} bps");
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Error fetching Network info: {ex.Message}");
            }
        }
        
        static void Main(string[] args)
        {
            bool isStopped = false;

            while (!isStopped)
            {
                try
                {
                    WriteLine("------- System Resources Control -------");
                    WriteLine("Select an option:");
                    WriteLine("1. CPU Information");
                    WriteLine("2. GPU Information");
                    WriteLine("3. RAM Information");
                    WriteLine("4. Disk Information");
                    WriteLine("5. Network Information");
                    WriteLine("6. Exit");
                    int option = int.Parse(ReadLine());

                    switch (option)
                    {
                        case 1:
                            GetCpuInformation();
                            break;
                        case 2:
                            GetGpuInformation();
                            break;
                        case 3:
                            GetRamInformation();
                            break;
                        case 4:
                            GetDiskInformation();
                            break;
                        case 5:
                            GetNetworkInformation();
                            break;
                        case 6:
                            isStopped = true;
                            WriteLine("Program terminated.");
                            break;
                        default:
                            WriteLine("Invalid option. Please choose again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    WriteLine($"\nError: {ex.Message}");
                }

                if (!isStopped)
                {
                    WriteLine("\nDo you want to continue? (y/n)");
                    var key = ReadKey(true).Key;

                    if (key == ConsoleKey.N)
                    {
                        isStopped = true;
                        WriteLine("Program terminated.");
                    }
                    else
                    {
                        WriteLine("\nRestarting monitoring...\n");
                    }
                }
            }
        }
    }
}
