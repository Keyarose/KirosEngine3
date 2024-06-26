using KirosEngine3.Config;
using KirosEngine3.Exceptions;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.XML;
using OpenTK.Graphics.OpenGL4;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

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
        private int _size;
        private float _charPaddingX = 0.0f;

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
        /// Basic constructor for a font object.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <param name="filePath">The file path for the font data XML.</param>
        /// <exception cref="ArgumentException">Throw if the font fails to load due to an issue with the file given by filePath</exception>
        public Font(string name, string filePath)
        {
            _name = name;
            _filePath = filePath;

            _fontTextures = [];
            _charData = [];

            if (!LoadFontXML())
            {
                throw new ArgumentException(string.Format("Font failed to load with given file name: {0}", filePath), nameof(filePath));
            }
        }

        /// <summary>
        /// Basic constructor for a font object with a texture defined outside the font xml.
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="filePath">The file path for the font data xml</param>
        /// <param name="tex">The file path for the font bitmap</param>
        /// <exception cref="ArgumentException">Thrown if the font fails to load due to an issue with the file given by filePath</exception>
        public Font(string name, string filePath, string tex)
        {
            _name = name;
            _filePath = filePath;
            _fontTextures = [tex];

            _charData = [];

            if (!LoadFontXML())
            {
                throw new ArgumentException(string.Format("Font failed to load with given file name: {0}", filePath), nameof(filePath));
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

                    if (!ConfigManager.TryGetVar(ConfigKeys.D_DIR_FONT_KEY, out string? fontDir))
                        throw new MissingConfigException(string.Format("Default Font Directory not set with key: {0}", ConfigKeys.D_DIR_FONT_KEY));

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
        /// Get the width the text would have when rendered.
        /// </summary>
        /// <param name="text">The text to get the width of.</param>
        /// <returns>The width of the text.</returns>
        public float TextWidth(string text)
        {
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

            TexturedVertex2D[] textVerts = new TexturedVertex2D[text.Length * 4];
            uint[] textIndices = new uint[text.Length * 6];

            uint counterV = 0;
            int counterI = 0;
            //todo: kerning support
            foreach (char c in text)
            {
                CharInfo ci = _charData[c];
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
            TexturedVertex2D[] textVerts = new TexturedVertex2D[4];
            uint[] textIndices = new uint[6];

            uint counterV = 0;
            int counterI = 0;

            //todo: kerning support
            CharInfo ci = _charData[c];
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

            //shift start pos for next letter
            pos.X += ci.XAdvance + _charPaddingX;

            Tuple<TexturedVertex2D[], uint[]> result = new Tuple<TexturedVertex2D[], uint[]> (textVerts, textIndices);
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
            TexturedVertex2D[] textVerts = new TexturedVertex2D[4 * str.Length];
            uint[] textIndices = new uint[6 * str.Length];

            int vIndex = 0;
            int iIndex = 0;
            foreach(char c in str) 
            {
                var cRes = QuadForChar(c, pos);

                for (int i = 0; i < 4; i++)
                {
                    textVerts[vIndex + i] = cRes.Item1[i];
                }

                for (int i = 0; i < 6; i ++)
                {
                    textIndices[iIndex + i] = cRes.Item2[i] + (uint)vIndex;
                }

                pos.X += _charData[c].XAdvance + _charPaddingX;
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

            LoadFontXML();
        }
    }
}
