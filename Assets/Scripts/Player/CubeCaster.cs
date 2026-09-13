using UnityEngine;

public class CubeCaster : MonoBehaviour
{
    //’n–Ê‚ÆŒð·‚µ‚Ä‚¢‚é‚È‚çtrueA‹ó’†‚É‚¢‚é‚È‚çfalse
    private bool isCasted = false;

    public bool IsCasted { get => isCasted; private set => isCasted = value; }

    [SerializeField]
    private Vector3 _hlfSize = Vector3.one;
    [SerializeField]
    private Vector3 _centerOffset = Vector3.zero;

    [SerializeField]
    private LayerMask _targetLayer = default;

    public void OnFixedUpdate()
    {
        if (Physics.CheckBox(transform.position + _centerOffset, _hlfSize, Quaternion.identity, _targetLayer))
        {
            IsCasted = true;
        }
        else
        {
            IsCasted = false;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = IsCasted ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + _centerOffset, _hlfSize * 2f);
    }
}