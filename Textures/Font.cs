using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Textures
{
    internal class Font
    {
        private int _charWidth;
        private int _charHeight;

        private string _name;
        private string _filePath;

        private Texture _fontText;

        private readonly List<Vec2> _charCoords = new List<Vec2>();

        /// <summary>
        /// The width of chars for the font
        /// </summary>
        public int CharWidth
        {
            get { return _charWidth; } 
        }

        /// <summary>
        /// The height of chars for the font
        /// </summary>
        public int CharHeight
        {
            get { return _charHeight; } 
        }

        /// <summary>
        /// The name of the font
        /// </summary>
        public string Name
        {
            get { return _name; } 
        }

        public List<Vec2> CharCoords
        {
            get { return _charCoords; }
        }

        public Font(string name, string filePath, Texture tex)
        {
            _name = name;
            _filePath = filePath;
            _fontText = tex;
            
            LoadFont();
            //todo: check that texture is loaded
        }

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
