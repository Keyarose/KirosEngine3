using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KirosEngine3.Shaders
{
    /// <summary>
    /// Data type of a shader input.
    /// </summary>
    [Serializable]
    public enum ShaderValueType
    {
        [XmlEnum(Name = "unknown")]
        Unknown,
        [XmlEnum(Name = "float")]
        Float,
        [XmlEnum(Name = "vec2")]
        Vec2,
        [XmlEnum(Name = "vec3")]
        Vec3,
        [XmlEnum(Name = "vec4")]
        Vec4,
        [XmlEnum(Name = "mat2")]
        Mat2,
        [XmlEnum(Name = "mat3")]
        Mat3,
        [XmlEnum(Name = "mat4")]
        Mat4
    }

    /// <summary>
    /// Determines which attributes to activate in a shader.
    /// </summary>
    [Serializable]
    [Flags]
    public enum ShaderAttribFlags
    {
        /// <summary>
        /// Set none.
        /// </summary>
        None = 0,

        /// <summary>
        /// Set the Position attribute.
        /// </summary>
        Position = 1,

        /// <summary>
        /// Set the Normal attribute.
        /// </summary>
        Normal = 2,

        /// <summary>
        /// Set the Color attribute.
        /// </summary>
        Color = 4,

        /// <summary>
        /// Set the UV attribute.
        /// </summary>
        UV = 8,

        /// <summary>
        /// Set both Position and Normal attributes.
        /// </summary>
        PosAndNormal = Position | Normal,

        /// <summary>
        /// Set both Position and Color attributes.
        /// </summary>
        PosAndColor = Position | Color,
    }
}
