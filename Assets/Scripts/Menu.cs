using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sensiblityAmount;
    [SerializeField] int senseAmount;
    private void Start() {
        sensiblityAmount.text = PlayerPrefs.GetInt("Sense",200).ToString();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("LVL1");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void AddSense()
    {
        PlayerPrefs.SetInt("Sense",PlayerPrefs.GetInt("Sense",200) + senseAmount);
        sensiblityAmount.text = PlayerPrefs.GetInt("Sense",200).ToString();
    }
    public void MinusSense()
    {
        PlayerPrefs.SetInt("Sense",PlayerPrefs.GetInt("Sense",200) - senseAmount);
        sensiblityAmount.text = PlayerPrefs.GetInt("Sense",200).ToString();
    }

}
