using KirosEngine3.Math.Vector;
using KirosEngine3.Shaders;
using KirosEngine3.Mesh;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Textures
{
    public class Text
    {
        protected Font _font;
        protected string _shader;//todo: set as default text shader
        protected string _text;
        protected Vec2 _pos;
        protected SentenceData _sentence;

        /// <summary>
        /// The font to draw the text with
        /// </summary>
        public Font Font { get { return _font; } set { _font = value; } }

        /// <summary>
        /// The shader to be used
        /// </summary>
        public string Shader { get { return _shader; } set { _shader = value; } }

        /// <summary>
        /// The text itself
        /// </summary>
        public string Verse { get { return _text; } set { _text = value; UpdateSentence(); } }

        /// <summary>
        /// The position of the text
        /// </summary>
        public Vec2 Position { get { return _pos; } set { _pos = value; } }

        /// <summary>
        /// The color of the text
        /// </summary>
        public Vec4 Color { get { return _sentence.Color; } set { _sentence.Color = value; } }

        /// <summary>
        /// Basic constructor for a Text object
        /// </summary>
        /// <param name="pos">The screen origin position of the text</param>
        public Text(Vec2 pos)
        {
            _font = Font.Default;
            _shader = "text"; //todo: define environment var for default text shader
            _text = string.Empty;
            _pos = pos;

            _sentence = new SentenceData
            {
                VertexBuffer = [],
                IndexBuffer = [],
                Color = new Vec4(0.0f, 0.0f, 0.0f, 1.0f) //default to black
            };
        }

        public Text(Vec2 pos, Font font, string text)
        {
            _font = font;
            _shader = "text";
            _text = text;
            _pos = pos;

            _sentence = font.TextForString(text, pos.AsVec3());
            _sentence.Color = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);
        }

        /// <summary>
        /// Update the sentence data for any changes in position or text
        /// </summary>
        private void UpdateSentence()
        {
            //todo: check for text or position change, if only position just apply vector addition
            SentenceData ns = _font.TextForString(_text, _pos.AsVec3());

            _sentence.VertexBuffer = ns.VertexBuffer;
            _sentence.IndexBuffer = ns.IndexBuffer;
        }

        /// <summary>
        /// Set the text to the given string
        /// </summary>
        /// <param name="text">The string to set the text to</param>
        public void SetText(string text)
        {
            Verse = text;
        }

        public void Draw(ViewMatrixes vm, TextureUnit tu)
        {
            _font.UseFont(tu);

            Shader sh = ShaderManager.Instance[_shader];
            sh.UseGL();
            sh.SetUniformIntGL("texture0", (int)tu);

            //setup vertex array
            int vertexArray = GL.GenVertexArray(); //todo: move array stuff to load
            GL.BindVertexArray(vertexArray);

            //vertex and index buffers
            int vertexBuffer = GL.GenBuffer();
            int indexBuffer = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _sentence.VertexBuffer.Length * TexturedVertex.SizeInBytesU, _sentence.VertexBuffer, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _sentence.IndexBuffer.Length * sizeof(int), _sentence.IndexBuffer, BufferUsageHint.DynamicDraw);

            //debug
            foreach (var v in _sentence.VertexBuffer)
            {
                Console.WriteLine(v.ToString());
            }
            

            TexturedVertex.SetVertexPositionAttrib(sh, "aPosition");

            TexturedVertex.SetVertexUVAttrib(sh, "aUV");

            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("view", vm.View);
            sh.SetUniformMat4GL("proj", vm.Orthographic);

            sh.SetUniformVec4GL("aTextColor", _sentence.Color);
            
            GL.DrawElements(PrimitiveType.Triangles, _sentence.IndexBuffer.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Update()
        {

        }

        public void Dispose()
        {

        }
    }

    public struct SentenceData
    {
        public TexturedVertex[] VertexBuffer;
        public uint[] IndexBuffer;
        public Vec4 Color;
    }
}
