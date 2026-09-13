using HammerSmash;
using UnityEngine;

namespace HammerSmash
{
    public class PlayerDeadArea : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var layer = 1 << other.gameObject.layer;

            //Layer Player‚Æ‚Ì”»’è
            if ((layer & 0x80) != 0)
            {
                other.GetComponent<PlayerController>().OnDead();
            }
        }
    }

}