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

namespace KirosEngine3.Mesh
{
    /// <summary>
    /// Defines a drawable point with color
    /// </summary>
    public class Point : IDisposable
    {
        protected ColorVertex[] _point = new ColorVertex[1];

        protected int _VAO;
        protected int _VBO;

        protected bool _disposed = false;

        public Vec3 Position { get { return _point[0].Position; } set { _point[0].Position = value; } }
        public Vec4 Color { get { return _point[0].Color; } set { _point[0].Color = value; } }

        public Point(Vec3 position, Vec4 color) 
        {
            _point[0].Position = position;
            _point[0].Color = color;

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU, _point, BufferUsageHint.StaticDraw);

            _disposed = true;
        }

        /// <summary>
        /// Draws the point on the screen using the named shader
        /// </summary>
        /// <param name="shaderName">The name of the shader to use</param>
        public void Draw(string shaderName)
        {
            if (!_disposed)
            {
                Logger.WriteToLog("Attempt to draw unloaded point object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded point object: {0}", this);
                //todo: write to debug console
                return;
            }

            Shader color = ShaderManager.Instance[shaderName];

            ColorVertex.SetVertexPositionAttrib(color, "aPosition");
            ColorVertex.SetVertexColorAttrib(color, "aColor");

            color.UseGL();

            GL.DrawArrays(PrimitiveType.Points, 0, 1);
        }

        protected void DisposeGL(bool disposing)
        {
            if (!_disposed) 
            {
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
            DisposeGL(true);
            GC.SuppressFinalize(this);
        }

        ~Point()
        {
            if (_disposed == false)
            {
                Console.WriteLine("Drawable Point not properly disposed of.");
                Logger.WriteToLog("Drawable Point not properly disposed of.");
            }
        }
    }
}
