using System;
using System.Collections.Generic;
using System.Text;

namespace DelegatesLinQ.Homework
{
    public delegate string DataProcessor(string input);
    public delegate void ProcessingEventHandler(string stage, string input, string output);

    public class DataProcessingPipeline
    {
        public event ProcessingEventHandler ProcessingStageCompleted;

        public static string RemoveSpaces(string input)
        {
            return input.Replace(" ", "");
        }

        public static string ToUpperCase(string input)
        {
            return input.ToUpper();
        }

        public static string AddTimestamp(string input)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {input}";
        }

        public static string ReverseString(string input)
        {
            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string EncodeBase64(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes);
        }

        public static string ValidateInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be null or empty");
            }
            return input;
        }

        public string ProcessData(string input, DataProcessor pipeline)
        {
            try
            {
                string currentInput = input;
                foreach (DataProcessor processor in pipeline.GetInvocationList())
                {
                    string stageName = processor.Method.Name;
                    string output = processor(currentInput);
                    OnProcessingStageCompleted(stageName, currentInput, output);
                    currentInput = output;
                }
                return currentInput;
            }
            catch (Exception ex)
            {
                OnProcessingStageCompleted("Error", input, $"Error: {ex.Message}");
                throw;
            }
        }

        protected virtual void OnProcessingStageCompleted(string stage, string input, string output)
        {
            ProcessingStageCompleted?.Invoke(stage, input, output);
        }
    }

    public class ProcessingLogger
    {
        public void OnProcessingStageCompleted(string stage, string input, string output)
        {
            Console.WriteLine($"Stage: {stage}");
            Console.WriteLine($"Input: {input}");
            Console.WriteLine($"Output: {output}");
            Console.WriteLine("---");
        }
    }

    public class PerformanceMonitor
    {
        private readonly Dictionary<string, int> _stageCounts = new Dictionary<string, int>();
        private readonly Dictionary<string, double> _stageTimes = new Dictionary<string, double>();
        private readonly List<double> _processingTimes = new List<double>();

        public void OnProcessingStageCompleted(string stage, string input, string output)
        {
            DateTime startTime = DateTime.Now;
            
            if (!_stageCounts.ContainsKey(stage))
            {
                _stageCounts[stage] = 0;
                _stageTimes[stage] = 0;
            }
            
            _stageCounts[stage]++;
            double duration = (DateTime.Now - startTime).TotalMilliseconds;
            _stageTimes[stage] += duration;
            _processingTimes.Add(duration);
        }

        public void DisplayStatistics()
        {
            Console.WriteLine("\nPerformance Statistics:");
            Console.WriteLine($"Total processing stages executed: {_processingTimes.Count}");
            
            foreach (var stage in _stageCounts)
            {
                double avgTime = _stageTimes[stage.Key] / stage.Value;
                Console.WriteLine($"{stage.Key}:");
                Console.WriteLine($"  Executions: {stage.Value}");
                Console.WriteLine($"  Average Time: {avgTime:F2} ms");
            }
            
            if (_processingTimes.Count > 0)
            {
                double totalTime = _processingTimes.Sum();
                Console.WriteLine($"Total Processing Time: {totalTime:F2} ms");
                Console.WriteLine($"Average Processing Time: {(totalTime / _processingTimes.Count):F2} ms");
            }
        }
    }

    public class DelegateChain
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== HOMEWORK 2: CUSTOM DELEGATE CHAIN ===");
            Console.WriteLine();

            DataProcessingPipeline pipeline = new DataProcessingPipeline();
            ProcessingLogger logger = new ProcessingLogger();
            PerformanceMonitor monitor = new PerformanceMonitor();

            // Subscribe to events
            pipeline.ProcessingStageCompleted += logger.OnProcessingStageCompleted;
            pipeline.ProcessingStageCompleted += monitor.OnProcessingStageCompleted;

            // Create initial processing chain
            DataProcessor processingChain = DataProcessingPipeline.ValidateInput;
            processingChain += DataProcessingPipeline.RemoveSpaces;
            processingChain += DataProcessingPipeline.ToUpperCase;
            processingChain += DataProcessingPipeline.AddTimestamp;

            // Test the pipeline
            string testInput = "Hello World from C#";
            Console.WriteLine($"Input: {testInput}");
            try
            {
                string result = pipeline.ProcessData(testInput, processingChain);
                Console.WriteLine($"Output: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handled: {ex.Message}");
            }

            // Demonstrate adding more processors
            Console.WriteLine("\nAdding ReverseString and EncodeBase64 to pipeline...");
            processingChain += DataProcessingPipeline.ReverseString;
            processingChain += DataProcessingPipeline.EncodeBase64;

            try
            {
                string result = pipeline.ProcessData("Extended Pipeline Test", processingChain);
                Console.WriteLine($"Extended Output: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handled: {ex.Message}");
            }

            // Demonstrate removing a processor
            Console.WriteLine("\nRemoving ReverseString from pipeline...");
            processingChain -= DataProcessingPipeline.ReverseString;
            try
            {
                string result = pipeline.ProcessData("Without Reverse", processingChain);
                Console.WriteLine($"Modified Output: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handled: {ex.Message}");
            }

            // Test error handling
            Console.WriteLine("\nTesting error handling with null input...");
            try
            {
                string result = pipeline.ProcessData(null, processingChain);
                Console.WriteLine($"Output: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handled: {ex.Message}");
            }

            // Display performance statistics
            monitor.DisplayStatistics();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}