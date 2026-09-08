using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MathWizard.Core.SceneManagement
{
    [Serializable]
    public class SceneReference
    {
        [SerializeField]
        private string sceneName;

#if UNITY_EDITOR
        [SerializeField]
        private SceneAsset sceneAsset;
#endif

        public string SceneName => sceneName;

#if UNITY_EDITOR
        public void SyncSceneName()
        {
            if (sceneAsset != null)
            {
                sceneName = sceneAsset.name;
            }
            else
            {
                sceneName = string.Empty;
            }
        }

        public string ScenePath
        {
            get
            {
                if (sceneAsset == null)
                {
                    return string.Empty;
                }

                return AssetDatabase.GetAssetPath(sceneAsset);
            }
        }
#endif
    }
}