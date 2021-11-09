using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En_Snake : EnemyController
{
    private enum State { Idle = 0, Walk = 1, Falling = 2, CeilingIdle = 3 }

    [Header("Statemachine")]
    [SerializeField] private State currentState = State.Idle;
    [SerializeField, Vector("Min", "Max")] private Vector2 idleTime = new Vector2(3, 5);
    [SerializeField, Vector("Min", "Max")] private Vector2 walkTime = new Vector2(3, 5);
    [SerializeField, Vector("Min", "Max")] private Vector2 startFallTime = new Vector2(1, 3);

    [Header("Stats")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float fallingGravityScale = 3f;

    private Vector2 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        OnStateEnter(State.CeilingIdle);
        ChangeState(State.CeilingIdle);
    }

    // Update is called once per frame
    void Update()
    {
        EnterGroundState();
        StartFallingState();
    }

    void StartFallingState()
    {
        bool started = false;

        if(IsVisible && !started && currentState == State.CeilingIdle)
        {
            StartCoroutine(EnterFallingState());
            started = true;
        }
    }

    void EnterGroundState()
    {
        if(IsGrounded && currentState == State.Falling)
        {
            ChangeState(State.Idle);
        }
    }

    private void ChangeState(State state)
    {
        if (currentState != state)
            OnStateEnter(state);

        switch (state)
        {
            case State.Idle:
                currentState = State.Idle;
                //animator.Play(HashIdle);
                StartCoroutine(IdleState());
                break;

            case State.Walk:
                currentState = State.Walk;
                //animator.Play(HashWalk);
                StartCoroutine(WalkState());
                break;

            case State.Falling:
                currentState = State.Falling;
                MyRigidbody.gravityScale = fallingGravityScale;
                break;

            case State.CeilingIdle:
                currentState = State.CeilingIdle;
                MyRigidbody.gravityScale = 0f;
                break;
        }
    }

    private void OnStateEnter(State state)
    {
        switch (state)
        {
            case State.Idle:
                currentState = State.Idle;
                //animator.Play(HashIdle);
                //StartCoroutine(IdleState());
                break;

            case State.Walk:
                currentState = State.Walk;
                //animator.Play(HashWalk);
                //StartCoroutine(WalkState());
                break;
            case State.Falling:
                currentState = State.Falling;
                MyRigidbody.gravityScale = fallingGravityScale;
                break;

            case State.CeilingIdle:
                currentState = State.CeilingIdle;
                MyRigidbody.gravityScale = 0f;
                break;
        }
    }

    private IEnumerator IdleState()
    {
        yield return new WaitForSeconds(Random.Range(idleTime.x, idleTime.y));

        //if it hits a wall on idle, ai will turn around the other way. Else pick a random state that isnt idle state.
        if (IsHittingWall)
            TurnAround();
        else
            Flip((FacingDirection)Random.Range(0, 2));

        int nextMove = 1;
        ChangeState((State)nextMove);
    }

    private IEnumerator WalkState()
    {
        float time = Mathf.Abs(Random.Range(walkTime.x, walkTime.y));

        while (time > 0.0f)
        {
            MyRigidbody.velocity = new Vector2(CharacterRight.x * moveSpeed, MyRigidbody.velocity.y);
            time -= Time.deltaTime;

            if (IsHittingWall)
                time = 0;

            yield return null;
        }

        MyRigidbody.velocity = Vector2.zero;

        //if it hits a wall after walking, ai will try to jump over the wall. Else, pick a random state
        if (IsHittingWall)
        {
            TurnAround();
        }
        else
        {
            int nextMove = 1;
            ChangeState((State)nextMove);
        }
    }

    private IEnumerator EnterFallingState()
    {
        yield return new WaitForSeconds(Random.Range(startFallTime.x, startFallTime.y));

        ChangeState(State.Falling);
    }
}