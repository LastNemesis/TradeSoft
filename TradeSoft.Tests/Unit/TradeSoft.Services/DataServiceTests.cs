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
        public void FetchData_WhenFileExists_ReturnsCorrectCount()
        {
            // Arrange
            DataService dataService = new DataService();
            string filePath = Path.Combine("..", "..", "..", "..", "TradeSoft", "Resources", "tradesoft-ticks-sample.csv");

            // Act
            int expectedCount = 129324;
            int actualCount = dataService.FetchData(filePath);

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
