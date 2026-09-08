using UnityEngine;
using MathWizard.Core.SceneManagement;

namespace MathWizard.Features.Campaign
{
    public class CampaignOverlayController : MonoBehaviour
    {
        [SerializeField]
        private CampaignRegistry campaignRegistry;

        [SerializeField]
        private GameObject levelListRoot;

        [SerializeField]
        private GameObject levelDetailsRoot;

        private CampaignLevelDefinition selectedLevel;

        public void SelectLevel(CampaignLevelDefinition level)
        {
            if (level == null)
            {
                Debug.LogError(
                    "Cannot select campaign level: level is null.",
                    this);

                return;
            }

            selectedLevel = level;

            Debug.Log(
                $"Selected campaign level: {level.DisplayName}",
                this);

            // Future:
            // Update description
            // Update rewards
            // Update learnable spell
        }

        public void PlaySelectedLevel()
        {
            if (selectedLevel == null)
            {
                Debug.LogWarning(
                    "Cannot start campaign level: no level is selected.",
                    this);

                return;
            }

            GameApp.Instance.SceneLoader.Load(
                selectedLevel.SceneReference);
        }
    }
}