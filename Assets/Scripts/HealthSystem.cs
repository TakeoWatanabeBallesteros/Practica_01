using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthSystem : MonoBehaviour, IReset, IDamageable
{
    float currentHealth;
    float currentShield;
    float maxHealth;
    float maxShield;
    bool isAlive;

    //Events
    public delegate void SetUI(float health,float maxHealth,float shield,float maxShield);
    public delegate void DamageTaked(float actualHealth,float previousHealth,float actualShield,float previousShield);
    public delegate void HealthGained(float actualHealth,float previousHealth);
    public delegate void ShieldGained(float actualShield,float previousShield);
    public static event SetUI OnSetUI;
    public static event DamageTaked OnDamageTaked;
    public static event HealthGained OnHealthGained;
    public static event ShieldGained OnShieldGained;

    void Start()
    {
        isAlive = true;

        //Getters
        currentHealth = GameManager.GetGameManager().GetHealth();
        currentShield = GameManager.GetGameManager().GetShield();
        maxHealth = GameManager.GetGameManager().GetMaxHealth();
        maxShield = GameManager.GetGameManager().GetMaxShield();

        //Update UI
        OnSetUI?.Invoke(currentHealth,maxHealth,currentShield,maxShield);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(30);
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            Heal(20);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            Shield(15);
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
        OnDamageTaked?.Invoke(currentHealth,currentHealth+damageCounter,currentShield,currentShield+damageToShield*0.75f);

        //Comprobación de posible final de partida
        isAlive = currentHealth > 0;
        if(!isAlive)GameManager.GetGameManager().Die();
    }
    public void SaveStats()
    {
        GameManager.GetGameManager().SetHealth(currentHealth);
        GameManager.GetGameManager().SetShield(currentShield);
    }
    public void Heal(float healAmount)
    {
        if(!isAlive) return;
        //Update UI
        OnHealthGained?.Invoke(Mathf.Clamp(currentHealth + healAmount,0,maxHealth),currentHealth);
        currentHealth = Mathf.Clamp(currentHealth + healAmount,0,maxHealth);
    }
    public void Shield(float shieldAmount)
    {
        if(!isAlive) return;
        //Update UI
        OnShieldGained?.Invoke(Mathf.Clamp(currentShield + shieldAmount,0,maxShield),currentShield);
        currentShield = Mathf.Clamp(currentShield + shieldAmount,0,maxShield);
    }
    public bool CanHeal()
    {
        return currentHealth < maxHealth;
    }
    public bool CanShield()
    {
        return currentShield < maxShield;
    }
    public void Reset()
    {
        currentHealth = maxHealth;
        currentShield = 0;
        isAlive = true;
    }
}
