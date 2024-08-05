using KirosEngine3.Config;
using KirosEngine3.Debug;

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
        /// The current log file.
        /// </summary>
        public string CurrentLogFile
        {
            get { return _filePath; }
        }

        /// <summary>
        /// Logger initialization, called only once 
        /// </summary>
        protected Logger()
        {
            DateTime now = DateTime.Now;

            //default file path and name
            _filePath = ConfigManager.Instance[ConfigKeys.D_DIR_LOG_KEY] + string.Format("/eventlog_{0}.log", now.ToString("MM-dd-yyyy-HH-mm-ss-fff"));

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

                if (!_fileInfo.Exists)
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

            //setup Logger commands
            CommandManager.RegisterCommand("writeToLog", new Action<string>(WriteToLog));
            CommandManager.RegisterCommand("listLogFiles", new Action<int, int, int, int, int>(OutputLogList));
            CommandManager.RegisterCommand("listLogFiles", new Action<string>(OutputLogList));
            CommandManager.RegisterCommand("deleteAllLogs", new Func<bool>(DeleteAllLogs));
            CommandManager.RegisterCommand("deleteLog", new Func<string, bool>(DeleteLog));
        }

        //todo: write to fallback for when config fails to load need info for write to log

        /// <summary>
        /// Write a string to the log file
        /// </summary>
        /// <param name="message">The string to write</param>
        public static void WriteToLog(string message)
        {
#if !UNIT_TEST
            if (Instance._enabled && (Instance._fileInfo != null))
            {
                if (!Instance.IsLogFull())
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

            if (_fileInfo != null)
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
            if (_fileInfo != null)
            {
                if (_currentLineCount < _maxLogSizeL && _fileInfo.Length / 1000000 < _maxLogSizeMB)
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

        #region Cmd Methods
        /// <summary>
        /// Output a list of log files that match the search pattern.
        /// </summary>
        /// <param name="searchPattern">The optional pattern to filter the log files by.</param>
        public static void OutputLogList(string searchPattern = "")
        {
            (string, string)[] list = ListLogs(searchPattern);

            for (int i = 0; i < list.Length; i++)
            {
                Client.Report("{0}\t{1}", list[i].Item1, list[i].Item2);
            }
        }

        /// <summary>
        /// Output a list of log files that match the given values.
        /// </summary>
        /// <param name="month">The month of the log.</param>
        /// <param name="day">Optional, the day of the log. 0 to wildcard.</param>
        /// <param name="year">Optional, the year of the log. 0 to wildcard.</param>
        /// <param name="hour">Optional, the hour of the log. 0 to wildcard.</param>
        /// <param name="minute">Optional, the minute of the log. 0 to wildcard.</param>
        public static void OutputLogList(int month, int day = 0, int year = 0, int hour = 0, int minute = 0)
        {
            (string, string)[] list = ListLogs(month, day, year, hour, minute);

            for (int i = 0; i < list.Length; i++)
            {
                Client.Report("{0}\t{1}", list[i].Item1, list[i].Item2);
            }
        }
        #endregion

        #region Log File Management
        /// <summary>
        /// Get a list of log files in the log directory.
        /// </summary>
        /// <param name="searchPattern">The optional pattern to filter the log files by.</param>
        /// <returns>The list as an array of tuple(fileName, date).</returns>
        public static (string, string)[] ListLogs(string searchPattern = "")
        {
            (string, string)[] result = [];
            string dir = ConfigManager.Instance[ConfigKeys.D_DIR_LOG_KEY];

            DirectoryInfo logDirInfo = new DirectoryInfo(dir);
            FileInfo[] files;

            if (searchPattern != string.Empty)
                files = logDirInfo.EnumerateFiles(searchPattern).ToArray();
            else
                files = logDirInfo.EnumerateFiles().ToArray();

            foreach (FileInfo file in files)
            {
                result = [.. result, (file.Name, file.LastWriteTime.ToString())];
            }

            return result;
        }

        /// <summary>
        /// Get a list of log files in the log directory filtered by month, and optionally day, year, hour, and/or minute.
        /// </summary>
        /// <param name="month">The month the log was created in.</param>
        /// <param name="day">Optional, the day the log was created.</param>
        /// <param name="year">Optional, the year the log was created.</param>
        /// <param name="hour">Optional, the hour the log was created.</param>
        /// <param name="minute">Optional, the minute the log was created.</param>
        /// <returns>The list of log files that fit the filter.</returns>
        public static (string, string)[] ListLogs(int month, int day = 0, int year = 0, int hour = 0, int minute = 0)
        {
            string searchPat = string.Format("eventlog_{0}", month.ToString("D2"));

            if (day > 0)
                searchPat += string.Format("-{0}", day.ToString("D2"));
            else
                searchPat += "-*";

            if (year > 0)
                searchPat += string.Format("-{0}", year.ToString());
            else
                searchPat += "-*";

            if (hour > 0)
                searchPat += string.Format("-{0}", hour.ToString("D2"));
            else
                searchPat += "-*";

            if (minute > 0)
                searchPat += string.Format("-{0}", minute.ToString("D2"));
            else
                searchPat += "-*";

            return ListLogs(searchPat);
        }

        /// <summary>
        /// Delete the specified log file.
        /// </summary>
        /// <param name="log">The log file to delete.</param>
        /// <returns>True if the log file is deleted, false otherwise.</returns>
        public static bool DeleteLog(string log)
        {
            FileInfo? logFile;
            try
            {
                logFile = new FileInfo(ConfigManager.Instance[ConfigKeys.D_DIR_LOG_KEY] + "/" + log);
            }
            catch (Exception e)
            {
                Client.Report(e.ToString());
                return false;
            }

            if (logFile != null && logFile.Exists)
            {
                //if the log file isn't the current log file delete it
                if (Instance._fileInfo == null || Instance._fileInfo.FullName != logFile.FullName)
                {
                    logFile.Delete();
                }
                else
                {
                    Client.Report("Cannot delete Log File: {0} as it is the active log file.", logFile.Name);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Delete the provided list of log files.
        /// </summary>
        /// <param name="logs">The log files to be deleted.</param>
        /// <returns>True if all logs delete successfully, false if any fail to be deleted.</returns>
        public static bool DeleteLogs(string[] logs)
        {
            bool complete = true;
            foreach (string log in logs) 
            {
                if (!DeleteLog(log))
                    complete = false;
            }

            return complete;
        }

        /// <summary>
        /// Delete all the logs other than the active one.
        /// </summary>
        /// <returns>True if all logs delete successfully, false if any fail to be deleted.</returns>
        public static bool DeleteAllLogs()
        {
            (string, string)[] list = ListLogs();
            string[] files = list.Select(x => x.Item1).ToArray();
            
            if (Instance._fileInfo != null)
            {
                files = files.Where((val) => (val != Instance._fileInfo.Name)).ToArray();
            }

            return DeleteLogs(files);
        }
        #endregion
    }
}
