using System;
using UnityEngine;

namespace MathWizard.Core.SceneManagement
{
    [Serializable]
    public class SceneRegistryEntry
    {
        [SerializeField]
        private SceneType sceneType;

        [SerializeField]
        private SceneReference sceneReference;

        public SceneType SceneType => sceneType;

        public SceneReference SceneReference => sceneReference;
    }
}