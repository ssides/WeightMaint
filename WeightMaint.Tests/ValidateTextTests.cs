using System.Collections.Generic;
using System.Linq;
using WeightMaint.BusinessObjects;
using WeightMaint.Interfaces;
using Xunit;

namespace WeightMaint.Tests
{
    public class ValidateTextTests
    {
        private readonly IServices _services;

        public ValidateTextTests()
        {
            _services = new Services.Services(new Config { Separator = "," });
        }

        [Fact]
        public void ItFlagsTextLines()
        {
            var lineNumber = 1;
            var lines = new List<FileLine>();

            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "a" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "a,a" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1," });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1,." });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1,1." });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1,3.3.0" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = "1a,4.5" });
            lines.Add(new FileLine { LineNumber = lineNumber++, Line = " 1, a45" });

            var errors = _services.ValidateText(lines);

            Assert.Equal(lines.Count(), errors.Count());
        }
    }
}
