using UnityEngine;

namespace ForsakenGraves.Gameplay.Data
{
    [CreateAssetMenu(fileName = "Player Config", menuName = "ForsakenGraves/Data/Player Config", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        public float MovementSpeed;
        public float RotationSpeed;
        public float JumpHeight;
        public float CameraMinXRotation;
        public float CameraMaxXRotation;
        public float HandsSlerpSpeed;
        public float HandsLerpSpeed;
    }
}