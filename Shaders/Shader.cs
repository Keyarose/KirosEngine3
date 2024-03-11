using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Shaders
{
    public class Shader
    {
        protected int _handle;
        protected bool _disposed;
        protected string _name;

        protected string _vertPath;
        protected string _fragPath;

        public int Handle
        { get { return _handle; } }

        public string Name
        { get { return _name; } }

        /// <summary>
        /// Construct a shader from a vertex shader and a fragment shader
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="vertPath">The path to the vertex shader file</param>
        /// <param name="fragPath">The path to the fragment shader file</param>
        public Shader(string name, string vertPath, string fragPath)
        {
            _name = name;
            _vertPath = vertPath;
            _fragPath = fragPath;

            LoadShaderGL();
        }

        /// <summary>
        /// Constructor for co-located shader programs with identical names
        /// </summary>
        /// <param name="name">The name of the shader</param>
        /// <param name="dualPath">The path to shader programs located in the same folder with the same name</param>
        /// <param name="vertexExt">Optional file extension for the vertex shader file, defaults to .vert</param>
        /// <param name="fragmentExt">Optional file extension for the fragment shader file, defaults to .frag</param>
        public Shader(string name, string dualPath, string vertexExt = ".vert", string fragmentExt = ".frag")
        {
            _name = name;
            _vertPath = dualPath + vertexExt;
            _fragPath = dualPath + fragmentExt;

            LoadShaderGL();
        }

        /// <summary>
        /// Handle loading and error checking the shader objects and program (OpenGL)
        /// </summary>
        protected void LoadShaderGL()
        {
            int vertexShader;
            int fragmentShader;

            string vertShaderSource;
            string fragShaderSource;

            try
            {
                //load the shader programs from their files
                vertShaderSource = File.ReadAllText(_vertPath);
                fragShaderSource = File.ReadAllText(_fragPath);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(ex.Message);
                Console.WriteLine(ex.Message);
                return;
            }

            //create the shader objects
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertShaderSource);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragShaderSource);


            //compile the vertex shader
            GL.CompileShader(vertexShader);

            //check for errors
            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int checkV);
            if (checkV == 0)
            {
                string info = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine(info);
                Logger.WriteToLog(info);
            }

            //compile the fragment shader
            GL.CompileShader(fragmentShader);

            //check for errors
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int checkF);
            if (checkF == 0)
            {
                string info = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine(info);
                Logger.WriteToLog(info);
            }

            //create the shader program handle
            _handle = GL.CreateProgram();

            GL.AttachShader(vertexShader, _handle);
            GL.AttachShader(fragmentShader, _handle);

            GL.LinkProgram(_handle);

            //check for program errors
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int checkP);
            if (checkP == 0)
            {
                string info = GL.GetProgramInfoLog(_handle);
                Console.WriteLine(info);
                Logger.WriteToLog(info);
            }

            //cleanup
            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        /// <summary>
        /// Provide access to the shader attribute locations (OpenGL)
        /// </summary>
        /// <param name="name">The name of the attribute to get the location of</param>
        /// <returns>The location integer of the attribute named</returns>
        public int GetAttribLocationGL(string name)
        {
            return GL.GetAttribLocation(_handle, name);
        }

        /// <summary>
        /// Activates the shader for use in the rendering pipeline (OpenGL)
        /// </summary>
        public void UseGL()
        {
            GL.UseProgram(_handle);
        }

        /// <summary>
        /// Set a uniform in the shader program to the given integer value (OpenGL)
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="value">The value to set it to</param>
        public void SetUniformIntGL(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);

            GL.Uniform1(location, value);
        }

        /// <summary>
        /// Disposes of the shader program and marks the shader as disposed (OpenGL)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void DisposeGL(bool disposing)
        {
            if (!_disposed)
            {
                GL.DeleteProgram(_handle);
                _disposed = true;
            }
        }

        /// <summary>
        /// Public access to dispose of the shader
        /// </summary>
        public void Dispose()
        {
            DisposeGL(true);
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            if (_disposed == false)
            {
                Console.WriteLine("Shader named: " + _name + ", not properly disposed of.");
                Logger.WriteToLog("Shader named: " + _name + ", not properly disposed of.");
            }
        }
    }
}
