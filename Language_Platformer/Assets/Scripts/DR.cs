using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;

public class DR : MonoBehaviour
{
    DictationRecognizer recognizer;
    [SerializeField] TextMeshProUGUI _textR;
    [SerializeField] TextMeshProUGUI _textH;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        recognizer = new DictationRecognizer(ConfidenceLevel.Low);
        recognizer.DictationResult += Recognizer_DictationResult;
        recognizer.DictationHypothesis += Recognizer_DictationHypothesis;
        recognizer.DictationComplete += Recognizer_DictationComplete;
        recognizer.Start();

        PhraseRecognitionSystem.Restart();

        // Doesn't initially work but works afterwards?
        // Test to see if it works in the morning when I can yell
    }

    private void Recognizer_DictationComplete(DictationCompletionCause cause)
    {
        if (cause != DictationCompletionCause.Complete)
        {
            print("Audio Recording Stopped cuz: " + cause);
        }
        if (cause == DictationCompletionCause.Canceled || cause == DictationCompletionCause.TimeoutExceeded)
        {
            // Was lazy, set this to a bool in update that checks if app is maximized then turns it back on or smthin
            Invoke("restart", 0.5f);
        }
    }

    private void Recognizer_DictationHypothesis(string text)
    {
        _textH.text = text;
    }

    private void Recognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        _textR.text = text;
    }

    private void restart()
    {
        recognizer.DictationResult -= Recognizer_DictationResult;
        recognizer.DictationHypothesis -= Recognizer_DictationHypothesis;
        recognizer.DictationComplete -= Recognizer_DictationComplete;
        recognizer.Dispose();
        recognizer = null;

        if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            PhraseRecognitionSystem.Shutdown();

        recognizer = new DictationRecognizer(ConfidenceLevel.Low);
        recognizer.DictationResult += Recognizer_DictationResult;
        recognizer.DictationHypothesis += Recognizer_DictationHypothesis;
        recognizer.DictationComplete += Recognizer_DictationComplete;
        recognizer.Start();

        PhraseRecognitionSystem.Restart();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
