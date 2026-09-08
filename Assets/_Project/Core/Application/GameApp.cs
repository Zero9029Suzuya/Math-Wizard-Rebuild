using MathWizard.Core.SceneManagement;
using System;

public sealed class GameApp
{
    public static GameApp Instance { get; private set; }

    public SceneLoader SceneLoader { get; }

    private GameApp(SceneRegistry registry)
    {
        SceneLoader = new SceneLoader(registry);
    }

    public static void Initialize(SceneRegistry registry)
    {
        if (Instance != null)
        {
            throw new InvalidOperationException(
                "Application has already been initialized.");
        }

        Instance = new GameApp(registry);
    }
}
