using UnityEngine;

public class SpaceFighter : MonoBehaviour {
    public float speed;
    
    private Rigidbody2D rb;
    private Vector2 initPosition;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        initPosition = rb.position;
    }
    
    private void Update() {
        if (GameManager.Instance.gameState.Value == GameState.Running) {
            var move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            rb.velocity = move * speed;
        }
    }

    public void ResetFighter() {
        rb.velocity = Vector2.zero;
        rb.position = initPosition;
    }
    
}
