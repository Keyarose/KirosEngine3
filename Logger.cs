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
        protected bool _enabled = true;

        //todo: configurable file path
        protected string _filePath = "log/"; //default file path
        protected FileInfo? _fileInfo;
        protected int _maxLogSizeMB = 512; //default log size in mb
        protected int _maxLogSizeL = 10000; //max log size in lines

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
            _filePath += string.Format("eventlog_{0}.log", now.ToString("MM/dd/yyyy-HH-mm-ss-fff"));

            //todo: load max log sizes from config file

            try
            {
                _fileInfo = new FileInfo(_filePath);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                //todo: write to in game console
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
                        //todo: write to in game console
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
                //todo: write to in game console
            }
        }

        /// <summary>
        /// Write a string to the log file
        /// </summary>
        /// <param name="message">The string to write</param>
        public static void WriteToLog(string message)
        {
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
                        //todo: write to in game console
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
                        //todo: write to in game console
                    }
                }
            }
        }

        public static void WriteToLog(int i)
        {
            WriteToLog(i.ToString());
        }

        public static void WriteToLog(float f)
        {
            WriteToLog(f.ToString());
        }

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
                Console.WriteLine(ex.Message);
                //todo: write to in game console
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
                    //todo: write to in game console
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
