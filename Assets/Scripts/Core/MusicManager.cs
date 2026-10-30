using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Musik / Music")]
    [SerializeField] private AudioClip preparationLoop;
    [SerializeField] private AudioClip waveLoop;
    [SerializeField] private AudioClip victoryStinger;
    [SerializeField] private AudioClip defeatStinger;

    [Header("Mix")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.6f;
    [SerializeField, Range(0f, 1f)] private float stingerVolume = 0.8f;
    [SerializeField] private float crossfadeTime = 2f;
    [SerializeField, Range(0f, 1f)] private float pausedVolumeFactor = 0.35f;

    private readonly AudioSource[] loops = new AudioSource[2];
    private AudioSource stingerSource;
    private int active;
    private float volumeFactor = 1f;
    private Coroutine fadeRoutine;

    private float TargetVolume => musicVolume * volumeFactor;

    private void Awake()
    {
        for (int i = 0; i < 2; i++)
        {
            loops[i] = gameObject.AddComponent<AudioSource>();
            loops[i].loop = true;
            loops[i].playOnAwake = false;
            loops[i].volume = 0f;
        }

        stingerSource = gameObject.AddComponent<AudioSource>();
        stingerSource.playOnAwake = false;
    }

    private void Start()
    {
        GameManager.Instance.OnPhaseChanged += HandlePhaseChanged;
        GameManager.Instance.OnPauseChanged += HandlePauseChanged;
        HandlePhaseChanged(GameManager.Instance.Phase);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnPhaseChanged -= HandlePhaseChanged;
        GameManager.Instance.OnPauseChanged -= HandlePauseChanged;
    }

    private void HandlePhaseChanged(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.Preparation: CrossfadeTo(preparationLoop); break;
            case GamePhase.Wave: CrossfadeTo(waveLoop); break;
            case GamePhase.Won: PlayStinger(victoryStinger); break;
            case GamePhase.Lost: PlayStinger(defeatStinger); break;
        }
    }

    // Musik in der Pause leiser / Duck music while paused
    private void HandlePauseChanged(bool paused)
    {
        volumeFactor = paused ? pausedVolumeFactor : 1f;
        if (fadeRoutine == null && loops[active].isPlaying)
            loops[active].volume = TargetVolume;
    }

    private void CrossfadeTo(AudioClip clip)
    {
        if (clip == null) return;
        if (loops[active].clip == clip && loops[active].isPlaying) return;

        AudioSource from = loops[active];
        active = 1 - active;
        AudioSource to = loops[active];

        to.clip = clip;
        to.volume = 0f;
        to.Play();

        StartFade(Fade(to, from, crossfadeTime));
    }

    private void PlayStinger(AudioClip clip)
    {
        StartFade(Fade(null, loops[active], 0.4f));
        if (clip != null) stingerSource.PlayOneShot(clip, stingerVolume);
    }

    private void StartFade(IEnumerator routine)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(routine);
    }

    // Unscaled, damit es auch bei timeScale 0 läuft / Unscaled so it works while timeScale is 0
    private IEnumerator Fade(AudioSource fadeIn, AudioSource fadeOut, float duration)
    {
        float outStart = fadeOut != null ? fadeOut.volume : 0f;
        float inStart = fadeIn != null ? fadeIn.volume : 0f;

        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float k = t / duration;
            if (fadeIn != null) fadeIn.volume = Mathf.Lerp(inStart, TargetVolume, k);
            if (fadeOut != null) fadeOut.volume = Mathf.Lerp(outStart, 0f, k);
            yield return null;
        }

        if (fadeIn != null) fadeIn.volume = TargetVolume;
        if (fadeOut != null)
        {
            fadeOut.volume = 0f;
            fadeOut.Stop();
        }

        fadeRoutine = null;
    }
}