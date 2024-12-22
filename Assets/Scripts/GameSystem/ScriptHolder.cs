using System.Collections;
using GameMechanic;
using UnityEngine;

public class ScriptHolder : MonoBehaviour
{
    [Header("Game System")]
    [SerializeField] SO_GameDatabase gameDatabase;
    [SerializeField] SO_MechanicSetting mechanicSetting;
    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private PanelUIScript panelUIScript;
    [SerializeField] private UICover uICover;
    [SerializeField] private SoundController soundController;
    public SoundController SoundController { get { return soundController; } }
    void OnEnable()
    {
        mainMenuManager.Initialize();
        panelUIScript.Initialize();
        uICover.Initialize();
        soundController.Initialize();
        gameDatabase.Initialize();
        mechanicSetting.Initialize();
        DontDestroyOnLoad(gameObject);
    }
}
