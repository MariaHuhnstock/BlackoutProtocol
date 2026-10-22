using TMPro;
using UnityEngine;

public class EnergyHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color deniedColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private float flashDuration = 0.4f;

    private float flashTimer;

    private void Start()
    {
        PowerManager.Instance.OnPowerChanged += UpdateText;
        PowerManager.Instance.OnPowerDenied += HandleDenied;
        UpdateText(PowerManager.Instance.Used, PowerManager.Instance.Capacity);
    }

    private void OnDestroy()
    {
        if (PowerManager.Instance == null) return;
        PowerManager.Instance.OnPowerChanged -= UpdateText;
        PowerManager.Instance.OnPowerDenied -= HandleDenied;
    }

    private void Update()
    {
        if (flashTimer <= 0f) return;

        flashTimer -= Time.deltaTime;
        energyText.color = Color.Lerp(normalColor, deniedColor, flashTimer / flashDuration);
    }

    private void UpdateText(int used, int capacity)
    {
        energyText.text = $"ENERGIE / POWER  {used} / {capacity}";
    }

    private void HandleDenied(IPowerConsumer consumer)
    {
        flashTimer = flashDuration;
        Debug.Log($"Nicht genug Energie für {consumer.DisplayName} ({consumer.PowerCost}) / Not enough power");
    }
}