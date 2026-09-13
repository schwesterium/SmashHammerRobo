using UnityEngine;

namespace HammerSmash
{
    //UŒ‚‚Ì‹­‚³‚âƒ`ƒƒ[ƒW’iŠK‚ğŠi”[‚·‚é
    public struct AttackInfo
    {
        public float Power;
        public Vector3 KnockBackDirection;
        public float KnockBackDistance;
        public PlayerHammer.PowerChargeState ChargeState;

        public AttackInfo(float power, Vector3 knockBackDirection, float knockBackDistance, PlayerHammer.PowerChargeState state)
        {
            Power = power;
            KnockBackDirection = knockBackDirection;
            KnockBackDistance = knockBackDistance;
            ChargeState = state;
        }
    }

}