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
    private const string ACCESS_KEY = "$YOUR_ACCESS_KEY"; // AccessKey obtained from Picovoice Console (https://console.picovoice.ai/)

    private static string basePath = Application.dataPath;
    private static List<string> keywordPaths = new List<string>() { Path.Combine(basePath, "/Assets/PorcupineFiles/Schere-Stein-Papier_de_windows_v3_0_0.ppn") }; // Liste mit Pfad zu allen erkennbaren Keywords/Wakewords

    private string modelPath = Path.Combine(Application.dataPath, "/Assets/PorcupineFiles/porcupine_params_de.pv"); // Pfad zum Deutschsprachigen Model

    PorcupineManager _porcupineManager;

    // Leap
    private Controller leapController;

    // Definition für Gesten
    enum Gesture { Rock, Paper, Scissors, None }
    private Gesture currentGesture = Gesture.None;
    private Gesture botGesture = Gesture.None;

    public Frame frame;

    public TMP_Text gestureText;
    public TMP_Text botText;
    public TMP_Text matchPoints;
    public TMP_Text outputText;

    private int playerPoints = 0;
    private int botPoints = 0;

    private System.Random random = new System.Random(); 

    // Start is called before the first frame update
    void Start()
    {
        leapController = new Controller();

        outputText.text = basePath + ", " + keywordPaths + ", " + modelPath;

        if (gestureText == null)
        {
            Debug.LogError("TMP Text component is not assigned!");
        }
        if (botText == null)
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
        switch (botGesture)
        {
            case Gesture.Rock:
                botText.text = "Rock";
                break;
            case Gesture.Paper:
                botText.text = "Paper";
                break;
            case Gesture.Scissors:
                botText.text = "Scissors";
                break;
        }


        if (currentGesture == botGesture)
        {
            outputText.text = "TIE";
        }
        else if ((currentGesture == Gesture.Rock && botGesture == Gesture.Paper) || (currentGesture == Gesture.Paper && botGesture == Gesture.Scissors) || (currentGesture == Gesture.Scissors && botGesture == Gesture.Rock))
        {
            botPoints++;
            outputText.text = "BOT POINT";
            matchPoints.text = playerPoints + ":" + botPoints;
        }
        else
        {
            playerPoints++;
            outputText.text = "PLAYER POINT";
            matchPoints.text = playerPoints + ":" + botPoints;
        }
        Invoke("TextReset", 1.5f); // Timer mit 1,5 Sekunden
    }

    private void TextReset() // Text nach abgelaufener Zeit wieder zurücksetzen
    {
        gestureText.text = " ";
        botText.text = " ";
    }

    private Gesture GetRandomGesture()
    {
        int randomIndex = random.Next(0, 3); // Generiert eine Zufallszahl zwischen 0 und 2. 0 -> Rock, 1 -> Paper, 2 -> Scissors
        
        return (Gesture)randomIndex; // Gib die entsprechende Geste zurück
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
