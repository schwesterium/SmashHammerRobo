using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameter", menuName = "Scriptable Objects/PlayerParameter")]
public class PlayerParameter : ScriptableObject
{
    //移動速度
    public float WalkSpeed = 1f;
    //ダッシュ速度
    public float DashSpeed = 2f;
    //回転速度
    public float RotationSpeed = 1f;
    //サイドステップ
    public float JumpPower = 5f;
    public float OverTimeAddPer = 0.3f;
}
