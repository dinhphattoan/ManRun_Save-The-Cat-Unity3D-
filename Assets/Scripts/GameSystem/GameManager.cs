using System.Collections;
using System.Collections.Generic;
using SaveTheCat;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameMechanic
{
    public class GameManager : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private SO_GameDatabase gameDatabase;
        public SO_GameDatabase GameDatabase { get { return gameDatabase; } }
        [SerializeField] private SO_MechanicSetting mechanicSetting;
        [Space]

        [Header("Scripts")]
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private CameraController cameraController;
        private PlayerController playerController;
        private PlayerBehavior playerBehavior;

        [Header("Prefabs")]
        [SerializeField] private GameObject playerPrefab;

        [SerializeField] private GameObject tsunamiPrefab;



        [Header("Transforms")]
        [SerializeField] private Transform finalizeUIPanel;
        [SerializeField] private Transform inputJoyStickTransform;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform tsunamiTransform;
        [SerializeField] private ProceduralLevel proceduralLevel;
        public Transform TsunamiTransform { get { return tsunamiTransform; } }
        public Transform FinishLineTransform { get; private set; }
        public List<Transform> SpawnedCats { get { return spawnedCats; } }
        [Header("Round level data")]
        [SerializeField] private List<Transform> spawnedCats = new();

        [Header("Components")]
        public PlayerBehavior PlayerBehavior { get { return playerBehavior; } }
        [Header("Final Board Hub")]
        [SerializeField] TextMeshProUGUI finishText;
        [SerializeField] TextMeshProUGUI saveCatText;
        [SerializeField] TextMeshProUGUI coinEarnText;
        [SerializeField] TextMeshProUGUI difficultyText;
        [SerializeField] TextMeshProUGUI coinEarnResult;

        [Header("UI")]
        //Speed hub
        [SerializeField] private UISpeed uISpeed;
        [SerializeField] private TextMeshProUGUI countingDownText;
        [SerializeField] float maxTimeWaitRecover = 2f; //2 seconds
        //UI Cover
        [SerializeField] private UICover uiCover;

        [Header("Game Mangager settings")]
        [SerializeField] private int startGameSecond = 3;
        [SerializeField] private static bool gameStart = false;
        public static bool GameStart => gameStart;
        private bool isEnergyRecovering = false;
        private float timeWaitRecoverCounter = 0f;
        private bool isRestart = false;
        private int nSavedCat => levelManager.CatSpawners.Count - spawnedCats.Count;
        private int tsunamiLevelIndex = 0;
        private void OnEnable()
        {
            proceduralLevel = FindFirstObjectByType<ProceduralLevel>();
            cameraController.Initialize();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(proceduralLevel.gameObject);
            gameStart = false;
        }
        #region Initialize
        public void Initialize()
        {
            if (!isRestart)
            {
                playerTransform = Instantiate(playerPrefab, levelManager.PlayerSpawnPoint.position, Quaternion.identity).transform;
                tsunamiTransform = Instantiate(tsunamiPrefab, levelManager.TsunamiSpawnPoint.position, Quaternion.identity).transform;


                DontDestroyOnLoad(playerTransform.gameObject);
                DontDestroyOnLoad(tsunamiTransform.gameObject);

                playerController = playerTransform.GetComponent<PlayerController>();
                playerBehavior = playerTransform.GetComponent<PlayerBehavior>();

                var carEvent = Object.FindFirstObjectByType<CarNavigationController>();
                if (carEvent != null)
                    carEvent.Initialize();
                cameraController.StopAllCoroutines();
                cameraController.Initialize();

                cameraController.Camera_GameStart();
                gameDatabase.Initialize();
                mechanicSetting.Initialize();
                playerBehavior.Initialize();

                SpawnAnimals();
                levelManager.GetSpawnedObject();
                StartCoroutine(InitializeLevelStart());
            }
        }
        private void SpawnAnimals()
        {
            foreach (var spawner in levelManager.CatSpawners)
            {
                spawnedCats.Add(spawner.GetComponent<CatSpawner>().SpawnCat(levelManager.SpawnedCatHolder));

            }
        }
        public void LoadLevelData(LevelManager _levelManager)
        {
            levelManager = _levelManager;
        }
        #endregion
        #region  InGame Runtime
        private void Update() => GameLoop();

        private void GameLoop()
        {
            if (gameStart)
            {
                inputJoyStickTransform.gameObject.SetActive(true);
                GameMechanic();
                MoveTsunami();
            }
            else
            {
                inputJoyStickTransform.gameObject.SetActive(false);
            }

        }
        private void GameMechanic()
        {
            EnergyRecovery();
            TsunamiRuntimeLevel();
        }
        public void EnergyRecovery()
        {
            //Handle energy recovery
            if (isEnergyRecovering == false && timeWaitRecoverCounter >= maxTimeWaitRecover)
            {
                isEnergyRecovering = true;
                StartCoroutine(FillEnergyBar());
            }
            else
            {
                timeWaitRecoverCounter = Mathf.MoveTowards(timeWaitRecoverCounter, maxTimeWaitRecover, Time.fixedDeltaTime);
            }
        }
        public void Convert_Energy()
        {
            isEnergyRecovering = false;
            timeWaitRecoverCounter = 0;
            mechanicSetting.TradeTempEnergyPerMaxSpeed();
        }
        public void TsunamiRuntimeLevel()
        {
            if (levelManager.TsunamiLevels.Length > 0 && levelManager.TsunamiLevels.Length > tsunamiLevelIndex)
            {
                if (levelManager.TsunamiLevels[tsunamiLevelIndex].IsReached(tsunamiTransform))
                {
                    mechanicSetting.SetTsunamiLevel(levelManager.TsunamiLevels[tsunamiLevelIndex++]);
                }
            }
        }
        private void MoveTsunami()
        {
            tsunamiTransform.transform.position += Vector3.forward * mechanicSetting.TsunamiMoveSpeed * Time.deltaTime;
        }
        public void RemoveCatTransform(Transform catTransform)
        {
            spawnedCats.Remove(catTransform);
        }

        public IEnumerator FillEnergyBar()
        {
            while (timeWaitRecoverCounter >= maxTimeWaitRecover)
            {
                mechanicSetting.RecoverPlayerEnergy();
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }

        #endregion
        #region Finalize round
        public void FinalizeWon()
        {
            gameStart = false;
            playerController.autoDirection = Vector3.forward;
            playerController.isAuto = true;

            finalizeUIPanel.gameObject.SetActive(true);
            finishText.text = "Round Finished!";
            int additionalCoinEarn = 50;
            saveCatText.text = nSavedCat.ToString();
            finishText.gameObject.SetActive(true);
            FillFinalizeBoard(additionalCoinEarn);
        }
        public void FinalizeLost()
        {
            gameStart = false;

            playerController.autoDirection = Vector3.zero;
            playerController.isAuto = true;

            finalizeUIPanel.gameObject.SetActive(true);
            finishText.text = "You lost!";
            saveCatText.text = nSavedCat.ToString();
            FillFinalizeBoard(0);
        }
        public void FillFinalizeBoard(int additionalCoinEarn)
        {
            coinEarnText.text = $"{10 * nSavedCat} ({nSavedCat} cats)";
            if (additionalCoinEarn != 0) coinEarnText.text += $"\n+{additionalCoinEarn} (Win)";
            float multiplier = 1f;
            //If level is custom level
            if (proceduralLevel.levelSettingTransforms != null && proceduralLevel.levelSettingTransforms.Length > 0)
            {
                LevelSetting levelSetting = proceduralLevel.levelSettingTransforms[0].GetComponent<LevelSetting>();
                multiplier += levelSetting.DifficultyScale;
            }
            difficultyText.text = "x" + multiplier.ToString("0.0") + "difficulty";
            int earnresult = (int)(nSavedCat * 10f * multiplier) + additionalCoinEarn;
            coinEarnResult.text = earnresult.ToString();
            gameDatabase.CurrencyValue += earnresult;
        }
        #endregion

        #region Round Settings

        public void UnloadLevel()
        {
            if (playerTransform != null && tsunamiTransform != null)
            {
                Destroy(playerTransform.gameObject);
                Destroy(tsunamiTransform.gameObject);
            }

            countingDownText.gameObject.SetActive(false);
            foreach (Transform cat in spawnedCats)
            {
                Destroy(cat.gameObject);
            }
            spawnedCats.Clear();
            playerBehavior = null;
            playerController = null;


        }
        public void ReloadLevel()
        {
            finalizeUIPanel.gameObject.SetActive(false);
            if (!isRestart && levelManager != null)
            {
                isRestart = true;
                StopAllCoroutines();
                StartCoroutine(Coroutine_ReloadLevel());
            }

        }
        public void HandleMainMenu()
        {
            if (!isRestart)
            {
                StopAllCoroutines();
                StartCoroutine(Coroutine_MainMenu());
            }

        }
        public IEnumerator InitializeLevelStart()
        {
            cameraController.Initialize();
            cameraController.Camera_GameStart();
            FinishLineTransform = levelManager.FinishTransforms[0];
            yield return new WaitUntil(() => !cameraController.IsInTransistion);
            StartCoroutine(CountingCountingDownStart());
        }
        public IEnumerator CountingCountingDownStart()
        {
            gameStart = false;
            countingDownText.gameObject.SetActive(true);
            for (int i = startGameSecond; i > 0; i--)
            {
                countingDownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            countingDownText.text = "GO!";

            yield return new WaitForSeconds(0.5f);
            playerController.isAuto = false;
            countingDownText.gameObject.SetActive(false);
            gameStart = true;
        }
        private IEnumerator Coroutine_MainMenu()
        {
            finalizeUIPanel.gameObject.SetActive(false);
            uiCover.gameObject.SetActive(true);
            StartCoroutine(uiCover.FadeIn());
            yield return new WaitUntil(() => !uiCover.IsInTransition);
            gameStart = false;
            isRestart = true;
            UnloadLevel();
            if (SceneManager.GetSceneByName("SaveTheCat1").isLoaded)
            {
                AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync("SaveTheCat1");
                asyncOperation.allowSceneActivation = false;
                while (asyncOperation.progress < 1f)
                {
                    uiCover.loadingSliderUI.value = asyncOperation.progress;
                    yield return null;
                }
            }
            else
            {
                proceduralLevel.UnloadLevel();
                levelManager = null;

            }
            MainMenuManager mainMenuManager = FindFirstObjectByType<ScriptHolder>().GetComponent<MainMenuManager>();
            isRestart = false;
            mainMenuManager.LoadMainMenu();
            cameraController.Camera_MainMenu();
            StartCoroutine(uiCover.FadeOut());
            yield return new WaitUntil(() => !uiCover.IsInTransition);
            uiCover.gameObject.SetActive(false);
        }
        private IEnumerator Coroutine_ReloadLevel()
        {
            if (levelManager != null)
            {
                UnloadLevel();
                uiCover.gameObject.SetActive(true);
                gameStart = false;
                isRestart = true;

                StartCoroutine(uiCover.FadeIn());
                yield return new WaitUntil(() => !uiCover.IsInTransition);
                levelManager.RandomizeLevel();
                mechanicSetting.Initialize();
                yield return new WaitUntil(() => !levelManager.IsProcess);
                isRestart = false;
                Initialize();
                StartCoroutine(uiCover.FadeOut()); // Fade out after loading

                yield return new WaitUntil(() => !uiCover.IsInTransition);
                uiCover.gameObject.SetActive(false);
            }
            yield return null;


        }
        #endregion


    }
    [System.Serializable]
    public struct TsunamiLevel
    {
        public string LevelName;
        public float Speed;
        public Transform TriggerTransform;

        public bool IsReached(Transform input)
        {
            return input.position.z >= TriggerTransform.position.z;
        }
    }
}