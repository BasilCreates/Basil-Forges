using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ModifyOperator //enums are a list of choices w a dropdown menu
{ 
    Add,
    Subtract
}


[System.Serializable] //To have it read by the system
public class StatsModify
{
    public string statTarget; //the target stat we want to change (name it accordingly to make it easier)
    public int modifyAmount; //the modify amount that we want to change it by
    public ModifyOperator modifyOperator;
}


public class StatsModifier : MonoBehaviour
{
    public List <StatsModify> ModifyList; //Creates the modify class list that allows you to customize the stat modifiers

    public void ApplyModifiers()
    {
        // Grab CurrentStats from the same GameObject
        CurrentStats currentStats = GetComponent<CurrentStats>();

        // Loop through each modifier in the list
        foreach (StatsModify modifier in ModifyList)
        {
            // Find the matching stat in CurrentStats by name
            foreach (DefaultStats stat in currentStats.defaultStats)
            {
                if (stat.statName == modifier.statTarget)
                {
                    // Apply the operator directly to CurrentStats
                    if (modifier.modifyOperator == ModifyOperator.Add)
                    {
                        stat.statAmount += modifier.modifyAmount;
                    }
                    else if (modifier.modifyOperator == ModifyOperator.Subtract)
                    {
                        stat.statAmount -= modifier.modifyAmount;
                    }
                }
            }
        }
    }
}
