using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KirosEngine3.Exceptions;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Textures
{
    /// <summary>
    /// Texture resource manager.
    /// </summary>
    public class TextureManager
    {
        private static TextureManager? _instance;

        private readonly Dictionary<string, Texture> _textures = [];
        private readonly Dictionary<string, int> _reservations = [];

        private bool _autoLoadTextures = false;

        private static TextureManager Instance
        { get { return _instance ??= new TextureManager(); } }

        /// <summary>
        /// Allow the manager to automatically load textures as they are added to it
        /// </summary>
        public static void EnableAutoLoadTextures()
        {
            Instance._autoLoadTextures = true;
        }

        /// <summary>
        /// Disallow the manager to automatically load textures
        /// </summary>
        public static void DisableAutoLoadTextures() 
        {
            Instance._autoLoadTextures = false;
        }

        /// <summary>
        /// Add a texture to the manager.
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="texture">The texture to be added</param>
        /// <exception cref="ArgumentException">Thrown when the name for the texture is already in use</exception>
        /// <exception cref="CollectionCleanupException">Thrown if the reservation collection is already using the name.</exception>
        public static void AddTexture(string name, Texture texture)
        {
            if (!Instance._textures.TryAdd(name, texture))
            {
                throw new ArgumentException(string.Format("Texture name: {0} is already in use. Use ReserveTexture if it's loaded elsewhere, or a different name if it's a new resource.", name));
            }
            //init the reservation counter for the texture
            if (!Instance._reservations.TryAdd(name, 1))
            {
                throw new CollectionCleanupException(string.Format("Reservation counter for name: {0} already exists even though a texture didn't!", name), nameof(_reservations));
            }

            if (Instance._autoLoadTextures)
            { texture.Load(); }
        }

        /// <summary>
        /// Add a texture to the manager using the given texture file.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="textureFile">The file the texture is stored in.</param>
        /// <exception cref="ArgumentException">Thrown when the name for the texture is already in use.</exception>
        public static void AddTexture(string name, string textureFile)
        {
            AddTexture(name, new Texture(name, textureFile));
        }

        /// <summary>
        /// Try to add a texture to the manager.
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="texture">The texture to add</param>
        /// <returns>True if the texture is added to the manager, false otherwise</returns>
        /// <exception cref="ArgumentException">Thrown when the reservation for the texture name already exists.</exception>
        public static bool TryAddTexture(string name, Texture texture)
        {
            if (Instance._textures.TryAdd(name, texture))
            {
                //init the reservation counter for the texture
                if (!Instance._reservations.TryAdd(name, 1))
                {
                    throw new CollectionCleanupException(string.Format("Reservation counter for name: {0} already exists even though a texture didn't!", name), nameof(_reservations));
                }

                if (Instance._autoLoadTextures)
                { texture.Load(); }
                return true;
            }

            Logger.WriteToLog("Texture name: {0} is already in use. Use ReserveTexture instead.", name);
            Console.WriteLine("Texture name: {0} is already in use. Use ReserveTexture instead.", name);
            DebugConsole.WriteLine("Texture name: {0} is already in use. Use ReserveTexture instead.", name);

            return false;
        }

        /// <summary>
        /// Try to add a texture to the manager.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="textureFile">The file that contains the texture.</param>
        /// <returns>True if the texture is added to the manager, false otherwise.</returns>
        public static bool TryAddTexture(string name, string textureFile)
        {
            return TryAddTexture(name, new Texture(name, textureFile));
        }

        /// <summary>
        /// Reserve a texture for use.
        /// </summary>
        /// <param name="name">The name of the texture to reserve.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool ReserveTexture(string name)
        {
            if (Instance._textures.ContainsKey(name))
            {
                Instance._reservations[name]++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Release a reservation on a texture.
        /// </summary>
        /// <param name="name">The name of the texture to release the reservation on.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool ReleaseTexture(string name)
        {
            if (Instance._textures.ContainsKey(name))
            {
                Instance._reservations[name]--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Load a texture into memory before use.
        /// </summary>
        /// <param name="name">The name of the texture to load.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool LoadTexture(string name)
        {
            if (Instance._textures.TryGetValue(name, out var texture))
            {
                texture.Load();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to remove a texture from the manager, but only do so if there are no reservations on it.
        /// </summary>
        /// <param name="name">The name of the texture to remove.</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryRemoveTexture(string name) 
        {
            //if the texture exists clean it up before removing it to prevent memory leaks
            if (Instance._textures.TryGetValue(name, out var texture))
            {
                if (Instance._reservations[name] == 0)
                {
                    texture.Dispose();
                    Instance._reservations.Remove(name);
                    return Instance._textures.Remove(name);
                }
                else if (Instance._reservations[name] < 0)
                {
                    Logger.WriteToLog("Texture name: {0} has been released more than it has been reserved. Check the releases and reservations.", name);
                    Console.WriteLine("Texture name: {0} has been released more than it has been reserved. Check the releases and reservations.", name);
                    DebugConsole.WriteLine("Texture name: {0} has been released more than it has been reserved. Check the releases and reservations.", name);
                }
            }

            return false;
        }

        /// <summary>
        /// Try to get the texture represented by the given name
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="texture">The texture for the name or null</param>
        /// <returns>True if the texture is found, false otherwise</returns>
        public static bool TryGetTexture(string name,[NotNullWhen(true)] out Texture? texture)
        {
            if (Instance._textures.TryGetValue(name, out texture))
            { return true; }

            texture = null;
            return false;
        }

        /// <summary>
        /// Get the handle for a texture of the given name
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <returns>The handle for the texture or -1 if not found</returns>
        public static int GetTextureHandle(string name)
        {
            if (Instance._textures.TryGetValue(name, out var tex))
            {
                return tex.Handle;
            }

            return -1;
        }

        /// <summary>
        /// Set a texture to be used in the rendering pipeline
        /// </summary>
        /// <param name="name">The name of the texture to be used</param>
        /// <param name="tu">The texture unit the texture is to be assigned to</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool UseTextureGL(string name, TextureUnit tu)
        {
            if (Instance._textures.TryGetValue(name, out var tex))
            {
                if (!tex.IsLoaded)
                {
                    tex.LoadGL();
                }
                tex.UseGL(tu);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clean up the manager in preparation for closing the program
        /// </summary>
        /// <exception cref="CollectionNotEmptyException">Thrown when the texture dictionary is not properly emptied</exception>
        public static void OnUnload()
        {
            foreach (var key in Instance._textures.Keys)
            {
                Instance._textures[key].Dispose();
                Instance._reservations.Remove(key);
            }
            Instance._textures.Clear();

            if (Instance._textures.Count > 0 )
            {
                throw new CollectionNotEmptyException("The Texture Manager collection is not empty after running OnUnload, something is wrong.");
            }
        }
    }
}
