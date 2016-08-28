using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace kMeans
{
    public class dataPoint
    {
        public Dictionary<string, double> categories;
        private int centroid;

        public dataPoint(string[] categoryArr)
        {
            categories = new Dictionary<string, double>();
            centroid = -1;

            string cat;
            for (int i = 0; i < categoryArr.Length; i++)
            {
                cat = categoryArr[i];
                categories[cat] = 0.0;
            }
        }

        public void setCategory(string key, double confidence)
        {
            categories[key] = confidence;
        }

        public double getCategory(string key)
        {
            return categories[key];
        }

        public void setCentroid(int centroid)
        {
            this.centroid = centroid;
        }

        public int getCentroid()
        {
            return centroid;
        }
    }

    public class kMeansAlg
    { 
        public void centroidInitializer(dataPoint[] dataPoints, Dictionary<string, double>[] centroids, int classifications)
        {
            Random rndKey = new Random();
            
            for (int i = 0; i < classifications; i++)
            {
                int key = rndKey.Next(0, dataPoints.Length);
                Dictionary<string, double> temp = centroids[i];
                /*foreach (KeyValuePair<string, double> k in temp)

                {
                    centroids[i][k.Key] = dataPoints[key].getCategory(k.Key);
                }*/
                var enumerator = dataPoints[0].categories.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var pair = enumerator.Current;
                    centroids[i][pair.Key] = dataPoints[key].getCategory(pair.Key);
                }
            }
        }



        public void clusterAssignment(dataPoint[] dataPoints, Dictionary<string, double>[] centroids)
        {
            int numberOfCentroids = centroids.Length;

            Console.WriteLine("iteration");
            for (int i = 0; i < dataPoints.Length; i++)
            {
                double minDistance = 9999999;
                for (int j = 0; j < numberOfCentroids; j++)
                {
                    double distance = 0;
                    foreach (KeyValuePair<string, double> k in dataPoints[i].categories)
                    {
                        double diff = k.Value - centroids[j][k.Key];
                        //Console.WriteLine("centroids = " + centroids[j][k.Key]);
                        distance += (diff * diff);
                    }
                    Console.WriteLine("distance: " + distance);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        dataPoints[i].setCentroid(j);
                    }
                }
            }
        }

        public void moveCentroid(dataPoint[] dataPoints, Dictionary<string, double>[] centroids, string[] categoryArr)
        {
            int numberOfCentroids = centroids.Length;
            int[] assignedDataPoints = new int[numberOfCentroids];
            double[] avg = new double[numberOfCentroids];

            for (int x = 0; x < numberOfCentroids; x++)
            {
                assignedDataPoints[x] = 0;
                avg[x] = 0;
            }

            for (int i = 0; i < dataPoints.Length; i++)
            {
                
                assignedDataPoints[dataPoints[i].getCentroid()]++;
            }

            for (int i = 0; i < categoryArr.Length; i++)
            {
                for (int j = 0; j < dataPoints.Length; j++)
                {
                    avg[dataPoints[j].getCentroid()] += dataPoints[j].getCategory(categoryArr[i]);
                }

                for (int j = 0; j < numberOfCentroids; j++)
                {
                    if(assignedDataPoints[j] != 0)
                    {
                        avg[j] /= assignedDataPoints[j];
                        centroids[j][categoryArr[i]] = avg[j];
                    }
                        
                    
                }

            }
        }

        public int[] kMeans(string[] categoryArr, Dictionary<string, double>[] imageDetails, int iterations, int classifications)
        {
            dataPoint[] dataPoints = new dataPoint[imageDetails.Length];
            Dictionary<string, double>[] centroids = new Dictionary<string, double>[classifications];
            int[] centroidMap = new int[imageDetails.Length];

            for (int i = 0; i < classifications; i++)
            {
                centroids[i] = new Dictionary<string, double>();
                string cat;
                for (int j = 0; j < categoryArr.Length; j++)
                {
                    cat = categoryArr[j];
                    centroids[i][cat] = 0.0;
                }
            }

            for (int i = 0; i < imageDetails.Length; i++)
            {
                dataPoints[i] = new dataPoint(categoryArr);
            }

            for (int i = 0; i < imageDetails.Length; i++)
            {
                if (imageDetails[i] != null)
                {
                    foreach (KeyValuePair<string, double> k in imageDetails[i])
                    {
                        dataPoints[i].setCategory(k.Key, k.Value);
                    }
                }
            }

            centroidInitializer(dataPoints, centroids, classifications);

            for (int i = 0; i < iterations; i++)
            {
                clusterAssignment(dataPoints, centroids);
                moveCentroid(dataPoints, centroids, categoryArr);
            }


            for (int i = 0; i < imageDetails.Length; i++)
            {
                centroidMap[i] = dataPoints[i].getCentroid();

            }

            return centroidMap;

        }

    }


}
