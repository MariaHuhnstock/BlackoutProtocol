using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ruin : MonoBehaviour
{
    private static readonly List<Ruin> all = new List<Ruin>();
    public static IReadOnlyList<Ruin> All => all;

    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private TMP_Text costLabel;
    [SerializeField] private Color ruinColor = new Color(0.45f, 0.45f, 0.5f, 0.8f);
    [SerializeField] private Color affordableColor = new Color(0.49f, 1f, 0.69f);
    [SerializeField] private Color tooExpensiveColor = new Color(1f, 0.4f, 0.4f);

    public StructureHealth Structure { get; private set; }
    public Vector2Int GridPosition => Structure.GridPosition;

    private void OnEnable() => all.Add(this);
    private void OnDisable() => all.Remove(this);

    public void Setup(StructureHealth structure)
    {
        Structure = structure;

        // Sprite und Größe vom zerstörten Gebäude übernehmen / Copy sprite and size from the destroyed structure
        SpriteRenderer source = structure.GetComponent<SpriteRenderer>();
        if (source != null)
        {
            icon.sprite = source.sprite;
            icon.transform.localScale = structure.transform.localScale;
        }
        icon.color = ruinColor;
    }

    public static Ruin FindAt(Vector2Int gridPosition)
    {
        foreach (Ruin ruin in all)
        {
            if (ruin.Structure != null && ruin.GridPosition == gridPosition)
                return ruin;
        }
        return null;
    }

    private void Update()
    {
        if (Structure == null) return;

        // Preis nur in der Vorbereitung anzeigen / Show price only during preparation
        bool preparing = GameManager.Instance != null && GameManager.Instance.Phase == GamePhase.Preparation;
        costLabel.gameObject.SetActive(preparing);
        if (!preparing) return;

        int cost = Structure.RebuildCost;
        costLabel.text = $"{Structure.DisplayName}\n{cost}";

        bool affordable = RebuildManager.Instance != null && RebuildManager.Instance.Scrap >= cost;
        costLabel.color = affordable ? affordableColor : tooExpensiveColor;
    }
}