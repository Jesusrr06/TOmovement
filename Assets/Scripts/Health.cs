using UnityEngine;
using System;

/// <summary>
/// Tracks current and maximum health, exposes TakeDamage and death events.
/// Notifies listeners when health changes and fires OnDeath when HP reaches zero.
/// </summary>
public class Health : MonoBehaviour
{
    public event Action<PlayerMovement> OnDeath; // "PC" es el jugador que murió
    public PlayerMovement owner; // El jugador dueño de esta Health

    public float maxHealth = 100f;
    public float currentHealth;

    public Action<float> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log($"{gameObject.name} Awake: health set to {currentHealth}/{maxHealth}");
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(1f);
        Debug.Log($"{gameObject.name} Start: OnHealthChanged invoked with {currentHealth / maxHealth}");
    }

    /// <summary>
    /// Reduces current health by damage, clamps to [0, maxHealth], and raises OnHealthChanged.
    /// If health reaches zero, Die() is invoked and OnDeath is fired.
    /// </summary>
    /// <param name="damage">Amount of health to subtract.</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        // Aquí podemos detectar si la vida llega a cero
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Called when health reaches zero. Fires OnDeath and stops the game time.
    /// </summary>
    private void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        Debug.Log("¡Fin del combate!");

        OnDeath?.Invoke(owner); // <-- Aquí dispara el evento a quien lo esté escuchando

        // Detiene toda la simulación de Unity
        Time.timeScale = 0f;
    }
}