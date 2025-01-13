using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Pv.Unity;
using TMPro;
using Leap;
using System.IO;

public class SSPGameplay : MonoBehaviour
{
    public TMP_Text AccessKeyText;


    private static string basePath = Application.streamingAssetsPath;

    private static List<string> keywordPaths = new List<string>() { basePath + "/Schere-Stein-Papier_de_windows_v3_0_0.ppn"}; // Liste mit Pfad zu allen erkennbaren Keywords/Wakewords

    private string modelPath = basePath + "/porcupine_params_de.pv"; // Pfad zum Deutschsprachigen Model

    PorcupineManager _porcupineManager;

    // Leap
    private Controller leapController;

    // Definition für Gesten
    enum Gesture { Rock, Paper, Scissors, None }
    private Gesture currentGesture = Gesture.None;
    private Gesture botGesture = Gesture.None;
    public Frame frame;

    // Textfelder
    public TMP_Text gestureText;
    public TMP_Text matchPoints;
    public TMP_Text outputText;
    public TMP_Text debugText;

    // Bildschirm ausgabe
    public Texture[] _imgs;
    public GameObject screen;

    private int playerPoints = 0;
    private int botPoints = 0;
    private int emotionalState = 0;

    private System.Random random = new System.Random(); 

    // StartGane is called after successfully entering an access key
    public void StartGame(string ACCESS_KEY)
    {
        leapController = new Controller();

        if (_imgs == null || screen == null)
        {
            Debug.LogError("Screen and image component not assigned!");
        }
        if (gestureText == null)
        {
            Debug.LogError("TMP Text component is not assigned!");
        }
        if (matchPoints == null)
        {
            Debug.LogError("TMP Text component is not assigned!");
        }
        if (outputText == null)
        {
            Debug.LogError("TMP Text component is not assigned!");
        }

        Renderer planeRenderer = screen.GetComponent<Renderer>();

        if (planeRenderer != null)
        {
            // Textur des Screens auf das Default Bild setzen
            planeRenderer.material.mainTexture = _imgs[1];
        }
        else
        {
            Debug.LogError("Screen has no Renderer!");
        }

        try
        {
            _porcupineManager = PorcupineManager.FromKeywordPaths(
                                                    ACCESS_KEY,
                                                    keywordPaths,
                                                    OnWakeWordDetected,
                                                    modelPath: modelPath);
            _porcupineManager.Start();
        }
        catch (PorcupineInvalidArgumentException ex)
        {
            Debug.LogError($"Porcupine initialization error: {ex.Message}");
        }
        catch (PorcupineActivationException)
        {
            Debug.LogError("AccessKey activation error");
        }
        catch (PorcupineActivationLimitException)
        {
            Debug.LogError("AccessKey reached its device limit");
        }
        catch (PorcupineActivationRefusedException)
        {
            Debug.LogError("AccessKey refused");
        }
        catch (PorcupineActivationThrottledException)
        {
            Debug.LogError("AccessKey has been throttled");
        }
        catch (PorcupineException ex)
        {
            Debug.LogError($"Porcupine initialization error: {ex.Message}");
        }

        SetFace(0); //turn on Screen
    }

    public void SetPorcupineManager(PorcupineManager manager)
    {
        _porcupineManager = manager;
    }

    private void OnWakeWordDetected(int keywordIndex)
    {

        if (keywordIndex == 0)
        {
            frame = leapController.Frame(); // Aktuellen Frame vom LeapMotion Controller erhalten

            // Nach Händen schauen
            foreach (Hand hand in frame.Hands)
            {
                if (hand.IsRight) // Aktuelle wird nur die rechte Hand behandelt (kann später erweitert werden)
                {
                    currentGesture = DetectGesture(hand); 
                    HandleGesture(currentGesture); 
                }
            }
        }
    }

    private Gesture DetectGesture(Hand hand) // Gestenerkennungsfunktion
    {
        int extendedFingers = 0;

        // Zählt Anzahl der Ausgestreckten Finger
        foreach (Finger finger in hand.fingers)
        {
            if (finger.IsExtended)
            {
                extendedFingers++;
            }
        }

        // Erkennung von Geste
        if (extendedFingers == 0)
        {
            return Gesture.Rock;  // Stein (keine Finger extended)
        }
        else if (extendedFingers == 5)
        {
            return Gesture.Paper; // Papier (Alle Finger extended)
        }
        else if (hand.fingers[1].IsExtended && hand.fingers[2].IsExtended && !(hand.fingers[4].IsExtended))
        {
            return Gesture.Scissors; // Zeigefinger und Mittelfinger sind extended und der kleine Finger is nicht extended. (Ring Finger und Daumen wegen Tracking Fehlern ignoriert)
        }

        return Gesture.None; // Keine Geste Erkannt
    }

    // Function to handle the detected gesture
    private void HandleGesture(Gesture gesture)
    {
        // Augabe welche Geste erkannt wurde.
        switch (gesture)
        {
            case Gesture.Rock:
                gestureText.text = "Rock";
                Debug.Log("Rock detected!");
                break;
            case Gesture.Paper:
                gestureText.text = "Paper";
                Debug.Log("Paper detected!");
                break;
            case Gesture.Scissors:
                gestureText.text = "Scissors";
                Debug.Log("Scissors detected!");
                break;
            case Gesture.None:
                gestureText.text = " ";
                return;
        }

        botGesture = GetRandomGesture();

        if (currentGesture == botGesture)
        {
            outputText.text = "TIE";
            StartCoroutine(DisplayBotChoice(botGesture, 0));
        }
        else if ((currentGesture == Gesture.Rock && botGesture == Gesture.Paper) || (currentGesture == Gesture.Paper && botGesture == Gesture.Scissors) || (currentGesture == Gesture.Scissors && botGesture == Gesture.Rock))
        {
            botPoints++;
            outputText.text = "BOT POINT";
            matchPoints.text = playerPoints + ":" + botPoints;
            emotionalState++;
            StartCoroutine(DisplayBotChoice(botGesture, 1));
        }
        else
        {
            playerPoints++;
            outputText.text = "PLAYER POINT";
            matchPoints.text = playerPoints + ":" + botPoints;
            emotionalState--;
            StartCoroutine(DisplayBotChoice(botGesture, -1));
        }
        Invoke("TextReset", 1.5f); // Timer mit 1,5 Sekunden
    }

    private void TextReset() // Text nach abgelaufener Zeit wieder zurücksetzen
    {
        gestureText.text = " ";
        outputText.text = " ";
    }

    private Gesture GetRandomGesture()
    {
        int randomIndex = random.Next(0, 3); // Generiert eine Zufallszahl zwischen 0 und 2. 0 -> Rock, 1 -> Paper, 2 -> Scissors
        
        return (Gesture)randomIndex; // Gib die entsprechende Geste zurück
    }

    private IEnumerator DisplayBotChoice(Gesture gesture, int botWin)
    {
        switch (gesture)
        {
            case Gesture.Rock:
                SetFace(5);
                break;
            case Gesture.Paper:
                SetFace(6);
                break;
            case Gesture.Scissors:
                SetFace(4);
                break;
        }

        yield return new WaitForSeconds(1.5f);

        // Check ob es einen gewinner gibt
        wincheck();

        // setzte das gesicht je nachdem wie es steht
        if (botWin == 1)
        {
            SetFace(1); // glücklich
        }
        else if (botWin == -1)
        {
            switch(emotionalState)
            {
                case < -1:
                    SetFace(3); // wütend
                    break;
                case 0:
                    SetFace(2); // neutral
                    break;
                case -1:
                    SetFace(2); // neutral
                    break;
                default: 
                    SetFace(1); // glücklich
                    break;
            }
        }
        else
        {
            SetFace(2); // neutral
        }
    }

    private void SetFace(int FaceNr)
    {
        Renderer planeRenderer = screen.GetComponent<Renderer>();
        planeRenderer.material.mainTexture = _imgs[FaceNr];
    }

    // Aktuell nur Best of 5
    private void wincheck()
    {
        if (botPoints == 3)
        {
            matchPoints.text = "BOT WIN";
            _porcupineManager.Stop();
        }
        else if (playerPoints == 3)
        {
            matchPoints.text = "PLAYER WIN";
            _porcupineManager.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
