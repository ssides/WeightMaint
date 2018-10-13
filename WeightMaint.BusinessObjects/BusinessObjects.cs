using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightMaint.BusinessObjects
{
    public class Config {
        public string Separator { get; set; }
        public int MinWeight { get; set; }
        public int MaxWeight { get; set; }
        public int MinDataPoints { get; set; }
    }

    public class FileLine
    {
        public int LineNumber { get; set; }
        public bool IntInt { get; set; }
        public bool IntFloat { get; set; }
        public string Line { get; set; }
    }

    public class FileData
    {
        public int LineNumber { get; set; }
        public double DayNumber { get; set; }
        public double Weight { get; set; }
    }

    public class Error
    {
        public string Line { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class Result
    {
        public double B { get; set; }
        public double A { get; set; }
        public double Now { get; set; }
        public double StdDevY { get; set; }
    }

}
