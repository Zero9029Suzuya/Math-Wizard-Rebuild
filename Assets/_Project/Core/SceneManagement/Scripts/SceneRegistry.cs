using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace MathWizard.Core.SceneManagement
{
    [CreateAssetMenu(
        fileName = "SceneRegistry",
        menuName = "Math Wizard/Core/Scene Registry"
    )]
    public class SceneRegistry : ScriptableObject
    {
        [SerializeField]
        private List<SceneRegistryEntry> scenes = new();

        public bool TryGetSceneName(
            SceneType sceneType,
            out string sceneName)
        {
            foreach (SceneRegistryEntry entry in scenes)
            {
                if (entry.SceneType == sceneType)
                {
                    sceneName = entry.SceneReference.SceneName;
                    return true;
                }
            }

            Debug.LogError(
                $"Scene '{sceneType}' is not registered.",
                this);

            sceneName = string.Empty;
            return false;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            SyncSceneNames();
            ValidateDuplicateSceneTypes();
            ValidateBuildSettings();
            ValidateEnumCoverage();
        }

        private void SyncSceneNames()
        {
            foreach (SceneRegistryEntry entry in scenes)
            {
                if (entry.SceneReference == null)
                {
                    continue;
                }

                entry.SceneReference.SyncSceneName();
            }
        }

        private void ValidateDuplicateSceneTypes()
        {
            HashSet<SceneType> registered = new();

            foreach (SceneRegistryEntry entry in scenes)
            {
                if (!registered.Add(entry.SceneType))
                {
                    Debug.LogError(
                        $"Duplicate SceneType: {entry.SceneType}",
                        this);
                }
            }
        }

        private void ValidateBuildSettings()
        {
            HashSet<string> buildScenes = new();

            foreach (EditorBuildSettingsScene buildScene
                in EditorBuildSettings.scenes)
            {
                if (!buildScene.enabled)
                {
                    continue;
                }

                buildScenes.Add(
                    Path.GetFileNameWithoutExtension(
                        buildScene.path));
            }

            foreach (SceneRegistryEntry entry in scenes)
            {
                if (entry.SceneReference == null)
                {
                    Debug.LogError(
                        $"SceneType '{entry.SceneType}' has no SceneReference.",
                        this);

                    continue;
                }

                string sceneName = entry.SceneReference.SceneName;

                if (string.IsNullOrEmpty(sceneName))
                {
                    Debug.LogError(
                        $"SceneType '{entry.SceneType}' has no scene assigned.",
                        this);

                    continue;
                }

                if (!buildScenes.Contains(sceneName))
                {
                    Debug.LogWarning(
                        $"Scene '{sceneName}' is not in Build Settings.",
                        this);
                }
            }
        }

        private void ValidateEnumCoverage()
        {
            foreach (SceneType sceneType
                in Enum.GetValues(typeof(SceneType)))
            {
                bool found = false;

                foreach (SceneRegistryEntry entry in scenes)
                {
                    if (entry.SceneType == sceneType)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.LogWarning(
                        $"SceneType '{sceneType}' has no registered scene.",
                        this);
                }
            }
        }

#endif
    }
}