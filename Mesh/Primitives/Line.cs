using KirosEngine3.Math.Geometry;
using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Mesh.Primitives
{
    /// <summary>
    /// Defines a drawable line with color
    /// </summary>
    public class Line : IDisposable, IRenderable
    {
        //mathematical representation of the line in 3D
        protected Line3D _line;//todo: change so that the math rep doesn't need to be stored and is created from vertex data as needed
        //the color of the line
        protected Vec4 _color;
        //the vertex data
        protected ColorVertex[] _verts = new ColorVertex[2];
        //vertex array object
        protected int _VAO;
        //vertex buffer object
        protected int _VBO;
        //name of the shader used in rendering
        protected string _shaderName;

        protected bool _disposed = false;

        /// <summary>
        /// The line's starting point
        /// </summary>
        public Vec3 Start
        {
            get { return _line.Start; }
            set
            {
                _line.Start = value;
                RecalculateVerts();
            }
        }

        /// <summary>
        /// The line's end point
        /// </summary>
        public Vec3 End
        {
            get { return _line.Direction; }
            set
            {
                _line.Direction = value;
                RecalculateVerts();
            }
        }

        /// <summary>
        /// The line's color
        /// </summary>
        public Vec4 Color { get { return _color; } set { _color = value; } }

        /// <summary>
        /// The mathematical representation of the line
        /// </summary>
        public Line3D MathLine { get { return _line; } }

        /// <summary>
        /// The name of the shader used to render the line
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// Basic constructor for a renderable line object
        /// </summary>
        /// <param name="start">The line's start point</param>
        /// <param name="end">The line's end point</param>
        /// <param name="color">The color of the line</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Line(Vec3 start, Vec3 end, Vec4 color, string shaderName = "color")
        {
            _line = new Line3D(start, end, true);
            _color = color;
            _shaderName = shaderName;

            _verts[0].Position = _line.Start;
            _verts[0].Color = _color;

            _verts[1].Position = _line.Direction;
            _verts[1].Color = _color;

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);

            Shader sh = ShaderManager.Instance[shaderName];

            ColorVertex.SetVertexPositionAttrib(sh, "aPosition");//todo: get the shader attrib names from the shader object
            ColorVertex.SetVertexColorAttrib(sh, "aColor");

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Constructor for a renderable line object
        /// </summary>
        /// <param name="line">A mathematical representation of the line</param>
        /// <param name="color">The color of the line</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Line(Line3D line, Vec4 color, string shaderName = "color") :
            this(line.Start, line.Direction, color, shaderName)
        { }

        /// <summary>
        /// Draws the point on the screen using the named shader (OpenGL)
        /// </summary>
        public void DrawGL()
        {
            if (_disposed)
            {
                Logger.WriteToLog("Attempt to draw unloaded line object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded line object: {0}", this);
                //todo: write to debug console
                return;
            }

            ShaderManager.TryUseShader(_shaderName);

            GL.BindVertexArray(_VAO);

            GL.DrawArrays(PrimitiveType.Lines, 0, _verts.Length);
            GL.BindVertexArray(0);//clear the bound vertex array
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
        /// Updates to the line called each frame
        /// </summary>
        public void UpdateGL()
        {

        }

        /// <summary>
        /// Update the vertices based on changes to the line data
        /// </summary>
        protected void RecalculateVerts()
        {
            _verts[0].Position = _line.Start;

            _verts[1].Position = _line.Direction;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);
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
        ~Line()
        {
            Dispose(false);
        }
    }
}
