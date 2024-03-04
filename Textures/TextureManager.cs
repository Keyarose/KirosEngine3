using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KirosEngine3.Exceptions;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Textures
{
    internal class TextureManager
    {
        private static TextureManager? _instance;

        private Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

        private bool _autoLoadTextures = false;

        private static TextureManager Instance
        { get { return _instance ??= new TextureManager(); } }

        /// <summary>
        /// Allow the manager to automatically load textures as they are added to it
        /// </summary>
        public void EnableAutoLoadTextures()
        {
            _autoLoadTextures = true;
        }

        /// <summary>
        /// Disallow the manager to automatically load textures
        /// </summary>
        public void DisableAutoLoadTextures() 
        {
            _autoLoadTextures = false;
        }

        /// <summary>
        /// Add a texture to the manager
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="texture">The texture to be added</param>
        /// <exception cref="ArgumentException">Thrown when the name for the texture is already in use</exception>
        public void AddTexture(string name, Texture texture)
        {
            if (!_textures.TryAdd(name, texture))
            {
                throw new ArgumentException(string.Format("Texture name: {0} is already in use.", name));
            }

            if (_autoLoadTextures)
            { texture.Load(); }
        }

        /// <summary>
        /// Add a texture to the manager using the given texture file
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="textureFile">The file the texture is stored in</param>
        /// <exception cref="ArgumentException">Thrown when the name for the texture is already in use</exception>
        public void AddTexture(string name, string textureFile)
        {
            if (!_textures.TryAdd(name, new Texture(name, textureFile)))
            {
                throw new ArgumentException(string.Format("Texture name: {0} is already in use.", name));
            }

            if (_autoLoadTextures)
            { _textures[name].Load(); }
        }

        /// <summary>
        /// Try to add a texture to the manager
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="texture">The texture to add</param>
        /// <returns>True if the texture is added to the manager, false otherwise</returns>
        public bool TryAddTexture(string name, Texture texture)
        {
            if (_textures.TryAdd(name, texture))
            {
                if (_autoLoadTextures)
                { texture.Load(); }
                return true;
            }

            Logger.Instance.WriteToLog(string.Format("Texture name: {0} is already in use.", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Try to add a texture to the manager
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="textureFile">The file that contains the texture</param>
        /// <returns>True if the texture is added to the manager, false otherwise</returns>
        public bool TryAddTexture(string name, string textureFile)
        {
            if (_textures.TryAdd(name, new Texture(name, textureFile)))
            {
                if (_autoLoadTextures)
                { _textures[name].Load(); }
                return true;
            }

            Logger.Instance.WriteToLog(string.Format("Texture name: {0} is already in use.", name));
            //todo: write to debug console
            return false;
        }

        /// <summary>
        /// Try to remove a texture from the manager
        /// </summary>
        /// <param name="name">The name of the texture to remove</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool TryRemoveTexture(string name) 
        {
            //if the texture exists clean it up before removing it to prevent memory leaks
            if (_textures.TryGetValue(name, out var texture))
            {
                texture.Dispose();
                return _textures.Remove(name);
            }

            return false;
        }

        /// <summary>
        /// Try to get the texture represented by the given name
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <param name="texture">The texture for the name or null</param>
        /// <returns>True if the texture is found, false otherwise</returns>
        public bool TryGetTexture(string name, out Texture? texture)
        {
            if (_textures.TryGetValue(name, out texture))
            { return true; }

            texture = null;
            return false;
        }

        /// <summary>
        /// Get the handle for a texture of the given name
        /// </summary>
        /// <param name="name">The name of the texture</param>
        /// <returns>The handle for the texture or -1 if not found</returns>
        public int GetTextureHandle(string name)
        {
            if (_textures.TryGetValue(name, out var tex))
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
        public bool UseTextureGL(string name, TextureUnit tu)
        {
            if (_textures.TryGetValue(name, out var tex))
            {
                if (!tex.IsLoaded)
                { tex.LoadGL(); }

                tex.UseGL(tu);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clean up the manager in preparation for closing the program
        /// </summary>
        /// <exception cref="CollectionNotEmptyException">Thrown when the texture dictionary is not properly emptied</exception>
        public void OnUnload()
        {
            foreach (var key in _textures.Keys)
            {
                _textures[key].Dispose();
            }
            _textures.Clear();

            if (_textures.Count > 0 )
            {
                throw new CollectionNotEmptyException("The Texture Manager collection is not empty after running OnUnload, something is wrong.");
            }
        }
    }
}
