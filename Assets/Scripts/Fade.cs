using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public static Fade instance;
    Animator anim;
    string sceneToLoad;
    private void Awake() {
        instance = this;
    }
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
    public void LoadNextScene()
    {
        GameManager.GetGameManager().LoadScene(sceneToLoad);
    }
    void FadeOut()
    {
        anim.Play("fadeOut");
    }
    public void FadeOutTranstion(string nextScene)
    {
        sceneToLoad = nextScene;
        anim.Play("fadeTrans");
    }
}
