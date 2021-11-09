using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En_PlantMonster : GenericEnemy
{
    private enum State { Idle, Shoot }
    private float _waitTimer = 0f;
    private Vector2 direction;

    private readonly int HashIdle = Animator.StringToHash("Idle");
    private readonly int HashShoot = Animator.StringToHash("Shoot");

    [Header("Statemachine")]
    [SerializeField] private State currentState = State.Idle;

    [Header("Weapon")]
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private float idleCooldown = 2f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float waitTimer = 2f;
    [SerializeField] private int shootDamage = 3;
    [SerializeField] private Transform shootExit = null;
    [SerializeField] private LayerMask targetLayer = default;
    [SerializeField] private bool IsAttacking;

    [Header("Sound")]
    [SerializeField] private AudioClip shootSfx;

    public Transform MyTarget { get; set; }

    private void Start()
    {
        MyTarget = FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        WaitTimer();
    }

    private void WaitTimer()
    {
        if (!IsAttacking)
        {
            if (_waitTimer <= 0f)
            {
                if (IsVisible)
                {
                    _waitTimer = waitTimer;
                    ChangeState(State.Idle);
                }
            }
            else
            {
                _waitTimer -= Time.deltaTime;
            }
        }
    }

    private void ChangeState(State state)
    {
        switch(state)
        {
            case State.Idle:
                currentState = State.Idle;
                animator.Play(HashIdle);

                if (IsVisible)
                    StartCoroutine(IdleCoroutine());
                else
                    _waitTimer = 0;

                break;
            case State.Shoot:
                currentState = State.Shoot;

                if (IsVisible)
                {
                    animator.Play(HashShoot);
                   // IsShielding = false;
                    StartCoroutine(ShootCoroutine());
                }
                else
                {
                    ChangeState(State.Idle);
                }

                break;
        }
    }

    private IEnumerator IdleCoroutine()
    {
        FaceTarget(MyTarget);

        yield return new WaitForSeconds(1f);

        ChangeState(State.Shoot);
    }

    private IEnumerator ShootCoroutine()
    {
        IsAttacking = true;

        yield return new WaitForSeconds(0.4f);

        SingleShot();

        yield return new WaitForSeconds(0.7f);

        IsAttacking = false;
        _waitTimer = 0;

        ChangeState(State.Idle);
    }

    private void SingleShot()
    {
        SFXManager.MyInstance.PlayOneShot(shootSfx, 2f);
        FaceTarget(MyTarget);

        for (int i = 0; i < 1; i++)
        {
            Bullet bullet;
            bullet = ObjectPoolManager.MyInstance.SpawnFromPool(bulletPrefab.name, shootExit.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.Initialized(speed, shootDamage, direction, transform, targetLayer);
        }
    }

    private void FaceTarget(Transform target)
    {
        if (target != null)
        {
            var dir = target.position.x - transform.position.x;

            if (dir < 0)
            {
                physicMover.Flip(PhysicMover.FacingDirection.Left);
                direction.x = -1;
            }
            else
            {
                physicMover.Flip(PhysicMover.FacingDirection.Right);
                direction.x = 1;
            }
        }
    }

    public override void OnTakeDamage(float damageAmount, Transform source)
    {
        if (CurrentHealth > 0 && currentState == State.Shoot)
        {
            base.OnTakeDamage(damageAmount, source);

            StopAllCoroutines();
            this.MyTarget = source;
            ChangeState(State.Idle);
        }
    }
}