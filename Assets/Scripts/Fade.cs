using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    Animator anim;
    private void OnEnable() {
        anim = GetComponent<Animator>();
        GameOver.OnFadeOut += FadeOut;
    }
    private void OnDisable() {
        GameOver.OnFadeOut -= FadeOut;
    }
    public void RestartGame()
    {
        GameManager.GetGameManager().ResetGame();
    }
    void FadeOut()
    {
        anim.Play("fadeOut");
    }
}
