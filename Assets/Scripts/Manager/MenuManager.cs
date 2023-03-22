using System;

public class MenuManager : Singleton<MenuManager> {
    public static void UpdateMenuState(GameState gameState, GameResult? gameResult) {
        StartMenu.Instance.HideMenu();
        PauseMenu.Instance.HideMenu();
        ResultMenu.Instance.HideMenu();
        
        switch (gameState) {
            case GameState.StartMenu:
                StartMenu.Instance.ShowMenu();
                break;
            case GameState.Running:
                break;
            case GameState.Pause:
                PauseMenu.Instance.ShowMenu();
                break;
            case GameState.Result:
                if (!gameResult.HasValue) throw new ArgumentException("Game Result is not assigned.");
                ResultMenu.Instance.ShowMenu(gameResult.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }
}
