using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        int numRepeat = 100;
        int max = 1000; //1000000;
        int min = 100;
        int stepsize = 100;
        int numsteps = (max - min) / stepsize;

        // Arrays to store average timings for each function
        float[] timeAverage = new float[numsteps];
        float[] timeBubbleSort = new float[numsteps];
        float[] timeMin = new float[numsteps];
        float[] timeMax = new float[numsteps];
        float[] timePrint = new float[numsteps];

        List<Result> results = new List<Result>();

        for (int i = 0; i < numsteps; i++)
        {
            Random rd = new Random();
            int numdrones = i * stepsize + min;
            Console.WriteLine("Current num drones = " + numdrones);
            Flock flock = new Flock(numdrones);
            flock.Init((int)(0.9 * numdrones));

            // Update the drones to randomize the values
            flock.Update();

            // Test the average, max, min, print, and bubblesort functions

            // Average test
            var watch = new Stopwatch();
            watch.Start();
            for (int rep = 0; rep < numRepeat; rep++)
            {
                flock.average("battery");
                flock.average("temperature");
                flock.average("wind");
            }
            watch.Stop();
            long time = watch.ElapsedTicks;
            float averageTicks = (float)time / numRepeat;
            timeAverage[i] = averageTicks * 100;

            // Max test
            watch = new Stopwatch();
            watch.Start();
            for (int rep = 0; rep < numRepeat; rep++)
            {
                flock.max("battery");
                flock.max("temperature");
                flock.max("wind");
            }
            watch.Stop();
            time = watch.ElapsedTicks;
            averageTicks = (float)time / numRepeat;
            timeMax[i] = averageTicks * 100;

            // Min test
            watch = new Stopwatch();
            watch.Start();
            for (int rep = 0; rep < numRepeat; rep++)
            {
                flock.min("battery");
                flock.min("temperature");
                flock.min("wind");
            }
            watch.Stop();
            time = watch.ElapsedTicks;
            averageTicks = (float)time / numRepeat;
            timeMin[i] = averageTicks * 100;

            // Print test
            watch = new Stopwatch();
            watch.Start();
            for (int rep = 0; rep < numRepeat; rep++)
            {
                int no = rd.Next((int)(0.9 * numdrones));
                Console.WriteLine("Print Drone ID: " + no);
                flock.print(no);
            }
            watch.Stop();
            time = watch.ElapsedTicks;
            averageTicks = (float)time / numRepeat;
            timePrint[i] = averageTicks * 100;

            // Bubble sort test (sorting based on battery, temperature, or wind)
            watch = new Stopwatch();
            watch.Start();
            for (int rep = 0; rep < numRepeat; rep++)
            {
                flock.bubblesort("battery");
                flock.bubblesort("temperature");
                flock.bubblesort("wind");
            }
            watch.Stop();
            time = watch.ElapsedTicks;
            averageTicks = (float)time / numRepeat;
            timeBubbleSort[i] = averageTicks * 100;

            // Add results for the current step to the list
            results.Add(new Result
            {
                NumDrones = numdrones,
                AverageTime = timeAverage[i],
                MinTime = timeMin[i],
                MaxTime = timeMax[i],
                PrintTime = timePrint[i],
                BubbleSortTime = timeBubbleSort[i]
            });
        }

        // Write results to CSV with file creation
        try
        {
            string filePath = "results.csv";
            using (var writer = new StreamWriter(filePath, false)) // false to overwrite the file if it exists
            {
                writer.WriteLine("NumDrones,AverageTime,MinTime,MaxTime,PrintTime,BubbleSortTime");
                foreach (var result in results)
                {
                    writer.WriteLine($"{result.NumDrones},{result.AverageTime},{result.MinTime},{result.MaxTime},{result.PrintTime},{result.BubbleSortTime}");
                }
            }
            Console.WriteLine($"Results successfully written to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing the file: {ex.Message}");
        }
    }

    // Define a class to hold results
    public class Result
    {
        public int NumDrones { get; set; }
        public float AverageTime { get; set; }
        public float MinTime { get; set; }
        public float MaxTime { get; set; }
        public float PrintTime { get; set; }
        public float BubbleSortTime { get; set; }
    }
}
