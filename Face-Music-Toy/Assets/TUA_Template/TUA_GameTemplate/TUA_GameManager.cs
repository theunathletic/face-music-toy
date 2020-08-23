using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class TUA_GameManager : MonoBehaviour
{
    //-------------------------------------------------------------------------------------------------------------------------
    enum EGameState
    {
        Setup,
        Play,
        Paused,
        GameOver,
    };

    //-------------------------------------------------------------------------------------------------------------------------
    public GameObject AppPrefab;

    //-------------------------------------------------------------------------------------------------------------------------
    private EGameState StateId;
    private EGameState previousStateId;
    //-------------------------------------------------------------------------------------------------------------------------
    private float setupTimer = 0.1f; //need a slight delay on setup to make the screen transition work.
    private float current_setupTimer;

    //-------------------------------------------------------------------------------------------------------------------------
    public static TUA_GameManager Instance { get; private set; } = null;

    void Awake()
    {
        // Is the controlling app already in existance?
        if (GameObject.Find("App") == null)
        {
            Debug.Log("Creating temporary TUA_App");

            Instantiate(AppPrefab);
        }

        if (Instance != null)
        {
            Debug.LogError("Multiple TUA_GameManager Instances exist!");
        }

        Instance = this;

        // Disable screen dimming
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }

    //-------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        Game_Setup();

    }

    //-------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        switch (StateId)
        {

            case EGameState.Setup:
                current_setupTimer -= Time.deltaTime;
                if (current_setupTimer < 0.0f)
                {
                    Game_Play();
                }
                break;

            case EGameState.Play:
                break;

            case EGameState.Paused:
                break;

            case EGameState.GameOver:
                break;
        }
    }

    void SwitchState(EGameState nextState)
    {
        previousStateId = StateId;
        StateId = nextState;
        print("TUA_GameManager: - SwitchState: " + StateId);

        switch (StateId)
        {
            //Do any game setup here. Spawn player and level etc.
            case EGameState.Setup:
                current_setupTimer = setupTimer;
                TUA_ScreenManager.Instance.OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_setup"), TUA_ScreenManager.ScreenTranstitionType.Instant, 0f);
               
                break;

            case EGameState.Play:
                Time.timeScale = 1f;
                
                TUA_ScreenManager.Instance.CloseCurrent(TUA_ScreenManager.ScreenTranstitionType.Fade, 2.0f);
                break;

            case EGameState.Paused:
                Time.timeScale = 0f;

                TUA_ScreenManager.Instance.OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_paused"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);

                break;

            case EGameState.GameOver:
                TUA_ScreenManager.Instance.OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_gameover"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
                break;

            default:
                Debug.LogError("Really shouldn't be here... illegal state id set.");

                // Auto recover.
                SwitchState(EGameState.Setup);
                break;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    //Begin the screen fade, possibly a countdown.
    public void Game_Setup()
    {
        SwitchState(EGameState.Setup);
    }

    //When control is given to the player.
    public void Game_Play()
    {
        SwitchState(EGameState.Play);
    }

    //Time is up or score is reached. Displays gameover screen. Could also be the end of asingle round.
    public void Game_GameOver()
    {
        SwitchState(EGameState.GameOver);
    }

    //Do any special resets here before moving back to game setup
    public void Game_Restart()
    {
        TUA_ScreenManager.Instance.CloseCurrent(TUA_ScreenManager.ScreenTranstitionType.Instant, 2.0f);
        SwitchState(EGameState.Setup);
    }

    public void Game_ExitToMainMenu()
    {
        TUA_App.Instance.LoadMainMenu();
    }

    public void Game_Pause()
    {
        SwitchState(EGameState.Paused);
    }

    public void Game_Pause_Resume()
    {
        SwitchState(EGameState.Play);
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public void Game_UI_GameOver()
    {
        TUA_AudioManager.Instance.PlayButtonSound();
        Game_GameOver();
    }

    //Do any special resets here before moving back to game setup
    public void Game_UI_Restart()
    {
        TUA_AudioManager.Instance.PlayButtonSound();
        Game_Restart();
    }

    public void Game_UI_ExitToMainMenu()
    {
        TUA_AudioManager.Instance.PlayButtonSound();
        Game_ExitToMainMenu();
    }

    public void Game_UI_Pause()
    {
        TUA_AudioManager.Instance.PlayButtonSound();
        Game_Pause();
    }

    public void Game_UI_Pause_Resume()
    {
        TUA_AudioManager.Instance.PlayButtonSound();
        Game_Pause_Resume();
    }
    //-------------------------------------------------------------------------------------------------------------------------

}
