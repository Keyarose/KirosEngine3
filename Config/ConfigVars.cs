using KirosEngine3.Exceptions;
using KirosEngine3.Shaders;
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
                            case "Logs":
                                AddVar(ConfigKeys.D_DIR_LOG_KEY, cdd.Directory);
                                break;
                            default:
                                Console.WriteLine("Unhanded directory name: {1} encountered in {0}, is it a typo or did someone forget a case?", xmlFile, cdd.DirectoryName);
                                Logger.WriteToLog("Unhanded directory name: {1} encountered in {0}, is it a typo or did someone forget a case?", xmlFile, cdd.DirectoryName);
                                //todo: write to debug
                                break;
                        }
                    }

                    //todo: change font loading to create entries in the font manager.
                    //default font config
                    AddVar(ConfigKeys.D_FONT_NAME_KEY, data.Defaults.DefaultFont.DefaultFontName);
                    AddVar(ConfigKeys.D_FONT_FILE_KEY, data.Defaults.DefaultFont.DefaultFontFile);

                    //load default shaders
                    LoadDefaultShaderData(data.Defaults.DefaultShaders);

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
        /// Load the default shaders defined in the config file.
        /// </summary>
        /// <param name="defaultShaders">The shader data from the config file.</param>
        /// <exception cref="MissingConfigException">Thrown if the shader directory config is missing.</exception>
        /// <exception cref="NotImplementedException">Thrown if the shader data contains an unsupported attribute type.</exception>
        private static void LoadDefaultShaderData(ConfigDefaultShader[] defaultShaders)
        {
            string? shaderDir = GetVar(ConfigKeys.D_DIR_SHADER_KEY) ?? throw new MissingConfigException("Default Shader Directory not defined in configuration.");

            foreach (var shader in defaultShaders)
            {
                string vertPath = shaderDir + "/" + shader.VertFile;
                string fragPath = shaderDir + "/" + shader.FragFile;

                //get the shader attribute names
                ShaderAttribNames attribNames = new ShaderAttribNames();
                foreach (var name in shader.ShaderAttributes)
                {
                    switch (name.AttribType)
                    {
                        case "position":
                            attribNames.Position = name.AttribName;
                            break;
                        case "color":
                            attribNames.Color = name.AttribName;
                            break;
                        case "uv":
                            attribNames.UV = name.AttribName;
                            break;
                        case "normal":
                            attribNames.Normal = name.AttribName;
                            break;
                        default:
                            throw new NotImplementedException(string.Format("Shader does not have an implementation for shader attribute of type: {0}", name.AttribType));
                    }
                }

                ShaderManager.CreateShader(shader.ShaderName, vertPath, fragPath, attribNames);
            }
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
        public static string? GetVar(string name)
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

    /// <summary>
    /// Container struct for config key constants.
    /// Not an enum as keys can be custom defined.
    /// These are core system keys.
    /// </summary>
    public struct ConfigKeys
    {
        /// <summary>
        /// Default font directory key.
        /// </summary>
        public const string D_DIR_FONT_KEY = "ddirfont";
        /// <summary>
        /// Default texture directory key.
        /// </summary>
        public const string D_DIR_TEXTURE_KEY = "ddirtexture";
        /// <summary>
        /// Default shader directory key.
        /// </summary>
        public const string D_DIR_SHADER_KEY = "ddirshader";
        /// <summary>
        /// Default log directory key.
        /// </summary>
        public const string D_DIR_LOG_KEY = "ddirlog";
        /// <summary>
        /// Default font name key.
        /// </summary>
        public const string D_FONT_NAME_KEY = "dFontName";
        /// <summary>
        /// Default font file key.
        /// </summary>
        public const string D_FONT_FILE_KEY = "dFontFile";
    }

    //todo: move to the xml namespace as ConfigDataStruct.cs
    #region General Configuration Data structs
    /// <summary>
    /// Data structure for general configuration data.
    /// </summary>
    [Serializable]
    [XmlRoot("config")]
    public struct GenConfigData
    {
        /// <summary>
        /// The defaults section of the config data.
        /// </summary>
        [XmlElement("defaults")]
        public ConfigDefaults Defaults { get; set; }
    }

    /// <summary>
    /// Data structure for configuration default data.
    /// </summary>
    public struct ConfigDefaults
    {
        /// <summary>
        /// The default directories section.
        /// </summary>
        [XmlArray("directories")]
        [XmlArrayItem("directory")]
        public ConfigDefaultDir[] DefaultDirectories { get; set; }

        /// <summary>
        /// The default font section.
        /// </summary>
        [XmlElement("font")]
        public ConfigDefaultFont DefaultFont { get; set; }

        /// <summary>
        /// The default shaders section.
        /// </summary>
        [XmlArray("shaders")]
        [XmlArrayItem("shader")]
        public ConfigDefaultShader[] DefaultShaders { get; set; }
    }

    /// <summary>
    /// Data structure for default directory data.
    /// </summary>
    public struct ConfigDefaultDir
    {
        /// <summary>
        /// The name of the directory.
        /// </summary>
        [XmlAttribute("name")]
        public string DirectoryName { get; set; }

        /// <summary>
        /// The path to the directory.
        /// </summary>
        [XmlText]
        public string Directory { get; set; }
    }

    /// <summary>
    /// Data structure for default font data.
    /// </summary>
    public struct ConfigDefaultFont
    {
        /// <summary>
        /// The name of the font.
        /// </summary>
        [XmlAttribute("name")]
        public string DefaultFontName { get; set; }

        /// <summary>
        /// The name of the font's file.
        /// </summary>
        [XmlAttribute("file")]
        public string DefaultFontFile { get; set; }

        /// <summary>
        /// The file type of the file.
        /// </summary>
        [XmlAttribute("fType")]
        public string FileType { get; set; }
    }

    //todo: move to xml as ShaderDataStruct.cs
    /// <summary>
    /// Data structure for default shader data.
    /// </summary>
    public struct ConfigDefaultShader
    {
        /// <summary>
        /// The name of the shader.
        /// </summary>
        [XmlAttribute("name")]
        public string ShaderName { get; set; }

        /// <summary>
        /// The vertex shader file.
        /// </summary>
        [XmlAttribute ("vertShader")]
        public string VertFile { get; set; }

        /// <summary>
        /// The fragment shader file.
        /// </summary>
        [XmlAttribute ("fragShader")]
        public string FragFile { get; set; }

        /// <summary>
        /// The shader attributes.
        /// </summary>
        [XmlElement ("shaderAttribute")]
        public ConfigShaderAttrib[] ShaderAttributes { get; set; }
    }

    /// <summary>
    /// Data structure for shader attributes data.
    /// </summary>
    public struct ConfigShaderAttrib
    {
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string AttribName { get; set; }

        /// <summary>
        /// The type of the attribute. i.e: position, color, uv, normal, etc.
        /// </summary>
        [XmlAttribute ("type")]
        public string AttribType { get; set; }//position, color, uv, etc.

        /*[XmlAttribute ("value")]
        public ShaderValueType Value { get; set; }*/
    }
    #endregion
}
