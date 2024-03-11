using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Textures
{
    struct CharData
    {
        public float x, y;
        public int width, height;
    }

    internal class Font
    {
        private string _name;
        private string _filePath;

        private Texture _fontTexture;
        private int _size;

        private readonly List<CharData> _charCoords = new List<CharData>();

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

        public List<CharData> CharCoords
        {
            get { return _charCoords; }
        }

        public Font(string name, string filePath, Texture tex)
        {
            _name = name;
            _filePath = filePath;
            _fontTexture = tex;
            
            LoadFont();
            //todo: check that texture is loaded
        }

        /// <summary>
        /// Load the font data from its file
        /// </summary>
        private void LoadFont() 
        {
            FileInfo fFile = new FileInfo(_filePath);

            StreamReader sr = fFile.OpenText();
            while(!sr.EndOfStream)
            {
                string? line = sr.ReadLine();

                //todo: process the line
            }
        }

        /// <summary>
        /// Reload the font from it's file
        /// </summary>
        public void Reload()
        {
            //todo: cleanup first?
            LoadFont();
        }
    }
}
