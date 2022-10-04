using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int[] playerKeys;
    [SerializeField] int numberOfTotalKeys;
    private void Awake() {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeKeys();
    }

    void InitializeKeys()
    {
        playerKeys = new int[numberOfTotalKeys];
        for (int i = 0; i < numberOfTotalKeys; i++)
        {
            playerKeys[i] = PlayerPrefs.GetInt("Key"+i.ToString(),0);
        }
    }

    public void GetKey(int keyNumber)
    {
        PlayerPrefs.SetInt("Key"+keyNumber.ToString(),1);
        playerKeys[keyNumber] = PlayerPrefs.GetInt("Key"+keyNumber.ToString(),0);
    }

    public int[] ReadKeys()
    {
        return playerKeys;
    }
    
}
