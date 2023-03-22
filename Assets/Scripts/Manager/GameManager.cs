using System;
using System.Collections;
using UnityEngine;

public enum GameState {
    StartMenu,
    Running,
    Pause,
    Result,
}

public enum GameResult {
    Win,
    LoseByNonLined,
    LoseByDebris,
    LoseByOverlay,
}

public class GameManager : Singleton<GameManager> {
    public SpaceFighter spaceFighter;
    public int winScoreThreshold;
    
    public Property<GameState> gameState = new(GameState.StartMenu);
    private GameResult? gameResult;
    
    private readonly Property<TimeSpan> duration = new(TimeSpan.Zero);
    public Property<string> durationString;
    private IEnumerator durationTimer;
    
    private void Start() {
        durationTimer = DurationTimer();
        gameState.ValueChanged += state => MenuManager.UpdateMenuState(state, gameResult);
        duration.ValueChanged += span => durationString.Value = span.ToString("mm':'ss");
        durationString.Value = duration.Value.ToString("mm':'ss");
        ResetGame();
        MenuManager.UpdateMenuState(GameState.StartMenu, gameResult);
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            switch (gameState.Value) {
                case GameState.Running:
                    PauseGame();
                    break;
                case GameState.Pause:
                    ResumeGame();
                    break;
                case GameState.Result:
                    ResetGame();
                    break;
                case GameState.StartMenu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private IEnumerator DurationTimer() {
        while (true) {
            duration.Value += TimeSpan.FromSeconds(1);
            yield return new WaitForSeconds(1);
        }
    }

    private void StartDurationTimer() {
        StartCoroutine(durationTimer);
    }

    private void StopDurationTimer() {
        StopCoroutine(durationTimer);
    }
    
    public void StartGame() {
        StartDurationTimer();
        ParticlesManager.Instance.StartParticleGenerator();
        gameState.Value = GameState.Running;
    }

    private void PauseGame() {
        Time.timeScale = 0f;
        StopDurationTimer();
        gameState.Value = GameState.Pause;
    }
    
    public void ResumeGame() {
        Time.timeScale = 1f;
        StartDurationTimer();
        gameState.Value = GameState.Running;
    }

    public void ResetGame() {
        Time.timeScale = 1f;
        StopDurationTimer();
        duration.Value = TimeSpan.Zero;
        ParticlesManager.Instance.ClearParticleList();
        StorageManager.Instance.ClearParticleStorage();
        spaceFighter.ResetFighter();
        gameState.Value = GameState.StartMenu;
        gameResult = null;
    }
    
    public void GameOver(GameResult result) {
        Time.timeScale = 0f;
        gameResult = result;
        gameState.Value = GameState.Result;
    }
}