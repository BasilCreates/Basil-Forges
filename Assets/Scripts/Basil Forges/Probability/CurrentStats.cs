using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



//A list for our JSON txt to be read
[System.Serializable]
public class DefaultStatsWrapper
{
    public List<DefaultStats> defaultStats;
}

//We create a new class for the stats before we create a populatable list (is that even a word?)
[System.Serializable]
public class DefaultStats 
{
    public string statName; //Keep the names straight to the point
    public int statAmount;

}


public class CurrentStats : MonoBehaviour
{
    public List<DefaultStats> defaultStats; //We create a new list to populate our new stats as much as we need


    public void Awake()
    {
        ReadFile(); //This is just to check when I play start if it pulls from my JSON file
    }
    public void ReadFile()
    {
        //I haven't done this in a while, got rusty. But getting our text file to drag out our default stats
        string path = Path.Combine(Application.streamingAssetsPath, "DefaultStats.txt");
        //Debug.Log(path); //Needed to make sure the file was actually being called

        //Next is reading the text from the file
        string rawText = File.ReadAllText(path);

        //Next is converting this text into our class to be read
        DefaultStatsWrapper wrapper = JsonUtility.FromJson<DefaultStatsWrapper>(rawText);

        //Next is to pull all that data out and assign it to our list properly
        defaultStats = wrapper.defaultStats;
        foreach (DefaultStats stat in defaultStats)
        {
            // Roll to decide which range to use
            float chance = Random.value;

            if (chance < 0.8f)
            {
                // 80% chance - middle range 40 to 70
                stat.statAmount = Random.Range(8, 15) * 5;
            }
            else
            {
                // 20% chance - extreme range, roll again to decide which end
                float extremeChance = Random.value;

                if (extremeChance < 0.5f)
                {
                    // Low extreme 0 to 35
                    stat.statAmount = Random.Range(0, 8) * 5;
                }
                else
                {
                    // High extreme 75 to 100
                    stat.statAmount = Random.Range(15, 21) * 5;
                }
            }
        }

    }
}
