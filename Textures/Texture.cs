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
    public class Texture : IDisposable
    {
        private int _handle;
        private string _name;
        private string _path;

        private bool _loaded = false;
        private bool _disposed = false;

        public int Handle
        { get { return _handle; } }

        public string Name
        { get { return _name; } }

        public bool IsLoaded
        { get { return _loaded; } }

        public Texture(string name, string path)
        {
            if(RuntimeVars.Instance[Client.GRAPHICSMODE_KEY] is string gm && gm.Equals(Client.GRAPHICSMODE_GL_VAL))
            {
                _handle = GL.GenTexture();
            }
            else
            {
                _handle = -1;//todo: replace with DX equivalent
            }

            _name = name;
            _path = path;
        }

        /// <summary>
        /// General load method that calls the relevant load method based on the current graphics mode
        /// </summary>
        internal void Load()
        {
            if (RuntimeVars.Instance[Client.GRAPHICSMODE_KEY] is string gm && gm.Equals(Client.GRAPHICSMODE_GL_VAL))
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
                GL.BindTexture(TextureTarget.Texture2D, _handle);

                StbImage.stbi_set_flip_vertically_on_load(1);

                ImageResult image = ImageResult.FromStream(File.OpenRead(_path), ColorComponents.RedGreenBlueAlpha); //todo: exception handling in loading

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

                //todo: allow greater flexibility in setting text params
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
                Console.WriteLine("Attempting to use texture:" + _name + "without first loading it!"); //todo: exception handling
                Logger.WriteToLog(string.Format("Attempt to use texture: {0} without first loading it.", _name));
                //todo: write to debug console
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
                switch (RuntimeVars.GetVar(Client.GRAPHICSMODE_KEY))
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

        ~Texture()
        {
            if (_disposed == false)
            {
                Console.WriteLine("Texture named: " + _name + " not properly disposed of."); //todo: exception and handling
            }
        }
    }
}
