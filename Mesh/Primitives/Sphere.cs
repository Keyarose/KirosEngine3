using KirosEngine3.Math.Data;
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
    /// <summary>
    /// Defines a primitive Spheroid mesh that can be rendered.
    /// </summary>
    public class Sphere
    {
        protected Vec3 _center;

        protected float _radius;

        protected SphereType _type;

        protected Vertex[] _verts;

        protected Color4 _color = Color4.Yellow;

        protected uint[] _indices;
        protected uint[] _testIndices = [0,1,24,46, 47];

        protected int _VAO;

        protected int _VBO;

        protected int _EBO;

        protected string _shaderName;

        protected bool _loaded = false;
        protected bool _disposed = false;
        protected bool _warnOnce = false;

        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        /// <summary>
        /// The center of the Sphere.
        /// </summary>
        public Vec3 Center 
        {
            get { return _center; }
            set { _center = value; }
        }

        /// <summary>
        /// The radius of the Sphere, acts as a multiplier for a unit sphere.
        /// </summary>
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        /// <summary>
        /// The points that define the Spheroid.
        /// </summary>
        public Vec3[] Points
        {
            get
            {
                return _verts.Select(vert => vert.Position).ToArray();
            }
        }

        /// <summary>
        /// The color to use when rendering.
        /// </summary>
        public Color4 Color
        {
            get { return _color; }
            set { _color = value; }
        }

        /// <summary>
        /// The mathematical representation of the Sphere.
        /// </summary>
        public Spheroid MathSphere { get { return new Spheroid(_center, _radius); } }

        /// <summary>
        /// The name of the shader to use in rendering.
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// The draw mode to be used during rendering.
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }

        public Sphere(Vec3 center, float radius, SphereType type, int latLines, int longLines, string shaderName)
        {
            _center = center;
            _radius = radius;
            _type = type;
            _shaderName = shaderName;

            switch (_type)
            {
                case SphereType.UVSphere:
                    Tuple<Vertex[], uint[]> data = GenerateUVSphere(latLines, longLines);
                    _verts = data.Item1;
                    _indices = data.Item2;
                    break;
                case SphereType.IcoSphere:
                case SphereType.QuadSphere:
                case SphereType.GoldbergPoly:
                default:
                    throw new NotImplementedException();
            }
        }

        protected static Tuple<Vertex[], uint[]> GenerateUVSphere(int latLines, int longLines)
        {
            float radius = 1.0f;
            //(longitude line count + 1 for UV seam) * latitude count + north and south poles
            int vertCount = latLines * (longLines + 1) + 2;
            //(longCount * 6, indexes per quad) * (latCount -1, rows of quads) + (longCount * 6, both caps)
            int indexCount = (longLines * 6) + (latLines - 1) * longLines * 6;

            Vertex[] verts = new Vertex[vertCount];
            uint[] indices = new uint[indexCount];

            int vIndex = 0;
            //north pole
            verts[vIndex++] = new Vertex { Position = new(0.0f, radius, 0.0f) };

            //south pole
            verts[vertCount - 1] = new Vertex { Position = new(0.0f, -radius, 0.0f) };

            float latSpacing = 1.0f / (latLines + 1.0f);
            float longSpacing = 1.0f / longLines;

            //layout verts
            for (int lat = 0; lat < latLines; lat++)
            {
                for (int lon = 0; lon <= longLines; lon++)
                {
                    //spherical coords
                    float theta = lon * longSpacing * 2.0f * MathF.PI;
                    float phi = (0.5f - ((lat + 1) * latSpacing)) * MathF.PI;

                    float cPhi = MathF.Cos(phi);


                    verts[vIndex++] = new Vertex
                    { 
                        Position = new Vec3(cPhi * MathF.Cos(theta), MathF.Sin(phi), cPhi * MathF.Sin(theta)) * radius
                    };
                }
            }

            int iIndex = 0;
            //layout indices
            //top cap 0..longLines+1
            for (int lon = 0; lon < longLines; lon++)
            {
                indices[iIndex++] = 0;
                indices[iIndex++] = (uint)(lon % longLines) + 1;
                indices[iIndex++] = (uint)(lon % longLines) + 2;
            }


            //quads
            for (int lat = 0; lat < latLines - 1; lat++)
            {
                for (int lon = 1; lon <= longLines; lon++)
                {
                    //tri 1
                    indices[iIndex++] = (uint)(lon + ((longLines + 1) * lat));
                    indices[iIndex++] = (uint)(lon + ((longLines + 1) * lat) + longLines + 1);
                    indices[iIndex++] = (uint)(lon + ((longLines + 1) * lat) + longLines + 2);

                    //tri 2
                    indices[iIndex++] = (uint)(lon + ((longLines + 1) * lat) + longLines + 2);
                    indices[iIndex++] = (uint)(lon + ((longLines + 1) * lat) + 1);
                    indices[iIndex++] = (uint)(lon + ((longLines + 1) * lat));
                }
            }//todo: integrate into vertex generation to reduce loops

            //bottom cap (vertCount-1)..(vertCount-2-longLines)
            for (int lon = 0; lon < longLines; lon++)
            {
                indices[iIndex++] = (uint)vertCount - 1;
                int vZCount = vertCount - 1;
                indices[iIndex++] = (uint)(vZCount - (lon % longLines) - 1);
                indices[iIndex++] = (uint)(vZCount - (lon % longLines) - 2);
            }

            return new Tuple<Vertex[], uint[]>(verts, indices);
        }

        public void Init(string? shaderName)
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.StaticDraw);

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices.Length, _indices, BufferUsageHint.StaticDraw);

            //if the shader fails to be found return
            if (!ShaderManager.TryGetShader(shaderName ?? _shaderName, out Shader? sh))
            {
                return;
            }

            if (shaderName != null)
            {
                _shaderName = shaderName;
            }

            sh.SetAttribsGL<Vertex>();

            GL.BindVertexArray(0);

            _loaded = true;
        }

        public void Draw(ViewMatrixes vm)
        {
            if (_disposed || !_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded sphere object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded sphere object: {0}", this);
                //todo: write to debug console
                return;
            }

            //if the shader fails to be added to the pipeline log it
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }
            sh.UseGL();

            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("view", vm.View);
            sh.SetUniformMat4GL("proj", vm.Projection);

            sh.SetUniformVec4GL("progColor", (Vec4)_color);

            GL.BindVertexArray(_VAO);

            GL.DrawElements(_drawMode, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }
    }

    /// <summary>
    /// The type of mesh to generate for the sphere
    /// </summary>
    public enum SphereType
    {
        UVSphere,
        IcoSphere,
        QuadSphere,
        GoldbergPoly
    }
}
