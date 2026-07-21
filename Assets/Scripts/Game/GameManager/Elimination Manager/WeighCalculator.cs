using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightCalculator : MonoBehaviour
{
    // Holds the results after calculation
    [System.Serializable]
    public class ContestantWeight
    {
        public string contestantTag;  // Which contestant this belongs to
        public int totalWeight;       // Their vote score
    }

    public List<ContestantWeight> results; // Populated after Calculate() is called


    // Steps:
    // Finds all contestant GameObjects by their tags,
    // calculates their vote score:
    //
    // VoteScore =
    // Random.Range(80,90)
    // + Audience Appeal
    // + Talent
    // - Toxicity
    //
    // Then sorts from highest to lowest.
    // Lowest gets eliminated.

    public void Calculate(List<string> contestantTags)
    {
        // Clear previous results
        results = new List<ContestantWeight>();

        // Loop through each contestant tag
        foreach (string tag in contestantTags)
        {
            // Find the contestant GameObject by tag
            GameObject contestant = GameObject.FindWithTag(tag);

            if (contestant != null)
            {
                // Grab their CurrentStats
                CurrentStats currentStats = contestant.GetComponent<CurrentStats>();

                int audienceAppeal = 0;
                int talent = 0;
                int toxicity = 0;

                // Grab specific stats
                foreach (DefaultStats stat in currentStats.defaultStats)
                {
                    if (stat.statName == "Audience Appeal")
                    {
                        audienceAppeal = stat.statAmount;
                    }

                    if (stat.statName == "Talent")
                    {
                        talent = stat.statAmount;
                    }

                    if (stat.statName == "Toxicity")
                    {
                        toxicity = stat.statAmount;
                    }
                }

                // Calculate vote score
                int total = Random.Range(80, 90)
                            + audienceAppeal
                            + talent
                            - toxicity;


                // Store the result
                results.Add(new ContestantWeight
                {
                    contestantTag = tag,
                    totalWeight = total
                });
            }
        }

        // Sort from highest to lowest
        results.Sort((a, b) => b.totalWeight.CompareTo(a.totalWeight));
    }


    // GET ELIMINATED
    // Returns the tag of whoever has the lowest vote score

    public string GetEliminated()
    {
        if (results.Count > 0)
        {
            // Last in the sorted list is always the lowest
            return results[results.Count - 1].contestantTag;
        }

        return "";
    }
}