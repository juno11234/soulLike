using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler _inputHandler;
    Animator _animator;

    void Start()
    {
        _inputHandler = GetComponent<InputHandler>();
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _inputHandler.isInteracting = _animator.GetBool("isInteracting");
        _inputHandler.rollFlag = false;
    }
}