using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticlesManager : Singleton<ParticlesManager> {
    public GameObject particlePrefab;
    public float generatorInternal;
    
    private readonly List<GameObject> particlesList = new();
    private IEnumerator particleGenerator;
    private ParticleType? lastType;

    private void Start() {
        particleGenerator = ParticleGenerator();
    }

    private IEnumerator ParticleGenerator() {
        while (true) {
            var particle = Instantiate(
                particlePrefab,
                new Vector2(Random.Range(0.0f, 0.0f), 11.0f),
                Quaternion.identity
            );
            lastType = GetNextType(lastType);
            particle.GetComponent<Particle>().SetType(lastType.Value);
            particlesList.Add(
                particle
            );
            yield return new WaitForSeconds(generatorInternal);
        }
    }

    public void StartParticleGenerator() {
        StartCoroutine(particleGenerator);
    }

    private void StopParticleGenerator() {
        StopCoroutine(particleGenerator);
    }

    public void ClearParticleList() {
        StopParticleGenerator();
        foreach (var item in particlesList) {
            item.GetComponent<Particle>().Destroy(()=>Destroy(item));
        }
        particlesList.Clear();
    }

    private static ParticleType GetNextType(ParticleType? ignoreType) {
        var typeNum = Enum.GetNames(typeof(ParticleType)).Length;
        var range = Enumerable.Range(0, typeNum).Where(i => !ignoreType.HasValue || i != (int)ignoreType);
        var index = Random.Range(0, typeNum - (ignoreType.HasValue ? 1 : 0));
        return (ParticleType)range.ElementAt(index);
    }
}
