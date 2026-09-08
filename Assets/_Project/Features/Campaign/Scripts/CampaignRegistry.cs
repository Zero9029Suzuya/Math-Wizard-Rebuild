using System.Collections.Generic;
using UnityEngine;

namespace MathWizard.Features.Campaign
{
    [CreateAssetMenu(
        fileName = "CampaignRegistry",
        menuName = "Math Wizard/Campaign/Registry"
    )]
    public class CampaignRegistry : ScriptableObject
    {
        [SerializeField]
        private List<CampaignLevelDefinition> levels = new();

        public IReadOnlyList<CampaignLevelDefinition> Levels => levels;

        public bool TryGetLevel(
            string levelId,
            out CampaignLevelDefinition level)
        {
            foreach (CampaignLevelDefinition entry in levels)
            {
                if (entry == null)
                {
                    continue;
                }

                if (entry.LevelId == levelId)
                {
                    level = entry;
                    return true;
                }
            }

            level = null;

            Debug.LogError(
                $"Campaign level '{levelId}' is not registered.",
                this);

            return false;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            ValidateNullEntries();
            ValidateDuplicateIds();
            ValidateSceneReferences();
        }

        private void ValidateNullEntries()
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] == null)
                {
                    Debug.LogError(
                        $"Campaign level entry at index {i} is null.",
                        this);
                }
            }
        }

        private void ValidateDuplicateIds()
        {
            HashSet<string> registeredIds = new();

            foreach (CampaignLevelDefinition level in levels)
            {
                if (level == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(level.LevelId))
                {
                    continue;
                }

                if (!registeredIds.Add(level.LevelId))
                {
                    Debug.LogError(
                        $"Duplicate Campaign Level ID: '{level.LevelId}'.",
                        this);
                }
            }
        }

        private void ValidateSceneReferences()
        {
            foreach (CampaignLevelDefinition level in levels)
            {
                if (level == null)
                {
                    continue;
                }

                if (level.SceneReference == null)
                {
                    Debug.LogError(
                        $"Campaign level '{level.DisplayName}' has no SceneReference.",
                        level);

                    continue;
                }

                if (string.IsNullOrEmpty(
                    level.SceneReference.SceneName))
                {
                    Debug.LogError(
                        $"Campaign level '{level.DisplayName}' has no scene assigned.",
                        level);
                }
            }
        }

#endif
    }
}