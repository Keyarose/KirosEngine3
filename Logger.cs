using KirosEngine3.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3
{
    /// <summary>
    /// Writes information to a dated log file during application activity.
    /// </summary>
    public class Logger
    {
        private static Logger? _instance;
        /// <summary>
        /// Flag to denote if logging is enabled or disabled.
        /// </summary>
        protected bool _enabled = true;

        /// <summary>
        /// The file path for the active log file.
        /// </summary>
        protected string _filePath;
        /// <summary>
        /// The information of the active log file.
        /// </summary>
        protected FileInfo? _fileInfo;
        /// <summary>
        /// The maximum allowed log size in MB.
        /// </summary>
        protected int _maxLogSizeMB = 512;
        /// <summary>
        /// The maximum allowed log size in lines.
        /// </summary>
        protected int _maxLogSizeL = 10000;

        /// <summary>
        /// The line count of the active log file.
        /// </summary>
        protected int _currentLineCount = 0;

        /// <summary>
        /// Accessor for the logger instance
        /// </summary>
        public static Logger Instance
        {
            get
            {
                _instance ??= new Logger();
                return _instance;
            }
        }

        /// <summary>
        /// The maximum log size in MB
        /// </summary>
        public int MaxLogSizeMB
        {
            get { return _maxLogSizeMB; }
        }

        /// <summary>
        /// The maximum log size in lines
        /// </summary>
        public int MaxLogSizeL
        {
            get { return _maxLogSizeL; }
        }

        /// <summary>
        /// The number of lines currently in the log
        /// </summary>
        public int LineCount
        {
            get { return _currentLineCount; }
        }

        /// <summary>
        /// Logger initialization, called only once 
        /// </summary>
        protected Logger() 
        {
            DateTime now = DateTime.Now;

            //default file path and name
            _filePath = ConfigManager.Instance[ConfigKeys.D_DIR_LOG_KEY] + string.Format("/eventlog_{0}.log", now.ToString("MM/dd/yyyy-HH-mm-ss-fff"));

            //todo: load max log sizes from config file

            try
            {
                _fileInfo = new FileInfo(_filePath);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                DebugConsole.WriteLine(ex.ToString());
                return;
            }

            //if file info is not null continue log setup else write an error to the consoles
            if (_fileInfo != null)
            {
                _fileInfo.Directory?.Create();

                if(!_fileInfo.Exists)
                {
                    try
                    {
                        //create the file and write initial data to it
                        using StreamWriter sw = _fileInfo.CreateText();
                        sw.WriteLine(string.Format("Log File for: {0}", now.ToString("MM/dd/yyyy-HH:mm:ss.fff")));
                        sw.WriteLine("Log Init");

                        _currentLineCount = 2;
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        DebugConsole.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    //the file already existed, so append rather than overwrite after getting the line count
                    _currentLineCount = File.ReadLines(_fileInfo.FullName).Count();

                    using StreamWriter sw = _fileInfo.AppendText();
                    sw.WriteLine(string.Format("Log Reinitialized at: {0}", DateTime.Now.ToString("MM/dd/yyyy-HH:mm:ss.fff")));
                    _currentLineCount++;
                }
            }
            else 
            {
                Console.WriteLine("Failure to setup the log file, see the last exception message.");
                DebugConsole.WriteLine("Failure to setup the log file, see the last exception message.");
            }
        }

        //todo: write to fallback for when config fails to load need info for write to log

        /// <summary>
        /// Write a string to the log file
        /// </summary>
        /// <param name="message">The string to write</param>
        public static void WriteToLog(string message)
        {
#if !UNIT_TEST
            if(Instance._enabled && (Instance._fileInfo != null))
            {
                if(!Instance.IsLogFull())
                {
                    try
                    {
                        //append the message to the log prefixed by the datetime to millisecond
                        using StreamWriter sw = Instance._fileInfo.AppendText();
                        sw.WriteLine(string.Format("{0} : {1}", DateTime.Now.ToString("MM/dd/yy-HH:mm:ss.fff"), message));
                        Instance._currentLineCount++;
                    }
                    catch (IOException ex) 
                    {
                        Console.WriteLine(ex.ToString());
                        DebugConsole.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    Instance.BeginNewLog();

                    try
                    {
                        //append the message to the log prefixed by the datetime to millisecond
                        using StreamWriter sw = Instance._fileInfo.AppendText();
                        sw.WriteLine(string.Format("{0} : {1}", DateTime.Now.ToString("MM/dd/yy-HH:mm:ss.fff"), message));
                        Instance._currentLineCount++;
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        DebugConsole.WriteLine(ex.ToString());
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Write a formatted string to the log file
        /// </summary>
        /// <param name="message">The string format to write</param>
        /// <param name="arg0">The data to be inserted into the format</param>
        public static void WriteToLog(string message, object? arg0)
        {
            WriteToLog(string.Format(message, arg0));
        }

        /// <summary>
        /// Write a formatted string to the log file
        /// </summary>
        /// <param name="message">The string format to write</param>
        /// <param name="args">Data to be inserted into the format</param>
        public static void WriteToLog(string message, params object?[] args)
        {
            WriteToLog(string.Format(message, args));
        }

        /// <summary>
        /// Write an int to the log file.
        /// </summary>
        /// <param name="i">The int to write.</param>
        public static void WriteToLog(int i)
        {
            WriteToLog(i.ToString());
        }

        /// <summary>
        /// Write a float to the log file.
        /// </summary>
        /// <param name="f">The float to write.</param>
        public static void WriteToLog(float f)
        {
            WriteToLog(f.ToString());
        }

        /// <summary>
        /// Write an exception to the log file.
        /// </summary>
        /// <param name="ex">The exception to write.</param>
        public static void WriteToLog(Exception ex)
        {
            WriteToLog(ex.ToString());
        }

        /// <summary>
        /// Create a new log file for the current date time and set it to be used. Called when the current log is full.
        /// </summary>
        protected void BeginNewLog()
        {
            DateTime now = DateTime.Now;

            _filePath = "log/" + string.Format("eventlog_{0}.log", now.ToString("MM/dd/yyyy-HH-mm-ss-fff"));

            try
            {
                _fileInfo = new FileInfo(_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                DebugConsole.WriteLine(ex.ToString());
            }

            if(_fileInfo != null)
            {
                try
                {
                    //create the file and write initial data to it
                    using StreamWriter sw = _fileInfo.CreateText();
                    sw.WriteLine(string.Format("Log File for: {0}", now.ToString("MM/dd/yyyy-HH:mm:ss.fff")));
                    sw.WriteLine("Log Init");

                    _currentLineCount = 2;
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.ToString());
                    DebugConsole.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Check that the log file is not larger than allowed.
        /// </summary>
        /// <returns>True if the file is full or doesn't exist, false if there is room to write to</returns>
        protected bool IsLogFull()
        {
            if(_fileInfo != null)
            {
                if(_currentLineCount < _maxLogSizeL && _fileInfo.Length / 1000000 < _maxLogSizeMB)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            //the file doesn't exist so it can't be written to anyway
            return true;
        }
    }
}
