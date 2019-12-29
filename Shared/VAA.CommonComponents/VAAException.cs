using System;
using System.Linq;

namespace VAA.CommonComponents
{
    /// <summary>
    /// VAA Exception
    /// </summary>
    public class VAAException : Exception
    {
        public VAAException(string message)
            : base(message)
        {
        }
    }
}
