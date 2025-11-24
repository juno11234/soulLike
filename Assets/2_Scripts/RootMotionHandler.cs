using System;
using UnityEngine;

public class RootMotionHandler : MonoBehaviour
{
    public Vector3 DeltaPos { get; private set; }
    public Quaternion DeltaRot { get; private set; }
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        DeltaPos = _animator.deltaPosition;
        DeltaRot = _animator.deltaRotation;
    }
}