# System Resources Control

System Resources Control is a console-based application written in C#. It provides detailed information and monitoring capabilities for system resources, including CPU and GPU, along with a CPU benchmarking tool.

## Features

### CPU Information
- **Detailed Processor Info**: Displays detailed information about the CPU, including the name, manufacturer, number of cores, clock speed, cache size, and more.
- **Monitor CPU Core Usage**: Real-time monitoring of individual CPU core usage.
- **CPU Benchmark**: Simulates a heavy workload on all CPU cores to calculate a performance score.

### GPU Information
- Fetches details about the installed GPU, including name, adapter RAM, driver version, and video processor.

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
   - `3`: Exit the program

### CPU Information Menu
- Select `1` for detailed CPU information.
- Select `2` to monitor CPU core usage in real-time. Press `q` to stop monitoring.
- Select `3` to run the CPU benchmark. The results will display the total score and completion time.
- Select `4` to return to the main menu.

### GPU Information Menu
- Displays detailed information about the installed GPU.

## Requirements
- .NET Framework or .NET Core (compatible version for C# projects)
- Windows OS (uses WMI for hardware queries)

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
3. Exit
```

### CPU Benchmark
```
--- CPU Benchmark ---
Simulating heavy workload on all cores and threads...
Thread 0 completed with score 12.34
Thread 1 completed with score 11.56
...
Benchmark completed in 1500 ms.
Total Score: 250.78 points
```

## Contributing
Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Submit a pull request with a detailed explanation of your changes.

## Acknowledgments
- [Microsoft Documentation](https://docs.microsoft.com/) for information on WMI and system management.
- Inspiration from performance monitoring and benchmarking tools.

---
Feel free to reach out if you have any questions or suggestions for improvement!

