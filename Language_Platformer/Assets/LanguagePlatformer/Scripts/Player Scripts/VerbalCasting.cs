using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VerbalCasting : MonoBehaviour
{
    // Drop him in the sewer and make him a chud
    KeywordRecognizer recognizer;
    // Runtime modifiable casting info to be put into the _dCommands (apple-y named since directory commands)
    [Serializable] public class CastingInfo
    {
        public string command;
        public int componentID;
    }
    [SerializeField] private List<CastingInfo> commands;
    private Dictionary<string, int> _dCommands;
    [SerializeField] private int[] currentCommands = new int[3];
    private int placement = 0;

    void Start()
    {
        _dCommands = new Dictionary<string, int>();
        foreach (CastingInfo cast in commands)
        {
            _dCommands[cast.command] = cast.componentID;
        }
        currentCommands[2] = currentCommands[1] = currentCommands[0] = -1;
        // commands

        // balls

        recognizer = new KeywordRecognizer(_dCommands.Keys.ToArray(), ConfidenceLevel.Low);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();
    }
    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        int id = -1;
        if (_dCommands.TryGetValue(args.text, out id))
        {
            currentCommands[placement] = id;
            placement++;
            if (placement == 3)
            {
                print("Elements: " + currentCommands[0] + ", " + currentCommands[1] + ", " + currentCommands[2]);
                // Replace the print with a function that is connected to an actual command reading list or smthing, prolly like the directory.
                currentCommands[2] = currentCommands[1] = currentCommands[0] = -1;
                placement = 0; // Israel
            }
        }
    }

    void Update()
    {
        
    }
}
