using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killfeed : MonoBehaviour {

    [SerializeField]
    GameObject killfeedItemPrefab;

	// Use this for initialization
	void Start () {
        GameManager.instance.onPlayerKilledCallBack += OnKill;	
	}

    public void OnKill(string player ,string source)
    {
        GameObject go = (GameObject)Instantiate(killfeedItemPrefab, this.transform);
        go.GetComponent<KillfeedItem>().Setup(player, source);

        Destroy(go, 4f);
    }

}
