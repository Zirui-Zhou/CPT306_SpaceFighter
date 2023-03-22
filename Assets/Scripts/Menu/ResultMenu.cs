using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultMenu : Singleton<ResultMenu> {
    private struct Notification {
        public string title;
        public string message;
        public Color color;
    }

    private readonly Dictionary<GameResult, Notification> notificationDict = new() {
        [GameResult.Win] = new Notification {
            title = "Win!",
            message = "Congratulations! You Won!",
            color = Color.yellow,
        },
        [GameResult.LoseByNonLined] = new Notification {
            title = "Lose!",
            message = "Oops! Ionised particles are not successfully lined in the storage!",
            color = new Color(1.0f, 0.64f, 0.0f),
        },
        [GameResult.LoseByDebris] = new Notification {
            title = "Lose!",
            message = "Oops! Debris are filled in the storage!",
            color = Color.black,
        },
        [GameResult.LoseByOverlay] = new Notification {
            title = "Lose!",
            message = "Oops! Space Fighter has been destroyed!",
            color = Color.red,
        },
    };

    private GameObject resultMenuGo;
    private TextMeshProUGUI titleTextMesh;
    private TextMeshProUGUI messageTextMesh;
    private Button restartButton;
    
    private void Start() {
        resultMenuGo = gameObject;
        titleTextMesh = resultMenuGo.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        messageTextMesh = resultMenuGo.transform.Find("Message").GetComponent<TextMeshProUGUI>();
        restartButton = resultMenuGo.transform.Find("RestartButton").GetComponent<Button>();
        
        restartButton.onClick.AddListener(GameManager.Instance.ResetGame);
    }
    
    public void ShowMenu(GameResult gameState) {
        var notification = notificationDict[gameState];
        titleTextMesh.text = notification.title;
        titleTextMesh.color = notification.color;
        messageTextMesh.text = notification.message;
        messageTextMesh.color = notification.color;
        resultMenuGo.SetActive(true);
    }

    public void HideMenu() {
        resultMenuGo.SetActive(false);
    }
}
