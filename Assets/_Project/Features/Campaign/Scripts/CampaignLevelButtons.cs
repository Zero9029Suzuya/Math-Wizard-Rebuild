using UnityEngine;

namespace MathWizard.Features.Campaign
{
    public class CampaignLevelButton : MonoBehaviour
    {
        [SerializeField]
        private CampaignLevelDefinition level;

        private CampaignOverlayController controller;

        public void Initialize(
            CampaignLevelDefinition levelDefinition,
            CampaignOverlayController overlayController)
        {
            level = levelDefinition;
            controller = overlayController;
        }

        public void Select()
        {
            if (level == null)
            {
                Debug.LogError(
                    "Campaign level button has no level assigned.",
                    this);

                return;
            }

            if (controller == null)
            {
                Debug.LogError(
                    "Campaign level button has no overlay controller.",
                    this);

                return;
            }

            controller.SelectLevel(level);
        }
    }
}