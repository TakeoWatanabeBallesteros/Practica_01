using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthSystem : MonoBehaviour
{
    float currentHealth;
    float currentShield;
    float maxHealth;
    float maxShield;
    bool isAlive;
    [SerializeField] TextMeshProUGUI healthTextDisplay;
    [SerializeField] Image healthImageDisplay;
    [SerializeField] Image healthImageBackDisplay;
    [SerializeField] Image shieldImageDisplay;
    [SerializeField] Image shieldImageBackDisplay;
    [SerializeField] GameObject maxShieldDisplay;

    void Start()
    {
        isAlive = true;

        //Getters
        currentHealth = GameManager.GetGameManager().GetHealth();
        currentShield = GameManager.GetGameManager().GetShield();
        maxHealth = GameManager.GetGameManager().GetMaxHealth();
        maxShield = GameManager.GetGameManager().GetMaxShield();
        
        //Update Full Display
        healthTextDisplay.text = ((int)Mathf.Clamp(currentHealth,1f,maxHealth)).ToString();
        healthImageDisplay.fillAmount = currentHealth/maxHealth;
        healthImageBackDisplay.fillAmount = currentHealth/maxHealth;
        shieldImageDisplay.fillAmount = currentShield/maxShield;
        shieldImageBackDisplay.fillAmount = currentShield/maxShield;
        maxShieldDisplay.SetActive(!CanShield());

    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(20);
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            Heal(10);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            Shield(5);
        }
    }
    public void TakeDamage(int damage)
    {
        if(!isAlive) return;
        
        //Calcular Damage recivido en funcion de escudos
        float damageToShield = Mathf.Clamp(currentShield/0.75f,0,damage);
        currentShield -= damageToShield*0.75f;
        float damageCounter = damageToShield * 0.25f;
        damageCounter += damage - damageToShield;
        currentHealth = Mathf.Clamp(currentHealth-damageCounter,0,maxHealth);

        //Update UI
        healthTextDisplay.text = ((int)Mathf.Clamp(currentHealth,1f,maxHealth)).ToString();
        healthImageDisplay.fillAmount = currentHealth/maxHealth;
        healthImageBackDisplay.fillAmount = currentHealth/maxHealth;
        shieldImageDisplay.fillAmount = currentShield/maxShield;
        shieldImageBackDisplay.fillAmount = currentShield/maxShield;
        maxShieldDisplay.SetActive(!CanShield());

        //Comprobación de posible final de partida
        isAlive = currentHealth > 0;
        if(!isAlive) 
        {
            GameManager.GetGameManager().Respawn(); 
            healthTextDisplay.text = 0.ToString();
        }
    }
    public void SaveStats()
    {
        GameManager.GetGameManager().SetHealth(currentHealth);
        GameManager.GetGameManager().SetShield(currentShield);
    }
    public void Heal(float healAmount)
    {
        if(!isAlive) return;
        currentHealth = Mathf.Clamp(currentHealth + healAmount,0,maxHealth);

        healthTextDisplay.text = ((int)Mathf.Clamp(currentHealth,1f,maxHealth)).ToString();
        healthImageDisplay.fillAmount = currentHealth/maxHealth;
        healthImageBackDisplay.fillAmount = currentHealth/maxHealth;
    }
    public void Shield(float shieldAmount)
    {
        if(!isAlive) return;
        currentShield = Mathf.Clamp(currentShield + shieldAmount,0,maxShield);

        shieldImageDisplay.fillAmount = currentShield/maxShield;
        shieldImageBackDisplay.fillAmount = currentShield/maxShield;
        maxShieldDisplay.SetActive(!CanShield());
    }
    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }
    public bool CanShield()
    {
        return currentShield < maxShield;
    }


}
