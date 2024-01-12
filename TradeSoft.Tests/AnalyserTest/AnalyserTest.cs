//using FluentAssertions;
//using FluentAssertions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TradeSoft.Services;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace TradeSoft.Tests.AnalyserTest
{
    public class AnalyserTest
    {
        private readonly Analysis _analysis;

        //constructor
        public AnalyserTest()
        {
            //sut
            _analysis = new Analysis();
        }

        [Fact]
        public void Analysis_SimpleReturn_ShouldCalculateSimpleReturn()
        {
            //Arrange - get variable, classes, ... to test
            float a = 8.0f;
            float b = 4.0f;
            float expectedSimpleReturn = ((a - b) / b) * 100;

            //Act - run the méthode to test
            float result = _analysis.SimpleReturn(a, b);

            //Assert - Waht it should return
            result.Should().Be(expectedSimpleReturn);
        }


        [Fact]
        public void Analysis_TReturn_ShouldCalculateTotalReturnFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new (){ 1.0f, 2.0f, 3.0f, 4.0f };
            float expectedSum = 1.0f + 2.0f + 3.0f + 4.0f;

            //Act - run the méthode to test
            float result = _analysis.TReturn(allReturns);

            //Assert - Waht it should return
            result.Should().Be(expectedSum);
        }

        [Fact]
        public void Analysis_EReturn_ShouldCalculateExpectedReturn()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new (){ 1.0f, 2.0f, 3.0f, 4.0f };
            float totalReturn = 1.0f + 2.0f + 3.0f + 4.0f;
            float expectedReturn = totalReturn / allReturns.Count;

            //Act - run the méthode to test
            float result = _analysis.EReturn(allReturns);

            //Assert - Waht it should return
            result.Should().Be(expectedReturn);
        }

        [Fact]
        public void Analysis_TLoss_ShouldSumAllNegativeReturnsFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f};
            float totalLoss = (-2.0f - 1.0f);

            //Act - run the méthode to test
            float result = _analysis.TLoss(allReturns);

            //Assert - Waht it should return
            result.Should().Be(totalLoss);
        }

        [Fact]
        public void Analysis_MLoss_ShouldValueOfMeanLossFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, 4.0f, -3.0f};
            float totalLoss = (-2.0f - 1.0f - 3.0f);
            int nbLoss = 3;
            float meanLoss = totalLoss / nbLoss;

            //Act - run the méthode to test
            float result = _analysis.MLoss(allReturns, totalLoss);

            //Assert - Waht it should return
            result.Should().Be(meanLoss);
        }

        [Fact]
        public void Analysis_TGain_ShouldSumAllPositiveReturnsFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f };
            float totalGain = (1.0f + 3.0f + 4.0f);

            //Act - run the méthode to test
            float result = _analysis.TGain(allReturns);

            //Assert - Waht it should return
            result.Should().Be(totalGain);
        }

        [Fact]
        public void Analysis_MGain_ShouldValueOfMeanGainFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, 4.0f, -3.0f };
            float totalGain = (1.0f + 3.0f + 4.0f + 4.0f);
            int nbGain = 4;
            float meanGain = totalGain / nbGain;

            //Act - run the méthode to test
            float result = _analysis.MGain(allReturns, totalGain);

            //Assert - Waht it should return
            result.Should().Be(meanGain);
        }

        [Fact]
        public void Analysis_Drawdown_ShouldCalculateDrawdown()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new() { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, -1.0f, 4.0f, -3.0f, -2.0f, -1.0f, 2.0f, 1.0f, -2.0f };
            float expectedDrawdown = (-6.0f);

            //Act - run the méthode to test
            float result = _analysis.Drawdown(allReturns);

            //Assert - Waht it should return
            result.Should().Be(expectedDrawdown);
        }
    }
}
