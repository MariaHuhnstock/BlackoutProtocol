using TMPro;
using UnityEngine;

public class WaveHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;

    private void Update()
    {
        GameManager game = GameManager.Instance;
        WaveManager waves = WaveManager.Instance;
        if (game == null || waves == null) return;

        switch (game.Phase)
        {
            case GamePhase.Preparation:
                int seconds = Mathf.CeilToInt(waves.PrepTimeLeft);
                waveText.text =
                    $"VORBEREITUNG / PREPARATION  {seconds}s\n" +
                    $"Nächste Welle / Next wave {waves.CurrentWave + 1} / {waves.TotalWaves}\n" +
                    "[Leertaste / Space] Start";
                break;

            case GamePhase.Wave:
                waveText.text =
                    $"WELLE / WAVE  {waves.CurrentWave} / {waves.TotalWaves}\n" +
                    $"Gegner / Enemies  {waves.EnemiesAlive + waves.EnemiesToSpawn}";
                break;

            case GamePhase.Won:
                waveText.text =
                    "SIEG / VICTORY\n" +
                    $"Zeit {FormatTime(game.ElapsedTime)}  |  Besiegt {game.EnemiesKilled}";
                break;

            case GamePhase.Lost:
                waveText.text =
                    "GENERATOR ZERSTÖRT\nGENERATOR DESTROYED\n" +
                    $"Zeit {FormatTime(game.ElapsedTime)}  |  Besiegt {game.EnemiesKilled}";
                break;
        }
    }

    private static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return $"{total / 60}:{total % 60:00}";
    }
}