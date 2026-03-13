using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections.Generic;
using System.Linq;

public class KWR : MonoBehaviour
{
    KeywordRecognizer recognizer;
    Dictionary<string, System.Action> commands;

    void Start()
    {
        commands = new Dictionary<string, System.Action>();

        commands.Add("the avid jump into mulch", uno);
        commands.Add("goblin", dos);
        commands.Add("fork", tres);
        commands.Add("chud", null);

        recognizer = new KeywordRecognizer(commands.Keys.ToArray(), ConfidenceLevel.Low);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;

        recognizer.Start();
    }

    void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action action;
        if (commands.TryGetValue(args.text, out action))
        {
            if (action != null)
                action.Invoke();
        }
    }

    void uno()
    {
        Debug.Log("Test activated");
    }

    void dos()
    {
        Debug.Log("Goblin activated");
    }

    void tres()
    {
        Debug.Log("Fork activated");
    }
}