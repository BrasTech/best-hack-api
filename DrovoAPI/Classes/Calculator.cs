using DrovoAPI.Interfaces;
using DrovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Classes
{
    public static class Calculator
    {
        /* The Winkler modification will not be applied unless the 
     * percent match was at or above the mWeightThreshold percent 
     * without the modification. 
     * Winkler's paper used a default value of 0.7
     */
        private static readonly double mWeightThreshold = 0.7;

        /* Size of the prefix to be concidered by the Winkler modification. 
         * Winkler's paper used a default value of 4
         */
        private static readonly int mNumChars = 4;


        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (perfect match) to 1 (no match). 
        /// </summary>
        /// <param name="aString1">First String</param>
        /// <param name="aString2">Second String</param>
        /// <returns></returns>
        public static double distance(string aString1, string aString2)
        {
            return 1.0 - proximity(aString1, aString2);
        }


        /// <summary>
        /// Returns the Jaro-Winkler distance between the specified  
        /// strings. The distance is symmetric and will fall in the 
        /// range 0 (no match) to 1 (perfect match). 
        /// </summary>
        /// <param name="aString1">First String</param>
        /// <param name="aString2">Second String</param>
        /// <returns></returns>
        public static double proximity(string aString1, string aString2)
        {
            int lLen1 = aString1.Length;
            int lLen2 = aString2.Length;
            if (lLen1 == 0)
                return lLen2 == 0 ? 1.0 : 0.0;

            int lSearchRange = Math.Max(0, Math.Max(lLen1, lLen2) / 2 - 1);

            // default initialized to false
            bool[] lMatched1 = new bool[lLen1];
            bool[] lMatched2 = new bool[lLen2];

            int lNumCommon = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                int lStart = Math.Max(0, i - lSearchRange);
                int lEnd = Math.Min(i + lSearchRange + 1, lLen2);
                for (int j = lStart; j < lEnd; ++j)
                {
                    if (lMatched2[j]) continue;
                    if (aString1[i] != aString2[j])
                        continue;
                    lMatched1[i] = true;
                    lMatched2[j] = true;
                    ++lNumCommon;
                    break;
                }
            }
            if (lNumCommon == 0) return 0.0;

            int lNumHalfTransposed = 0;
            int k = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                if (!lMatched1[i]) continue;
                while (!lMatched2[k]) ++k;
                if (aString1[i] != aString2[k])
                    ++lNumHalfTransposed;
                ++k;
            }
            // System.Diagnostics.Debug.WriteLine("numHalfTransposed=" + numHalfTransposed);
            int lNumTransposed = lNumHalfTransposed / 2;

            // System.Diagnostics.Debug.WriteLine("numCommon=" + numCommon + " numTransposed=" + numTransposed);
            double lNumCommonD = lNumCommon;
            double lWeight = (lNumCommonD / lLen1
                             + lNumCommonD / lLen2
                             + (lNumCommon - lNumTransposed) / lNumCommonD) / 3.0;

            if (lWeight <= mWeightThreshold) return lWeight;
            int lMax = Math.Min(mNumChars, Math.Min(aString1.Length, aString2.Length));
            int lPos = 0;
            while (lPos < lMax && aString1[lPos] == aString2[lPos])
                ++lPos;
            if (lPos == 0) return lWeight;
            return lWeight + 0.1 * lPos * (1.0 - lWeight);

        }
        public static void NormalizePopularity(List<IStoreMap> storeMap)
        {
            foreach (var storeData in storeMap)
            {
                int maxValue = storeData.Products.Max(u => u.CommentsCountValue);
                if (maxValue == 0)
                    return;
                foreach (var product in storeData.Products)
                {
                    product.PopularityValue = Math.Round((double)(100 * product.CommentsCountValue / maxValue), 2);
                }
            }
        }
        public static List<TotalProduct> GroupProducts(List<IStoreMap> storeMap, string query, double level = 0.8, double outputLevel = 0.4)
        {
            List<ProductProcessor> productProcessors = new List<ProductProcessor>();
            int productsCount = storeMap.Sum(u => u.ProductsCount);
            int count = 0;
            for(int i = 0; i < storeMap.Count; i++)
            {
                for(int j = 0; j < storeMap[i].ProductsCount; j++)
                {
                    productProcessors.Add(new ProductProcessor
                    (
                        count,
                        i,
                        storeMap[i].Products[j]
                    ));
                    count++;
                }
            }


            double[] vectorQuery = new double[productsCount];
            double[,] matrix = new double[productsCount, productsCount];

            for(int i = 0; i < productProcessors.Count; i++)
            {
                vectorQuery[i] = proximity(productProcessors[i].Product.Title, query);
                productProcessors[i].Priority = vectorQuery[i];
            }

            int startPoint = 1;
            for (int i = 0; i < productProcessors.Count; i++)
            {
                for (int j = startPoint; j < productProcessors.Count; j++)
                {
                    if (i != j)
                    {
                        matrix[i, j] = proximity(productProcessors[i].Product.Title, productProcessors[j].Product.Title);
                        if (matrix[i, j] < level)
                            matrix[i, j] = 0;
                        matrix[j, i] = matrix[i, j];
                    }
                }
                startPoint++;
            }

            bool[] visited = new bool[productsCount];
            List<List<int>> clusters = new List<List<int>>();


            while (true)
            {
                int unvisited = -1;
                for(int i = 0; i < visited.Length; i++)
                {
                    if (!visited[i])
                    {
                        unvisited = i;
                        break;
                    }
                }
                if (unvisited == -1)
                    break;
                List<int> cluster = new List<int>();
                DFS(unvisited, matrix,ref cluster,ref visited);
                clusters.Add(cluster);
            }

            List<TotalProduct> products = new List<TotalProduct>();
            foreach(var cluster in clusters)
            {
                // Продукты по кластеру!
                var items = productProcessors.Where(u => cluster.Contains(u.ProductId)).ToList();

                // Уникальные ид сайтов
                var sitesUniqueIds = items.Select(p => p.StoreId).Distinct().ToList();

                double maxValue = 0;
                int indexOfMaxValue = 0;
                for(int i = 0; i < items.Count; i++)
                {
                    double tempMax = vectorQuery[items[i].ProductId];
                    if (tempMax > maxValue)
                    {
                        maxValue = tempMax;
                        indexOfMaxValue = i;
                    }
                }

                var productEtal = items[indexOfMaxValue].Product;

                List<TotalMarket> tms = new List<TotalMarket>();

                double prices = 0;
                int itemsRealCount = 0;
                for(int i = 0; i < sitesUniqueIds.Count(); i++)
                {
                    IOrderedEnumerable<Product> bestProductsFromStore = items.Where(a => a.StoreId == sitesUniqueIds[i] && a.Priority >= outputLevel)
                        .Select(u => u.Product)
                        .OrderByDescending(l => l.Popularity)
                        .OrderByDescending(k => k.Rating);

                    if(bestProductsFromStore.Count() == 0)
                    {
                        bestProductsFromStore = items.Where(a => a.StoreId == sitesUniqueIds[i])
                        .Select(u => u.Product)
                        .OrderByDescending(l => l.Popularity)
                        .OrderByDescending(k => k.Rating);
                    }

                    var bestProductFromStore = bestProductsFromStore.FirstOrDefault();

                    if (bestProductFromStore != null)
                    {
                        TotalMarket tm = new TotalMarket
                        {
                            Name = storeMap[sitesUniqueIds[i]].Name,
                            Description = storeMap[sitesUniqueIds[i]].About,
                            Logo = storeMap[sitesUniqueIds[i]].Logo,
                            Price = bestProductFromStore.PriceValue,
                            Link = bestProductFromStore.Link,
                        };
                        tms.Add(tm);
                        prices += bestProductFromStore.PriceValue;
                        itemsRealCount++;
                    }
                }

                double avgPrice = 0;
                if(itemsRealCount != 0)
                {
                    avgPrice = Math.Round((double)(prices / itemsRealCount), 2);
                }

                TotalProduct tp = new TotalProduct
                {
                    Markets = tms,
                    Name = productEtal.Title,
                    Logo = productEtal.Image,
                    Rating = productEtal.RatingValue,
                    Popularity = productEtal.PopularityValue,
                    AveragePrice = avgPrice
                };
                products.Add(tp);


            }
            return products;
        }


        public static void DFS(int id, double[,]matrix, ref List<int> cluster, ref bool[] visited)
        {
            visited[id] = true;
            cluster.Add(id);
            for(int j = 0; j < matrix.GetLength(0); j++)
            {
                if(id != j)
                {
                    if (!visited[j])
                    {
                        if(matrix[id,j] != 0)
                        {
                            DFS(j, matrix, ref cluster, ref visited);
                        }
                    }
                }
            }
        }

    }
}
