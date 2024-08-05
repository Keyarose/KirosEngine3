using KirosEngine3.Config;
using KirosEngine3.Debug;
using KirosEngine3.Exceptions;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Textures
{
    /// <summary>
    /// A texture object.
    /// </summary>
    public class Texture : IDisposable
    {
        private int _handle;
        private readonly string _name;
        private readonly string _path;

        private bool _loaded = false;
        private bool _disposed = false;

        /// <summary>
        /// The texture handle for the graphics API.
        /// </summary>
        public int Handle
        { get { return _handle; } }

        /// <summary>
        /// The name of the texture.
        /// </summary>
        public string Name
        { get { return _name; } }

        /// <summary>
        /// Flag to mark if the texture is loaded.
        /// </summary>
        public bool IsLoaded
        { get { return _loaded; } }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="path">The path to the texture file.</param>
        public Texture(string name, string path)
        {
            _name = name;
            _path = path;
        }

        /// <summary>
        /// General load method that calls the relevant load method based on the current graphics mode
        /// </summary>
        internal void Load()
        {
            if (ConfigManager.GraphicsMode.Equals(Client.GRAPHICSMODE_GL_VAL))
            {
                LoadGL();
            }
            else
            {
                LoadDX();
            }
        }

        /// <summary>
        /// Texture loading for DirectX mode
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadDX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Texture loading for OpenGL mode
        /// </summary>
        public void LoadGL()
        {
            if (!_loaded)
            {
                _handle = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _handle);

                StbImage.stbi_set_flip_vertically_on_load(1);

                ImageResult? image;
                try
                {
                    image = ImageResult.FromStream(File.OpenRead(_path), ColorComponents.RedGreenBlueAlpha);
                }
                catch
                {
                    Console.WriteLine("Failed to read the image from file: {0}", _path);
                    Logger.WriteToLog("Failed to read the image from file: {0}", _path);
                    DebugConsole.WriteLine("Failed to read the image from file: {0}", _path);

                    return;
                }

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image!.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

                //todo: allow greater flexibility in setting text params, including mipmaps
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                _loaded = true;
            }
        }

        /// <summary>
        /// Set the texture to be used in the pipeline when in OpenGL mode
        /// </summary>
        /// <param name="unit">The texture unit the texture is to be assigned to</param>
        public void UseGL(TextureUnit unit)
        {
            if (!_loaded)
            {
                Console.WriteLine("Attempt to use texture: {0} without first loading it.", _name);
                Logger.WriteToLog("Attempt to use texture: {0} without first loading it.", _name);
                DebugConsole.WriteLine("Attempt to use texture: {0} without first loading it.", _name);
            }
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        /// <summary>
        /// Dispose of the texture using the appropriate graphics mode
        /// </summary>
        /// <param name="disposing">Disposal activity</param>
        /// <exception cref="InvalidGraphicsModeException">Thrown when the graphics mode is in an invalid state</exception>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                switch (ConfigManager.GraphicsMode)
                {
                    case Client.GRAPHICSMODE_GL_VAL:
                        {
                            GL.DeleteTexture(_handle);
                            break;
                        }
                    case Client.GRAPHICSMODE_DX_VAL:
                        {
                            //todo: texture disposal in dx mode
                            break;
                        }
                    default:
                        {
                            throw new InvalidGraphicsModeException("The graphics mode is in an invalid state, neither DirectX or OpenGL.");
                        }
                }
                _disposed = disposing;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~Texture()
        {
            if (_disposed == false)
            {
                Console.WriteLine("Texture named: " + _name + " not properly disposed of.");
                Logger.WriteToLog("Texture named: " + _name + " not properly disposed of.");
                //todo:write to debug
            }
        }

        /// <summary>
        /// Convert TextureUnit into it's int, not it's enum as int.
        /// </summary>
        /// <param name="unit">The TextureUnit to get the int of.</param>
        /// <returns>The int represented by the TextureUnit.</returns>
        public static int TextureUnitToInt(TextureUnit unit)
        {
            switch (unit)
            {
                case TextureUnit.Texture0:
                    return 0;
                case TextureUnit.Texture1: return 1;
                case TextureUnit.Texture2: return 2;
                case TextureUnit.Texture3: return 3;
                case TextureUnit.Texture4: return 4;
                case TextureUnit.Texture5: return 5;
                case TextureUnit.Texture6: return 6;
                case TextureUnit.Texture7: return 7;
                case TextureUnit.Texture8: return 8;
                case TextureUnit.Texture9: return 9;
                case TextureUnit.Texture10: return 10;
                case TextureUnit.Texture11: return 11;
                case TextureUnit.Texture12: return 12;
                case TextureUnit.Texture13: return 13;
                case TextureUnit.Texture14: return 14;
                case TextureUnit.Texture15: return 15;
                case TextureUnit.Texture16: return 16;
                case TextureUnit.Texture17: return 17;
                case TextureUnit.Texture18: return 18;
                case TextureUnit.Texture19:
                    return 19;
                case TextureUnit.Texture20:
                    return 20;
                case TextureUnit.Texture21:
                    return 21;
                case TextureUnit.Texture22:
                    return 22;
                case TextureUnit.Texture23:
                    return 23;
                case TextureUnit.Texture24:
                    return 24;
                case TextureUnit.Texture25:
                    return 25;
                case TextureUnit.Texture26:
                    return 26;
                case TextureUnit.Texture27:
                    return 27;
                case TextureUnit.Texture28:
                    return 28;
                case TextureUnit.Texture29:
                    return 29;
                case TextureUnit.Texture30:
                    return 30;
                case TextureUnit.Texture31:
                    return 31;
                default:
                    return -1;
            }
        }
    }
}
