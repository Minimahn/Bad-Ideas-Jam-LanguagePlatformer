using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    [Serializable] public class SpellInfo
    {
        public string spellName;
        public int[] commandRequirement = new int[3];
        public Action spellFunction;

        public SpellInfo(string SN, int[] CREQ, Action SF)
        {
            spellName = SN;
            commandRequirement = CREQ;
            spellFunction = SF;
        }
    }

    [SerializeField] private List<SpellInfo> spellList;
    [SerializeField] private List<CastingInfo> commands;
    private Dictionary<string, int> _dCommands;
    [SerializeField] private int[] currentCommands = new int[3];
    private int _placement = 0;

    void Start()
    {
        _dCommands = new Dictionary<string, int>();
        foreach (CastingInfo cast in commands)
        {
            _dCommands[cast.command] = cast.componentID;
        }
        currentCommands[2] = currentCommands[1] = currentCommands[0] = -1;

        // commands
        SpellInfo first = new SpellInfo("Rock n' Roll", new int[] { 2,2,2 }, PureEarth);
        spellList.Add(first);
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
            currentCommands[_placement] = id;
            _placement++;
            if (_placement == 3)
            {
                print("Elements: " + currentCommands[0] + ", " + currentCommands[1] + ", " + currentCommands[2]);
                SpellSearch(); // Spell search
                currentCommands[2] = currentCommands[1] = currentCommands[0] = -1;
                _placement = 0; // Israel
            }
        }
    }

    void SpellSearch()
    {
        foreach (SpellInfo spell in spellList)
        {
            if (spell.commandRequirement.SequenceEqual(currentCommands))
            {
                print("Casting " + spell.spellName);
                spell.spellFunction();
            }
        }
    }

    void PureEarth()
    {
        print("Rocky goodness");
    }
    void PureWater()
    {
        print("Too much water infact");
    }
    void PureFire()
    {
        print("You need to sound zesty to make this work");
    }

}
