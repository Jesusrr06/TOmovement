using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public Health target;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("HealthBar: target no asignado");
            return;
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
        target = h;

        if (target is not null)
        {
            target.OnHealthChanged += UpdateBar;
            UpdateBar(target.currentHealth / target.maxHealth);
        }
    }
}