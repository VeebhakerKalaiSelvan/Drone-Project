using System;
public class Flock
{
    Drone[] agents;
    int num;
    
    public Flock(int maxnum)
    {
        agents = new Drone[maxnum];
    }
    
    // actually add the drones
    public void Init(int num)
    {
        this.num = num;
        for (int i=0; i<num; i++)
        {
            agents[i] = new Drone(i);
        }
    }
    
    public void Update()
    {
        for (int i=0; i<num; i++)
        {
            agents[i].Update();
        }
    }

    // Function to calculate average (battery, temperature, wind)
    public float average(string option) 
    {
        float sum = 0;
        for (int i = 0; i < num; i++)
        {
            if (option == "battery")
                sum += agents[i].Battery;
            else if (option == "temperature")
                sum += agents[i].Temperature;
            else if (option == "wind")
                sum += agents[i].Wind;
        }

        float aver = sum / num;
        return aver;
    }

    // Function to find max (battery, temperature, wind)
    public float max(string option)
    {
        float maxValue = option == "battery" ? agents[0].Battery : option == "temperature" ? agents[0].Temperature : agents[0].Wind;
        
        for (int i = 1; i < num; i++)
        {
            float value = option == "battery" ? agents[i].Battery : option == "temperature" ? agents[i].Temperature : agents[i].Wind;

            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        return maxValue;
    }

    // Function to find min (battery, temperature, wind)
    public float min(string option)
    {
        float minValue = option == "battery" ? agents[0].Battery : option == "temperature" ? agents[0].Temperature : agents[0].Wind;
        
        for (int i = 1; i < num; i++)
        {
            float value = option == "battery" ? agents[i].Battery : option == "temperature" ? agents[i].Temperature : agents[i].Wind;

            if (value < minValue)
            {
                minValue = value;
            }
        }
        return minValue;
    }

    // Function to print drone details by ID
    public void print(int droneID)
    {
        Drone foundDrone = null;
        for (int i = 0; i < num; i++)
        {
            if (agents[i].ID == droneID)
            {
                foundDrone = agents[i];
                break;
            }
        }

        if (foundDrone != null)
        {
            Console.WriteLine($"Drone ID: {foundDrone.ID}, Battery: {foundDrone.Battery}, Temperature: {foundDrone.Temperature}, Wind: {foundDrone.Wind}");
        }
        else
        {
            Console.WriteLine($"Drone with ID {droneID} not found.");
        }
    }

    // Bubble sort based on battery, temperature, or wind
    public void bubblesort(string option)
    {
        for (int i = 0; i < num - 1; i++)
        {
            for (int j = 0; j < num - i - 1; j++)
            {
                float current = option == "battery" ? agents[j].Battery : option == "temperature" ? agents[j].Temperature : agents[j].Wind;
                float next = option == "battery" ? agents[j + 1].Battery : option == "temperature" ? agents[j + 1].Temperature : agents[j + 1].Wind;

                if (current > next)
                {
                    // Swap drones
                    Drone temp = agents[j];
                    agents[j] = agents[j + 1];
                    agents[j + 1] = temp;
                }
            }
        }
    }
}