using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOver;
    public delegate void FadeOut();
    public static event FadeOut OnFadeOut;
    private void OnEnable() {
        GameManager.OnDied += OpenGameOver;
    }
    private void OnDisable() {
        GameManager.OnDied -= OpenGameOver;
    }
    public void RestartButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        OnFadeOut?.Invoke();
        gameOver.SetActive(false);
        
    }
    void OpenGameOver()
    {
        gameOver.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OpenMenu()
    {
        GameManager.GetGameManager().LoadScene("Menu");
    }
}
