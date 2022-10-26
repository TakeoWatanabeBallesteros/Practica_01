using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouWin : MonoBehaviour
{
    [SerializeField] GameObject winMenu;
    public void WinMenu()
    {
        GameManager.GetGameManager().BlockPlayer();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        winMenu.SetActive(true);
    }
    public void GoMenu()
    {
        GameManager.GetGameManager().LoadScene("Menu");
    }
}
