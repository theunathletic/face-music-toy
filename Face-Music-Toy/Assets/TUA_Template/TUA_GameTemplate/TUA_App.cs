using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine.SceneManagement;

public class TUA_App : MonoBehaviour
{
	//-------------------------------------------------------------------------------------------------------------------------
	public	String						gameTitle = "Game Name";
	public	bool						displayedLogo = false;

	//-------------------------------------------------------------------------------------------------------------------------
	private string						currentScene;

	//-------------------------------------------------------------------------------------------------------------------------
	public static string				mainMenuScene	= "MainMenu";
	public static string				mainGameScene	= "MainGame";

    //-------------------------------------------------------------------------------------------------------------------------
    public static TUA_App Instance { get; private set; } = null;

    //-------------------------------------------------------------------------------------------------------------------------
    void Awake()
	{
		Debug.Log("TUA_App: Awake.");

		if (Instance != null )
		{
			Debug.LogError("Multiple TUA_App Singletons exist!");
		}

        Instance = this;

		// Are we in the Application Scene?
		if (SceneManager.GetActiveScene().name == "Application" )
		{
			// Make sure this object persists between scene loads.
			DontDestroyOnLoad(gameObject);

			LoadMainMenu();
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	void Start ()
	{
	}
	
	//-------------------------------------------------------------------------------------------------------------------------
	void Update ()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			Quit();
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	public void Quit()
	{
		Debug.Log("TUA_App: Quit.");

		Application.Quit();
	}

	//-------------------------------------------------------------------------------------------------------------------------
	public void LoadMainMenu()
	{
		Debug.Log("TUA_App: Loading MainMenu.");
        LoadScene( mainMenuScene);
	}

	//-------------------------------------------------------------------------------------------------------------------------
	public void LoadScene(string scene )
	{
		Debug.Log("TUA_App: Loading Scene, " + scene);

		currentScene = scene;
        
        SceneManager.LoadScene(currentScene);
	}

	//-------------------------------------------------------------------------------------------------------------------------
	public void ResetLevel()
	{
		Debug.Log("TUA_App: Restarting scene, " + currentScene );

        SceneManager.LoadScene(currentScene);
    }

}

