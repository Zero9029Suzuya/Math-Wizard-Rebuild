using UnityEngine;
using UnityEngine.SceneManagement;

namespace MathWizard.Core.SceneManagement
{
    public class SceneLoader
    {
        private readonly SceneRegistry sceneRegistry;

        public SceneLoader(SceneRegistry registry)
        {
            sceneRegistry = registry;
        }

        public void Load(SceneType sceneType)
        {
            if (!sceneRegistry.TryGetSceneName(
                sceneType,
                out string sceneName))
            {
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void Load(SceneReference sceneReference)
        {
            if (sceneReference == null)
            {
                Debug.LogError(
                    "Cannot load scene: SceneReference is null.");

                return;
            }

            if (string.IsNullOrEmpty(sceneReference.SceneName))
            {
                Debug.LogError(
                    "Cannot load scene: SceneReference has no scene assigned.");

                return;
            }

            SceneManager.LoadScene(sceneReference.SceneName);
        }
    }
}