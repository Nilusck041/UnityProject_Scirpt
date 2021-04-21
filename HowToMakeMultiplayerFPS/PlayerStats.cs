using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Text KillCount;
    public Text deathCount;


	// Use this for initialization
	void Start () {
        if(UserAcountManager.IsLoggedIn)
            UserAcountManager.instance.LoggedIn_LoadDataButtonPressed(OnReceivedData);
	}

    void OnReceivedData(string data)
    {
        if (KillCount == null || deathCount == null)
            return;

        KillCount.text = DataTranslator.DataToKills(data).ToString() + " KILLS";
        deathCount.text = DataTranslator.DataToDeaths(data).ToString() + " DEATHS";

    }

}
