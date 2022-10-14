using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataGameController", menuName = "Data/GameController", order = 1)]
public class DataGameController : ScriptableObject
{
    public int playerMaxHealth;
    public int playerMaxShield;
    public int numberOfKeys;
}
