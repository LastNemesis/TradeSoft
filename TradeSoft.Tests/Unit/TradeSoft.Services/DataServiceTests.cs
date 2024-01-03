using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using TradeSoft.Models;
using TradeSoft.Services;

namespace TradeSoft.Tests.Unit.TradeSoft.Services
{
    public class DataServiceTests
    {
        [Fact]
        public void FetchData_WhenFileExists_ReturnsCountAndNotNullValues()
        {
            // Arrange
            DataService dataService = new();
            string filePath = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-sample.csv");

            // Act
            List<Tick> tickList = dataService.FetchData(filePath);
            int expectedCount = 129324;
            int actualCount = tickList.Count;

            // Assert the number of lines
            Assert.Equal(expectedCount, actualCount);

            // Assert that the value of the first line is not null
            Assert.NotNull(tickList[0]);

            // Assert that the value of the last line is not null
            Assert.NotNull(tickList[expectedCount - 1]);
        }
    }
}
