using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Spider_Controller : MonoBehaviour, IDoDamage
{
    [SerializeField] private float damage = 3;
    [SerializeField] Collider2D _collider;

    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;

        this.transform.gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    float IDoDamage.DamageAmount
    {
        get
        {
            return damage;
        }
    }
}