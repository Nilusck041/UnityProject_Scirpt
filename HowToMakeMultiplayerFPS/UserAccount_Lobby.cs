using UnityEngine.UI;
using UnityEngine;

public class UserAccount_Lobby : MonoBehaviour {

    public Text usernameText;

    void Start()
    {
        if(UserAcountManager.IsLoggedIn)
        usernameText.text = "welcome, " + UserAcountManager.PlayerUsername;
    }

    public void LogOut()
    {
        if(UserAcountManager.IsLoggedIn)
        UserAcountManager.instance.LoggedOut();

    }

}
