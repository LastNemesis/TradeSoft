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
        public void GetProccessedTicks_ReturnsCount()
        {
            // Arrange
            DataService dataService = new();

            // Act
            List<Tick> tickList = dataService.GetAllProccessedTicks();

            int expectedCount = 1080;
            int actualCount = tickList.Count;

            // Assert the number of lines
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
