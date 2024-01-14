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
 * 
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
        private float initAmount = 10000.0f; //monant initial du portfilio, pour tester simplement on le met à 10 000
        private float currentAmount = 10000.0f; //au début le current amount = init amount 

        // --> il faudra creer un constructeur de la classe qui permet d'initialiser les valeur de initAmount et de currentAmount, mais pour le moment je ne le fais pas

        private float actualQuantity = 0; //c'est la variable qui va accumuler les quantité pour qu'on puisse connaitre la position, et donc à quel moment on doit calculer le rendement.

        private OrderType currentType;

        /*          About return            */

        private float currentReturn;
        private float expectedReturn; //expected return per transaction

        //Variable storing the amount of the total losses over all the period (do not taking gains into account)
        private float totalLoss;

        //Variable storing mean of losses par transactions (only over losses)
        private float meanLoss;

        //Variable storing the amount of the total gain over all the period (do not taking losses into account)
        private float totalGain;

        //Variable storing mean of gain par transactions (only over gains)
        private float meanGain;

        //Variable calculating the difference between higher and lowest price over the period
        private float maxDrawdown; // possible improvement : donné la période (entre quand et quand c'est arrivé)

        private ExecutionData executed;

        List<float> historicOfReturns = new List<float>();

        /*          About risk            */

        private float variance;
        private float SD;
        private float historicalVaR95;
        private float historicalVaR99;



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
            currentAmount += executed.Price; //update du montant du portefeuille

            /* step 1 : je regarde si la quantité est positive, 
             * si oui, on est en long, donc on calcule le rendement à la prochaine vente
             * si non, on est en short, donc on calcule le rendement au prochain buy 
             */
            if ((actualQuantity >= 0 & currentType == OrderType.sell) || (actualQuantity < 0 & currentType == OrderType.buy)) 
            {
                //Step 2 : calcul du rendement
                currentReturn = SimpleReturn(currentAmount, initAmount);
                historicOfReturns.Add(currentReturn);

                if(historicOfReturns.Count > 0) //Si c'est pas la première opération, alors on peut ressortir des analyses selon les précédantes executions
                {
                    expectedReturn = EReturn(historicOfReturns);

                    if(currentReturn > 0) //alors on retourne le totalGain et le meanGain
                    {
                        totalGain = TGain(historicOfReturns);
                        meanGain = MGain(historicOfReturns, totalGain);
                    }else{ //alors on retourne le totalLoss et le meanLoss
                        totalLoss = TLoss(historicOfReturns);
                        meanLoss = MLoss(historicOfReturns, totalLoss);
                    }

                    variance = Var(historicOfReturns, expectedReturn);
                    SD = (float)Math.Sqrt(variance);
                    historicalVaR95 = HVaR95(historicOfReturns);
                    historicalVaR99 = HVaR99(historicOfReturns);

                }
                else{
                    expectedReturn = currentReturn;
                    variance = 0;
                    SD = 0;
                }
            }
            actualQuantity += executed.Quantity; //on update la quantité
        }

        public float SimpleReturn(float a, float b)
        {
            float simpleReturn;
            simpleReturn = (a - b) / b * 100;
            return simpleReturn;
        }

        // Calculation of the Expected Return
        public float EReturn(List<float> listOfReturns)
        {
            float sum = 0;
            for(int i = 0; i < listOfReturns.Count; i++)
            {
                sum += listOfReturns[i];
            }
            
            expectedReturn = sum / listOfReturns.Count;
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
        public float MLoss(List<float> returns, float sumOfLoss)
        {
            //Counting the number of loss
            int numberOfLoss = 0;
            foreach (float r in returns)
            {
                if (r < 0)
                    numberOfLoss++;
            }

            //Caluculating mean of losses
            float mean = sumOfLoss / numberOfLoss;
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
        public float MGain(List<float> returns, float sumOfGain)
        {
            //Counting the number of gain
            int numberOfGain = 0;
            foreach (float r in returns)
            {
                if (r > 0)
                    numberOfGain++;
            }

            //Caluculating mean of gain
            float mean = sumOfGain / numberOfGain;
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
            return String.Format("currentReturn: {0}, expectedReturn: {1}, totalLoss: {2}, meanLoss: {3}, totalGain: {4}, meanGain: {5}", currentReturn, expectedReturn, totalLoss, meanLoss, totalGain, meanGain);
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

