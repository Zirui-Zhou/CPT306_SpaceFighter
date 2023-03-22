using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ScoreType {
    Red,
    Green,
    Blue,
    RGB,
    Debris,
}

public class StorageManager : Singleton<StorageManager> {
    private static readonly GameObject[][] Storage = JaggedArray.CreateJaggedArray<GameObject[][]>(5, 5);

    private IEnumerator detectGroupTimer;
    
    public readonly Dictionary<ScoreType, Property<int>> particleGroup = new() {
        [ScoreType.Red] = new Property<int>(0),
        [ScoreType.Green] = new Property<int>(0),
        [ScoreType.Blue] = new Property<int>(0),
        [ScoreType.RGB] = new Property<int>(0),
        [ScoreType.Debris] = new Property<int>(0),
    };

    private readonly List<ScoreType> groupType = new() {
        ScoreType.Red,
        ScoreType.Green,
        ScoreType.Blue,
        ScoreType.RGB,
    };

    public readonly Property<int> totalScore = new(0);
    public readonly Property<int> totalGroup = new(0);

    private readonly Dictionary<ScoreType, int> scoreWeight = new() {
        [ScoreType.Red] = 10,
        [ScoreType.Green] = 10,
        [ScoreType.Blue] = 10,
        [ScoreType.RGB] = 15,
        [ScoreType.Debris] = 5,
    };

    private static int _debrisCount;
    
    private readonly ParticleType[, ] verticalOrders = {
        {ParticleType.Red, ParticleType.Red, ParticleType.Red},
        {ParticleType.Green, ParticleType.Green, ParticleType.Green},
        {ParticleType.Blue, ParticleType.Blue, ParticleType.Blue}
    };

    private Action[] GetVerticalScoreFunc() {
        return new Action[] {
            () => particleGroup[ScoreType.Red].Value += 1,
            () => particleGroup[ScoreType.Green].Value += 1,
            () => particleGroup[ScoreType.Blue].Value += 1,
        };
    } 

    private readonly ParticleType[, ] horizontalOrders = {
        {ParticleType.Red, ParticleType.Red, ParticleType.Red},
        {ParticleType.Green, ParticleType.Green, ParticleType.Green},
        {ParticleType.Blue, ParticleType.Blue, ParticleType.Blue},
        {ParticleType.Red, ParticleType.Green, ParticleType.Blue},
    };

    private Action[] GetHorizontalScoreFunc() {
        return new Action[] {
            () => particleGroup[ScoreType.Red].Value += 1,
            () => particleGroup[ScoreType.Green].Value += 1,
            () => particleGroup[ScoreType.Blue].Value += 1,
            () => particleGroup[ScoreType.RGB].Value += 1,
        };
    }

    // Start is called before the first frame update
    private void Start() {
        foreach (var item in particleGroup) {
            item.Value.ValueChanged += _ => {
                totalScore.Value = CalcTotalScore();
                totalGroup.Value = CalcTotalGroup();
                DetectIsGameOver(GameResult.Win);
            };
        }
        detectGroupTimer = DetectGroupTimer();
        StartCoroutine(detectGroupTimer);
    }

    public void AddParticle(int col, GameObject particleGo) {
        var particle = particleGo.GetComponent<Particle>();
        _debrisCount += particle.particleType == ParticleType.Debris ? 1 : 0;
        for (var row = 4; row >= 0; row--) {
            if (Storage[row][col]) continue;
            Storage[row][col] = particleGo;
            DetectIsGameOver(GameResult.LoseByDebris);
            return;
        }
        
        DetectIsGameOver(GameResult.LoseByNonLined);
        particle.PlayCrossedAnimation();
    }

    private static void HandleParticleGravity(int space, int rowEnd, int colStart, int colEnd) {
        for(var col = colStart; col <= colEnd; col++) {
            for (var row = rowEnd; row >= 0; row--) {
                Storage[row+space][col] = Storage[row][col];
                Storage[row][col] = null;
            }
        }
    }

    private void DetectIsGameOver(GameResult gameResult) {
        switch (gameResult) {
            case GameResult.Win:
                if (totalScore.Value >= GameManager.Instance.winScoreThreshold) {
                    GameManager.Instance.GameOver(gameResult);
                }
                break;
            case GameResult.LoseByDebris:
                if(_debrisCount > 5) {
                    GameManager.Instance.GameOver(gameResult);
                }
                break;
            case GameResult.LoseByNonLined:
                GameManager.Instance.GameOver(gameResult);
                break;
            case GameResult.LoseByOverlay:
            default:
                throw new ArgumentOutOfRangeException(nameof(gameResult), gameResult, null);
        }
    }
    
    public void CountDebrisHit() {
        particleGroup[ScoreType.Debris].Value += 1;
    }

    private int CalcTotalScore() {
        return particleGroup
            .Sum(item => scoreWeight[item.Key] * item.Value.Value);
    }

    private int CalcTotalGroup() {
        return particleGroup
            .Where(item => groupType.Contains(item.Key))
            .Sum(item=>item.Value);
    }
    
    private IEnumerator DetectGroupTimer() {
        while (true) {
            DetectParticleGroup();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DetectParticleGroup() {
        // Detect vertical
        for(var col = 0; col < 5; col++) {
            for(var row = 0; row < 3; row++) {
                for(var order = 0; order < verticalOrders.GetLength(0); order++) {
                    var result = true;
                    for(var idx = 0; idx < 3; idx++) {
                        var currentObject = Storage[row+idx][col];
                        if(!currentObject || currentObject.GetComponent<Particle>().particleState == ParticleState.Active) {
                            result = false;
                            break;
                        }
                        result &= currentObject.GetComponent<Particle>().particleType == verticalOrders[order, idx];
                    }
                    if (result) {
                        for(var idx = 0; idx < 3; idx++) {
                            Storage[row+idx][col].GetComponent<Particle>().Destroy();
                            Storage[row+idx][col] = null;
                        }
                        GetVerticalScoreFunc()[order]();
                        HandleParticleGravity(3, row-1, col, col);
                        return;
                    }
                }
            }
        }
        // Detect horizontal
        for(var row = 0; row < 5; row++) {
            for(var col = 0; col < 3; col++) {
                for(var order = 0; order < horizontalOrders.GetLength(0); order++) {
                    var result = true;
                    for(var idx = 0; idx < 3; idx++) {
                        var currentObject = Storage[row][col+idx];
                        if(!currentObject || currentObject.GetComponent<Particle>().particleState == ParticleState.Active) {
                            result = false;
                            break;
                        }
                        result &= currentObject.GetComponent<Particle>().particleType == horizontalOrders[order, idx];
                    }
                    if (result) {
                        for(var idx = 0; idx < 3; idx++) {
                            Storage[row][col+idx].GetComponent<Particle>().Destroy();
                            Storage[row][col+idx] = null;
                        }
                        GetHorizontalScoreFunc()[order]();
                        HandleParticleGravity(1, row-1, col, col+2);
                        return;
                    }
                }
            }
        }
    }

    public void ClearParticleStorage() {
        foreach (var subArray in Storage) {
            Array.Clear(subArray, 0, subArray.Length);
        }
        foreach (var item in particleGroup) {
            item.Value.Value = 0;
        }
        totalScore.Value = 0;
        totalGroup.Value = 0;
        _debrisCount = 0;
    }
}
