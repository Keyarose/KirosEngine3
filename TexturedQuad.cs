using KirosEngine3.Math.Data;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Mesh.Primitives;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3
{
    internal class TexturedQuad
    {
        private Quad _quad;
        private string _textureName;

        private int _VAO;
        private int _VBO;
        private int _EBO;

        private string _shaderName = "texture";

        private bool _loaded = false;

        public TexturedQuad(Quad quad, string textureName)
        {
            _quad = quad;
            _textureName = textureName;
        }

        public TexturedQuad(Vec3[] points, uint[] indices, string textureName)
        {
            _quad = new Quad(points, indices, Color4.Black);
            _textureName = textureName;
        }

        public void Init()
        {
            TexturedVertex[] verts = VertexHelpers.TextureVertFromColorVert(_quad.GetVertexData(), [new(0f, 1f), new(1f, 1f), new(1f, 0f), new(0f, 0f)]);
            uint[] indices = _quad.GetIndices();

            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex.SizeInBytesU * verts.Length, verts, BufferUsageHint.StaticDraw);

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            Shader sh = ShaderManager.Instance[_shaderName];

            sh.SetAttribsGL<TexturedVertex>();

            GL.BindVertexArray(0);

            _loaded = true;
        }

        public void Draw(ViewMatrixes vm, TextureUnit tu)
        {
            if (!_loaded)
            {
                Logger.WriteToLog("Attempt to draw unloaded cube object: {0}", this);
                Console.WriteLine("Attempt to draw unloaded cube object: {0}", this);
                //todo: write to debug console
                return;
            }

            //if the shader fails to be added to the pipeline log it
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }
            sh.UseGL();

            TextureManager.UseTextureGL(_textureName, tu);
            sh.SetUniformIntGL("texture0", (int)tu);

            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("view", vm.View);
            sh.SetUniformMat4GL("proj", vm.Orthographic);

            GL.BindVertexArray(_VAO);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }
    }
}
