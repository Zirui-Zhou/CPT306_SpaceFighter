using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : Singleton<StartMenu> {
    private GameObject startMenuGo;
    private TextMeshProUGUI message;
    private Button startButton;
    private Button helpButton;
    private Button aboutButton;
    private Button quitButton;

    private const string DefaultMessage =
        "Space Fighter";
    
    private const string HelpMessage = 
        "WASD to move the space fighter\n" +
        "Space to pause the game";
    
    private const string AboutMessage = 
        "Programmer: Zirui Zhou\n" + 
        "Contact: zirui.zhou19@student.xjtlu.edu.cn";

    private void Start() {
        startMenuGo = gameObject;
        message = startMenuGo.transform.Find("Message").GetComponent<TextMeshProUGUI>();
        startButton = startMenuGo.transform.Find("StartButton").GetComponent<Button>();
        helpButton = startMenuGo.transform.Find("HelpButton").GetComponent<Button>();
        aboutButton = startMenuGo.transform.Find("AboutButton").GetComponent<Button>();
        quitButton = startMenuGo.transform.Find("QuitButton").GetComponent<Button>();
        
        startButton.onClick.AddListener(()=> {
            message.text = DefaultMessage;
            GameManager.Instance.StartGame();
        });
        helpButton.onClick.AddListener(()=>message.text=HelpMessage);
        aboutButton.onClick.AddListener(()=>message.text=AboutMessage);
        quitButton.onClick.AddListener(Application.Quit);

        message.text = DefaultMessage;
    }

    public void ShowMenu() {
        startMenuGo.SetActive(true);
    }

    public void HideMenu() {
        startMenuGo.SetActive(false);
    }
}
