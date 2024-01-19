using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TradeSoft.Models;


/* This is Analysis class.
 * This class will be instanciated by each strategies to perform some analysis about the performance of the strategy
 * 
 * 
 * --> rappeler que le module est appelé à chaque executionData from broker
 * 
 * --> liste de rendements pour le retourner à la fin 
 * --> expected return on va le garder et l'update, il sera calculer à partir des veleur du ExecuteData actuelle
 * et des rendement précédants stockés dans la liste, de sorte que le trader puissent voir la moyennede rendement
 * qu'il produit avec sa stratégie à tout moment.
 * 
 * Récupération du rendement: (ça normalement c'est fait)
 * on fait un total des Qty à chaque order.
 * Si Qty > 0 : A la prochaine Vente(Sell) on récup le rendement
 * Si Qty < 0 : Au prochain Achat(Buy) on récup le rendement
 * 
 * 
 * --> rechercher comment calculer la volatilité (ecart-type) en backtesting
 * (a chaque execution data, sans que ça explose quand on a 1324652243 execution data)
 * 
 * On garde TLoss et MLoss pour les afficher que quand le rendement actuel est négatif
 * On garde TGain et MGain pour les afficher que quand le rendement actuel est positif
 */

namespace TradeSoft.Services
{
    public class Analysis
    {
        private float initAmount; //monant initial du portfilio
        private float currentAmount;

        private float actualQuantity = 0; //c'est la variable qui va accumuler les quantité pour qu'on puisse connaitre la position, et donc à quel moment on doit calculer le rendement.

        private OrderType currentType;

        /*          About return            */

        private float cumulativeReturn; //c'est un %, return par rapport au montant initial
        private float currentReturn; // par rapport à l'état précédant (montant précédant) (%)
        private float expectedReturn; //expected return per transaction (%)

        //Variable storing the amount of the total losses over all the period (do not taking gains into account)
        //private float totalLoss;

        //Variable storing mean of losses par transactions (only over losses)
        private float meanLoss;

        //Variable storing the amount of the total gain over all the period (do not taking losses into account)
        //private float totalGain;

        //Variable storing mean of gain par transactions (only over gains)
        private float meanGain;

        //Variable calculating the difference between higher and lowest price over the period
        private float maxDrawdown; // possible improvement : donné la période (entre quand et quand c'est arrivé)
        private float worst = 0f;
        private float best = 0f;

        private ExecutionData executed;

        List<float> historicOfReturns = new List<float>();
        Dictionary<float, float> historicReturns = new Dictionary<float, float>(); //key = current amount, value = poucentage of return compare to previous amount

        /*          About risk            */

        //private float variance;
        private float volatility;       //standard deviation
        private float expectancy;
        private float SQN;              //System Quality Number
        private float historicalVaR95;
        private float historicalVaR99;

        /*          Constructor            */
        public Analysis(float initAmount)
        {
            this.initAmount = initAmount;
            currentAmount = initAmount;
        }


        /*      Method for calculation      */
        /* Pour effectuer les méthodes, je me suis demandé si il était préférable de :
         * A) faire des méthodes void qui modifient directement les attribue de ma classe
         * B) faire des méthodes de types return, et ajouter une méthode à la fin qui me permet de toutes les exécuter
         * et d'affecter les returns de ces méthodes aux attributs de la classe
         * 
         * Au final, je choisi l'option B, parce que les types void sont trop difficile à tester (test unitaire).
         * Les types retour me permettrons de faire de bons tests unitaires.
         */


        public void runMethods() //méthode dans laquelle on appelle les autres méthodes et ou on fait les check pour savoir quoi faire
        {
            currentType = executed.Type;     //je récupère le type, car on en a besoin pour savoir si on doit calculer le rendement ou non

            //update amount of portfolio
            currentAmount += executed.Price * executed.Quantity; //pas besoin de vérifier si c'est un sell ou un buy car la qunatité est négative en cas de sell donc le prix sera négatif

            /* step 1 : je regarde si la quantité est positive, 
             * si oui, on est en long, donc on calcule le rendement à la prochaine vente
             * si non, on est en short, donc on calcule le rendement au prochain buy 
             */
            if ((actualQuantity > 0 & currentType == OrderType.sell) || (actualQuantity < 0 & currentType == OrderType.buy)) 
            {
                //Step 2 : calculs of returns
                cumulativeReturn = SimpleReturn(currentAmount, initAmount);

                if (historicReturns.Count == 0)
                {
                    currentReturn = cumulativeReturn;
                }else{
                    currentReturn = SimpleReturn(currentAmount, historicReturns.Keys.Last()); //historicReturns.Keys.Last()
                }
                historicReturns.Add(currentAmount, currentReturn);

                if (historicReturns.Count > 1) //Si c'est pas la première opération, alors on peut ressortir des analyses selon les précédantes executions
                {
                    expectedReturn = EReturn(historicReturns);

                    if(cumulativeReturn > 0) //alors la moyenne de gain, en excluant les pertes
                    {
                        meanGain = MGain(historicReturns);
                    }else{ //alors on retourne la moyenne des pertes quand on exclu les gains
                        meanLoss = MLoss(historicReturns);
                    }

                    //Step 3 : Risk measurement
                    volatility = (float)Math.Sqrt(Var(historicReturns, expectedReturn));

                    //Simplifier en faisant 1 seule méthode, et on passe en paramètre le % de précision
                    historicalVaR95 = HVaR95(historicOfReturns) / initAmount * 100; //en pourcentage par rapport au portefeuille de base
                    historicalVaR99 = HVaR99(historicOfReturns) / initAmount * 100;

                }
                else{
                    expectedReturn = cumulativeReturn;
                    //variance = 0;
                    volatility = 0;
                }
            }
            //Updating best/worst returns if necessary
            if (currentReturn > best)
                best = currentReturn;
            if (currentReturn < worst)
                worst = currentReturn;

            //Calculation of SQN (System Quality Number) used to evaluate the quality of a trading system taking into account both: performance and volatility
            expectancy = Expectancy(historicReturns, meanGain, meanLoss);
            SQN = expectancy / volatility * (float)Math.Sqrt(historicReturns.Count);

            actualQuantity += executed.Quantity; //on update la quantité

        }

        public float SimpleReturn(float current, float init)
        {
            float simpleReturn;
            simpleReturn = (current - init) / init * 100;
            return simpleReturn;
        }

        // Calculation of the Expected Return
        public float EReturn(Dictionary<float, float> dicoReturns)
        {
            float sum = 0;

            foreach (float simpleReturn in dicoReturns.Values)
            {
                sum += simpleReturn;
            }

            expectedReturn = sum / dicoReturns.Count;
            return expectedReturn;
        }

        //Calculate the mean of loss, when return is a loss
        public float MLoss(Dictionary<float, float> dicoReturns)
        {
            //Counting the number of loss
            int numberOfLoss = 0;
            float sumOfLosses = 0f;
            foreach (float returns in dicoReturns.Values)
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
        public float MGain(Dictionary<float, float> dicoReturns)
        {
            //Counting the number of loss
            int numberOfLoss = 0;
            float sumOfLosses = 0f;
            foreach (float returns in dicoReturns.Values)
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

        public float Var(Dictionary<float, float> dicoReturns, float mean)
        {
            float sustraction;
            float sum = 0;

            foreach (float returns in dicoReturns.Values)
            {
                sustraction = returns - mean;
                sum += sustraction * sustraction;
            }

            return sum / dicoReturns.Count;
        }

        public float Expectancy(Dictionary<float, float> dicoReturns, float meanGain, float meanLoss)
        {
            float gainProbability = 0;
            float lossProbability = 0;
            float expectancy = 0;

            foreach (float returns in dicoReturns.Values)
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

        public float HVaR95(List<float> returns)
        {
            returns.Sort();
            float confidenceLevel = 0.05f;
            int index = (int)Math.Floor(confidenceLevel * returns.Count);
            return returns[index];
        }

        public float HVaR99(List<float> returns)
        {
            returns.Sort();
            float confidenceLevel = 0.01f;
            int index = (int)Math.Floor(confidenceLevel * returns.Count);
            return returns[index];
        }





        override
        public String ToString()
        {
            return String.Format("cumulativeReturn: {0}, expectedReturn: {1}, meanLoss: {2}, meanGain: {3}", cumulativeReturn, expectedReturn, meanLoss, meanGain);
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

