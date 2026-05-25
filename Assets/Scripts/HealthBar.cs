using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Binds a UI fill image to a Health component and updates it when health changes.
/// </summary>
public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public Health target;

    private void Start()
    {
        if (target == null)
        { 
            WaitForEndOfFrame frame = new WaitForEndOfFrame(); 
            Debug.LogWarning($"HealthBar '{gameObject.name}' has no target assigned.");
            return  ;
        }

        if (fillImage == null)
        {
            Debug.LogWarning($"HealthBar '{gameObject.name}' has no fillImage assigned.");
        }

        target.OnHealthChanged += UpdateBar;
        UpdateBar(target.currentHealth / target.maxHealth);
    }

    private void UpdateBar(float value)
    {
        fillImage.fillAmount = value;
    }
    public void SetTarget(Health h)
    {
        // unsubscribe previous
        if (target != null)
            target.OnHealthChanged -= UpdateBar;

        target = h;

        if (target is not null)
        {
            target.OnHealthChanged += UpdateBar;
            UpdateBar(target.currentHealth / target.maxHealth);
        }
    }

    private void OnDestroy()
    {
        if (target != null)
            target.OnHealthChanged -= UpdateBar;
    }
}