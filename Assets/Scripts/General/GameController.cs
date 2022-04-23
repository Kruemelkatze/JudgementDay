using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using General;
using System;

public enum PlayerRequests
{
    NextSubject,
    Sentence
}

public delegate void PlayerRequestDelegate(PlayerRequests request);
public delegate void VoidDelegate();
public class GameController : Singleton<GameController>
{
    public PlayerRequestDelegate onPlayerRequest;
    [SerializeField] private GameState gameState;
    
    [Header("UI")] [SerializeField] private GameObject gameUi;
    [SerializeField] private GameObject pauseUi;
    
    public GameState GameState => gameState;

    private GameState _prePauseState = GameState.Starting;

    public EInterviewState interviewState = EInterviewState.NoSubject;


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
        AudioController.Instance.PlayDefaultMusic();

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
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            switch(interviewState)
            {
                case EInterviewState.NoSubject:
                    onPlayerRequest?.Invoke(PlayerRequests.NextSubject);
                    break;
                case EInterviewState.AwaitingSentenceInput:
                    onPlayerRequest?.Invoke(PlayerRequests.Sentence);
                    break;
            }
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