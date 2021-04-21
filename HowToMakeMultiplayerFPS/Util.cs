//doing stuff like set layer of all gameobject in hierarchy
using UnityEngine;

public class Util  {

    public static void SetLayerRecursively(GameObject _obj,int _newLayer)
    {
        if (_obj == null)
            return;

        _obj.layer = _newLayer;

        foreach(Transform _child in _obj.transform)
        {
            if (_child == null)
                continue;

            SetLayerRecursively(_child.gameObject, _newLayer);
        }


    }

}
