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
    public class Line : IDisposable, IRenderable, IFormattable
    {
        /// <summary>
        /// The vertex data.
        /// </summary>
        protected ColorVertex[] _verts = new ColorVertex[2];
        /// <summary>
        /// The Vertex array object.
        /// </summary>
        protected int _VAO;
        /// <summary>
        /// The Vertex buffer object.
        /// </summary>
        protected int _VBO;

        /// <summary>
        /// The name of the shader to be used in rendering.
        /// </summary>
        protected string _shaderName;

        /// <summary>
        /// Flag denoting if the Line has been loaded.
        /// </summary>
        protected bool _loaded = false;
        /// <summary>
        /// Flag denoting if the Line has been unloaded.
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// The draw mode to be used in rendering.
        /// </summary>
        protected PrimitiveType _drawMode = PrimitiveType.Lines;

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
        /// The end points of the Line
        /// </summary>
        public Vec3[] Points
        {
            get
            {
                return [_verts[0].Position, _verts[1].Position];
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

        /// <summary>
        /// The draw mode to be used during rendering
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }
        
        /// <summary>
        /// Basic constructor for a renderable line object
        /// </summary>
        /// <param name="start">The line's start point</param>
        /// <param name="end">The line's end point</param>
        /// <param name="colors">The colors of the line</param>
        /// <param name="shaderName">The name of the shader to use in drawing</param>
        public Line(Vec3 start, Vec3 end, Color4[] colors, string shaderName = "color")
        {
            if (shaderName == string.Empty)
                throw new ArgumentException("No shader name specified.", nameof(shaderName));

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
        /// Sets both vertices to the given color
        /// Init needs to be recalled if _loaded is true
        /// </summary>
        /// <param name="color">The color to set the vertices</param>
        public void SetColor(Color4 color)
        {
            _verts[0].Color = color;
            _verts[1].Color = color;
        }

        /// <summary>
        /// Sets the vertices to the given colors, cycling through the array until all
        /// vertices are updated
        /// Init needs to be recalled if _loaded is true
        /// </summary>
        /// <param name="colors">The colors to assign to the vertices</param>
        public void SetColors(Color4[] colors)
        {
            int j = 0;
            for (int i = 0; i < _verts.Length; i++)
            {
                _verts[i].Color = colors[j];

                j = (j + 1) % colors.Length;
            }
        }

        #region Loading
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

            Shader sh = ShaderManager.Instance[_shaderName];//todo: handle shader not found

            sh.SetAttribsGL<ColorVertex>();

            GL.BindVertexArray(0);

            _loaded = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the point on the screen using the named shader (OpenGL)
        /// </summary>
        public void DrawGL(ViewMatrixes vm)
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded line object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded line object: {0}", this);
                DebugConsole.WriteLine("Attempt to draw unloaded line object: {0}", this);

                return;
            }

            //can't draw, the shader failed to be added to the pipeline, logged in TryGetShader
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }
            sh.UseGL();

            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("view", vm.View);
            sh.SetUniformMat4GL("proj", vm.Projection);

            GL.BindVertexArray(_VAO);

            GL.DrawArrays(_drawMode, 0, _verts.Length);
            GL.BindVertexArray(0);//clear the bound vertex array
        }

        /// <inheritdoc/>
        public ColorVertex[] GetVertexData()
        {
            return _verts;
        }

        /// <inheritdoc/>
        public PrimitiveType GetDrawMode()
        {
            return _drawMode;
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
            ReloadVerts();
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


        #region ToString
        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public string ToString(string? format)
        {
            return ToString(format, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public string ToString(IFormatProvider? formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            string result = string.Format("Line mesh: \n\tPoints: ");

            foreach (var v in Points)
            {
                result += v.ToString(format, formatProvider) + "\n\t\t";
            }

            return result;
        }
        #endregion

        #region Dispose
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

                if (_loaded)
                {
                    GL.DeleteBuffer(_VBO);
                    GL.DeleteVertexArray(_VAO);
                }
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
        #endregion
    }
}
