using System;
using System.Collections.Generic;
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
        /// <returns>True if there are no issues false if there are.</returns>
        public abstract bool IsGeometricallyCorrect(out string? message);
    }
}
