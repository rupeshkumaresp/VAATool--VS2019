using System;
using System.Collections.Generic;
using System.IO;
namespace VAA.BusinessComponents.Interfaces
{
    /// <summary>
    /// Flight Schedule operations
    /// </summary>
    public interface IFlightSchedule
    {
        void UploadFlightSchedule(Stream stream, bool clearSchedule);
        void ImportFlightSchedule(Dictionary<string, string> data);
    }
}