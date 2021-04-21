using UnityEngine.UI;
using UnityEngine;

public class KillfeedItem : MonoBehaviour {

    [SerializeField]
    Text text;

    public void Setup(string player, string source)
    {
        text.text = "<b>" + source + "</b>" + " killed " + "<i>" + player + "</i>";
    }

}
