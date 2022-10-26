using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vignette : MonoBehaviour, IReset
{
    public static Vignette instance;
    [SerializeField] Image vignette;
    [SerializeField] Color hitColor;
    [SerializeField] Color healColor;
    [SerializeField] Color shieldColor;
    [SerializeField] Color ammoColor;
    [SerializeField] float speed;
    [SerializeField] float stayTime;
    [Range(0, 1)]
    [SerializeField] float maxAlpha;
    private void Awake() {
        instance = this;
    }
    private void Start() {
        vignette.gameObject.SetActive(false);
    }
    public void HitVignette()
    {
        StopAllCoroutines();
        StartCoroutine(PopUp(hitColor));
    }
    public void HealVignette()
    {
        StopAllCoroutines();
        StartCoroutine(PopUp(healColor));
    }
    public void ShieldVignette()
    {
        StopAllCoroutines();
        StartCoroutine(PopUp(shieldColor));
    }
    public void AmmoVignette()
    {
        StopAllCoroutines();
        StartCoroutine(PopUp(ammoColor));
    }
    IEnumerator PopUp(Color v_Color)
    {
        vignette.gameObject.SetActive(true);
        vignette.color = new Color(v_Color.r,v_Color.g,v_Color.b,0);
        while(vignette.color.a < maxAlpha)
        {
            vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, vignette.color.a + speed * Time.deltaTime);
            yield return null;
        }

        vignette.color = new Color(v_Color.r,v_Color.g,v_Color.b,maxAlpha);
        yield return new WaitForSeconds(stayTime);

        while(vignette.color.a > 0)
        {
            vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, vignette.color.a - speed * Time.deltaTime);
            yield return null;
        }

        vignette.gameObject.SetActive(false);
    }
    public void Reset() {
        if(vignette.gameObject != null)vignette.gameObject.SetActive(false);
        StopAllCoroutines();
    }
}
