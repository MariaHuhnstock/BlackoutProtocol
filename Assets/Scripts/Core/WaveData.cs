using System;
using UnityEngine;

[Serializable]
public class SpawnGroup
{
    public EnemyController prefab;
    public int count = 5;
    [Tooltip("Sekunden nach Wellenbeginn / Seconds after wave start")]
    public float startDelay = 0f;
    [Tooltip("Sekunden zwischen zwei Gegnern / Seconds between two enemies")]
    public float interval = 1f;
}

[CreateAssetMenu(fileName = "NewWave", menuName = "Blackout Protocol/Wave Data")]
public class WaveData : ScriptableObject
{
    public string displayName = "Welle / Wave";
    public SpawnGroup[] groups;
}