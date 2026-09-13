using UnityEngine;
using System.Collections.Generic;

public class SetShadow : MonoBehaviour
{
    [SerializeField]
    private Renderer _shadowMesh; // 1ñáÇÃínñ ÉÅÉbÉVÉÖ
    [SerializeField]
    private List<Transform> _characters = new List<Transform>();

    private const int MAX_CHARACTERS = 16;
    private Vector4[] _positions;
    private MaterialPropertyBlock _mpb;

    private static readonly int s_positionsID = Shader.PropertyToID("_CharacterPositions");
    private static readonly int s_countID = Shader.PropertyToID("_CharacterCount");

    int _count = 0;

    void Start()
    {
        _positions = new Vector4[MAX_CHARACTERS];
        _mpb = new MaterialPropertyBlock();

        _count = Mathf.Min(_characters.Count, MAX_CHARACTERS);
    }

    void LateUpdate()
    {
        if (_shadowMesh == null) return;

        for (int i = 0; i < _count; i++)
        {
            _positions[i] = _characters[i].position;
        }

        _shadowMesh.GetPropertyBlock(_mpb);
        _mpb.SetVectorArray(s_positionsID, _positions);
        _mpb.SetInt(s_countID, _count);
        _shadowMesh.SetPropertyBlock(_mpb);
    }

    public void RegisterCharacter(Transform t)
    {
        if (!_characters.Contains(t)) _characters.Add(t);
    }

    public void UnregisterCharacter(Transform t)
    {
        _characters.Remove(t);
    }
}