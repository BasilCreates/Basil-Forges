using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //the card parameterrsssss
    [Header("Card Info")]
    public bool targetsChosenCharacter;
    public string cardName;
    public string targetStat;
    public int modifyAmount;
    public ModifyOperator modifyOperator;
    public int suspicionAmount;
}