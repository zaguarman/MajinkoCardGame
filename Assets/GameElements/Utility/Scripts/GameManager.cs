using System.Collections;
using UnityEngine;
using Audio;
using Utility.TurnManagement;
using UnityEngine.SceneManagement;
using Overworld;
using DG.Tweening;

namespace Utility
{
    internal class GameManager : MonoBehaviour
    {
        #region Fields and Properties

        internal static GameManager Instance { get; private set; }
        private AudioManager _audioManager;

        [SerializeField] private GameState _gameState;

        internal GameState GameState
        {
            get { return _gameState; }
            private set { _gameState = value; }
        }

        private int _turn = 1;

        internal int Turn
        {
            get { return _turn; }
            private set { _turn = value; }
        }

        #endregion

        #region Functions

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            _audioManager = AudioManager.Instance;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            UtilityEvents.OnGameReset += OnGameReset;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            UtilityEvents.OnGameReset -= OnGameReset;
        }

        internal IEnumerator SwitchState(GameState state)
        {
            _gameState = state;

            switch (state)
            {
                case (GameState.MainMenu):
                    AudioManager.Instance.PlayGameTrack(Track._0002_MainMenu);
                    AudioManager.Instance.FadeInGameTrack(Track._0002_MainMenu);
                    AudioManager.Instance.FadeOutGameTrack(Track._0001_LevelOne);
                    break;

                case (GameState.StartLevel):
                    _audioManager.PlayGameTrack(Track._0001_LevelOne);
                    _audioManager.FadeInGameTrack(Track._0001_LevelOne);
                    _audioManager.FadeOutGameTrack(Track._0002_MainMenu);
                    break;

                case (GameState.LevelWon):
                    break;

                case (GameState.NewLevel):
                    StartCoroutine(LoadWithFade(SceneName.WorldOne));
                    break;

                case (GameState.GameOver):
                    UtilityEvents.RaiseGameReset();
                    StartCoroutine(LoadWithFade(SceneName.GameOver));
                    break;

                case (GameState.Quit):
                    break;
            }

            yield return null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Contains("Level"))
                StartCoroutine(WaitThenChangeState(GameState.StartLevel));

            if (scene.name.Contains(SceneName.MainMenu.ToString()))
                StartCoroutine(SwitchState(GameState.MainMenu));
        }

        private IEnumerator WaitThenChangeState(GameState state)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(Instance.SwitchState(state));
        }

        private IEnumerator LoadWithFade(SceneName sceneName)
        {
            if (FadeCanvas.Instance != null)
                FadeCanvas.Instance.FadeImage.DOFade(1, LoadHelper.LoadDuration);

            yield return new WaitForSeconds(LoadHelper.LoadDuration);
            LoadHelper.LoadSceneWithLoadingScreen(sceneName);
        }

        private void OnGameReset()
        {
            LoadHelper.DeleteSceneKey();
        }

        #endregion
    }

    internal enum GameState
    {
        MainMenu,
        StartLevel,
        LevelWon,
        NewLevel,
        GameOver,
        Quit,
    }
}
