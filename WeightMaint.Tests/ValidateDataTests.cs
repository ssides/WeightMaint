using System;
using System.Collections.Generic;
using System.Linq;
using WeightMaint.BusinessObjects;
using WeightMaint.Interfaces;
using Xunit;

namespace WeightMaint.Tests
{
    public class ValidateDataTests
    {
        private readonly IServices _services;

        public ValidateDataTests()
        {
            _services = new Services.Services(new Config
            {
                Separator = ",",
                MaxWeight = 100,
                MinWeight = 50,
            });
        }

        [Fact]
        public void ItFlagsDataLines()
        {
            // Arrange
            List<FileLine> lines = DataSetup();
            var data = _services.GetFileData(lines);

            // Act
            var dataErrors = _services.ValidateData(data, lines);

            // Assert
            Assert.Equal(lines.Count(), dataErrors.Count());
        }

        private List<FileLine> DataSetup()
        {
            var lineNumber = 1;
            var lines = new List<FileLine>();

            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1,1" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1,51 " });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "2,49" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "11, 70" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = " 10 , 110.0" });

            var textErrors = _services.ValidateText(lines);
            if (textErrors.Count() != 0)
            {
                var e = textErrors.FirstOrDefault();
                throw new Exception($"Text error: Line: {e.Line}: {e.ErrorMessage}");
            }

            return lines;
        }
    }
}
