using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KirosEngine3.Config;
using KirosEngine3.Exceptions;
using static System.Net.Mime.MediaTypeNames;

namespace KirosEngine3.Textures
{
    /// <summary>
    /// Loads, stores and manages all fonts used by the program.
    /// </summary>
    public class FontManager
    {
        private static FontManager? _instance;

        private readonly Dictionary<string, Font> _fonts = [];

        /// <summary>
        /// The default font.
        /// </summary>
        private static Font? _default;

        /// <summary>
        /// The singleton _instance of the manager.
        /// </summary>
        public static FontManager Instance
        { get { return _instance ??= new FontManager(); } }

        /// <summary>
        /// The program's default font.
        /// </summary>
        public static Font Default
        {
            get
            {
                if (_default != null)
                    return _default;
                else
                    throw new MissingConfigException("Default font is not set.");
            }
        }

        /// <summary>
        /// Set the default font to the given one.
        /// </summary>
        /// <param name="font">The font to set as default.</param>
        public static void SetDefault(Font font)
        {
            _default = font;
        }

        /// <summary>
        /// Create a new font through the manager.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <param name="fileName">The file name of the font.</param>
        /// <exception cref="ArgumentException">Thrown if the font name is already in use.</exception>
        public static void CreateFont(string name, string fileName)
        {
            Font nFont = new Font(name, fileName);

            if (!Instance._fonts.TryAdd(name, nFont))
            {
                throw new ArgumentException(string.Format("Font name: {0} is already in use.", name));
            }
        }

        /// <summary>
        /// Add a font to the manager
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="font">The font to be added</param>
        /// <exception cref="ArgumentException">Thrown if the name identifier is already in use</exception>
        public static void AddFont(string name, Font font)
        {
            if (!Instance._fonts.TryAdd(name, font))
            {
                throw new ArgumentException(string.Format("Font name: {0} is already in use.", name));
            }
        }

        /// <summary>
        /// Try to add a texture to the manager
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="font">The font to be added</param>
        /// <returns>True if the font is successfully added</returns>
        public static bool TryAddFont(string name, Font font)
        {
            if (Instance._fonts.TryAdd(name, font))
            {
                return true;
            }

            Logger.WriteToLog(string.Format("Font name: {0} is already in use.", name));
            Console.WriteLine(string.Format("Font name: {0} is already in use.", name));
            return false;
        }

        /// <summary>
        /// Try to remove a font from the manager
        /// </summary>
        /// <param name="name">The name of the font to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryRemoveFont(string name)
        {
            return Instance._fonts.Remove(name);
        }

        /// <summary>
        /// Get the font represented by the given name if it exists.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <returns>The font for the name, or null.</returns>
        public static Font? GetFont(string name)
        {
            Instance._fonts.TryGetValue(name, out Font? fnt);

            return fnt;
        }

        /// <summary>
        /// Try to get the font represented by the given name
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="font">The font for the name or null</param>
        /// <returns>True if the font is found, false otherwise</returns>
        public static bool TryGetFont(string name, [NotNullWhen(true)] out Font? font) 
        {
            if (Instance._fonts.TryGetValue(name, out font))
            { return true; }

            font = null;
            return false;
        }
    }
}
