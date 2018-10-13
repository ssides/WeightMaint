using System.Collections.Generic;
using WeightMaint.BusinessObjects;

namespace WeightMaint.Interfaces
{
    public interface IServices
    {
        IEnumerable<FileLine> GetFileLines(string filename);
        IEnumerable<Error> ValidateText(IEnumerable<FileLine> fileText);
        IEnumerable<FileData> GetFileData(IEnumerable<FileLine> fileText);
        IEnumerable<Error> ValidateData(IEnumerable<FileData> fileData, IEnumerable<FileLine> fileText);
        Result GetResult(IEnumerable<FileData> fileData);
    }
}
