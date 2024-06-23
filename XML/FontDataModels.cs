using System.Xml.Serialization;

namespace KirosEngine3.XML
{
    /// <summary>
    /// Data structure for font data.
    /// </summary>
    [XmlRoot("font")]
    public struct FontData
    {
        /// <summary>
        /// Information about the font itself.
        /// </summary>
        [XmlElement("info")]
        public FontInfo Info { get; set; }

        /// <summary>
        /// Data common to all characters in the font.
        /// </summary>
        [XmlElement("common")]
        public FontCommon Common { get; set; }

        /// <summary>
        /// Data about the bitmap pages of the font.
        /// </summary>
        [XmlArray("pages")]
        [XmlArrayItem("page")]
        public FontPage[] Pages { get; set; }

        /// <summary>
        /// Data about each character.
        /// </summary>
        [XmlArray("chars")]
        [XmlArrayItem("char")]
        public CharInfo[] Chars { get; set; }

        /// <summary>
        /// Data about kerning pairs in the font.
        /// </summary>
        [XmlArray("kernings")]
        [XmlArrayItem("kerning")]
        public Kerning[] Kernings { get; set; }
    }

    /// <summary>
    /// Font info XML
    /// </summary>
    public struct FontInfo
    {
        /// <summary>
        /// The face of the font.
        /// </summary>
        [XmlAttribute("face")]
        public string Face { get; set; }

        /// <summary>
        /// The size of the font.
        /// </summary>
        [XmlAttribute("size")]
        public int Size { get; set; }

        /// <summary>
        /// Flag to denote if the font is bold.
        /// </summary>
        [XmlAttribute("bold")]
        public bool Bold { get; set; }

        /// <summary>
        /// Flag to denote if the font is italic.
        /// </summary>
        [XmlAttribute("italic")]
        public bool Italic { get; set; }
    }

    /// <summary>
    /// Font Common XML
    /// </summary>
    public struct FontCommon
    {
        /// <summary>
        /// The height of a line rendered in the font.
        /// </summary>
        [XmlAttribute("lineHeight")]
        public int LineHeight { get; set; }

        /// <summary>
        /// The font's baseline.
        /// </summary>
        [XmlAttribute("base")]
        public int Base { get; set; }

        /// <summary>
        /// The width of the font's bitmaps.
        /// </summary>
        [XmlAttribute("scaleW")]
        public float ScaleW { get; set; }

        /// <summary>
        /// The height of the font's bitmaps.
        /// </summary>
        [XmlAttribute("scaleH")]
        public float ScaleH { get; set; }
    }

    /// <summary>
    /// Font texture page XML
    /// </summary>
    public struct FontPage
    {
        /// <summary>
        /// The ID of the page.
        /// </summary>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// The file name of the page.
        /// </summary>
        [XmlAttribute("file")]
        public string File { get; set; }
    }

    /// <summary>
    /// Info of a char
    /// </summary>
    public struct CharInfo
    {
        /// <summary>
        /// Character Unicode ID.
        /// </summary>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// The X coordinate of the character on the bitmap.
        /// </summary>
        [XmlAttribute("x")]
        public float X { get; set; }

        /// <summary>
        /// The Y coordinate of the character on the bitmap.
        /// </summary>
        [XmlAttribute("y")]
        public float Y { get; set; }

        /// <summary>
        /// The width of the character on the bitmap.
        /// </summary>
        [XmlAttribute("width")]
        public int Width { get; set; }

        /// <summary>
        /// The height of the character on the bitmap.
        /// </summary>
        [XmlAttribute("height")]
        public int Height { get; set; }

        /// <summary>
        /// The X offset from the base line for the character.
        /// </summary>
        [XmlAttribute("xoffset")]
        public int XOffset { get; set; }

        /// <summary>
        /// The Y offset from the base line for the character.
        /// </summary>
        [XmlAttribute("yoffset")]
        public int YOffset { get; set; }

        /// <summary>
        /// The position advancement before drawing the next character.
        /// </summary>
        [XmlAttribute("xadvance")]
        public int XAdvance { get; set; }

        /// <summary>
        /// The bitmap page the character is on.
        /// </summary>
        [XmlAttribute("page")]
        public int Page { get; set; }
    }

    /// <summary>
    /// Kerning data
    /// </summary>
    public struct Kerning
    {
        /// <summary>
        /// The first character to consider.
        /// </summary>
        [XmlAttribute("first")]
        public int First { get; set; }

        /// <summary>
        /// The second character to consider.
        /// </summary>
        [XmlAttribute("second")]
        public int Second { get; set; }

        /// <summary>
        /// The X position adjustment when drawing the second character after the first.
        /// </summary>
        [XmlAttribute("amount")]
        public int Amount { get; set; }
    }
}
