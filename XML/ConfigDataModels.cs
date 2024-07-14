using System.Xml.Serialization;

namespace KirosEngine3.XML
{
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

        /// <summary>
        /// The system UI section of the config data.
        /// </summary>
        [XmlElement("systemUI")]
        public ConfigSystemUI SystemUI { get; set; }
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
        [XmlArray("fonts")]
        [XmlArrayItem("font")]
        public ConfigDefaultFont[] DefaultFonts { get; set; }

        /// <summary>
        /// The default shaders section.
        /// </summary>
        [XmlArray("shaders")]
        [XmlArrayItem("shader")]
        public ConfigDefaultShader[] DefaultShaders { get; set; }

        /// <summary>
        /// The default colors section.
        /// </summary>
        [XmlArray("colors")]
        [XmlArrayItem("color")]
        public ConfigDefaultColor[] DefaultColors { get; set; }
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

        /// <summary>
        /// Flag to indicate if the font is the default, should be set true for only one.
        /// </summary>
        [XmlAttribute("default")]
        public bool Default { get; set; }
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
        [XmlAttribute("vertShader")]
        public string VertFile { get; set; }

        /// <summary>
        /// The fragment shader file.
        /// </summary>
        [XmlAttribute("fragShader")]
        public string FragFile { get; set; }

        /// <summary>
        /// Marks if the shader is a default for a specific type of rendering.
        /// </summary>
        [XmlAttribute("defaultFor")]
        public string DefaultFor { get; set; }

        /// <summary>
        /// The shader attributes.
        /// </summary>
        [XmlElement("shaderAttribute")]
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
        [XmlAttribute("type")]
        public string AttribType { get; set; }//position, color, uv, etc.

        /*[XmlAttribute ("value")]
        public ShaderValueType Value { get; set; }*/
    }

    /// <summary>
    /// Data structure for default color.
    /// </summary>
    public struct ConfigDefaultColor
    {
        /// <summary>
        /// The name of the color.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        /// <summary>
        /// The color's r value
        /// </summary>
        [XmlAttribute("r")]
        public string RValue { get; set; }
        /// <summary>
        /// The color's g value
        /// </summary>
        [XmlAttribute("g")]
        public string GValue { get; set; }
        /// <summary>
        /// The color's b value
        /// </summary>
        [XmlAttribute("b")]
        public string BValue { get; set; }
        /// <summary>
        /// The color's a value
        /// </summary>
        [XmlAttribute("a")]
        public string AValue { get; set; }
    }

    /// <summary>
    /// Data structure for system UI.
    /// </summary>
    public struct ConfigSystemUI
    {
        /// <summary>
        /// The settings for the debug console.
        /// </summary>
        [XmlElement("debugConsole")]
        public ConfigDebugConsole DebugConsoleValues { get; set; }
    }

    /// <summary>
    /// Data structure for the debug console settings.
    /// </summary>
    public struct ConfigDebugConsole
    {
        /// <summary>
        /// The X position of the debug console in screen coordinates.
        /// </summary>
        [XmlAttribute("posX")]
        public string PosX { get; set; }

        /// <summary>
        /// The Y position of the debug console in screen coordinates.
        /// </summary>
        [XmlAttribute("posY")]
        public string PosY { get; set; }

        /// <summary>
        /// The width of the debug console.
        /// </summary>
        [XmlAttribute("width")]
        public string Width { get; set; }

        /// <summary>
        /// The height of the debug console.
        /// </summary>
        [XmlAttribute("height")]
        public string Height { get; set; }

        /// <summary>
        /// The maximum number of lines in the console's textbox.
        /// </summary>
        [XmlAttribute("maxLines")]
        public string MaxLines { get; set; }

        /// <summary>
        /// The name of the font to use in the console.
        /// </summary>
        [XmlAttribute("font")]
        public string Font { get; set; }
    }
}
