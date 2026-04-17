using UnityEngine;

public class DoorController : BaseTriggerZone
{
    [Header("Door References")]
    [SerializeField] private Collider2D physicalBlocker;

    private Animator _animator;
    private const string ANIM_IS_OPEN = "IsOpen";

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // POLYMORPHISM: override abstract methods from BaseTriggerZone
    protected override void OnPlayerEnter() => SetDoorState(isOpen: true);
    protected override void OnPlayerExit() => SetDoorState(isOpen: false);

    // ENCAPSULATION: door state change in one private method
    private void SetDoorState(bool isOpen)
    {
        _animator?.SetBool(ANIM_IS_OPEN, isOpen);

        if (physicalBlocker != null)
            physicalBlocker.enabled = !isOpen;
    }
}
