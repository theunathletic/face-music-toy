using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//[RequireComponent(typeof(ScreenFade))]

public class TUA_MainMenu : MonoBehaviour
{
    //-------------------------------------------------------------------------------------------------------------------------
    enum EMainMenuState
    {
        Startup,
        DisplayLogo,
        Menu,
    };

    //-------------------------------------------------------------------------------------------------------------------------
    public GameObject AppPrefab;

    //-------------------------------------------------------------------------------------------------------------------------
    private EMainMenuState StateId;

    //------------------------------------------------------------------------------------------------------------------------
    public GameObject logoGO;
    public bool showCustomLogo;
    public float logoDisplayTime;
    private float logoDisplayTimer;

    //-------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        Debug.Log("TUA_MainMenu: Starting.");

        // Is the controlling App already in existence?
        if (GameObject.Find("App") == null)
        {
            Debug.Log("Creating temporary TUA_App");

            Instantiate(AppPrefab);
        }

        //StateId = EMainMenuState.Startup;
        SwitchState(EMainMenuState.Startup);

    }

    //-------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        switch (StateId)
        {
            case EMainMenuState.Startup:
                Debug.Log("TUA_MainMenu: State: Startup.");

                // Here we would do any menu preparation work.


               
                break;

            case EMainMenuState.DisplayLogo:
                logoDisplayTimer -= Time.deltaTime;

                if (logoDisplayTimer < 0.0f)
                {
                    //mf_screenManager.instance.OpenPanel(mf_screenManager.instance.screens[1], 4, 2.0f);
                    TUA_ScreenManager.Instance.CloseCurrent(TUA_ScreenManager.ScreenTranstitionType.Fade, 2.0f);
                   // TUA_ScreenManager.Instance.OpenPanel(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_menu"),TUA_ScreenManager.ScreenTranstitionType.Fade,2.0f);
                    SwitchState(EMainMenuState.Menu);
                }
                break;

            case EMainMenuState.Menu:

                break;

            default:
                Debug.LogError("Really shouldn't be here... illegal state id set.");

                // Auto recover.
                SwitchState(EMainMenuState.Startup);
                break;
        }
    }

    void SwitchState(EMainMenuState nextState)
    {
        StateId = nextState;
        print("TUA_MainMenu: - SwitchState: " + StateId);
        switch (StateId)
        {

            case EMainMenuState.Startup:
                if (showCustomLogo)
                {
                    // Has the logo been displayed already?
                    if (TUA_App.Instance.displayedLogo)
                    {
                        // Move to the next state.
                        //mf_screenManager.instance.OpenPanel(mf_screenManager.instance.screens[1], 5, 2.0f);
                        //TUA_ScreenManager.Instance.OpenPanel(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_menu"),TUA_ScreenManager.ScreenTranstitionType.Instant,0f);
                        SwitchState(EMainMenuState.Menu);
                    }
                    else
                    {
                        TUA_ScreenManager.Instance.OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_logo"),TUA_ScreenManager.ScreenTranstitionType.Instant,0f);
                        SwitchState(EMainMenuState.DisplayLogo);
                    }
                }
                else
                {
                    //TUA_ScreenManager.Instance.OpenPanel(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_menu"), TUA_ScreenManager.ScreenTranstitionType.Instant, 0f);                                                                                
                    SwitchState(EMainMenuState.Menu);
                }
                break;

            case EMainMenuState.DisplayLogo:
                //logoGO.SetActive(true);
               
                logoDisplayTimer = logoDisplayTime;
                TUA_App.Instance.displayedLogo = true;
                break;

            case EMainMenuState.Menu:
                //logoGO.SetActive(false);
                break;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public void MainMenu_UI_Quit()
    {
        Debug.Log("TUA_MainMenu: Button - Quit");
        TUA_App.Instance.Quit();
    }

    public void MainMenu_UI_StartGame()
    {
        Debug.Log("TUA_MainMenu: Button - StartGame.");
        TUA_AudioManager.Instance.PlayButtonSound();
        TUA_App.Instance.LoadScene(TUA_App.mainGameScene);
    }

    public void MainMenu_UI_Options_Open()
    {
        Debug.Log("TUA_MainMenu: Button - Optiones Open.");
        TUA_AudioManager.Instance.PlayButtonSound();
        TUA_ScreenManager.Instance.OpenPanelOverTop(TUA_ScreenManager.Instance.GetScreenAnimByName("screen_options"), TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);


    }

    public void MainMenu_UI_Options_Close()
    {
        Debug.Log("TUA_MainMenu: Button - Options Close.");
        TUA_AudioManager.Instance.PlayButtonSound();
        TUA_ScreenManager.Instance.CloseCurrent(TUA_ScreenManager.ScreenTranstitionType.Fade, 2f);
    }




}
