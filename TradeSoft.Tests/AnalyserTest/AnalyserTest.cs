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
            _analysis = new Analysis(10000f);
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


        /*[Fact]
        public void Analysis_TReturn_ShouldCalculateTotalReturnFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new (){ 1.0f, 2.0f, 3.0f, 4.0f };
            float expectedSum = 1.0f + 2.0f + 3.0f + 4.0f;

            //Act - run the méthode to test
            float result = _analysis.TReturn(allReturns);

            //Assert - Waht it should return
            result.Should().Be(expectedSum);
        }*/

        [Fact]
        public void Analysis_EReturn_ShouldCalculateExpectedReturn()
        {
            //Arrange - get variable, classes, ... to test
            Dictionary<float, float> dicoReturns = new Dictionary<float, float> {{ 80, -20 },
                                                                                 { 70, -12.5f},
                                                                                 { 90, 28.5f}};
            float totalReturn = -20 - 12.5f + 28.5f;
            float expectedReturn = totalReturn / dicoReturns.Count;

            //Act - run the méthode to test
            float result = _analysis.EReturn(dicoReturns);

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
            //List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, 4.0f, -3.0f};
            Dictionary<float, float> dicoReturns = new Dictionary<float, float>() {{80f, -20f},
                                                                                 { 70, -12.5f},
                                                                                 { 90, 28.5f},
                                                                                 { 95, 5.56f},
                                                                                 { 92, -3.16f}};
            float totalLoss = (-20f - 12.5f - 3.16f);
            int nbLoss = 3;
            float meanLoss = totalLoss / nbLoss;

            //Act - run the méthode to test
            float result = _analysis.MLoss(dicoReturns);

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
            //List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, 4.0f, -3.0f };
            Dictionary<float, float> dicoReturns = new Dictionary<float, float>() {{80f, -20f},
                                                                                 { 70, -12.5f},
                                                                                 { 90, 28.5f},
                                                                                 { 95, 5.56f},
                                                                                 { 92, -3.16f}};
            float totalGain = (28.5f + 5.56f);
            int nbGain = 2;
            float meanGain = totalGain / nbGain;

            //Act - run the méthode to test
            float result = _analysis.MGain(dicoReturns);

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

        [Fact]
        public void Analysis_Var_ShouldReturnVariance()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new() { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, 4.0f};
            float mean = (1.0f - 2.0f + 3.0f + 4.0f - 1.0f + 4.0f)/6;
            float variance = (0.25f + 12.25f + 2.25f + 6.25f + 6.25f + 6.25f)/6;

            //Act - run the méthode to test
            float result = _analysis.Var(allReturns, mean);

            //Assert - Waht it should return
            result.Should().Be(variance);
        }

        [Fact]
        public void Analysis_HVaR95_ShouldCalculateValueAtRiskAt95Percent()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new() { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, -1.0f, 4.0f, -3.0f, -2.0f, -1.0f, 2.0f, 1.0f, -2.0f };
            float VaR = (-3.0f);

            //Act - run the méthode to test
            float result = _analysis.HVaR95(allReturns);

            //Assert - Waht it should return
            result.Should().Be(VaR);
        }
    }
}
