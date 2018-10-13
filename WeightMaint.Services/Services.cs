using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using WeightMaint.BusinessObjects;
using WeightMaint.Interfaces;

namespace WeightMaint.Services
{
    public class Services : IServices
    {
        private Config _config;
        private Regex _intInt;
        private Regex _intFloat;

        public Services(Config config)
        {
            _config = config;
            _intInt = new Regex($"^ *([0-9]+) *\\{_config.Separator} *([0-9]+) *$");
            _intFloat = new Regex($"^ *([0-9]+) *\\{_config.Separator} *([0-9]+\\.[0-9]+) *$");
        }

        public IEnumerable<FileData> GetFileData(IEnumerable<FileLine> fileText)
        {
            var result = new List<FileData>();

            foreach (var l in fileText)
            {
                Match m;

                if (l.IntInt)
                    m = _intInt.Match(l.Line);
                else
                    m = _intFloat.Match(l.Line);

                double dayNumber;
                double.TryParse(m.Groups[1].Value, out dayNumber);
                double weight;
                double.TryParse(m.Groups[2].Value, out weight);
                var d = new FileData { LineNumber = l.LineNumber, DayNumber = dayNumber, Weight = weight };
                result.Add(d);
            }

            return result;
        }

        public IEnumerable<FileLine> GetFileLines(string filename)
        {
            var result = new List<FileLine>();
            var i = 1;

            using (var sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Trim().Length > 0)
                        result.Add(new FileLine { LineNumber = i++, Line = line });
                }
            }

            return result;
        }


        public IEnumerable<Error> ValidateData(IEnumerable<FileData> fileData, IEnumerable<FileLine> fileText)
        {
            var e1 = fileData
                 .GroupBy(f => f.DayNumber)
                 .Select(f => new { C = f.Count(), f.FirstOrDefault().LineNumber })
                 .Where(f => f.C > 1)
                 .Join(fileText, v => v.LineNumber, ft => ft.LineNumber, (v, ft) => new { v, ft })
                 .Select(ft => new Error { Line = ft.ft.Line, ErrorMessage = "Duplicate day number" });

            var e2 = fileData
                .Where(fd => fd.Weight > _config.MaxWeight || fd.Weight < _config.MinWeight)
                 .Join(fileText, v => v.LineNumber, ft => ft.LineNumber, (v, ft) => new { v, ft })
                 .Select(ft => new Error { Line = ft.ft.Line, ErrorMessage = "Weight out of range" });

            var e3 = fileData
                .Where(fd => fd.DayNumber > 10 || fd.DayNumber == 0)
                 .Join(fileText, v => v.LineNumber, ft => ft.LineNumber, (v, ft) => new { v, ft })
                 .Select(ft => new Error { Line = ft.ft.Line, ErrorMessage = "Day number must be between 1 and 10" });

            return e1.Union(e2).Union(e3).ToList();
        }

        public IEnumerable<Error> ValidateText(IEnumerable<FileLine> fileText)
        {
            foreach (var l in fileText)
            {
                l.IntFloat = _intFloat.IsMatch(l.Line);
                l.IntInt = _intInt.IsMatch(l.Line);
            }

            return fileText.Where(va => !va.IntFloat && !va.IntInt)
                  .Join(fileText, va => va.LineNumber, ft => ft.LineNumber, (va, ft) => new { VA = va, FT = ft })
                  .Select(e => new Error
                  {
                      Line = e.FT.Line,
                      ErrorMessage = "Invalid input."
                  })
                  .ToList();
        }

        public Result GetResult(IEnumerable<FileData> fileData)
        {
            var M = Matrix<double>.Build;
            var V = Vector<double>.Build;
            var xdata = fileData.Select(x => x.DayNumber).ToArray();
            var ydata = fileData.Select(y => y.Weight).ToArray();

            var X = M.DenseOfColumnVectors(
                  V.Dense(xdata.Length, 1.0),
                  V.Dense(xdata));

            var denseY = V.Dense(ydata);

            var p = X.QR().Solve(denseY);

            var r = new Result
            {
                A = p[1],
                B = p[0],
                StdDevY = Statistics.PopulationStandardDeviation(ydata)
            };

            return r;
        }

    }
}
