using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardItem;

    [SerializeField]
    Transform playerScoreboardList;

     void OnEnable()
    {
        // Get an array players
        Player[] players = GameManager.GetAllPlayers();

        foreach(Player player in players)
        {
           GameObject itemGO  =  (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if(item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }



    }

     void OnDisable()
    {
        // Clean up our list of items
        foreach(Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }

}
