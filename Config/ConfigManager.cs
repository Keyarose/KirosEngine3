using KirosEngine3.Exceptions;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using KirosEngine3.XML;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace KirosEngine3.Config
{
    /// <summary>
    /// Loads, stores and makes accessible Configuration values
    /// </summary>
    public class ConfigManager
    {
        private static ConfigManager? _instance;

        private readonly Dictionary<string, string> _vars = [];

        /// <summary>
        /// Singleton constructor
        /// </summary>
        private ConfigManager() { }

        /// <summary>
        /// Singleton accessor
        /// </summary>
        public static ConfigManager Instance
        { get { return _instance ??= new ConfigManager(); } }

        /// <summary>
        /// Accessor for the variables collection
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The variable's value, or empty string if not found.</returns>
        public string this[string name]
        {
            get
            {
                if (_vars.TryGetValue(name, out var value)) 
                    return value;
                else
                {
                    Console.WriteLine("Config variable named: {0} does not exist.", name);
                    Logger.WriteToLog("Config variable named: {0} does not exist.", name);
                    //todo:write to debug
                    return string.Empty;
                }
            }

            set
            {
                if (_vars.ContainsKey(name))
                    _vars[name] = value;
                else
                    TryAddVar(name, value);
            }
        }

        /// <summary>
        /// The graphics mode for the application.
        /// </summary>
        public static string GraphicsMode
        {
            get
            {
                if (TryGetVar(Client.GRAPHICSMODE_KEY, out string? mode))
                    return mode;
                else
                    throw new MissingConfigException(string.Format("Graphics mode is not configured or incorrect. Name: {0}", Client.GRAPHICSMODE_KEY));
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
                                
                                break;
                        }
                    }

                    //load fonts and set the default one
                    foreach (ConfigDefaultFont df in data.Defaults.DefaultFonts)
                    {
                        FontManager.CreateFont(df.DefaultFontName, df.DefaultFontFile + df.FileType);

                        if (df.Default)
                        {
                            if (FontManager.TryGetFont(df.DefaultFontName, out Font? fnt))
                            {
                                FontManager.SetDefault(fnt);
                            }
                            else
                            {
                                throw new Exception(string.Format("Failed to initialize default font. Name: {0}", df.DefaultFontName));
                            }
                        }
                    }

                    //load default shaders
                    LoadDefaultShaderData(data.Defaults.DefaultShaders);

                    //load debug console, requires both fonts and shaders to load first
                    LoadDebugConsole(data.SystemUI.DebugConsoleValues);

                    //load colors
                    var clearColor = data.Defaults.DefaultColors.Where(colors => colors.Name.Equals("clearColor")).First();
                    string cColor = clearColor.RValue + "," + clearColor.GValue + "," + clearColor.BValue + "," + clearColor.AValue;
                    AddVar(ConfigKeys.D_CLEAR_COLOR_KEY, cColor);
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

        private static void LoadDebugConsole(ConfigDebugConsole consoleSettings)
        {
            if (!TryGetVar("ScreenWidth", out string? widthStr))
                throw new MissingConfigException("Screen Width was not set.");

            int screenWidth = int.Parse(widthStr);

            if (!TryGetVar("ScreenHeight", out string? heightStr))
                throw new MissingConfigException("Screen Height was not set.");

            int screenHeight = int.Parse(heightStr);

            //todo: exception handling
            //parse settings
            float posPercent = float.Parse(consoleSettings.PosX.TrimEnd(['%', ' '])) / 100f;
            int posX = (int)(posPercent * screenWidth);

            float posYPer = float.Parse(consoleSettings.PosY.TrimEnd(['%', ' '])) / 100f;
            int posY = (int)(posYPer * screenHeight);

            float widthPer = float.Parse(consoleSettings.Width.TrimEnd(['%', ' '])) / 100f;
            int width = (int)(widthPer * screenWidth);

            float heightPer = float.Parse(consoleSettings.Height.TrimEnd(['%', ' '])) / 100f;
            int height = (int)(heightPer * screenHeight);

            int maxLine = int.Parse(consoleSettings.MaxLines);

            if (!FontManager.TryGetFont(consoleSettings.Font, out Font? font))
            {
                Console.WriteLine("Font for Debug Console not found, are you trying to load it after the console?");
                Logger.WriteToLog("Font for Debug Console not found, are you trying to load it after the console?");
            }

            DebugConsole.Create(posX, posY, width, height, maxLine, font);
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

                if (shader.DefaultFor != null && shader.DefaultFor.Equals("text"))
                {
                    AddVar(ConfigKeys.D_SHADER_TEXT_NAME_KEY, shader.ShaderName);
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
        public static bool TryGetVar(string name, [NotNullWhen(true)] out string? value)
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
        /// Default shader for text name key.
        /// </summary>
        public const string D_SHADER_TEXT_NAME_KEY = "dShTextName";
        /// <summary>
        /// Default clear color key.
        /// </summary>
        public const string D_CLEAR_COLOR_KEY = "dClearColor";
    }
}
