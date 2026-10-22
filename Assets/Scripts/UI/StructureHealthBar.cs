using UnityEngine;

public class StructureHealthBar : MonoBehaviour
{
    [SerializeField] private StructureHealth health;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer fill;
    [SerializeField] private Color fullColor = new Color(0.3f, 1f, 0.5f);
    [SerializeField] private Color lowColor = new Color(1f, 0.25f, 0.25f);
    [SerializeField] private bool hideWhenFull = true;

    private void Start()
    {
        if (health == null) health = GetComponentInParent<StructureHealth>();
        if (health == null) return;

        health.OnHealthChanged += Refresh;
        Refresh(health.CurrentHealth, health.MaxHealth);
    }

    private void OnDestroy()
    {
        if (health != null) health.OnHealthChanged -= Refresh;
    }

    private void Refresh(float current, float max)
    {
        float t = max > 0f ? Mathf.Clamp01(current / max) : 0f;

        // Füllung schrumpft von rechts nach links / Fill shrinks from right to left
        Transform fillTransform = fill.transform;
        fillTransform.localScale = new Vector3(t, fillTransform.localScale.y, 1f);
        fillTransform.localPosition = new Vector3((t - 1f) * 0.5f, fillTransform.localPosition.y, 0f);
        fill.color = Color.Lerp(lowColor, fullColor, t);

        bool visible = !(hideWhenFull && t >= 1f);
        background.enabled = visible;
        fill.enabled = visible;
    }
}