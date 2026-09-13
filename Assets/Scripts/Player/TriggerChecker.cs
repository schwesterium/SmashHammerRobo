using System;
using UnityEngine;

//OnTrigger‚Ìˆ—‚ğ‚·‚é
public class TriggerChecker : MonoBehaviour
{
    public event Action<Collider> OnEnter;
    public event Action<Collider> OnExit;

    [SerializeField]
    [Tooltip("”»’è‚ğæ‚éƒŒƒCƒ„[‚ğw’è‚·‚é")]
    private LayerMask targetLayer = default;

    private void OnTriggerEnter(Collider other)
    {
        var layer = 1 << other.gameObject.layer;

        if ((layer & targetLayer) != 0)
        {
            OnEnter?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var layer = 1 << other.gameObject.layer;

        if ((layer & targetLayer) != 0)
        {
            OnExit?.Invoke(other);
        }
    }

    private void OnDestroy()
    {
        OnEnter = null;
        OnExit = null;
    }
}
