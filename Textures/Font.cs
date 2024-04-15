using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KirosEngine3.Textures
{
    /// <summary>
    /// Defines the location and size of the char
    /// </summary>
    public struct CharData
    {
        [XmlAttribute]
        public int x;
        [XmlAttribute]
        public int y;
        [XmlAttribute]
        public int width;
        [XmlAttribute]
        public int height;
    }

    /// <summary>
    /// Defines a Font for use in text rendering
    /// </summary>
    public class Font
    {
        private readonly string _name;
        private readonly string _filePath;

        private readonly string _fontTexture;
        private int _size;
        private readonly float _spaceSize = 3.0f;//todo: accessors
        private float _charSpaceSize = 1.0f;
        
        private readonly Dictionary<char, CharInfo> _charData = [];
        private Vec2 _bitmapScale;

        /// <summary>
        /// The name of the font
        /// </summary>
        public string Name
        {
            get { return _name; } 
        }

        /// <summary>
        /// The font size
        /// </summary>
        public int Size
        { get { return _size; } }

        /// <summary>
        /// The font data
        /// </summary>
        public Dictionary<char, CharInfo> CharData
        {
            get { return _charData; }
        }

        /// <summary>
        /// The default font
        /// </summary>
        public static Font Default = new Font("", "", "");//todo: define environment vars for default font and textures

        /// <summary>
        /// Basic constructor for a font object
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="filePath">The file path for the font data xml</param>
        /// <param name="tex">The file path for the font bitmap</param>
        /// <exception cref="ArgumentException">Thrown if the font fails to load due to an issue with the file given by filePath</exception>
        public Font(string name, string filePath, string tex)
        {
            _name = name;
            _filePath = filePath;
            _fontTexture = tex;
            
            _charData = [];

            if(!LoadFontXML())
            {
                throw new ArgumentException(string.Format("Font failed to load with given file name: {0}", filePath), nameof(filePath));
            }
            
            //if the font's texture isn't loaded then do so
            if (TextureManager.TryGetTexture(tex, out Texture? fontText))
            {
                if (!fontText.IsLoaded)
                {
                    fontText.Load();
                }
            }
        }

        /// <summary>
        /// Load the font data from its XML file
        /// </summary>
        /// <returns>True if loading is successful, false otherwise</returns>
        private bool LoadFontXML() 
        {
            XmlSerializer serialize = new XmlSerializer(typeof(FontData));

            FontData data;

            try
            {
                using Stream sr = new FileStream(_filePath, FileMode.Open);

                if (sr != null)
                {
                    data = (FontData)serialize.Deserialize(sr)!;

                    _size = data.Info.Size;
                    _bitmapScale = new Vec2(data.Common.ScaleW, data.Common.ScaleH);

                    foreach (CharInfo ci in data.Chars)
                    {
                        //convert char x/y to bitmap ratio
                        CharInfo c = ci;
                        c.X /= data.Common.ScaleW;
                        c.Y /= data.Common.ScaleH;

                        //copy each char data to the collection with the key being the respective char
                        _charData.Add((char)ci.Id, c);
                    }
                    sr.Close();

                    return true;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                Logger.WriteToLog(string.Format("Font {0} failed to load.", _name));
                Logger.WriteToLog(ex.Message);
                //todo: write to debug
            }

            return false;
        }

        /// <summary>
        /// Construct a Text object for the given string
        /// </summary>
        /// <param name="text">the string text to be turned into a Text object</param>
        /// <returns>The equivalent Text object for the string</returns>
        public SentenceData TextForString(string text, Vec3 pos)
        {
            return TextForString(text, pos, false);
        }

        /// <summary>
        /// Construct a Text object for the given string
        /// </summary>
        /// <param name="text">The string text to be turned into a Text object</param>
        /// <param name="kerning">Whether or not to use kerning</param>
        /// <returns>The equivalent Text object for the string</returns>
        public SentenceData TextForString(string text, Vec3 pos, bool kerning)
        {
            SentenceData result = new SentenceData();

            TexturedVertex[] textVerts = new TexturedVertex[text.Length * 4];
            uint[] textIndices = new uint[text.Length * 6];

            uint counterV = 0;
            int counterI = 0;

            foreach (char c in text) 
            {
                if (c == ' ')
                {
                    //for a space just shift the start position by space size
                    pos.X =+ _spaceSize;
                }
                else
                {
                    CharInfo ci = _charData[c];
                    //opengl uv 0,0 is bottom left
                    //tri 1
                    //top left -4
                    textVerts[counterV].Position = pos;
                    textVerts[counterV].UV = new Vec2(ci.X, ci.Y);
                    counterV++;

                    //bottom right -3
                    textVerts[counterV].Position = pos + new Vec3(ci.Width, -ci.Height, 0.0f);
                    textVerts[counterV].UV = new Vec2(ci.X + (float)(ci.Width / _bitmapScale.X), ci.Y + (float)(ci.Height / _bitmapScale.Y));
                    counterV++;

                    //bottom left -2
                    textVerts[counterV].Position = pos + new Vec3(0.0f, -ci.Height, 0.0f);
                    textVerts[counterV].UV = new Vec2(ci.X, ci.Y + (float)(ci.Height / _bitmapScale.Y));
                    counterV++;

                    //tri 2
                    //top left

                    //top right -1
                    textVerts[counterV].Position = pos + new Vec3(ci.Width, 0.0f, 0.0f);
                    textVerts[counterV].UV = new Vec2(ci.X + (float)(ci.Width / _bitmapScale.X), ci.Y);
                    counterV++; //increment for the next char

                    //bottom right

                    //indices
                    //tri 1
                    textIndices[counterI] = counterV - 4;
                    textIndices[counterI + 1] = counterV - 3;
                    textIndices[counterI + 2] = counterV - 2;
                    //tri 2
                    textIndices[counterI + 3] = counterV - 4;
                    textIndices[counterI + 4] = counterV - 1;
                    textIndices[counterI + 5] = counterV - 3;
                    counterI += 6; //increment for the next char

                    //shift start pos for next letter
                    pos.X += ci.Width + _charSpaceSize;
                }
            }

            result.VertexBuffer = textVerts;
            result.IndexBuffer = textIndices;

            return result;
        }

        /// <summary>
        /// Set the font to be used by the rending engine
        /// </summary>
        /// <param name="tu">The texture unit for the font's texture</param>
        /// <returns>True if successfully set, false otherwise</returns>
        public bool UseFont(TextureUnit tu)
        {
            return TextureManager.UseTextureGL(_fontTexture, tu);
        }

        /// <summary>
        /// Reload the font from it's file
        /// </summary>
        public void Reload()
        {
            //cleanup first
            _charData.Clear();

            LoadFontXML();
        }
    }

    [XmlRoot("font")]
    public struct FontData
    {
        [XmlElement("info")]
        public FontInfo Info { get; set; }

        [XmlElement("common")]
        public FontCommon Common { get; set; }

        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public FontPage[] Pages { get; set; }

        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public CharInfo[] Chars { get; set; }

        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public Kerning[] Kernings { get; set; }
    }

    /// <summary>
    /// Font info XML
    /// </summary>
    public struct FontInfo
    {
        [XmlAttribute("face")]
        public string Face { get; set; }

        [XmlAttribute("size")]
        public int Size { get; set; }

        [XmlAttribute("bold")]
        public bool Bold { get; set; }

        [XmlAttribute("italic")]
        public bool Italic { get; set; }
    }

    /// <summary>
    /// Font Common XML
    /// </summary>
    public struct FontCommon
    {
        [XmlAttribute("lineHeight")]
        public int LineHeight { get; set; }

        [XmlAttribute("base")]
        public int Base { get; set; }

        [XmlAttribute("scaleW")]
        public float ScaleW { get; set; }

        [XmlAttribute("scaleH")]
        public float ScaleH { get; set; }
    }

    /// <summary>
    /// Font texture page XML
    /// </summary>
    public struct FontPage
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("file")]
        public string File { get; set; }
    }

    /// <summary>
    /// Info of a char
    /// </summary>
    public struct CharInfo
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("x")]
        public float X { get; set; }

        [XmlAttribute("y")]
        public float Y { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("xoffset")]
        public int XOffset { get; set; }

        [XmlAttribute("yoffset")]
        public int YOffset { get; set; }

        [XmlAttribute("xadvance")]
        public int XAdvance { get; set; }

        [XmlAttribute("page")]
        public int Page { get; set; }
    }

    /// <summary>
    /// Kerning data
    /// </summary>
    public struct Kerning
    {
        [XmlAttribute("first")]
        public int First { get; set; }

        [XmlAttribute("second")]
        public int Second { get; set; }

        [XmlAttribute("amount")]
        public int Amount { get; set; }
    }
}
