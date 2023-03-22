using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ParticleType {
    Red,
    Blue,
    Green,
    Debris,
}

public enum ParticleState {
    Active,
    Static,
}

public enum CollisionState {
    None,
    Particle,
    Fighter,
    Both,
}

public class Particle : MonoBehaviour {
    private readonly Dictionary<ParticleType, Color> particleColors = new() {
        { ParticleType.Red, Color.red },
        { ParticleType.Blue, Color.blue },
        { ParticleType.Green, Color.green },
        { ParticleType.Debris, Color.grey }
    };

    public ParticleType particleType;
    public ParticleState particleState;
    public CollisionState collisionState;
    private Rigidbody2D rb;
    private Animator animator;
    
    private bool isStored;
    
    private const int AngleBlockNum = 16;
    private const float AngleBlockMin = - (360 / AngleBlockNum);
    private const float AngleBlockMax = 360 / AngleBlockNum;
    
    private readonly Dictionary<ParticleType, float> particleGravity = new() {
        { ParticleType.Red, 0.9f },
        { ParticleType.Blue, 0.9f },
        { ParticleType.Green, 0.9f },
        { ParticleType.Debris, 0.6f },
    };
    
    private readonly List<ArrayList> collisionStateList = new() {
        new ArrayList {CollisionState.None, CollisionState.Fighter, "SpaceFighter"},
        new ArrayList {CollisionState.None, CollisionState.Particle, "Particle"},
        new ArrayList {CollisionState.Particle, CollisionState.Both, "SpaceFighter"},
        new ArrayList {CollisionState.Fighter, CollisionState.Both, "Particle"},
    };

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collisionState = CollisionState.None;

        var myCollider = GetComponent<Collider2D>();
        var colliders = GameObject.Find("/AreaBoundary/Play").GetComponentsInChildren<EdgeCollider2D>();
        foreach (var otherCollider in colliders) {
            Physics2D.IgnoreCollision(otherCollider, myCollider);
        }
    }

    // Update is called once per frame
    private void Update() {
        particleState = rb.velocity.y == 0 ? ParticleState.Static : ParticleState.Active;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (GameManager.Instance.gameState != GameState.Running) return;
        if (!col.gameObject.Equals(GameObject.Find("/AreaBoundary/Storage/Top"))) return;
        if (isStored) return;
        
        EnterStorage();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (GameManager.Instance.gameState != GameState.Running) return;

        DetectOverlayEnter(collision);
        
        if (particleType != ParticleType.Debris || collision.gameObject.name != "SpaceFighter") return;
        DetectDebrisDestroy(collision);
    }

    private void OnCollisionExit2D(Collision2D collision) {
        DetectOverlayExit(collision);
    }

    private void EnterStorage() {
        var position = transform.position;
        var column = (int)(position.x + 5) / 2;
        var step = 50f * Time.deltaTime;

        isStored = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        transform.position = Vector2.MoveTowards(
            position, 
            new Vector2((column - 2) * 2, position.y), 
            step
        );
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        StorageManager.Instance.AddParticle(column, gameObject);
    }

    private void DetectOverlayEnter(Collision2D collision) {
        foreach (var item in collisionStateList
                     .Where(item => 
                         collisionState == (CollisionState)item[0] 
                         && collision.gameObject.CompareTag((string)item[2]))) {
            collisionState = (CollisionState)item[1];
        }

        if (collisionState == CollisionState.Both) {
            GameManager.Instance.GameOver(GameResult.LoseByOverlay);
        }
    }

    private void DetectOverlayExit(Collision2D collision) {
        foreach (var item in collisionStateList
                     .Where(item => 
                         collisionState == (CollisionState)item[1] 
                         && collision.gameObject.CompareTag((string)item[2]))) {
            collisionState = (CollisionState)item[0];
        }
    }
    
    private void DetectDebrisDestroy(Collision2D collision) {
        Vector2 newVector = transform.position - collision.gameObject.transform.position;
        var angle = Vector2.SignedAngle(newVector, Vector2.up);
        if (angle is not (>= AngleBlockMin and <= AngleBlockMax)) return;
        Destroy(() => StorageManager.Instance.CountDebrisHit());
    }
    
    public void Destroy(Action afterAction = null) {
        afterAction ??= () => { };
        afterAction += () => gameObject.SetActive(false);
        if (gameObject.activeSelf) {
            StartCoroutine(PlayDestroyAnimation(afterAction));
        } else {
            afterAction();
        }
    }

    public void PlayCrossedAnimation() {
        animator.Play("Crossed");
    }
    
    private IEnumerator PlayDestroyAnimation(Action afterAction) {
        animator.Play("Destroy");
        var animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(animationLength);
        afterAction?.Invoke();
    }

    public void SetType(ParticleType type) {
        particleType = type;
        GetComponent<SpriteRenderer>().color = particleColors[particleType];
        rb.gravityScale = particleGravity[type];
    }
}