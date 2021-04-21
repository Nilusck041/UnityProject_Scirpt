using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserAcountManager : MonoBehaviour
{

    public static UserAcountManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);

    }

    public static string PlayerUsername { get; protected set; }
    private static string PlayerPassword = " ";

    public static bool IsLoggedIn { get; protected set; }


    public string loggedInSceneName = "Lobby";
    public string loggedOutSceneName = "LoginMenu";

    public delegate void OnDataReceivedCallback(string data);



    public void LoggedOut()
    {
        //Called when the player hits the 'Logout' button. Switches back to Login UI and forgets the player's username and password.
        //Note: Database Control doesn't use sessions, so no request to the server is needed here to end a session.
        PlayerUsername = "";
        PlayerPassword = "";

        SceneManager.LoadScene(loggedOutSceneName);
        Debug.Log("User logged out");
        IsLoggedIn = false;


    }
    public void LoggedIn(string _username, string _password)
    {

        PlayerUsername = _username;
        PlayerPassword = _password;

        SceneManager.LoadScene(loggedInSceneName);
        Debug.Log("Logged in as: " + _username);

        IsLoggedIn = true;

    }

    public void LoggedIn_SaveDataButtonPressed(string data)
    {

        StartCoroutine(SetData(data));
    }

    public void LoggedIn_LoadDataButtonPressed(OnDataReceivedCallback onDataReceived)
    {
        StartCoroutine(GetData(onDataReceived));
    }

    IEnumerator GetData(OnDataReceivedCallback onDataReceived)
    {
        string data = "ERROR";
        IEnumerator e = DCF.GetUserData(PlayerUsername, PlayerPassword); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.

            PlayerUsername = "";
            PlayerPassword = "";

            Debug.Log("Error: Unknown Error. Please try again later.");
        }
        else
        {

            data = response;
        }
        if (onDataReceived != null)
            onDataReceived.Invoke(data);
    }

    IEnumerator SetData(string data)
    {
        IEnumerator e = DCF.SetUserData(PlayerUsername, PlayerPassword, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            //The data string was set correctly. Goes back to LoggedIn UI
            Debug.Log("Data string was set corrrectly");
        }

    }


}