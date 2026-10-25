using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        pausePanel.SetActive(false);

        resumeButton.onClick.AddListener(() => GameManager.Instance.SetPaused(false));
        restartButton.onClick.AddListener(() => GameManager.Instance.Restart());
        GameManager.Instance.OnPauseChanged += HandlePauseChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPauseChanged -= HandlePauseChanged;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame)
            GameManager.Instance.TogglePause();
    }

    private void HandlePauseChanged(bool paused) => pausePanel.SetActive(paused);
}