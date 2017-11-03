using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Worker;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Editor 
{
	public class SnapshotMenu : MonoBehaviour
	{
		[MenuItem("Improbable/Snapshots/Generate Default Snapshot")]
		private static void GenerateDefaultSnapshot()
		{
			var snapshotEntities = new Dictionary<EntityId, Entity>();
			var currentEntityId = 1;

			Debug.Log("generate");

            var entities = FindObjectsOfType<EntityTemplate>()
            .Select(t => t.gameObject.GetComponent<EntityTemplate>().EntityBuilder().Build());
            
            
            snapshotEntities = entities.ToDictionary(e => new EntityId(currentEntityId++));
            snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreatePlayerCreatorTemplate());
			snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateCubeTemplate());
            //snapshotEntities.Add(new EntityId(currentEntityId++), EntityTemplateFactory.CreateRampa1Template());
            SaveSnapshot(snapshotEntities);
		}

		private static void SaveSnapshot(IDictionary<EntityId, Entity> snapshotEntities)
		{
			File.Delete(SimulationSettings.DefaultSnapshotPath);
			var maybeError = Snapshot.Save(SimulationSettings.DefaultSnapshotPath, snapshotEntities);

			if (maybeError.HasValue)
			{
				Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", maybeError.Value);
			}
			else
			{
				Debug.LogFormat("Successfully generated initial world snapshot at {0}", SimulationSettings.DefaultSnapshotPath);
			}
		}


        [MenuItem("Improbable/Snapshots/Scene from Default Snapshot")]
        private static void CreateSceneFromDefaultSnapshot() {
            // Load default snapshot
            IDictionary<EntityId, Entity> snapshot;
            if (!TryLoadSnapshot(out snapshot)) {
                return;
            }

            // Create and open a new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Add a prefab for each entity
            PopulateSceneFromSnapshot(snapshot);

            // Save populated scene
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();
        }


        private static bool TryLoadSnapshot(out IDictionary<EntityId, Entity> snapshot) {
            var errorOpt = Snapshot.Load(SimulationSettings.DefaultSnapshotPath, out snapshot);
            if (errorOpt.HasValue) {
                Debug.LogErrorFormat("Error loading snapshot: {0}", errorOpt.Value);
            }
            return !errorOpt.HasValue;
        }

        private static void PopulateSceneFromSnapshot(IDictionary<EntityId, Entity> snapshot) {
            foreach (var pair in snapshot) {
                var entity = pair.Value;
                if (entity.Get<Metadata>().HasValue) {
                    var prefabName = entity.Get<Metadata>().Value.Get().Value.entityType;
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EntityPrefabs/" + prefabName + ".prefab");

                    var gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                    var position = entity.Get<Position>().Value.Get().Value.coords.ToUnityVector();
                    gameObject.transform.position = position;
                }
            }
        }
    }
}
