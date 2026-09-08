using System;
using UnityEngine;
using MathWizard.Core.SceneManagement;

namespace MathWizard.Features.Campaign
{
    [CreateAssetMenu(
        fileName = "CampaignLevel",
        menuName = "Math Wizard/Campaign/Level"
    )]
    public class CampaignLevelDefinition : ScriptableObject
    {
        [Header("Identity")]

        [SerializeField]
        private string levelId;

        [SerializeField]
        private string displayName;

        [Header("Presentation")]

        [TextArea(3, 8)]
        [SerializeField]
        private string description;

        [Header("Scene")]

        [SerializeField]
        private SceneReference sceneReference;

        public string LevelId => levelId;

        public string DisplayName => displayName;

        public string Description => description;

        public SceneReference SceneReference => sceneReference;
    }
}