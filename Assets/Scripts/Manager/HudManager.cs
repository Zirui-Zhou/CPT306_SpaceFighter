using System.Collections.Generic;

public class HudManager : Singleton<HudManager> {
    public enum HudType {
        DH,
        IPG,
        S,
        T,
        R,
        G,
        B,
        RGB,
    }
    
    public struct HudData {
        public string prefix;
        public IProperty value;
    }
    
    public Dictionary<HudType, HudData> hudItemDict = new();

    private new void Awake() {
        base.Awake();
        hudItemDict = new Dictionary<HudType, HudData> {
            [HudType.DH] = new() {
                prefix = "DH: ",
                value = StorageManager.Instance.particleGroup[ScoreType.Debris],
            },
            [HudType.IPG] = new() {
                prefix = "IPG: ",
                value = StorageManager.Instance.totalGroup,
            },
            [HudType.S] = new() {
                prefix = "S: ",
                value = StorageManager.Instance.totalScore,
            },
            [HudType.T] = new() {
                prefix = "T: ",
                value = GameManager.Instance.durationString,
            },
            [HudType.R] = new() {
                prefix = "R: ",
                value = StorageManager.Instance.particleGroup[ScoreType.Red],
            },
            [HudType.G] = new() {
                prefix = "G: ",
                value = StorageManager.Instance.particleGroup[ScoreType.Green],
            },
            [HudType.B] = new() {
                prefix = "B: ",
                value = StorageManager.Instance.particleGroup[ScoreType.Blue],
            },
            [HudType.RGB] = new() {
                prefix = "RGB: ",
                value = StorageManager.Instance.particleGroup[ScoreType.RGB],
            },
        };
    }
}
