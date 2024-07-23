using KirosEngine3.Config;
using KirosEngine3.Exceptions;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.XML;
using OpenTK.Graphics.OpenGL4;
using System.Xml.Serialization;

namespace KirosEngine3.Textures
{
    /// <summary>
    /// Defines a Font for use in text rendering
    /// </summary>
    public class Font
    {
        private readonly string _name;
        private readonly string _filePath;

        private string[] _fontTextures;
        private string _face = "";
        private int _size;
        private int _tabSize = 8;//defaults to 8 spaces
        private float _charPaddingX = 0.0f;

        private readonly Dictionary<char, CharInfo> _charData = [];
        private Vec2 _bitmapScale;

        private bool _loaded;

        /// <summary>
        /// The name of the font
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The face name of the font.
        /// </summary>
        public string Face
        {
            get { return _face; }
        }

        /// <summary>
        /// The font size
        /// </summary>
        public int Size
        { get { return _size; } }

        /// <summary>
        /// The font's tab size in spaces.
        /// </summary>
        public int TabSize
        {
            get { return _tabSize; }
            set { _tabSize = value; }
        }

        /// <summary>
        /// The size of the font's space.
        /// </summary>
        public int SpaceSize
        {
            get { return _charData[' '].Width; }
        }

        /// <summary>
        /// The amount of padding between each character in a text.
        /// </summary>
        /// <remarks>Default value of 1.0</remarks>
        public float CharPaddingSize
        { get { return _charPaddingX; } set { _charPaddingX = value; } }

        /// <summary>
        /// The font data
        /// </summary>
        public Dictionary<char, CharInfo> CharData
        {
            get { return _charData; }
        }

        /// <summary>
        /// Predefined CharInfo for unknown character or U+25A1 from the font.
        /// </summary>
        public CharInfo UnknownChar
        {
            get
            {
                if (_charData.TryGetValue((char)9633, out CharInfo value))
                {
                    return value;
                }
                return new CharInfo
                {
                    Height = _size,
                    Width = _charData[' '].Width,
                    XOffset = 0,
                    YOffset = 0,
                    X = 0,
                    Y = 0,
                    Page = 0,
                    XAdvance = 6
                };
            }
        }

        /// <summary>
        /// Basic constructor for a font object.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <param name="filePath">The file path for the font data XML.</param>
        public Font(string name, string filePath) : this(name, filePath, true)
        {
        }

        /// <summary>
        /// Basic constructor for a font object with a texture defined outside the font xml.
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="filePath">The file path for the font data xml</param>
        /// <param name="tex">The file path for the font bitmap</param>
        public Font(string name, string filePath, string tex)
        {
            _name = name;
            _filePath = filePath;
            _fontTextures = [tex];

            _charData = [];

            if (!LoadFontXMLDefDir())
            {
                Console.WriteLine("Font failed to load with given file name: {0}", filePath);
                Logger.WriteToLog("Font failed to load with given file name: {0}", filePath);
                DebugConsole.WriteLine("Font failed to load with given file name: {0}", filePath);
            }
        }

        /// <summary>
        /// Constructor for a font object that may load from a directory other than the default
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <param name="filePath">The file path for the data file.</param>
        /// <param name="fromDefaultDir">If true load from the application's default directory, if false treat the file path as the whole path.</param>
        public Font(string name, string filePath, bool fromDefaultDir)
        {
            _name = name;
            _filePath = filePath;
            _fontTextures = [];

            _charData = [];

            if (fromDefaultDir)
            {
                if (!LoadFontXMLDefDir())
                {
                    Console.WriteLine("Font failed to load with given file name: {0}", filePath);
                    Logger.WriteToLog("Font failed to load with given file name: {0}", filePath);
                    DebugConsole.WriteLine("Font failed to load with given file name: {0}", filePath);
                }
            }
            else
            {
                if (!LoadFontXML())
                {
                    Console.WriteLine("Font failed to load with given file name: {0}", filePath);
                }
            }
        }

        /// <summary>
        /// Load the font for unit testing.
        /// </summary>
        /// <returns>True if successfully loaded, false otherwise.</returns>
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

                    string fontDir = new FileInfo(_filePath).DirectoryName ?? "";

                    foreach (var pg in data.Pages)
                    {
                        string textureName = string.Format("Font_{0}_pg_{1}", Name, pg.Id);
                        TextureManager.AddTexture(textureName, fontDir + "/" + pg.File);
                        _fontTextures = [.. _fontTextures, textureName];
                    }

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

                    _loaded = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Load the font data from its XML file
        /// </summary>
        /// <returns>True if loading is successful, false otherwise</returns>
        private bool LoadFontXMLDefDir()
        {
            XmlSerializer serialize = new XmlSerializer(typeof(FontData));

            FontData data;

            try
            {
                if (!ConfigManager.TryGetVar(ConfigKeys.D_DIR_FONT_KEY, out string? fontDir))
                    throw new MissingConfigException(string.Format("Default Font Directory not set with key: {0}", ConfigKeys.D_DIR_FONT_KEY));

                using Stream sr = new FileStream(fontDir + "/" + _filePath, FileMode.Open);

                if (sr != null)
                {
                    data = (FontData)serialize.Deserialize(sr)!;

                    _face = data.Info.Face;
                    _size = data.Info.Size;
                    _bitmapScale = new Vec2(data.Common.ScaleW, data.Common.ScaleH);

                    foreach (var pg in data.Pages)
                    {
                        string textureName = string.Format("Font_{0}_pg_{1}", Name, pg.Id);
                        TextureManager.AddTexture(textureName, fontDir + "/" + pg.File);
                        _fontTextures = [.. _fontTextures, textureName];
                    }

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

                    _loaded = true;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.WriteToLog("Font {0} failed to load.", _name);
                Logger.WriteToLog(ex.Message);
                DebugConsole.WriteLine("Font {0} failed to load.", _name);
            }

            return false;
        }

        /// <summary>
        /// Get the width the text would have when rendered.
        /// </summary>
        /// <param name="text">The text to get the width of.</param>
        /// <returns>The width of the text.</returns>
        public float TextWidth(string text)
        {
            if (!_loaded)
                Console.WriteLine("Font is not loaded!");

            float result = 0f;

            foreach (char c in text)
            {
                CharInfo ci = _charData[c];

                result += ci.Width;
            }

            return result;
        }

        /// <summary>
        /// Construct a Text object for the given string
        /// </summary>
        /// <param name="text">the string text to be turned into a Text object</param>
        /// <param name="pos">The starting position for the text vertices.</param>
        /// <returns>The equivalent Text object for the string</returns>
        public SentenceData TextForString(string text, Vec2 pos)
        {
            return TextForString(text, pos, false);
        }

        /// <summary>
        /// Construct a Text object for the given string
        /// </summary>
        /// <param name="text">The string text to be turned into a Text object</param>
        /// <param name="pos">The starting position for the text vertices.</param>
        /// <param name="kerning">Whether or not to use kerning</param>
        /// <returns>The equivalent Text object for the string</returns>
        public SentenceData TextForString(string text, Vec2 pos, bool kerning)
        {
            SentenceData result = new SentenceData();

            Vec2 startPos = pos;

            TexturedVertex2D[] textVerts = new TexturedVertex2D[text.Length * 4];
            uint[] textIndices = new uint[text.Length * 6];

            uint counterV = 0;
            int counterI = 0;
            //todo: kerning support
            foreach (char c in text)//todo: handle newline and tab
            {
                CharInfo ci;

                try
                {
                    ci = _charData[c];
                }
                catch (KeyNotFoundException)
                {
                    ci = UnknownChar;//the character is not supported by the current font
                }

                if (c == '\n')//handle new line char
                {
                    pos = new(startPos.X, pos.Y + _size);
                    continue;
                }

                if (c == '\t')//handle tab char as TabSize number of spaces
                {
                    ci = _charData[' '];
                    ci.Width *= TabSize;
                    ci.XAdvance *= TabSize;
                }
                //OpenGL uv 0,0 is bottom left
                //tri 1
                //top left -4
                textVerts[counterV].Position = pos + new Vec2(ci.XOffset, ci.YOffset);
                textVerts[counterV].UV = new Vec2(ci.X, 1 - ci.Y);
                textIndices[counterI] = counterV;
                counterV++;
                counterI++;

                //top right -3
                textVerts[counterV].Position = pos + new Vec2(ci.Width, 0.0f) + new Vec2(ci.XOffset, ci.YOffset);
                textVerts[counterV].UV = new Vec2(ci.X + (float)(ci.Width / _bitmapScale.X), 1 - ci.Y);
                textIndices[counterI] = counterV;
                counterV++;
                counterI++;

                //bottom right -2
                textVerts[counterV].Position = pos + new Vec2(ci.Width, ci.Height) + new Vec2(ci.XOffset, ci.YOffset);
                textVerts[counterV].UV = new Vec2(ci.X + (float)(ci.Width / _bitmapScale.X), 1 - ci.Y - (float)(ci.Height / _bitmapScale.Y));
                textIndices[counterI] = counterV;
                counterV++;
                counterI++;

                //tri 2
                //bottom right
                textIndices[counterI] = counterV - 1;
                counterI++;

                //bottom left -1
                textVerts[counterV].Position = pos + new Vec2(0.0f, ci.Height) + new Vec2(ci.XOffset, ci.YOffset);
                textVerts[counterV].UV = new Vec2(ci.X, 1 - ci.Y - (float)(ci.Height / _bitmapScale.Y));
                textIndices[counterI] = counterV;
                counterV++;//increment for the next char
                counterI++;

                //top left
                textIndices[counterI] = counterV - 4;
                counterI++;//increment for the next char

                //shift start pos for next letter
                pos.X += ci.XAdvance + _charPaddingX;
            }

            result.Vertices = textVerts;
            result.Indexes = textIndices;

            return result;
        }

        /// <summary>
        /// Get the vertex data for a single char.
        /// </summary>
        /// <param name="c">The char to get the data for.</param>
        /// <param name="pos">The position for the char.</param>
        /// <returns>The resulting data arrays.</returns>
        public Tuple<TexturedVertex2D[], uint[]> QuadForChar(char c, Vec2 pos)
        {
            Vec2 startPos = pos;

            TexturedVertex2D[] textVerts = new TexturedVertex2D[4];
            uint[] textIndices = new uint[6];

            uint counterV = 0;
            int counterI = 0;

            //todo: kerning support
            CharInfo ci;

            try
            {
                ci = _charData[c];
            }
            catch (KeyNotFoundException)
            {
                ci = UnknownChar;//the character is not supported by the current font
            }

            if (c == '\n')//handle new line char
            {
                ci = _charData[' '];
            }

            if (c == '\t')//handle tab char as TabSize number of spaces
            {
                ci = _charData[' '];
                ci.Width *= TabSize;
                ci.XAdvance *= TabSize;
            }
            //OpenGL uv 0,0 is bottom left
            //tri 1
            //top left -4
            textVerts[counterV].Position = pos + new Vec2(ci.XOffset, ci.YOffset);
            textVerts[counterV].UV = new Vec2(ci.X, 1 - ci.Y);
            textIndices[counterI] = counterV;
            counterV++;
            counterI++;

            //top right -3
            textVerts[counterV].Position = pos + new Vec2(ci.Width, 0.0f) + new Vec2(ci.XOffset, ci.YOffset);
            textVerts[counterV].UV = new Vec2(ci.X + (float)(ci.Width / _bitmapScale.X), 1 - ci.Y);
            textIndices[counterI] = counterV;
            counterV++;
            counterI++;

            //bottom right -2
            textVerts[counterV].Position = pos + new Vec2(ci.Width, ci.Height) + new Vec2(ci.XOffset, ci.YOffset);
            textVerts[counterV].UV = new Vec2(ci.X + (float)(ci.Width / _bitmapScale.X), 1 - ci.Y - (float)(ci.Height / _bitmapScale.Y));
            textIndices[counterI] = counterV;
            counterV++;
            counterI++;

            //tri 2
            //bottom right
            textIndices[counterI] = counterV - 1;
            counterI++;

            //bottom left -1
            textVerts[counterV].Position = pos + new Vec2(0.0f, ci.Height) + new Vec2(ci.XOffset, ci.YOffset);
            textVerts[counterV].UV = new Vec2(ci.X, 1 - ci.Y - (float)(ci.Height / _bitmapScale.Y));
            textIndices[counterI] = counterV;
            counterI++;

            //top left
            textIndices[counterI] = counterV - 3;

            Tuple<TexturedVertex2D[], uint[]> result = new Tuple<TexturedVertex2D[], uint[]>(textVerts, textIndices);
            return result;
        }

        /// <summary>
        /// Get the vertex data for a string.
        /// </summary>
        /// <param name="str">The string to get the data for.</param>
        /// <param name="pos">The starting position for the data.</param>
        /// <returns>The resulting data.</returns>
        public Tuple<TexturedVertex2D[], uint[]> QuadsForString(string str, Vec2 pos)
        {
            Vec2 startPos = pos;

            TexturedVertex2D[] textVerts = new TexturedVertex2D[4 * str.Length];
            uint[] textIndices = new uint[6 * str.Length];

            int vIndex = 0;
            int iIndex = 0;
            foreach (char c in str)
            {
                var cRes = QuadForChar(c, pos);

                for (int i = 0; i < 4; i++)
                {
                    textVerts[vIndex + i] = cRes.Item1[i];
                }

                for (int i = 0; i < 6; i++)
                {
                    textIndices[iIndex + i] = cRes.Item2[i] + (uint)vIndex;
                }

                float posXInc = 0f;
                try
                {
                    posXInc = _charData[c].XAdvance + _charPaddingX;
                }
                catch (KeyNotFoundException) 
                {
                    posXInc = UnknownChar.XAdvance + _charPaddingX;
                }

                if (c == '\t')//handle tab char
                {
                    posXInc = _charData[' '].XAdvance * 8 + _charPaddingX;
                }

                pos.X += posXInc;

                if (c == '\n')//handle new line char
                {
                    pos = new(startPos.X, pos.Y + _size);
                }

                vIndex += 4;
                iIndex += 6;
            }

            Tuple<TexturedVertex2D[], uint[]> result = new Tuple<TexturedVertex2D[], uint[]>(textVerts, textIndices);
            return result;
        }

        /// <summary>
        /// Set the font to be used by the rending engine
        /// </summary>
        /// <param name="tu">The texture units for the font's textures</param>
        /// <returns>True if successfully set, false otherwise</returns>
        public bool UseFont(params TextureUnit[] tu)
        {
            if (tu.Length != _fontTextures.Length)
            {
                Console.WriteLine("Too few texture units provided to font: {0}", Name);
                Logger.WriteToLog("Too few texture units provided to font: {0}", Name);
                return false;
            }

            int i = 0;
            foreach (var texture in _fontTextures)//assign each texture for the font a TextureUnit
            {
                if (!TextureManager.UseTextureGL(texture, tu[i]))
                    return false;
                i++;
            }
            return true;
        }

        /// <summary>
        /// Reload the font from it's file
        /// </summary>
        public void Reload()
        {
            //cleanup first
            _charData.Clear();

            LoadFontXMLDefDir();
        }
    }
}
