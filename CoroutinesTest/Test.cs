using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform[] path;
    IEnumerator currentMoveCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        string[] messages = { "Welcome", "to", "this", "amazing", "game" };
        StartCoroutine(PrintMessage(messages, .5f));
        StartCoroutine(FollowPath());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(currentMoveCoroutine != null)
            {
                StopCoroutine(currentMoveCoroutine);                      //先停止上一个coroutine防止系统卡死
            }
            currentMoveCoroutine = Move(Random.onUnitSphere * 5, 8);
            StartCoroutine(currentMoveCoroutine);
        }
    }

    IEnumerator FollowPath()
    {
        foreach(Transform wayPoint in path)
        {
            currentMoveCoroutine = Move(wayPoint.position, 8);
           yield return StartCoroutine(Move(wayPoint.position, 8));
        }
    }

    IEnumerator Move(Vector3 destination, float speed)
    {
        while(transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }

   IEnumerator PrintMessage(string[] messages, float delay)
    {
        foreach(string msg in messages)
        {
            print(msg);
            yield return new WaitForSeconds(delay);
        }
    }
}
