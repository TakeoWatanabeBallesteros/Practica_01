using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    int[] playerKeys;
    Transform player;
    int numberOfTotalKeys;
    int playerHealth;
    int playerShield;
    private void Start() {
        DontDestroyOnLoad(this.gameObject);
    }
    public static GameManager GetGameManager()
    {
        if(instance == null)
        {
            instance = new GameObject("GameManager").AddComponent<GameManager>();
            instance.InitializeData();
            instance.InitializeKeys();
        }

        return instance;
    }
    public static void DestroySingletone()
    {
        if (instance != null)
        {
            GameObject.Destroy(instance);
        }
        else
        {
            instance = null;
        }
    }

    void InitializeKeys()
    {
        playerKeys = new int[numberOfTotalKeys];
        for (int i = 0; i < numberOfTotalKeys; i++)
        {
            playerKeys[i] = 0;
        }
    }

    public void SetKey(int keyNumber)
    {
        playerKeys[keyNumber] = 1;
    }

    public int GetKey(int keyNumber)
    {
        return playerKeys[keyNumber];
    }

    public Transform GetPlayer()
    {
        return player;
    }
    public void SetPlayer(Transform _player)
    {
        player = _player;
    }

    void InitializeData()
    {
        DataGameController data = Resources.Load<DataGameController>("DataGameController");
        instance.numberOfTotalKeys = data.numberOfKeys;
    }
    
}
