# System Resources Control

System Resources Control is a console-based application written in C#. It provides detailed information and monitoring capabilities for system resources, including CPU, GPU, RAM, storage devices, and temperatures, along with a CPU benchmarking tool.

## Features

### CPU Information
- **Detailed Processor Info**: Displays detailed information about the CPU, including the name, manufacturer, number of cores, clock speed, cache size, and more.
- **Monitor CPU Core Usage**: Real-time monitoring of individual CPU core usage.
- **CPU Benchmark**: Simulates a heavy workload on all CPU cores to calculate a performance score.

### GPU Information
- Fetches details about the installed GPU, including name, adapter RAM, driver version, and video processor.

### RAM Information
- **Overall RAM Details**: Displays the total installed RAM in the system.
- **Per-Module Details**: Lists detailed information for each RAM module, including capacity, manufacturer, speed, and part number.

### Disk Information
- Retrieves details about installed storage devices, including capacity, model, and available space.

### Temperature Information
- **System-Wide Temperatures**: Displays the temperature readings for various components, including CPU and GPU, if available.

### System Overview
- **Summary Output**: Provides a concise overview of the system, including CPU, GPU, RAM, storage, and temperature data.

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/illusiOxd/CSharpSystemResourcesControl.git
   ```
2. Open the project in Visual Studio or your preferred IDE.
3. Build the project to restore dependencies and compile the application.
4. Run the application.

## Usage
1. Launch the application.
2. Use the menu to navigate through the options:
   - `1`: CPU Information
   - `2`: GPU Information
   - `3`: RAM Information
   - `4`: Disk Information
   - `5`: Temperature Information
   - `6`: System Overview
   - `7`: Exit the program

### CPU Information Menu
- Select `1` for detailed CPU information.
- Select `2` to monitor CPU core usage in real-time. Press `q` to stop monitoring.
- Select `3` to run the CPU benchmark. The results will display the total score and completion time.
- Select `4` to return to the main menu.

### GPU Information Menu
- Displays detailed information about the installed GPU.

### RAM Information Menu
- Shows total installed RAM in the system.
- Lists detailed information for each memory module, with numbered entries for easier identification.

### Disk Information Menu
- Displays details about storage devices, including model, total capacity, and free space available.

### Temperature Information Menu
- Displays temperature readings for various system components. If access is denied, ensure the application is running with administrator privileges.

### System Overview Menu
- Provides a summary of all key system details in a concise format.

## Requirements
- .NET Framework or .NET Core (compatible version for C# projects)
- Windows OS (uses WMI for hardware queries)
- Administrator privileges for temperature monitoring

## How the CPU Benchmark Works
The benchmark uses all available CPU cores to perform a heavy mathematical workload. Each core's performance is measured and combined into a total score. The workload involves:
- Calculating square roots and trigonometric functions
- Running multiple iterations to stress the CPU

### Formula for Score Calculation
- Base score is inversely proportional to the thread's workload.
- Time bonus is added for faster completion.
- Final score = Base score + Time bonus.

## Example Output
### Main Menu
```
------- System Resources Control -------
Select an option:
1. CPU information
2. GPU information
3. RAM information
4. Disk information
5. Temperature information
6. System overview
7. Exit
```

### RAM Information
```
--- RAM Information ---
Total Installed RAM: 16 GB

Module 1:
  Capacity: 8 GB
  Manufacturer: Corsair
  Speed: 3200 MHz
  Part Number: CMK16GX4M2B3200C16

Module 2:
  Capacity: 8 GB
  Manufacturer: Corsair
  Speed: 3200 MHz
  Part Number: CMK16GX4M2B3200C16
```

### Temperature Information
```
--- Temperature Information ---
Device: CPU0 - Temperature: 45.2 °C
Device: GPU0 - Temperature: 50.5 °C
```

### System Overview
```
--- System Overview ---
CPU: Intel Core i7-9700K @ 3.60 GHz
GPU: NVIDIA GeForce GTX 1080
Total RAM: 16 GB
Disk: Samsung SSD 970 EVO Plus 1TB - 850 GB free of 1TB
Temperatures:
  CPU: 45.2 °C
  GPU: 50.5 °C
```

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Submit a pull request with a detailed explanation of your changes.

## Acknowledgments
- [Microsoft Documentation](https://docs.microsoft.com/) for information on WMI and system management.
- [OpenHardwareMonitor](https://github.com/openhardwaremonitor/openhardwaremonitor) for inspiration on monitoring hardware sensors.
- Inspiration from performance monitoring and benchmarking tools.

---
Feel free to reach out if you have any questions or suggestions for improvement!

