using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using General;
using System;

public enum PlayerRequests
{
    Login,
    NextSubject,
    Sentence,
    Review,
    SkipReview,
}

public delegate void PlayerRequestDelegate(PlayerRequests request, int value);

public delegate void VoidDelegate();

public class GameController : Singleton<GameController>
{
    public PlayerRequestDelegate onPlayerRequest;
    [SerializeField] private GameState gameState;

    [Header("UI")] [SerializeField] private GameObject gameUi;
    [SerializeField] private GameObject pauseUi;

    public GameState GameState => gameState;

    private GameState _prePauseState = GameState.Starting;

    public EInterviewState interviewState = EInterviewState.Setup;


    private void Awake()
    {
        if (!ThisIsTheSingletonInstance())
        {
            return;
        }

        Hub.Register<GameController>(this);
    }

    private void Start()
    {
        //AudioController.Instance.PlayDefaultMusic();

        gameState = GameState.Starting;

        Hub.Get<Interview>().onInterviewProgression += NotifyInterviewProgression;
        // Do load Stuff
        gameState = GameState.Playing;
    }

    private void NotifyInterviewProgression(SubjectInformation currentSubject, EInterviewState interviewState)
    {
        this.interviewState = interviewState;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetPause(gameState != GameState.Paused);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            switch (interviewState)
            {
                case EInterviewState.NoSubject:
                    //RequestNextSubject();
                    break;
                case EInterviewState.AwaitingSentenceInput:
                    //ChooseSentence(0);
                    break;
                case EInterviewState.Confession:
                    Hub.Get<VideoBackground>().videoPlayer.playbackSpeed = 10.0f;
                    break;
            }
        }
    }

    public void ChooseSentence(int value)
    {
        if (interviewState == EInterviewState.AwaitingSentenceInput)
        {
            onPlayerRequest?.Invoke(PlayerRequests.Sentence, value);
        }
    }

    public void Login()
    {
        onPlayerRequest?.Invoke(PlayerRequests.Login, 0);
    }

    public void RequestSubject()
    {
        onPlayerRequest?.Invoke(PlayerRequests.NextSubject, 0);
    }

    public void RequestNextReview()
    {
        if (interviewState == EInterviewState.ReviewAwaiting || interviewState == EInterviewState.Review)
        {
            onPlayerRequest?.Invoke(PlayerRequests.Review, 0);
        }
    }

    public void RequestSkipReview()
    {
        if (interviewState == EInterviewState.Review)
        {
            onPlayerRequest?.Invoke(PlayerRequests.SkipReview, 0);
        }
    }


    //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PUBLIC  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void PauseGame() => SetPause(true);
    public void ContinueGame() => SetPause(false);

    public void Finished()
    {
        gameState = GameState.Finished;
        Debug.Log("Finished");
    }


    //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PRIVATE  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private void SetPause(bool paused)
    {
        if (paused)
        {
            _prePauseState = gameState;
            gameState = GameState.Paused;
        }
        else
        {
            gameState = _prePauseState;
        }

        if (gameUi)
        {
            gameUi.SetActive(!paused);
        }

        if (pauseUi)
        {
            pauseUi.SetActive(paused);
        }

        // Stopping time depends on your game! Turn-based games maybe don't need this
        Time.timeScale = paused ? 0 : 1;

        // Whatever else there is to do...
        // Deactivate other UI, etc.
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameController))]
    public class GameControlTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var gct = target as GameController;

            if (gct == null)
                return;

            if (!Application.isPlaying)
                return;

            if (GUILayout.Button("Restart"))
            {
                SceneController.Instance.RestartScene();
            }
        }
    }
#endif
}