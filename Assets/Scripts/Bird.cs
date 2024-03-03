using CodeMonkey;
using System;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private State state;

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    [SerializeField]
    private float jumpSpeed = 40f;

    [SerializeField]
    private BirdSO birdSO;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;

        //decompose the BirdSO
        GetComponent<SpriteRenderer>().sprite = birdSO.spriteRenderer;
        GetComponent<Animator>().runtimeAnimatorController = birdSO.runtimeAnimatorController;
        GetComponent<CircleCollider2D>().radius = birdSO.radius;
        rigidbody2D.mass = birdSO.mass;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    OnStartedPlaying?.Invoke(this, EventArgs.Empty);
                    state = State.Playing;
                    rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                break;
            case State.Dead:
                rigidbody2D.bodyType = RigidbodyType2D.Static;
                break;
        }
    }

    private void Jump()
    {
        // Preserve the horizontal velocity
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            GameObject collidedObject = collision.gameObject;
            Ability ability = collidedObject.GetComponent<Ability>();
            ability.PerformAbility(this.gameObject);
        }
        else
        {
            OnDied?.Invoke(this, EventArgs.Empty);
            state = State.Dead;
        }
        
    }

    public void SetJumpSpeed(float value)
    {
        jumpSpeed = value;
    }

    public float GetJumpSpeed()
    {
        return jumpSpeed;
    }
}
