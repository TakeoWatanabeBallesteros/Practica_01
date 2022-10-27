using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;   

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    DataGameManager data;
    bool[] playerKeys;
    Transform player;
    float playerHealth;
    float playerShield;
    Vector3 currentCheckpointPos;
    Quaternion currentCheckpointRot;
    int checkpointPreference;
    List<IReset> resetablesList;
    public delegate void Died();
    public static event Died OnDied;
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
        playerKeys = new bool[(int)Keys.NumberOfKeys];
        for (int i = 0; i < (int)Keys.NumberOfKeys; i++)
        {
            playerKeys[i] = false;
        }
    }

    public void SetKey(Keys key)
    {
        playerKeys[(int)key] = true;
    }

    public bool GetKey(Keys key)
    {
        return playerKeys[(int)key];
    }

    public Transform GetPlayer()
    {
        return player;
    }
    public void SetPlayer(Transform _player)
    {
        player = _player;
        currentCheckpointPos = new Vector3(player.position.x,player.position.y,player.position.z);
        currentCheckpointRot = new Quaternion(player.rotation.x,player.rotation.y,player.rotation.z,player.rotation.w);
        checkpointPreference = 0;
        resetablesList = new List<IReset>();
        resetablesList = InitializeResetables();
        AddWeapons();
    }
    void AddWeapons()
    {
        List<IReset> temp = FindObjectOfType<WeaponSwitching>().GetWeaponsReset();
        foreach (var item in temp)
        {
            resetablesList.Add(item);
        }
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
        playerHealth = data.playerMaxHealth;
        playerShield = 0;
    }
    public void Die()
    {
        OnDied?.Invoke();
        player.GetComponent<PlayerController>().enabled = false;
        FindObjectOfType<WeaponSwitching>().enabled = false;
        FindObjectOfType<WeaponBehavior>().enabled = false;
    }
    public void ResetGame()
    {
        foreach (IReset obj in resetablesList) {
            obj.Reset();
        }
        StartCoroutine(TeleportToCheckpoint());
        playerHealth = data.playerMaxHealth;
        playerShield = 0;
        player.position = GetCheckpointPos();
        player.rotation = GetCheckpointRot();
    }
    IEnumerator TeleportToCheckpoint()
    {
        yield return new WaitForFixedUpdate();
        player.position = GetCheckpointPos();
        player.rotation = GetCheckpointRot();
        player.GetComponent<PlayerController>().enabled = true;
        FindObjectOfType<WeaponSwitching>().enabled = true;
        FindObjectOfType<WeaponBehavior>().enabled = true;
    }
    public void BlockPlayer()
    {
        player.GetComponent<PlayerController>().enabled = false;
        FindObjectOfType<WeaponSwitching>().enabled = false;
        FindObjectOfType<WeaponBehavior>().enabled = false;
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void SetCheckpoint(Transform checkpoint, int preference)
    {
        currentCheckpointPos = new Vector3(checkpoint.position.x,checkpoint.position.y,checkpoint.position.z);
        currentCheckpointRot = new Quaternion(checkpoint.rotation.x,checkpoint.rotation.y,checkpoint.rotation.z,checkpoint.rotation.w);
        checkpointPreference = preference;
    }
    public int GetCheckpointPref()
    {
        return checkpointPreference;
    }
    public Vector3 GetCheckpointPos()
    {
        return currentCheckpointPos;
    }
    public Quaternion GetCheckpointRot()
    {
        return currentCheckpointRot;
    }
    public List<IReset> InitializeResetables()
    {
        List<IReset> temporal =  new List<IReset>();
        var resetables = FindObjectsOfType<MonoBehaviour>().OfType<IReset>();
        foreach (IReset obj in resetables) {
            temporal.Add(obj);
        }
        return temporal;
    }
    public void AddResetables(List<IReset> listToAdd)
    {
        foreach (var item in listToAdd)
        {
            resetablesList.Add(item);
        }
    }
}

public enum Keys
{
    Key1,
    Key2,
    Key3,
    Key4,
    NumberOfKeys
}
