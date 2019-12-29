using System.Collections.Generic;

namespace VAA.CommonComponents.Interfaces
{
    public interface IRecordImporter
    {
        IEnumerable<string> GetDataSetNames();
        IEnumerable<Dictionary<string, string>> Import(string dataSetName);
        int TotalNumRows { get; }
    }
}