using TMPro;
using UnityEngine;

public class ScrapHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text scrapText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color deniedColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private float flashDuration = 0.4f;

    private float flashTimer;

    private void Start()
    {
        RebuildManager.Instance.OnRebuildDenied += HandleDenied;
    }

    private void OnDestroy()
    {
        if (RebuildManager.Instance != null)
            RebuildManager.Instance.OnRebuildDenied -= HandleDenied;
    }

    private void HandleDenied(Ruin ruin) => flashTimer = flashDuration;

    private void Update()
    {
        RebuildManager manager = RebuildManager.Instance;
        if (manager == null) return;

        string text = $"SCHROTT / SCRAP  {manager.Scrap}";

        bool preparing = GameManager.Instance != null && GameManager.Instance.Phase == GamePhase.Preparation;
        if (preparing && Ruin.All.Count > 0)
            text += "\nRuine anklicken zum Wiederaufbau / Click a ruin to rebuild";

        scrapText.text = text;

        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            scrapText.color = Color.Lerp(normalColor, deniedColor, Mathf.Clamp01(flashTimer / flashDuration));
        }
        else
        {
            scrapText.color = normalColor;
        }
    }
}