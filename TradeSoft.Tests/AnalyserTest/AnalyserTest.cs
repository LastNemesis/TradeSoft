﻿//using FluentAssertions;
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

        [Fact]
        public void Analysis_EReturn_ShouldCalculateExpectedReturn()
        {
            //Arrange - get variable, classes, ... to test
            List<float> listReturns = new List<float> { -20, -12.5f, 28.5f };

            float totalReturn = -20 - 12.5f + 28.5f;
            float expectedReturn = totalReturn / listReturns.Count;

            //Act - run the méthode to test
            float result = _analysis.EReturn(listReturns);

            //Assert - Waht it should return
            result.Should().Be(expectedReturn);
        }

        [Fact]
        public void Analysis_MLoss_ShouldValueOfMeanLossFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            //List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, 4.0f, -3.0f};
            List<float> listReturns = new List<float> { -20f, -12.5f, 28.5f, 5.56f, -3.16f };
            float totalLoss = (-20f - 12.5f - 3.16f);
            int nbLoss = 3;
            float meanLoss = totalLoss / nbLoss;

            //Act - run the méthode to test
            float result = _analysis.MLoss(listReturns);

            //Assert - Waht it should return
            result.Should().Be(meanLoss);
        }

        [Fact]
        public void Analysis_MGain_ShouldValueOfMeanGainFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            //List<float> allReturns = new () { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, 4.0f, -3.0f };
            List<float> listReturns = new List<float> { -20f, -12.5f, 28.5f, 5.56f, -3.16f };
            float totalGain = (28.5f + 5.56f);
            int nbGain = 2;
            float meanGain = totalGain / nbGain;

            //Act - run the méthode to test
            float result = _analysis.MGain(listReturns);

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
            List<float> listReturns = new List<float> { -20f, -12.5f, 28.5f, 5.56f, -3.16f };
            float mean = (-20f - 12.5f + 28.5f + 5.56f - 3.16f) / 5; //-0,32
            float variance = (387.3024f + 148.3524f + 830.5924f + 34.5744f + 8.0656f) / 5f;

            //Act - run the méthode to test
            float result = _analysis.Var(listReturns, mean);

            //Assert - Waht it should return
            result.Should().Be(variance);
        }

        [Fact]
        public void Analysis_Expectancy_ShouldReturnExpectancy()
        {
            //Arrange - get variable, classes, ... to test
            List<float> listReturns = new List<float> { -20f, -12.5f, 28.5f, 5.56f, -3.16f };
            float meanGain = 17.03f;
            float meanLoss = -11.87f;
            float expectancy = -(meanGain * 0.4f + meanLoss * 0.6f) / meanLoss;


            //Act - run the méthode to test
            float result = _analysis.Expectancy(listReturns, meanGain, meanLoss);

            //Assert - Waht it should return
            result.Should().Be(expectancy);
        }


        [Fact]
        public void Analysis_HVaR_ShouldCalculateValueAtRiskAt95Percent()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new() { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f, -1.0f, 4.0f, -3.0f, -2.0f, -1.0f, 2.0f, 1.0f, -2.0f };
            float VaR = (-3.0f);

            //Act - run the méthode to test
            float result = _analysis.HVaR(allReturns, 95);

            //Assert - Waht it should return
            result.Should().Be(VaR);
        }


        [Fact]
        public void Analysis_SharpRatio_ShouldCalculateFloat() //Return a percentage
        {
            //Arrange
            List<float> listReturns = new List<float> { -20f, -12.5f, 28.5f, 5.56f, -3.16f };
            float mean = (-20f - 12.5f + 28.5f + 5.56f - 3.16f) / 5f; //-0,32
            float variance = (387.3024f + 148.3524f + 830.5924f + 34.5744f + 8.0656f) / 5f;
            float stDeviation = (float)Math.Sqrt(variance);
            //rf = risk free return
            float rf = 3.876f;
            float result = (mean - rf) / stDeviation;

            //Act - Run the method
            float sharpRatio = _analysis.SharpRatio(mean, stDeviation);

            //Assert 
            result.Should().Be(sharpRatio);
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
}

        [Fact]
        public void Analysis_TLoss_ShouldSumAllNegativeReturnsFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new() { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f };
            float totalLoss = (-2.0f - 1.0f);

            //Act - run the méthode to test
            float result = _analysis.TLoss(allReturns);

            //Assert - Waht it should return
            result.Should().Be(totalLoss);
        }

        [Fact]
        public void Analysis_TGain_ShouldSumAllPositiveReturnsFromListOfReturns()
        {
            //Arrange - get variable, classes, ... to test
            List<float> allReturns = new() { 1.0f, -2.0f, 3.0f, 4.0f, -1.0f };
            float totalGain = (1.0f + 3.0f + 4.0f);

            //Act - run the méthode to test
            float result = _analysis.TGain(allReturns);

            //Assert - Waht it should return
            result.Should().Be(totalGain);
        }*/
    }
}
