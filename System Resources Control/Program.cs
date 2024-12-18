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
            Thread[] threads = new Thread[threadCount];
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < threadCount; i++)
            {
                int threadIndex = i;
                threads[i] = new Thread(() => SimulateHeavyWorkload(threadIndex));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();
            WriteLine($"\nBenchmark completed in {stopwatch.ElapsedMilliseconds} ms.");
        }

        static void SimulateHeavyWorkload(int threadIndex)
        {
            double result = 0;
            for (int i = 0; i < 10_000_000; i++)
            {
                result += Math.Sqrt(i) * Math.Sin(i);
            }

            WriteLine($"Thread {threadIndex} completed.");
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

        static void Main(string[] args)
        {
            bool isStopped = false;

            while (!isStopped)
            {
                try
                {
                    WriteLine("------- System Resources Control -------");
                    WriteLine("Select an option:");
                    WriteLine("1. CPU information");
                    WriteLine("2. GPU information (not finished) ");
                    WriteLine("3. Exit");
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
