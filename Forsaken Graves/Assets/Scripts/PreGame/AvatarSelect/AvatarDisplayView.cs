using UnityEngine;

namespace ForsakenGraves.PreGame.AvatarSelect
{
    public class AvatarDisplayView : MonoBehaviour
    {
        [SerializeField] private Transform _avatarHolderTransform;

        public Transform AvatarHolderTransform => _avatarHolderTransform;
    }
}