using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

public class VerbalCasting : MonoBehaviour
{
    // Drop him in the sewer and make him a chud
    KeywordRecognizer recognizer;
    // Runtime modifiable casting info to be put into the _dCommands (apple-y named since directory commands)
    [Serializable] public class Spell
    {
        public string spellName;
        public UnityEvent spellFunction;
        public bool spellActivated;
        // public int spellCost;
    }

    [SerializeField] private List<Spell> spells;

    void Start()
    {
        // commands
        string[] spellNames = spells.Select(s => s.spellName).ToArray();
        // balls
        recognizer = new KeywordRecognizer(spellNames, ConfidenceLevel.Low);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();
    }
    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        // searches for a spell using a lamda expression to specifically
        // compare the names of the spells to the args text
        // (which is what the player says) then using the function
        spells.Find(s => s.spellName == args.text).spellFunction.Invoke();
        // idk why but I felt like I should of explained this one
    }

    [Header("Spells and their stuffs")]
    [SerializeField] private GameObject fireball;

    public void PureEarth()
    {
        print("Rocky goodness");
    }
    public void PureWater()
    {
        print("Too much water infact");
    }
    public void PureFire()
    {
        GameObject castPos = GameObject.Find("SSP");
        Instantiate(fireball, castPos.transform.position, castPos.transform.rotation);
    }

}
