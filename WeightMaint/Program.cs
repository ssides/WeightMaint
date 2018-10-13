using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeightMaint.BusinessObjects;
using WeightMaint.Interfaces;

namespace WeightMaint
{
    class Program
    {
        private static IServices _services;
        private static Config _config;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            _config = GetConfig();

            _services = new Services.Services(_config);

            if (args.Count() != 1)
            {
                Usage();
                return;
            }

            var fileText = _services.GetFileLines(args[0]);
            var errors = _services.ValidateText(fileText);
            if (errors.Count() > 0)
            {
                DisplayErrors(errors);
                return;
            }

            var data = _services.GetFileData(fileText);
            errors = _services.ValidateData(data, fileText);
            if (errors.Count() > 0)
            {
                DisplayErrors(errors);
                return;
            }

            if (data.Count() < _config.MinDataPoints)
            {
                Console.Write($"\r\nThere is insufficient data.  Please enter at least {_config.MinDataPoints} measurements.\r\n");
                return;
            }

            var r = _services.GetResult(data);
            DisplayResult(r);
        }

        private static void DisplayResult(Result r)
        {
            var dblFormat = "0.000";

            Console.Write($"b = {r.B.ToString("0.0")}   a = {r.A.ToString(dblFormat)}\r\n"
                + $"now = {(r.A * 10 + r.B).ToString("0.0")} \u00b1 {r.StdDevY.ToString(dblFormat)} lb.\r\n");
        }

        private static void Usage()
        {
            Console.Write("Usage: WeightMaint <filename>\r\n\r\n"
                + "file format is <daynumber>,<weight> one pair per line\r\n"
                + "daynumber is 1 to 10 of type integer\r\n"
                + "weight is floating point in the configured range\r\n\r\n");
        }

        private static void DisplayErrors(IEnumerable<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.Write($"Error: '{error.Line}'\r\n"
                    + $"   {error.ErrorMessage}\r\n");
            }
        }

        private static Config GetConfig()
        {
            var result = new Config();

            var reader = new System.Configuration.AppSettingsReader();
            result.Separator = (string)reader.GetValue("Separator", typeof(string));
            result.MinWeight = (int)reader.GetValue("MinWeight", typeof(int));
            result.MaxWeight = (int)reader.GetValue("MaxWeight", typeof(int));
            result.MinDataPoints = (int)reader.GetValue("MinDataPoints", typeof(int));
            return result;
        }
    }
}
