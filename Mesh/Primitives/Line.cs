using KirosEngine3.Math.Data;
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
            get { return _verts[0].Position; }
            set
            {
                _verts[0].Position = value;
                ReloadVerts();
            }
        }

        /// <summary>
        /// The line's end point
        /// </summary>
        public Vec3 End
        {
            get { return _verts[1].Position; }
            set
            {
                _verts[1].Position = value;
                ReloadVerts();
            }
        }

        /// <summary>
        /// The line's color
        /// </summary>
        public Color4[] Colors
        { 
            get { return [_verts[0].Color, _verts[1].Color]; }
        }

        /// <summary>
        /// The mathematical representation of the line
        /// </summary>
        public Line3D MathLine { get { return new Line3D([_verts[0].Position, _verts[1].Position], false); } }

        /// <summary>
        /// The name of the shader used to render the line
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        #region Loading
        /// <summary>
        /// Basic constructor for a renderable line object
        /// </summary>
        /// <param name="start">The line's start point</param>
        /// <param name="end">The line's end point</param>
        /// <param name="colors">The colors of the line</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Line(Vec3 start, Vec3 end, Color4[] colors, string shaderName = "color")
        {
            _shaderName = shaderName;

            _verts[0].Position = start;
            _verts[0].Color = colors[0];

            _verts[1].Position = end;
            _verts[1].Color = colors[1];
        }

        /// <summary>
        /// Constructor for a renderable line object
        /// </summary>
        /// <param name="line">A mathematical representation of the line</param>
        /// <param name="colors">The colors of the line</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Line(Line3D line, Color4[] colors, string shaderName = "color") :
            this(line.Start, line.Direction, colors, shaderName)
        { }

        /// <summary>
        /// Constructor for a renderable line object
        /// </summary>
        /// <param name="start">The line's start point</param>
        /// <param name="end">The line's end point</param>
        /// <param name="color">The color for the whole line</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Line(Vec3 start, Vec3 end, Color4 color, string shaderName = "color") :
            this(start, end, [color, color], shaderName)
        { }

        /// <summary>
        /// Called to initialize the renderable object if it is not being drawn as part of a group
        /// </summary>
        public void Init()
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);

            Shader sh = ShaderManager.Instance[_shaderName];

            ColorVertex.SetVertexPositionAttrib(sh, "aPosition");//todo: get the shader attrib names from the shader object
            ColorVertex.SetVertexColorAttrib(sh, "aColor");

            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Called to initialize the renderable object for drawing as a part of similar objects
        /// </summary>
        public void BindToGroup()
        {
            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);

            Shader sh = ShaderManager.Instance[_shaderName];

            ColorVertex.SetVertexPositionAttrib(sh, "aPosition");//todo: get the shader attrib names from the shader object
            ColorVertex.SetVertexColorAttrib(sh, "aColor");
        }
        #endregion

        #region Draw
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
        /// Draws the point as part of a group of similar objects (OpenGL)
        /// </summary>
        public void DrawInGroupGL()
        {
            if (_disposed)
            {
                Logger.WriteToLog("Attempt to draw unloaded line object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded line object: {0}", this);
                //todo: write to debug console
                return;
            }

            //shader and vertex array should be set in the calling method

            GL.DrawArrays(PrimitiveType.Lines, 0, _verts.Length);
        }

        /// <summary>
        /// Draws the point on the screen using the named shader (DirectX)
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void DrawDX()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates to the line called each frame
        /// </summary>
        public void UpdateGL()
        {

        }

        /// <summary>
        /// Set the color of the specified vertex, 0 for the start vertex, 1 for the end vertex
        /// </summary>
        /// <param name="vIndex">The index of the vertex</param>
        /// <param name="color">The color to set the vertex to</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the vertex index is outside the allowed range</exception>
        public void SetVertexColor(int vIndex, Color4 color)
        {
            if (vIndex > 1 || vIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vIndex));
            }

            _verts[vIndex].Color = color;
        }

        /// <summary>
        /// Update the vertices based on changes to the line data
        /// </summary>
        protected void ReloadVerts()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, ColorVertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);
        }
        #endregion

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
