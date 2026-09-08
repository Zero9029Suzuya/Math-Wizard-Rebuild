using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Add here the Overlays. Don't forget to set it to inactive first.
    [SerializeField] private GameObject campaignOverlay;
    [SerializeField] private GameObject onlineOverlay;
    [SerializeField] private GameObject settingsOverlay;


    // Maybe add some sort of Validation for each entry here.

    public void OpenCampaignOverlay()
    {
        campaignOverlay.SetActive(true);
    }

    public void OpenOnlineOverlay()
    {
        // We need to add a validation after we finished the online stuff... 
        // This will not be functional for now...

        // onlineOverlay.SetActive(true);

        Debug.Log("WOW ITS ONLINE OVERLAY (Not yet implemented)");
    }

    public void OpenSettingsOverlay(){
        settingsOverlay.SetActive(true);
    }

}