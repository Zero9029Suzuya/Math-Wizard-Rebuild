using System.Collections;
using UnityEngine;
using MathWizard.Core.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField]
    private SceneRegistry sceneRegistry;

    private IEnumerator Start()
    {
        GameApp.Initialize(sceneRegistry);


        // TODO: When Game requires async.
        yield return Initialize();

        GameApp.Instance.SceneLoader.Load(SceneType.MainMenu);
    }

    private IEnumerator Initialize()
    {
        yield return null;
    }
}