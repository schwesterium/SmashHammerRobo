using UnityEngine;

namespace HammerSmash
{
    [CreateAssetMenu(fileName = "HammerParameter", menuName = "Scriptable Objects/HammerParameter")]
    public class HammerParameter : ScriptableObject
    {
        public float MaxPower = 10f;
        public float MaxKnockBackDistance = 20f;
        public float AddChargeSpeedMulPer = 0.5f;
    }

}