using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KirosEngine3.Mesh.Primitives
{
    /// <summary>
    /// Defines a drawable point with color
    /// </summary>
    public class Point : IDisposable, IRenderable
    {
        //vertex data
        protected ColorVertex[] _point = new ColorVertex[1];

        //vertex array object
        protected int _VAO;
        //vertex buffer object
        protected int _VBO;

        protected string _shaderName;

        protected bool _disposed = false;

        /// <summary>
        /// The position of the point
        /// </summary>
        public Vec3 Position { get { return _point[0].Position; } set { _point[0].Position = value; } }

        /// <summary>
        /// The color of the point
        /// </summary>
        public Vec4 Color { get { return _point[0].Color; } set { _point[0].Color = value; } }

        /// <summary>
        /// The name of the shader used to render the line
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// Basic constructor for a renderable point object
        /// </summary>
        /// <param name="position">The point's position</param>
        /// <param name="color">The color of the point</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Point(Vec3 position, Vec4 color, string shaderName = "color")
        {
            _point[0].Position = position;
            _point[0].Color = color;

            _shaderName = shaderName;

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _point.Length, _point, BufferUsageHint.DynamicDraw);

            Shader sh = ShaderManager.Instance[shaderName];

            ColorVertex.SetVertexPositionAttrib(sh, "aPosition");//todo: get the shader attrib names from the shader object
            ColorVertex.SetVertexColorAttrib(sh, "aColor");
        }

        /// <summary>
        /// Draws the point on the screen using the named shader (OpenGL)
        /// </summary>
        public void DrawGL()
        {
            if (_disposed)
            {
                Logger.WriteToLog("Attempt to draw unloaded point object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded point object: {0}", this);
                //todo: write to debug console
                return;
            }

            Shader sh = ShaderManager.Instance[_shaderName];
            sh.UseGL();

            GL.BindVertexArray(_VAO);

            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Draws the point on the screen using the named shader (DirectX)
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void DrawDX()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposal of unmanaged objects
        /// </summary>
        /// <param name="disposing">If true the user code is calling, false means the GC system is</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //clear managed items
                }

                GL.DeleteBuffer(_VBO);
                GL.DeleteVertexArray(_VAO);
                _disposed = true;
            }
        }

        /// <summary>
        /// Release the point's resources for unloading
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        ~Point()
        {
            Dispose(false);
        }
    }
}
