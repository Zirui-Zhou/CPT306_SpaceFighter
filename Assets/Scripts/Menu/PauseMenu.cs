using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Singleton<PauseMenu> {
    private GameObject pauseMenuGo;
    private Button resumeButton;
    private Button restartButton;
    
    private void Start() {
        pauseMenuGo = gameObject;
        resumeButton = pauseMenuGo.transform.Find("ResumeButton").GetComponent<Button>();
        restartButton = pauseMenuGo.transform.Find("RestartButton").GetComponent<Button>();
        
        resumeButton.onClick.AddListener(GameManager.Instance.ResumeGame);
        restartButton.onClick.AddListener(GameManager.Instance.ResetGame);
    }

    public void ShowMenu() {
        pauseMenuGo.SetActive(true);
    }

    public void HideMenu() {
        pauseMenuGo.SetActive(false);
    }
}
