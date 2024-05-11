using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    /// <summary>
    /// Defines methods common to Geometrical objects
    /// </summary>
    public interface IGeometrical
    {
        /// <summary>
        /// Perform mathematical checks on a geometric object to ensure its data is mathematically consistent.
        /// </summary>
        /// <param name="message">The message explaining why the object is not geometrically correct.</param>        
        /// <returns>True if there are no issues false if there are.</returns>
        public abstract bool IsGeometricallyCorrect([NotNullWhen(false)]out string? message);
    }
}
