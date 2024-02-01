﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TradeSoft.Models;


/* This is Analysis class.
 * This class will be instanciated by each strategies to perform some analysis about the performance of the strategy.
 * Analysis module is called for each execution
 */

namespace TradeSoft.Services
{
    public class Analysis
    {
        /*    Amount of portfolio at different time (t0, t and t-1),and quantity    */
        private float initAmount;         
        private float currentAmount;
        private float previousAmount;

        private float actualQuantity = 0; //compute qauntity so that we can know the position (shrt or long)


        /*          About return            */

        private float cumulativeReturn; // return according to initial amount  (in %)
        private float currentReturn;    // return according to previous amount (in %)
        private float expectedReturn;   // expected return per transaction     (in %)

        //private float totalLoss; //cumulation of losses over all the period
        private float meanLoss;

        //private float totalGain; //cumulation of losses over all the period
        private float meanGain;

        private float maxDrawdown; // possible improvement : give period (when does it occurs ?)
        private float worst = 0f;
        private float best = 0f;

        /*          About risk            */

        //private float variance;
        private float volatility;       //standard deviation
        private float sharpRatio;       //My portfolio execess returns per unit of the portfolio total risk Bigger is better 
        private float expectancy;
        private float SQN;              //System Quality Number
        private float historicalVaR95;
        private float historicalVaR99;

        /*             Other               */
        //Initialisation of an internal clock to get the average time of an open position to be close
        Stopwatch sw = new Stopwatch();
        List<TimeSpan> openPositionTimes = new List<TimeSpan>();

        public ExecutionBit executed { get; set; }

        List<float> historicOfReturns = new List<float>();

        /*          Constructor            */
        public Analysis(float initAmount)
        {
            this.initAmount = initAmount;
            currentAmount = initAmount;
        }


        // Main method of the class
        public void runMethods()
        {
            //Start my inerClock when we take a position
            if (actualQuantity == 0)
            {
                sw.Start();
            }

            //update amount of portfolio
            currentAmount += executed.Price * executed.Quantity;

            /* step 1 : check if quantity > 0, 
             * yes --> long position, we compute return on the next sell
             * no --> short position, we compute return on the next buy
             */
            if ((actualQuantity > 0) || (actualQuantity < 0)) 
            {
                //Step 2 : calculs of returns
                cumulativeReturn = SimpleReturn(currentAmount, initAmount);

                if (historicOfReturns.Count == 0)
                {
                    currentReturn = cumulativeReturn;
                }else{
                    currentReturn = SimpleReturn(currentAmount, previousAmount);
                }

                historicOfReturns.Add(currentReturn);
                previousAmount = currentAmount;

                if (historicOfReturns.Count > 1) //If it's not the first operation, we can make analysis according to previous results
                {
                    expectedReturn = EReturn(historicOfReturns);

                    if(cumulativeReturn > 0)
                    {
                        meanGain = MGain(historicOfReturns);
                    }else{
                        meanLoss = MLoss(historicOfReturns);
                    }

                    //Step 3 : Risk measurement
                    volatility = (float)Math.Sqrt(Var(historicOfReturns, expectedReturn));
                    sharpRatio = SharpRatio(expectedReturn, volatility);

                    historicalVaR95 = HVaR(historicOfReturns, 95) / initAmount * 100; // in % according to initial initial portfolio
                    historicalVaR99 = HVaR(historicOfReturns, 99) / initAmount * 100;

                }
                else{
                    expectedReturn = cumulativeReturn;
                    volatility = 0;
                }

            }
            //Updating best/worst returns if necessary
            if (currentReturn > best)
                best = currentReturn;
            if (currentReturn < worst)
                worst = currentReturn;

            //Calculation of SQN (System Quality Number) used to evaluate the quality of a trading system taking into account both: performance and volatility
            expectancy = Expectancy(historicOfReturns, meanGain, meanLoss);

            /* If SQN is between:
             * 1.6 - 1.9 Below average
             * 2.0 - 2.4 Average
             * 2.5 - 2.9 Good
             * 3.0 - 5.0 Excellent
             * 5.1 - 6.9 Superb
             * 7.0 - Holy Grail?
             * SQN is deemed reliable if nbr of trade >= 30
             */
            SQN = expectancy / volatility * (float)Math.Sqrt(historicOfReturns.Count);

            //When the position close meaning when quantity = 0 we stop the clock
            if ((actualQuantity > 0 && actualQuantity + executed.Quantity < 0) || (actualQuantity < 0 && actualQuantity + executed.Quantity > 0))
            {
                sw.Stop();
                openPositionTimes.Add(sw.Elapsed);
                sw.Restart();
            }else if (executed.Quantity + actualQuantity == 0)
            {
                sw.Stop();
                openPositionTimes.Add(sw.Elapsed);
                sw.Reset();
            }

            actualQuantity += executed.Quantity; //on update la quantité
        }

        public float SimpleReturn(float current, float init)
        {
            float simpleReturn;
            simpleReturn = (current - init) / init * 100;
            return simpleReturn;
        }

        // Calculation of the Expected Return
        public float EReturn(List<float> dicoReturns)
        {
            float sum = 0;

            foreach (float simpleReturn in dicoReturns)
            {
                sum += simpleReturn;
            }

            expectedReturn = sum / dicoReturns.Count;
            return expectedReturn;
        }

        //Calculate the mean of loss, when return is a loss
        public float MLoss(List<float> dicoReturns)
        {
            //Counting the number of loss
            int numberOfLoss = 0;
            float sumOfLosses = 0f;
            foreach (float returns in dicoReturns)
            {
                if (returns < 0)
                {
                    numberOfLoss++;
                    sumOfLosses += returns;
                }
            }

            //Caluculating mean of losses
            float mean = sumOfLosses / numberOfLoss;
            return mean;
        }

        //Calculation of mean gain (from only positive return)
        public float MGain(List<float> dicoReturns)
        {
            //Counting the number of loss
            int numberOfLoss = 0;
            float sumOfLosses = 0f;
            foreach (float returns in dicoReturns)
            {
                if (returns >= 0)
                {
                    numberOfLoss++;
                    sumOfLosses += returns;
                }
            }

            //Caluculating mean of losses
            float mean = sumOfLosses / numberOfLoss;
            return mean;
        }

        //Calculation of maximum Drawdown (see definition at the end) 
        public float Drawdown(List<float> returns)
        {
            float sumOfFollowingLoss = 0;
            float drawdown = 0;

            for (int i = 1; i < returns.Count; i++)
            {
                if (returns[i] < 0)
                {
                    sumOfFollowingLoss += returns[i];
                }else{
                    sumOfFollowingLoss = 0;
                }

                if (sumOfFollowingLoss < drawdown)
                    drawdown = sumOfFollowingLoss;
            }
            return drawdown;
        }

        //Computation of the Variance
        public float Var(List<float> dicoReturns, float mean)
        {
            float sustraction;
            float sum = 0;

            foreach (float returns in dicoReturns)
            {
                sustraction = returns - mean;
                sum += sustraction * sustraction;
            }

            return sum / dicoReturns.Count;
        }

        //Compute the Sharp Ratio
        public float SharpRatio(float expectedReturns, float st_deviation)
        {
            //rf = Risk free rate, taken from: https://fr.tradingview.com/markets/bonds/prices-france/ the 23/03/2024
            float rf = 3.876f / 86400000f; //(%), and multiplied by convertor factor because 3.876% is the daily expected return.

            float sharp_ratio = (expectedReturns - rf) / st_deviation;
            return sharp_ratio;
        }

        //Computation of the Value at Risk using Historical data
        public float HVaR(List<float> returns, float confidenceLevel)
        {
            returns.Sort();
            confidenceLevel = (100 - confidenceLevel)/100;
            int index = (int)Math.Floor(confidenceLevel * returns.Count);
            return returns[index];
        }


        //To be able to calculate the System Quality Number (SQN)
        public float Expectancy(List<float> dicoReturns, float meanGain, float meanLoss)
        {
            float gainProbability = 0;
            float lossProbability = 0;
            float expectancy = 0;

            foreach (float returns in dicoReturns)
            {
                if(returns > 0)
                {
                    gainProbability++;
                }else{
                    lossProbability ++;
                }
            }

            gainProbability = gainProbability / dicoReturns.Count;
            lossProbability = lossProbability / dicoReturns.Count;

            expectancy = -(gainProbability * meanGain + lossProbability * meanLoss) / meanLoss;

            return expectancy;
        }

  
        override
        public String ToString()
        {
            TimeSpan Sum = TimeSpan.Zero;
            foreach(TimeSpan position in openPositionTimes)
            {
                Sum += position;
            }
            if(openPositionTimes.Count > 0)
            {
                TimeSpan averageTime = TimeSpan.FromTicks(Sum.Ticks / openPositionTimes.Count());
                return String.Format("cumulativeReturn: {0}, actualReturn: {1}, meanLoss: {2}, meanGain: {3}, Volatility: {4}, Sharp_Ratio: {5}, Historique VaR: {6}, Average opentime position: {7}", cumulativeReturn, historicOfReturns.Count > 0 ? historicOfReturns[^1] : 0.0f, meanLoss, meanGain, volatility, sharpRatio, historicalVaR95, averageTime);
            } else
            {
                return String.Format("cumulativeReturn: {0}, actualReturn: {1}, meanLoss: {2}, meanGain: {3}, Volatility: {4}, Sharp_Ratio: {5}, Historique VaR: {6}, Average opentime position: {7}", cumulativeReturn, historicOfReturns.Count > 0 ? historicOfReturns[^1] : 0.0f, meanLoss, meanGain, volatility, sharpRatio, historicalVaR95, null);
            }
        }

        /*
//Calculation of total gain (only positive returns)
public float TGain(List<float> returns)
{
    float sum = 0;
    foreach (float r in returns)
    {
        if (r > 0)
            sum += r;
    }
    return sum;

//Calculation of the total of losses (negatives returns)
public float TLoss(List<float> returns)
{
    float sumOfLosses = 0;
    foreach (float r in returns)
    {
        if (r < 0)
            sumOfLosses += r;
    }

    return sumOfLosses;
}

public float Var(List<float> returns, float mean)
{
    float sustraction;
    float sum = 0;
    for (int i = 0; i < returns.Count; i++)
    {
        sustraction = returns[i] - mean;
        sum += sustraction * sustraction;
    }
    return sum/returns.Count;
}*/


        /*
        private void DrawDown(Order[] orders)
        {
            float minPrice = 0;
            float maxPrice = 0;
            
            //Find min and max values
            foreach (Order order in orders)
            {
                if (order.ExecutionData.Price < minPrice)
                    minPrice = order.ExecutionData.Price;
                if (order.ExecutionData.Price > maxPrice)
                    maxPrice = order.ExecutionData.Price;
            }

            drawDown = maxPrice - minPrice;
        }

        private void Drawdown(Order[] orders)
        {
            float minPrice = 0;
            float maxPrice = 0;
            float decrease = 0;
            float drawdown = 0;

            bool isDecreasing = false;

            for (int i = 0; i < orders.Length; i++)
            {
                if (i == 0)
                    continue;
                else
                {
                    if (orders[i].ExecutionData.Price < orders[i-1].ExecutionData.Price)
                    {
                        isDecreasing = true;
                        maxPrice = orders[i-1].ExecutionData.Price;

                        //checking if it continues to decreas so that we can obtain the lowest price.
                        do
                        {
                            if (i < orders.Length)
                            {
                                i++;
                                if (orders[i].ExecutionData.Price > orders[i - 1].ExecutionData.Price)
                                    isDecreasing = false;
                            }
                            else
                                isDecreasing = false;

                            minPrice = orders[i].ExecutionData.Price;

                        }while (isDecreasing);

                        //calculating the difference 
                        decrease = maxPrice - minPrice;
                        if (decrease > drawdown)
                            drawdown = decrease;

                    }
                }

            }
        }
        */
    }
}


/* The maximum drawdown is the largest gap between a top and a trench.
 * In other words, it’s the largest downward slope.
 * Mathematically, it can be defined as the largest cumulative loss
 */

