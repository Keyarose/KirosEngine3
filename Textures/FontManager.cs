using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KirosEngine3.Config;
using KirosEngine3.Exceptions;

namespace KirosEngine3.Textures
{
    internal class FontManager
    {
        private static FontManager? _instance;

        private readonly Dictionary<string, Font> _fonts = [];

        public static FontManager Instance
        { get { return _instance ??= new FontManager(); } }

        public static Font Default
        {
            get
            {
                if (TryGetFont(ConfigVars.Instance[ConfigKeys.D_FONT_NAME_KEY], out Font? f))
                    return f;
                else
                    throw new MissingConfigException(string.Format("Default font name is not configured or incorrect. Name: {0}", ConfigKeys.D_FONT_NAME_KEY));
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
