using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KirosEngine3.Config
{
    /// <summary>
    /// Loads, stores and makes accessible Configuration values
    /// </summary>
    internal class ConfigVars
    {
        private static ConfigVars? _instance;

        private readonly Dictionary<string, string> _vars = [];

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private ConfigVars() { }

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static ConfigVars Instance
        { get { return _instance ??= new ConfigVars(); } }

        /// <summary>
        /// Accessor for the variables collection
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The variable's value</returns>
        public string this[string name]
        {
            get
            {
                return _vars[name];
            }

            set
            {
                _vars[name] = value;
            }
        }

        /// <summary>
        /// Load general configuration data from the provided xml file
        /// </summary>
        /// <param name="xmlFile">The general configuration xml file</param>
        /// <returns>True if successful</returns>
        public static bool LoadFromXML(string xmlFile)
        {
            XmlSerializer configSerialize = new XmlSerializer(typeof(GenConfigData));

            GenConfigData data;

            try
            {
                using Stream sr = new FileStream(xmlFile, FileMode.Open);

                if (sr != null)
                {
                    data = (GenConfigData)configSerialize.Deserialize(sr)!;

                    //default directory config
                    foreach (ConfigDefaultDir cdd in data.Defaults.DefaultDirectories)
                    {
                        switch (cdd.DirectoryName)
                        {
                            case "Fonts":
                                AddVar(ConfigKeys.D_DIR_FONT_KEY, cdd.Directory);
                                break;
                            case "Textures":
                                AddVar(ConfigKeys.D_DIR_TEXTURE_KEY, cdd.Directory);
                                break;
                            case "Shaders":
                                AddVar(ConfigKeys.D_DIR_SHADER_KEY, cdd.Directory);
                                break;
                            default:
                                Console.WriteLine("Unhanded directory name: {1} encountered in {0}, is it a typo or did someone forget a case?", xmlFile, cdd.DirectoryName);
                                Logger.WriteToLog("Unhanded directory name: {1} encountered in {0}, is it a typo or did someone forget a case?", xmlFile, cdd.DirectoryName);
                                //todo: write to debug
                                break;
                        }
                    }

                    //default font config
                    AddVar(ConfigKeys.D_FONT_NAME_KEY, data.Defaults.DefaultFont.DefaultFontName);
                    AddVar(ConfigKeys.D_FONT_FILE_KEY, data.Defaults.DefaultFont.DefaultFontFile);

                    sr.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.WriteToLog(string.Format("Failed to load general config file: {0}", xmlFile));
                Logger.WriteToLog(ex.Message);
                //todo: write to debug
            }

            return false;
        }

        /// <summary>
        /// Add a variable for app wide access
        /// </summary>
        /// <param name="name">The name for the variable</param>
        /// <param name="value">The value to be stored in the variable</param>
        /// <exception cref="ArgumentException">Thrown if the name is already in use</exception>
        public static void AddVar(string name, string value)
        {
            if (!Instance._vars.TryAdd(name, value))
            {
                throw new ArgumentException(string.Format("Variable already registered for the name: {0}", name));
            }
        }

        /// <summary>
        /// Try to add a variable for app wide access
        /// </summary>
        /// <param name="name">The name for the variable</param>
        /// <param name="value">The value to be store in the variable</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryAddVar(string name, string value)
        {
            if (Instance._vars.TryAdd(name, value))
            {
                return true;
            }

            Logger.WriteToLog(string.Format("Variable already registered for the name: {0}", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Try to remove a variable from app wide access
        /// </summary>
        /// <param name="name">The name of the variable to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryRemoveVar(string name)
        {
            return Instance._vars.Remove(name);
        }

        /// <summary>
        /// Get the variable for the given name
        /// </summary>
        /// <param name="name">The name of the variable to get</param>
        /// <returns>The value of the variable</returns>
        public static object? GetVar(string name)
        {
            if (Instance._vars.TryGetValue(name, out var obj))
            {
                return obj;
            }
            return null;
        }

        /// <summary>
        /// Try to get a variable using the given name
        /// </summary>
        /// <param name="name">The name of the variable to get</param>
        /// <param name="value">The value returned</param>
        /// <returns>True if the variable is found, false otherwise</returns>
        public static bool TryGetVar(string name, out string? value)
        {
            if (Instance._vars.TryGetValue(name, out value))
            {
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try to set the variable for the given name
        /// </summary>
        /// <param name="name">The name to set the variable as</param>
        /// <param name="value">The value of the variable</param>
        public static void SetVar(string name, string value)
        {
            if (Instance._vars.ContainsKey(name))
            {
                Instance._vars[name] = value;
            }
            else
            {
                AddVar(name, value);
            }
        }
    }

    public struct ConfigKeys
    {
        public const string D_DIR_FONT_KEY = "ddirfont";
        public const string D_DIR_TEXTURE_KEY = "ddirtexture";
        public const string D_DIR_SHADER_KEY = "ddirshader";
        public const string D_FONT_NAME_KEY = "dFontName";
        public const string D_FONT_FILE_KEY = "dFontFile";
    }

    #region General Configuration Data structs
    [Serializable]
    [XmlRoot("config")]
    public struct GenConfigData
    {
        [XmlElement("defaults")]
        public ConfigDefaults Defaults { get; set; }
    }

    public struct ConfigDefaults
    {
        [XmlArray("directories")]
        [XmlArrayItem("directory")]
        public ConfigDefaultDir[] DefaultDirectories { get; set; }

        [XmlElement("font")]
        public ConfigDefaultFont DefaultFont { get; set; }
    }

    public struct ConfigDefaultDir
    {
        [XmlAttribute("name")]
        public string DirectoryName { get; set; }

        [XmlText]
        public string Directory { get; set; }
    }

    public struct ConfigDefaultFont
    {
        [XmlAttribute("name")]
        public string DefaultFontName { get; set; }

        [XmlAttribute("file")]
        public string DefaultFontFile { get; set; }

        [XmlAttribute("fType")]
        public string FileType { get; set; }
    }
    #endregion
}
