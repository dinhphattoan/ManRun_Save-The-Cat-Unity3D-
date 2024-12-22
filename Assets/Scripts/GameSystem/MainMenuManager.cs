using System.Collections;
using System.Collections.Generic;
using GameMechanic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SO_GameDatabase gameDatabase;
    public SO_GameDatabase GameDatabase => gameDatabase;
    private ScriptHolder scriptHolder;
    [Header("Transform")]
    [SerializeField] private Transform poolingHolder;
    [SerializeField] private List<Transform> tapUIPoolings = new();
    [SerializeField] private Transform tapUIObjectToPool;
    [SerializeField] private Transform uiMainMainHubTransform;
    [SerializeField] private int poolSize = 10;
    [Header("References")]
    [SerializeField] private ProceduralLevel proceduralLevel;
    [SerializeField] private AutoPlayerController autoPlayerController;


    [Header("UI Elements")]
    [SerializeField] private UICover uiCover;
    [SerializeField] private Slider difficultySlider;
    [SerializeField] private Slider catSlider;

    private void Start()
    {
        proceduralLevel = FindFirstObjectByType<ProceduralLevel>();
        scriptHolder = FindFirstObjectByType<ScriptHolder>();
    }
    public void Initialize()
    {
        InitializePooling();
    }


    public void HandleScreenTap(Vector2 localPoint) => HandleObjectPooling(localPoint);

    public void HandleUpgradeTap()
    {
        if (gameDatabase.TradeCurrencyPerMaxSpeed())
        {
            scriptHolder.SoundController.PlayAudioClip(gameDatabase.UpgradeClip, 0);
            autoPlayerController.HandleUpgrade();
        }
    }

    void HandleObjectPooling(Vector2 localPoint)
    {
        GameObject poolObject = GetPooledObject();
        poolObject.transform.position = localPoint;
        poolObject.SetActive(true);
    }
    public void InitializePooling()
    {
        tapUIPoolings = new List<Transform>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(tapUIObjectToPool.gameObject, poolingHolder);
            obj.SetActive(false);
            tapUIPoolings.Add(obj.transform);
        }
    }
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < tapUIPoolings.Count; i++)
        {
            if (!tapUIPoolings[i].gameObject.activeInHierarchy)
            {
                return tapUIPoolings[i].gameObject;
            }
        }

        GameObject obj = Instantiate(tapUIObjectToPool.gameObject);
        tapUIPoolings.Add(obj.transform);
        return obj;
    }
    public void HandleStartGame()
    {
        scriptHolder.SoundController.PlayAudioClip(gameDatabase.ButtonClickClip, 0);
        StartCoroutine(LoadScene());
    }
    public void UnloadMainMenu()
    {
        foreach (Transform pooling in tapUIPoolings)
        {
            pooling.gameObject.SetActive(false);
        }
        //Inactive the environment
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        uiMainMainHubTransform.gameObject.SetActive(false);
    }
    public void LoadMainMenu()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        uiMainMainHubTransform.gameObject.SetActive(true);

    }
    public void HandleStartCustomLevel(Transform panel)
    {
        StartCoroutine(Couroutine_LoadCustomLevel(panel));

    }

    public void HandleClose(Transform objectExit)
    {
        scriptHolder.SoundController.PlayAudioClip(gameDatabase.ButtonClickClip, 0);
        objectExit.gameObject.SetActive(false);
    }
    public void HandleOpen(Transform objectOpen)
    {
        scriptHolder.SoundController.PlayAudioClip(gameDatabase.ButtonClickClip, 0);
        objectOpen.gameObject.SetActive(true);

    }
    public void HandleSlider(TextMeshProUGUI textMeshProUGUI)
    {
        scriptHolder.SoundController.PlayAudioClip(gameDatabase.ButtonClickClip, 0);
        Slider slider = textMeshProUGUI.transform.parent.GetComponent<Slider>();
        textMeshProUGUI.text = $"{slider.value}/{slider.maxValue}";
    }


    private IEnumerator Couroutine_LoadCustomLevel(Transform panel)
    {
        panel.gameObject.SetActive(false);
        uiCover.gameObject.SetActive(true);
        StartCoroutine(uiCover.FadeIn());
        yield return new WaitUntil(() => !uiCover.IsInTransition);
        UnloadMainMenu();
        float difficultyValue = difficultySlider.value;
        float catValue = catSlider.value;
        LevelSetting levelSetting = proceduralLevel.LevelPrefab.GetComponent<LevelSetting>();
        levelSetting.numCatSpawner = (int)catValue;
        levelSetting.difficulityScale = difficultyValue / 10f;

        LevelManager levelManager = proceduralLevel.Initialize(1)[0].GetComponent<LevelManager>();
        levelManager.Initialize();
        yield return new WaitUntil(() => !levelManager.IsProcess);
        StartCoroutine(uiCover.FadeOut());
        yield return new WaitUntil(() => !uiCover.IsInTransition);
        uiCover.gameObject.SetActive(false);
    }
    private IEnumerator LoadScene()
    {
        StartCoroutine(uiCover.FadeIn());
        yield return new WaitUntil(() => !uiCover.IsInTransition);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("SaveTheCat1", LoadSceneMode.Additive);
        asyncOperation.allowSceneActivation = true;
        while (asyncOperation.progress >= 0.9f)
        {
            uiCover.slideLoading.value = asyncOperation.progress;

            yield return null;
        }
        UnloadMainMenu();
        StartCoroutine(uiCover.FadeOut());
        uiCover.gameObject.SetActive(false);
        yield return new WaitUntil(() => !uiCover.IsInTransition);
    }
}
