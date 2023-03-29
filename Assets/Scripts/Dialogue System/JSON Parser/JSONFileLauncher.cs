using Pratik.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONFileLauncher : MonoBehaviour
{
    public string filePath;

    private void Start()
    {
        JSONFileParser.SetFilePath(filePath);
        var text = JSONFileParser.ReadFile();
        var people = JSONFileParser.ReadPeople(text);

        
    }
}
