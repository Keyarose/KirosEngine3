using KirosEngine3.Math.Geometry;
using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Mesh.Primitives
{
    public class Triangle : IDisposable
    {
        //protected Triangle3D _tri;

        protected Vec4 _color;

        protected ColorVertex[] _verts = new ColorVertex[3];

        protected int _VAO;

        protected int _VBO;

        protected string _shaderName;

        protected bool _disposed = false;

        /// <summary>
        /// The points of the triangle
        /// </summary>
        public Vec3[] Points 
        { 
            get { return [_verts[0].Position, _verts[1].Position, _verts[2].Position]; }
        }

        /// <summary>
        /// The color of the triangle
        /// </summary>
        public Vec4 Color { get { return _color; } set { _color = value; } }

        /// <summary>
        /// The mathematical representation of the triangle
        /// </summary>
        public Triangle3D MathTriangle { get { return new Triangle3D(Points); } }

        /// <summary>
        /// The name of the shader to use in rendering
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        public Triangle(Vec3[] points, Vec4 color, string shaderName = "color")
        {
            if (points.Length < 3)
            {
                throw new ArgumentException(string.Format("Triangle requires three points, only {0} were provided.", points.Length));
            }

            _color = color;
            _shaderName = shaderName;

            _verts[0].Position = points[0];
            _verts[0].Color = color;

            _verts[1].Position = points[1];
            _verts[1].Color = color;

            _verts[2].Position = points[2];
            _verts[2].Color = color;

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);

            Shader sh = ShaderManager.Instance[shaderName];

            ColorVertex.SetVertexPositionAttrib(sh, "aPosition");
            ColorVertex.SetVertexColorAttrib(sh, "aColor");

            GL.BindVertexArray(0);
        }

        public void DrawGL()
        {
            if (_disposed)
            {
                Logger.WriteToLog("Attempt to draw unloaded triangle object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded triangle object: {0}", this);
                //todo: write to debug console
                return;
            }

            ShaderManager.TryUseShader(_shaderName);

            GL.BindVertexArray(_VAO);

            GL.DrawArrays(PrimitiveType.Triangles, 0, _verts.Length);
            GL.BindVertexArray(0);
        }

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
        /// Release the triangle's resources for unloading
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Triangle()
        {
            Dispose(false);
        }
    }
}
