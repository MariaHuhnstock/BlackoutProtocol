using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        resultPanel.SetActive(false);
        restartButton.onClick.AddListener(() => GameManager.Instance.Restart());
        GameManager.Instance.OnPhaseChanged += HandlePhaseChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
    }

    private void HandlePhaseChanged(GamePhase phase)
    {
        if (phase != GamePhase.Won && phase != GamePhase.Lost) return;

        GameManager game = GameManager.Instance;
        bool won = phase == GamePhase.Won;

        titleText.text = won
            ? "SIEG / VICTORY"
            : "GENERATOR ZERSTÖRT\nGENERATOR DESTROYED";

        rankText.text = game.Rank;
        rankText.color = RankColor(game.Rank);

        int total = Mathf.FloorToInt(game.ElapsedTime);
        statsText.text =
            $"Zeit / Time   {total / 60}:{total % 60:00}\n" +
            $"Gegner besiegt / Enemies defeated   {game.EnemiesKilled}\n" +
            $"Gebäude verloren / Structures lost   {game.StructuresLost}\n" +
            $"Ø Energieauslastung / Avg. power usage   {Mathf.RoundToInt(game.AveragePowerUsage * 100f)} %";

        resultPanel.SetActive(true);
    }

    private static Color RankColor(string rank)
    {
        switch (rank)
        {
            case "S": return new Color(1f, 0.85f, 0.3f);
            case "A": return new Color(0.49f, 1f, 0.69f);
            case "B": return new Color(0.79f, 0.63f, 1f);
            case "C": return new Color(1f, 0.55f, 0.3f);
            default: return new Color(0.7f, 0.7f, 0.75f);
        }
    }
}