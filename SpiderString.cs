using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderString : MonoBehaviour
{
    [SerializeField] private float requiredDistance = 3f;
    [SerializeField] private Transform distancePoint;

    [SerializeField] ExtendableLine m_ExtendableLine;

    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");

        if (!m_ExtendableLine)
        {
            m_ExtendableLine = GetComponent<ExtendableLine>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistnace();
    }

    void CheckDistnace()
    {
        if (DistanceFromPlayer() < requiredDistance)
            m_ExtendableLine.ExtendLine();
        else
            m_ExtendableLine.RetractLine();
    }

    float DistanceFromPlayer()
    {
        float d = Vector2.Distance(distancePoint.position, target.transform.position);
        return d;
    }
}