using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataGameManager", menuName = "Data/GameManager", order = 1)]
public class DataGameManager : ScriptableObject
{
    public int playerMaxHealth;
    public int playerMaxShield;
}
