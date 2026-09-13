using System;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action EventA;
    public event Action EventB;
    public event Action EventC;
    public event Action EventD;

    public void InvokeA() { EventA?.Invoke(); }
    private void InvokeB() { EventB?.Invoke(); }
    private void InvokeC() { EventC?.Invoke(); }
    private void InvokeD() { EventD?.Invoke(); }
}
