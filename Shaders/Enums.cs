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
        /// <summary>
        /// Value is unknown type.
        /// </summary>
        [XmlEnum(Name = "unknown")]
        Unknown,
        /// <summary>
        /// Value is float.
        /// </summary>
        [XmlEnum(Name = "float")]
        Float,
        /// <summary>
        /// Value is vector2.
        /// </summary>
        [XmlEnum(Name = "vec2")]
        Vec2,
        /// <summary>
        /// Value is vector3.
        /// </summary>
        [XmlEnum(Name = "vec3")]
        Vec3,
        /// <summary>
        /// Value is vector4.
        /// </summary>
        [XmlEnum(Name = "vec4")]
        Vec4,
        /// <summary>
        /// Value is matrix2.
        /// </summary>
        [XmlEnum(Name = "mat2")]
        Mat2,
        /// <summary>
        /// Value is matrix3.
        /// </summary>
        [XmlEnum(Name = "mat3")]
        Mat3,
        /// <summary>
        /// Value is matrix4
        /// </summary>
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
