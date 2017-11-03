using Improbable.Core;
using Improbable.Unity.Common.Core.Math;
using Improbable.Unity.Core.Acls;
using Improbable.Unity.Entity;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Gamelogic.EntityTemplates {
    [ExecuteInEditMode]
    public class EntityTemplate : MonoBehaviour {
        public virtual IComponentAdder EntityBuilder() {
#if UNITY_EDITOR
            var prefab = PrefabUtility.GetPrefabParent(gameObject);
            var rotation = new Improbable.Core.Quaternion(transform.rotation.x, transform.rotation.y,
                transform.rotation.z, transform.rotation.w);
            Debug.Log("found a entity");
            return Improbable.Unity.Entity.EntityBuilder.Begin()
                .AddPositionComponent(transform.position, CommonRequirementSets.PhysicsOnly)
                .AddMetadataComponent(entityType: prefab.name)
                .SetPersistence(true)
                .SetReadAcl(CommonRequirementSets.PhysicsOrVisual)
                .AddComponent(new Rotation.Data(rotation), CommonRequirementSets.PhysicsOnly);
#else
            return null;
#endif
        }
    }
}