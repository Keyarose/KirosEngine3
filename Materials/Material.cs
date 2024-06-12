using KirosEngine3.Math.Data;
using KirosEngine3.Mesh;
using KirosEngine3.Shaders;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace KirosEngine3.Materials
{
    /// <summary>
    /// Defines a Material to be applied to a renderable object.
    /// </summary>
    public class Material : IXmlSerializable
    {
        /// <summary>
        /// The name of the material.
        /// </summary>
        protected string _name = "";
        /// <summary>
        /// A description of the material.
        /// </summary>
        protected string _description = "";

        /// <summary>
        /// The ambient color of the material.
        /// </summary>
        protected Color4 _ambientColor;
        /// <summary>
        /// The diffuse color of the material.
        /// </summary>
        protected Color4 _diffuseColor;
        /// <summary>
        /// The specular color of the material.
        /// </summary>
        protected Color4 _specularColor;
        /// <summary>
        /// The shininess of the material.
        /// </summary>
        protected float _shininess;

        /// <summary>
        /// The names of the textures used by the material.
        /// </summary>
        protected string[] _textures = [];

        /// <summary>
        /// The name of the shader used by the material.
        /// </summary>
        protected string _shaderName = "";

        /// <summary>
        /// The name of the Material.
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// The description of the Material.
        /// </summary>
        public string Description { get { return _description; } set { _description = value; } }

        /// <summary>
        /// The ambient color of the Material.
        /// </summary>
        public Color4 AmbientColor { get { return _ambientColor; } set { _ambientColor = value; } }

        /// <summary>
        /// The diffuse color of the Material.
        /// </summary>
        public Color4 DiffuseColor { get { return _diffuseColor; } set { _diffuseColor = value; } }

        /// <summary>
        /// The specular color of the Material.
        /// </summary>
        public Color4 SpecularColor { get { return _specularColor; } set { _specularColor = value; } }

        /// <summary>
        /// The shininess of the Material.
        /// </summary>
        public float Shininess { get { return _shininess; } set { _shininess = value; } }

        /// <summary>
        /// The name of the shader to be used by the Material.
        /// </summary>
        public string ShaderName { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        public Material() { }

        #region Load
        /// <summary>
        /// Set a shader attribute for the shader being used by the material.
        /// </summary>
        /// <param name="attribFlag">The attribute to set.</param>
        /// <param name="settings">The attribute settings.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool SetShaderAttrib(ShaderAttribFlags attribFlag, ShaderAttribSettings settings)
        {
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                Logger.WriteToLog("Material: {0}, references missing shader named: {1}", this, _shaderName);
                Console.WriteLine("Material: {0}, references missing shader named: {1}", this, _shaderName);

                return false;
            }

            if (attribFlag.HasFlag(ShaderAttribFlags.Position))
            {
                sh.SetPositionAttribGL(settings);
            }
            else if (attribFlag.HasFlag(ShaderAttribFlags.Normal))
            {
                //sh.SetNormalAttribGL(settings);
            }
            else if (attribFlag.HasFlag(ShaderAttribFlags.Color))
            {
                sh.SetColorAttribGL(settings);
            }
            else if (attribFlag.HasFlag(ShaderAttribFlags.UV))
            {
                sh.SetUVAttribGL(settings);
            }

            return true;
        }

        /// <summary>
        /// Set a shader attribute on the shader being used by the material that isn't one of the common attribute types.
        /// </summary>
        /// <param name="settings">The settings for the attribute.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool SetCustomShaderAttrib(ShaderAttribSettings settings)
        {
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                Logger.WriteToLog("Material: {0}, references missing shader named: {1}", this, _shaderName);
                Console.WriteLine("Material: {0}, references missing shader named: {1}", this, _shaderName);

                return false;
            }

            sh.SetAttribGL(settings);

            return true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// Apply the material to the caller.
        /// </summary>
        /// <param name="vm">The view matrices for the object the material is being applied to.</param>
        /// <returns>True if successfully applied, false otherwise.</returns>
        public bool Apply(ViewMatrixes vm)
        {
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                Logger.WriteToLog("Material: {0}, references missing shader named: {1}", this, _shaderName);
                Console.WriteLine("Material: {0}, references missing shader named: {1}", this, _shaderName);

                return false;
            }

            sh.UseGL();

            //set uniforms

            return true;
        }
        #endregion

        #region Xml
        /// <inheritdoc/>
        public XmlSchema? GetSchema()
        {
            return null;
        }

        /// <inheritdoc/>
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
