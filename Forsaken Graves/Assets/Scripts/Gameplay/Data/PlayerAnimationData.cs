using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.KAnimationCore.Runtime.Rig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ForsakenGraves.Gameplay.Data
{
    [CreateAssetMenu(fileName = "Player Animation Data", menuName = "ForsakenGraves/Animation", order = 0)]
    public class PlayerAnimationData : SerializedScriptableObject, IRigUser
    {
        public KRig RigAsset;
        public KRigElement WeaponBone = new KRigElement(-1, FPSANames.IkWeaponBone);
        public KRig GetRigAsset() => RigAsset;
    }
}