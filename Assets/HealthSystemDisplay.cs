using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthSystemDisplay : MonoBehaviour
{
    [SerializeField] Color damageColor;
    [SerializeField] Color healColor;
    [SerializeField] float followSpeed;
    [SerializeField] TextMeshProUGUI healthTextDisplay;
    [SerializeField] Image healthImageDisplay;
    [SerializeField] Image healthImageBackDisplay;
    [SerializeField] Image shieldImageDisplay;
    [SerializeField] Image shieldImageBackDisplay;
    [SerializeField] GameObject maxShieldDisplay;
    float maxHealth;
    float maxShield;
    Coroutine healthCoroutine;
    Coroutine shieldCoroutine;
    private void OnEnable() {
        HealthSystem.OnSetUI += InitializeAll;
        HealthSystem.OnDamageTaked += TakeDamage;
        HealthSystem.OnHealthGained += GainHealth;
        HealthSystem.OnShieldGained += GainShield;
    }
    private void OnDisable() {
        HealthSystem.OnSetUI -= InitializeAll;
        HealthSystem.OnDamageTaked -= TakeDamage;
        HealthSystem.OnHealthGained -= GainHealth;
        HealthSystem.OnShieldGained -= GainShield;
    }

    void InitializeAll(float health,float maxHP,float shield,float maxSH)
    {
        maxHealth = maxHP;
        maxShield = maxSH;

        healthTextDisplay.text = ((int)Mathf.Clamp(health,1f,maxHealth)).ToString();
        healthImageDisplay.fillAmount = health/maxHealth;
        healthImageBackDisplay.fillAmount = health/maxHealth;
        shieldImageDisplay.fillAmount = shield/maxShield;
        shieldImageBackDisplay.fillAmount = shield/maxShield;
        maxShieldDisplay.SetActive(shield==maxShield);
    }
    void TakeDamage(float actualHealth,float previousHealth,float actualShield,float previousShield)
    {
        healthTextDisplay.text = actualHealth > 0 ? ((int)Mathf.Clamp(actualHealth,1f,maxHealth)).ToString() : 0.ToString();
        if(healthCoroutine!=null)StopCoroutine(healthCoroutine);
        if(shieldCoroutine!=null)StopCoroutine(shieldCoroutine);
        healthCoroutine = StartCoroutine(Damage(healthImageDisplay,healthImageBackDisplay,actualHealth,previousHealth,maxHealth));
        shieldCoroutine = StartCoroutine(Damage(shieldImageDisplay,shieldImageBackDisplay,actualShield,previousShield,maxShield));
        maxShieldDisplay.SetActive(actualShield==maxShield);
    }
    void GainHealth(float actualHealth,float previousHealth)
    {
        healthTextDisplay.text = ((int)Mathf.Clamp(actualHealth,1f,maxHealth)).ToString();
        if(healthCoroutine!=null)StopCoroutine(healthCoroutine);
        healthCoroutine = StartCoroutine(Heal(healthImageDisplay,healthImageBackDisplay,actualHealth,previousHealth,maxHealth));
    }
    void GainShield(float actualShield,float previousShield)
    {
        shieldImageDisplay.fillAmount = actualShield/maxShield;
        if(shieldCoroutine!=null)StopCoroutine(shieldCoroutine);
        shieldCoroutine = StartCoroutine(Heal(shieldImageDisplay,shieldImageBackDisplay,actualShield,previousShield,maxShield));
    }

    IEnumerator Damage(Image imageFront,Image imageBack, float current, float previous, float max)
    {
        imageFront.fillAmount = current/max;
        imageBack.fillAmount = previous/max;
        imageBack.color = damageColor;
        while(imageBack.fillAmount > imageFront.fillAmount)
        {
            imageBack.fillAmount -= followSpeed*Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator Heal(Image imageFront,Image imageBack, float current, float previous, float max)
    {
        imageFront.fillAmount = previous/max;
        imageBack.fillAmount = current/max;
        imageBack.color = healColor;
        while(imageFront.fillAmount < imageBack.fillAmount)
        {
            imageFront.fillAmount += followSpeed*Time.deltaTime;
            yield return null;
        }
        imageFront.fillAmount = current/max;
    }
}
