using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TradeSoft.Models;


/* This is Analysis class.
 * This class will be instanciated by each strategies to perform some analysis about the performance of the strategy
 * 
 */

namespace TradeSoft.Services
{
    public class Analysis
    {
        //Variable storing the amount of the total return over all the period
        private float totalReturn;

        //Variable storing the amount of the expected return per transaction
        private float expectedReturn;

        //Variable storing the amount of the total losses over all the period (do not taking gains into account)
        private float totalLoss;

        //Variable storing mean of losses par transactions (only over losses)
        private float meanLoss;

        //Variable storing the amount of the total gain over all the period (do not taking losses into account)
        private float totalGain;

        //Variable storing mean of gain par transactions (only over gains)
        private float meanGain;

        //Variable calculating the difference between higher and lowest price over the period
        private float maxDrawdown;


        //list of orders
        private Order[] allOrders;

        //List<float> allReturn = new List<float>();


        /*      Method for calculation      */
        /* Pour effectuer les méthodes, je me suis demandé si il était préférable de :
         * A) faire des méthodes void qui modifient directement les attribue de ma classe
         * B) faire des méthodes de types return, et ajouter une méthode à la fin qui me permet de toutes les exécuter
         * et d'affecter les returns de ces méthodes aux attributs de la classe
         * 
         * Au final, je choisi l'option B, parce que les types void sont trop difficile à tester (test unitaire).
         * Les types retour me permettrons de faire de bons tests unitaires.
         */

        //Calculation of all simple returns and puting them into a list
        public float SimpleReturn(float a, float b)
        {
            float simpleReturn;
            simpleReturn = (a - b) / b * 100;
            return simpleReturn;
        }

        //Calculation of the total return
        public float TReturn(List<float> returns)
        {
            float sum = 0;
            foreach (float r in returns)
            {
                sum += r;
            }

            return sum;
        }

        // Calculation of the Expected Return
        public float EReturn(int lenghtListReturn) //on passera en paramètre le nombre d'élément dans la liste de return
        {
            expectedReturn = totalReturn / (lenghtListReturn); 
            return expectedReturn;
        }

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

        //Calculate the mean of loss, when return is a loss
        public float MLoss(List<float> returns)
        {
            //Counting the number of loss
            int numberOfLoss = 0;
            foreach (float r in returns)
            {
                if (r < 0)
                    numberOfLoss++;
            }

            //Caluculating mean of losses
            float mean = totalLoss / numberOfLoss;
            return mean;
        }

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
        }

        //Calculation of mean gain (from only positive return)
        public float MGain(List<float> returns)
        {
            //Counting the number of gain
            int numberOfGain = 0;
            foreach (float r in returns)
            {
                if (r < 0)
                    numberOfGain++;
            }

            //Caluculating mean of gain
            float mean = totalLoss / numberOfGain;
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
                }
                else
                {
                    sumOfFollowingLoss = 0;
                }

                if (sumOfFollowingLoss < drawdown)
                {
                    drawdown = sumOfFollowingLoss;
                }

            }
            return drawdown;

        }


        //Method that call all other methodes to assigne value to the attributes of the class
        public void runMethods(Order[] orders)
        {
            /* Run SimpleReturn method in order to calculate all returns, and then put them into a list of simple returns */

            //First, we need a list of all simple returns, and a variable to store tha returend value of the function
            List<float> allSimpleReturns = new ();
            float simpleReturn;

            //Go through the list of order, and 
            for (int i = 0; i < orders.Length; i++)
            {
                if (i > 0) //because according to the formula of simple return, there is no return for the first value
                {
                    simpleReturn = SimpleReturn(orders[i].EData.Price, orders[i - 1].EData.Price);
                    allSimpleReturns.Add(simpleReturn);
                }
            }

            /* Run TReturn method and assigne the result to totalReturn */
            totalReturn = TReturn(allSimpleReturns);

            /* Run EReturn method and assigne the result to expectedReturn */
            expectedReturn = EReturn(allSimpleReturns.Count);

            /* Run TLoss method and assigne the result to expectedReturn */
            totalLoss = TLoss(allSimpleReturns);

            /* Run MLoss method and assigne the result to meanLoss */
            meanLoss = MLoss(allSimpleReturns);

            /* Run TLoss method and assigne the result to expectedReturn */
            totalGain = TLoss(allSimpleReturns);

            /* Run MLoss method and assigne the result to meanLoss */
            meanGain = MLoss(allSimpleReturns);

            /* Run MLoss method and assigne the result to meanLoss */
            maxDrawdown = Drawdown(allSimpleReturns);
        }


        /*
        private void DrawDown(Order[] orders)
        {
            float minPrice = 0;
            float maxPrice = 0;
            
            //Find min and max values
            foreach (Order order in orders)
            {
                if (order.EData.Price < minPrice)
                    minPrice = order.EData.Price;
                if (order.EData.Price > maxPrice)
                    maxPrice = order.EData.Price;
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
                    if (orders[i].EData.Price < orders[i-1].EData.Price)
                    {
                        isDecreasing = true;
                        maxPrice = orders[i-1].EData.Price;

                        //checking if it continues to decreas so that we can obtain the lowest price.
                        do
                        {
                            if (i < orders.Length)
                            {
                                i++;
                                if (orders[i].EData.Price > orders[i - 1].EData.Price)
                                    isDecreasing = false;
                            }
                            else
                                isDecreasing = false;

                            minPrice = orders[i].EData.Price;

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

