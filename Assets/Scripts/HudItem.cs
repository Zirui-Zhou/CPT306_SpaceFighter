using UnityEngine;
using TMPro;

public class HudItem : MonoBehaviour {
    public HudManager.HudType type;

    private TextMeshProUGUI meshText;
    
    private void Start() {
        var hudData = HudManager.Instance.hudItemDict[type];
        meshText = GetComponent<TextMeshProUGUI>();
        hudData.value.ValueChanged += newValue => meshText.text = hudData.prefix + newValue;
        meshText.text = hudData.prefix + hudData.value.Value;
    }
}
