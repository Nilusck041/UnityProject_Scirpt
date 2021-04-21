using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> groupDitionary = new Dictionary<string, AudioClip[]>();

    private void Awake()
    {
        foreach (SoundGroup soundGroup in soundGroups)
        {
            groupDitionary.Add(soundGroup.groupID, soundGroup.group);
        }
    }

    public AudioClip GetClipFromName(string name)
    {
        if(groupDitionary.ContainsKey(name))
        {
             AudioClip[] sounds = groupDitionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
    }
}
