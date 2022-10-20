using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    DataGameManager data;
    int[] playerKeys;
    Transform player;
    int numberOfTotalKeys;
    float playerHealth;
    float playerShield;
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
    public void SetHealth(float health)
    {
        playerHealth = health;
    }
    public float GetHealth()
    {
        return playerHealth;
    }
    public void SetShield(float shield)
    {
        playerShield = shield;
    }
    public float GetShield()
    {
        return playerShield;
    }
    public float GetMaxHealth()
    {
        return data.playerMaxHealth;
    }
    public float GetMaxShield()
    {
        return data.playerMaxShield;
    }
    

    void InitializeData()
    {
        data = Resources.Load<DataGameManager>("DataGameManager");
        numberOfTotalKeys = data.numberOfKeys;
        playerHealth = data.playerMaxHealth;
        playerShield = 0;
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.C))
        {
            FindObjectOfType<HealthSystem>().SaveStats();
            SceneManager.LoadScene("LVL2");
        }
        
    }
    public void Respawn()
    {
        playerHealth = data.playerMaxHealth;
        playerShield = 0;
        StartCoroutine(Die());
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(2f);
        LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

public enum Keys
{
    Key1,
    Key2,
    Key3,
    Key4,
    NumberOfTypes
}
