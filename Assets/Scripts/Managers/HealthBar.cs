using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float WaitBeforeStartRecovery;
    public float RecoverHealthPerSecond;

    private RectTransform healthBarTransform;
    private Image healthBarImage;
    private Color defaultColor;                 // Opaque Default Color

    private bool canRecover;

    private void Start()
    {
        healthBarImage = GetComponentInChildren<Image>();
        healthBarTransform = healthBarImage.GetComponent<RectTransform>();
        
        defaultColor = healthBarImage.color;
        defaultColor.a = 0;
        healthBarImage.color = defaultColor;
        defaultColor.a = 1;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarImage.color = defaultColor;
        healthBarTransform.localScale = new Vector3(currentHealth / maxHealth, 1, 1);

        StopAllCoroutines();
        //canRecover = false;
        //StartCoroutine(StartRecovery(WaitBeforeStartRecovery));

        if (currentHealth == maxHealth) StartCoroutine(SlowlyHide(0.75f));
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //SlowlyIncreaseHealth();
    }

    private IEnumerator StartRecovery(float Delay)
    {
        yield return new WaitForSeconds(Delay);

        canRecover = true;
    }
    private void SlowlyIncreaseHealth()
    {
        if (canRecover)
        {
            float newHealth = Mathf.Clamp01(healthBarTransform.localScale.x + (RecoverHealthPerSecond * Time.deltaTime));

            if (newHealth == 1)
            {
                canRecover = false;
                StartCoroutine(SlowlyHide(0.75f));
            }
            healthBarTransform.localScale = new Vector3(newHealth, 1, 1);
        }
    }

    private IEnumerator SlowlyHide(float disappearTime)
    {
        float countdown = disappearTime;
        Color c = defaultColor;
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;

            c.a = countdown / disappearTime;
            healthBarImage.color = c;
            
            yield return null;
        }
    }
}
