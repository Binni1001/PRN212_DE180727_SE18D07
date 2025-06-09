using System;
using System.Collections.Generic;
using System.Threading;

namespace DesignPatterns.Homework
{
    // This homework is based on the Singleton Pattern with a practical application
    // Students will implement a basic logging system using the Singleton pattern
    
    public class Logger
    {
        // Private static instance of the Logger class
        private static Logger _instance;
        
        // Private lock object for thread safety
        private static readonly object _lock = new object();
        
        // Counter to track instance creation attempts
        private static int _instanceCount = 0;
        
        // Collection to hold log messages
        private List<string> _logMessages;
        
        // Private constructor to prevent instantiation from outside
        private Logger()
        {
            _logMessages = new List<string>();
            _instanceCount++;
            Console.WriteLine($"Logger instance created. Instance count: {_instanceCount}");
        }
        
        // Public static method to get the single instance
        public static Logger GetInstance
        {
            get
            {
                // Double-check locking pattern
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }
        
        // Alternative implementation - Option #1 (eager initialization)
        // Comment: This approach creates the instance when the class is loaded
        /*
        private static readonly Logger _eagerInstance = new Logger();
        
        public static Logger GetEagerInstance
        {
            get
            {
                return _eagerInstance;
            }
        }
        */
        
        // Alternative implementation - Option #2 (simple thread-safe using lock only)
        public static Logger GetSimpleThreadSafeInstance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                    return _instance;
                }
            }
        }
        
        // Public property to access instance count (for demonstration)
        public static int InstanceCount
        {
            get { return _instanceCount; }
        }
        
        // Method to log an information message
        public void LogInfo(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [INFO] {message}";
            _logMessages.Add(logEntry);
            Console.WriteLine(logEntry);
        }
        
        // Method to log an error message
        public void LogError(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] {message}";
            _logMessages.Add(logEntry);
            Console.WriteLine(logEntry);
        }
        
        // Method to log a warning message
        public void LogWarning(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [WARNING] {message}";
            _logMessages.Add(logEntry);
            Console.WriteLine(logEntry);
        }
        
        // Method to display all log entries
        public void DisplayLogs()
        {
            Console.WriteLine("\n----- LOG ENTRIES -----");
            if (_logMessages.Count == 0)
            {
                Console.WriteLine("No log entries found.");
            }
            else
            {
                foreach (var log in _logMessages)
                {
                    Console.WriteLine(log);
                }
            }
            Console.WriteLine("----- END OF LOGS -----\n");
        }
        
        // Method to clear all logs
        public void ClearLogs()
        {
            _logMessages.Clear();
            Console.WriteLine("All logs have been cleared.");
        }
    }
    
    // Example application classes using the logger
    
    public class UserService
    {
        private Logger _logger;
        
        public UserService()
        {
            _logger = Logger.GetInstance;
        }
        
        public void RegisterUser(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("Username cannot be empty");
                }
                
                _logger.LogInfo($"User '{username}' registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to register user: {ex.Message}");
            }
        }
    }
    
    public class PaymentService
    {
        private Logger _logger;
        
        public PaymentService()
        {
            _logger = Logger.GetInstance;
        }
        
        public void ProcessPayment(string userId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    throw new ArgumentException("Payment amount must be positive");
                }
                
                _logger.LogInfo($"Payment of ${amount} processed for user '{userId}'");
                
                if (amount > 1000)
                {
                    _logger.LogWarning($"Large payment of ${amount} detected for user '{userId}'. Verification required.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Payment processing failed: {ex.Message}");
            }
        }
    }
    
    // Demonstrate threading issues with singletons
    public class ThreadingDemo
    {
        public static void RunThreadingTest()
        {
            Console.WriteLine("\n----- THREADING TEST -----");
            Console.WriteLine("Creating logger instances from multiple threads...");
            
            Thread[] threads = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Getting logger instance");
                    Logger logger = Logger.GetInstance;
                    logger.LogInfo($"Log from thread {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(100);
                });
                
                threads[i].Start();
            }
            
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            
            Console.WriteLine($"Threading test complete. Instance count: {Logger.InstanceCount}");
            Console.WriteLine("----- END THREADING TEST -----\n");
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Singleton Pattern Homework - Logger System\n");
            
            Console.WriteLine("Creating first logger instance...");
            Logger logger1 = Logger.GetInstance;
            
            Console.WriteLine("\nCreating second logger instance...");
            Logger logger2 = Logger.GetInstance;
            
            Console.WriteLine($"\nAre both loggers the same instance? {ReferenceEquals(logger1, logger2)}");
            Console.WriteLine($"Total instances created: {Logger.InstanceCount} (should be 1)\n");
            
            ThreadingDemo.RunThreadingTest();
            
            UserService userService = new UserService();
            PaymentService paymentService = new PaymentService();
            
            userService.RegisterUser("john_doe");
            paymentService.ProcessPayment("john_doe", 99.99m);
            
            userService.RegisterUser("");
            paymentService.ProcessPayment("jane_doe", -50);
            
            paymentService.ProcessPayment("big_spender", 5000m);
            
            Logger.GetInstance.DisplayLogs();
            
            Logger.GetInstance.ClearLogs();
            
            Logger.GetInstance.LogInfo("Application shutting down");
            
            Logger.GetInstance.DisplayLogs();
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}