using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeightMaint.BusinessObjects;
using WeightMaint.Interfaces;
using Xunit;

namespace WeightMaint.Tests
{
    public class GetResultTests
    {
        private readonly IServices _services;

        public GetResultTests()
        {
            _services = new Services.Services(new Config
            {
                Separator = ",",
                MaxWeight = 100,
                MinWeight = 50,
            });
        }

        [Fact]
        public void ItWorks()
        {
            //Act
            var data = DataSetup();

            //Arrange
            var result = _services.GetResult(data);

            //Assert
            Assert.True((result.B - 167.2) < 0.01);
            Assert.True((result.A + 0.049) < 0.0001);
            Assert.True((result.Now - 166.7) < 0.01);
            Assert.True((result.StdDevY - 0.875) < 0.0001);
        }

        private List<FileData> DataSetup()
        {
            var lineNumber = 1;
            var lines = new List<FileLine>();

            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1,167" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "2,166 " });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "4,168" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "5,167.5" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = " 6,168" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = " 7,166.5" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "8,166" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = " 9,165.5" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "10,167.5" });

            var textErrors = _services.ValidateText(lines);
            if (textErrors.Count() != 0)
            {
                var e = textErrors.FirstOrDefault();
                throw new Exception($"Text error: Line: {e.Line}: {e.ErrorMessage}");
            }

            return _services.GetFileData(lines).ToList();
        }
    }
}
