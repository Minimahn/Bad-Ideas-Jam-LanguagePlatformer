using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

        public Spell(string _spellName, UnityEvent _spellFunction, bool _spellActivated)
        {
            spellName = _spellName;
            spellFunction = _spellFunction;
            spellActivated = _spellActivated;
        }
    }

    [SerializeField] private List<Spell> spells;

    void Start()
    {
        recognizerReset();
    }

    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        // searches for a spell using a lamda expression to specifically
        // compare the names of the spells to the args text
        // (which is what the player says) then using the function
        Spell holder = spells.Find(s => s.spellName == args.text);
        if (holder.spellActivated)
        {
            holder.spellFunction.Invoke();
        }
        // idk why but I felt like I should of explained this one
    }

    private void recognizerReset()
    {
        if (recognizer != null)
        {
            recognizer.OnPhraseRecognized -= OnPhraseRecognized;
            recognizer.Stop();
            recognizer.Dispose();
        }
        string[] spellNames = spells.Select(s => s.spellName).ToArray();
        recognizer = new KeywordRecognizer(spellNames, ConfidenceLevel.Low);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();
    }

    public void AddSpell(Spell spell)
    {
        spells.Add(spell);
        recognizerReset();
    }

    public void AddSpell(string _spellName, UnityEvent _spellFunction, bool _spellActivated)
    {
        spells.Add(new Spell(_spellName, _spellFunction, _spellActivated));
        recognizerReset();
    }

    public void DeleteSpell(string _spellName)
    {
        Spell holder = spells.Find(s => s.spellName == _spellName);
        spells.Remove(holder);
    }

    [Header("Spells and their stuffs")]
    [SerializeField] private GameObject fireball;

    public void PureFire()
    {
        GameObject castPos = GameObject.Find("SSP");
        Instantiate(fireball, castPos.transform.position, castPos.transform.rotation);
    }

    [SerializeField] private GameObject lightball;
    public void Lighten()
    {
        GameObject castPos = GameObject.Find("SSP");
        Instantiate(lightball, castPos.transform.position, castPos.transform.rotation);
    }

}
