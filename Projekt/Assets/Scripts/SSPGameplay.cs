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
using Leap.Attributes;

public class SSPGameplay : MonoBehaviour
{
    public TMP_Text AccessKeyText;


    private static string basePath = Application.streamingAssetsPath;

    private static List<string> keywordPaths = new List<string>() { basePath + "/Schere-Stein-Papier_de_windows_v3_0_0.ppn", basePath + "/Spielen_de_windows_v3_0_0.ppn", basePath + "/Spiel-pausieren_de_windows_v3_0_0.ppn", basePath + "/Spiel-beenden_de_windows_v3_0_0.ppn",
    basePath + "/nochmal-spielen_de_windows_v3_0_0.ppn", basePath + "/weiter-spielen_de_windows_v3_0_0.ppn", basePath + "/Schnick-Schnack-Schnuck_de_windows_v3_0_0.ppn" }; // Liste mit Pfad zu allen erkennbaren Keywords/Wakewords

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
    public TMP_Text statisticHeaderText;
    public TMP_Text statisticText;
    public TMP_Text nochmalText;
    public TMP_Text weiterText;
    public TMP_Text pauseText;

    // Bildschirm ausgabe
    public Texture[] _imgs;
    public GameObject screen;

    // UI Overlays
    public GameObject uiOverlay_Menu;
    public GameObject uiOverlay_Game;

    private int playerPoints = 0;
    private int playerScissors = 0;
    private int playerRock = 0;
    private int playerPaper = 0;
    private int botPoints = 0;
    private int botScissors = 0;
    private int botRock = 0;
    private int botPaper = 0;
    private int tieCounter = 0;
    private int emotionalState = 0;

    private bool isLeftHand = false;
    public Toggle leftHandToggle;
    private bool isPaused = true;
    private int gameState = 0;

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
            Debug.LogError("TMP Text component gestureText is not assigned!");
        }
        if (matchPoints == null)
        {
            Debug.LogError("TMP Text component matchPoints is not assigned!");
        }
        if (statisticHeaderText == null)
        {
            Debug.LogError("TMP Text component statisticHeaderTex is not assigned!");
        }
        if (statisticText == null)
        {
            Debug.LogError("TMP Text component statisticText is not assigned!");
        }
        if (nochmalText == null)
        {
            Debug.LogError("TMP Text component nochmalText is not assigned!");
        }
        if (weiterText == null)
        {
            Debug.LogError("TMP Text component weiterText is not assigned!");
        }
        if (pauseText == null)
        {
            Debug.LogError("TMP Text component pauseText is not assigned!");
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

        if (leftHandToggle != null)
        {
            isLeftHand = leftHandToggle.isOn;
        }

        SetFace(0); //turn on Screen
    }

    public void SetPorcupineManager(PorcupineManager manager)
    {
        _porcupineManager = manager;
    }

    private void OnWakeWordDetected(int keywordIndex)
    {
        switch (keywordIndex)
        {
            case 0: // Schere Stein Papier eingabe
            case 6:
                if (!isPaused && (gameState == 0 || gameState == 2))
                {
                    frame = leapController.Frame(); // Aktuellen Frame vom LeapMotion Controller erhalten

                    // Nach Händen schauen
                    foreach (Hand hand in frame.Hands)
                    {
                        if (isLeftHand) // Welche Hand je nach modus benutzt wird.
                        {
                            if (hand.IsLeft) // Aktuelle wird nur die rechte Hand behandelt (kann später erweitert werden)
                            {
                                currentGesture = DetectGesture(hand);
                                HandleGesture(currentGesture);
                            }
                        }
                        else
                        {
                            if (hand.IsRight) // Aktuelle wird nur die rechte Hand behandelt (kann später erweitert werden)
                            {
                                currentGesture = DetectGesture(hand);
                                HandleGesture(currentGesture);
                            }
                        }
                    }
                }
                break;
            case 1: // Spielen eingabe
                if (gameState == 0 || gameState == 2)
                {
                    uiOverlay_Game.SetActive(true);
                    uiOverlay_Menu.SetActive(false);
                    isPaused = false;
                }
                break;
            case 2: // Spiel pausieren eingabe
                if (gameState == 0 || gameState == 2)
                {
                    statisticHeaderText.text = "STATISTIKEN";
                    UpdateStatistics();
                    uiOverlay_Game.SetActive(false);
                    uiOverlay_Menu.SetActive(true);
                    isPaused = true;
                }
                break;
            case 3: // Spiel beenden eingabe
                if (isPaused || gameState == 1) { QuitGame(); }
                break;
            case 4: // nochmal spielen
                if (gameState == 1)
                {
                    playerPoints = 0;
                    playerScissors = 0;
                    playerRock = 0;
                    playerPaper = 0;
                    botPoints = 0;
                    botScissors = 0;
                    botRock = 0;
                    botPaper = 0;
                    tieCounter = 0;
                    emotionalState = 0;
                    gameState = 0;

                    SetFace(0);

                    matchPoints.text = playerPoints + ":" + botPoints;
                    nochmalText.text = "";
                    weiterText.text = "";
                    pauseText.text = "SPIEL PAUSIEREN";
                }
                break;
            case 5: // weiter spielen
                if (gameState == 1)
                {
                    gameState = 2;

                    matchPoints.text = playerPoints + ":" + botPoints;
                    nochmalText.text = "";
                    weiterText.text = "";
                    pauseText.text = "SPIEL PAUSIEREN";

                }
                break;
            default: // Sollte nicht vorkommen
                break;
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
                gestureText.text = "Stein";
                playerRock++;
                Debug.Log("Rock detected!");
                break;
            case Gesture.Paper:
                gestureText.text = "Papier";
                playerPaper++;
                Debug.Log("Paper detected!");
                break;
            case Gesture.Scissors:
                gestureText.text = "Schere";
                playerScissors++;
                Debug.Log("Scissors detected!");
                break;
            case Gesture.None:
                gestureText.text = " ";
                return;
        }

        botGesture = GetRandomGesture(); // Zufällige Bot Geste wählen

        // Bot Gesten Zähler erhöhen
        if (botGesture == Gesture.Rock) { botRock++; }
        else if (botGesture == Gesture.Paper) { botPaper++; }
        else if (botGesture == Gesture.Scissors) { botScissors++; }

        // Checken wer gewonnen hat
        if (currentGesture == botGesture)
        {
            tieCounter++;
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
        if (gameState == 0) { wincheck(); }

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
            nochmalText.text = "NOCHMAL SPIELEN";
            weiterText.text = "WEITER SPIELEN";
            pauseText.text = "SPIEL\nBEENDEN";
            gameState = 1; // gameState setzten für Overlay
        }
        else if (playerPoints == 3)
        {
            matchPoints.text = "PLAYER WIN";
            nochmalText.text = "NOCHMAL SPIELEN";
            weiterText.text = "WEITER SPIELEN";
            pauseText.text = "SPIEL\nBEENDEN";
            gameState = 1; // gameState setzten für Overlay
        }
    }

    private void UpdateStatistics()
    {
        // Anzahl der Runden
        int numRounds = playerPoints + botPoints + tieCounter;

        // Meistgespielte Hand des Spielers
        string playerFav;
        int[] playerValues = { playerRock, playerPaper, playerScissors };
        int maxPlayerValue = playerValues.Max();
        int countPlayerMax = playerValues.Count(v => v == maxPlayerValue);

        if (countPlayerMax == 3)
        {
            playerFav = "Alle gleich";
        }
        else if (countPlayerMax == 2)
        {
            playerFav = MaxValuesToString(playerValues, maxPlayerValue);
        }
        else
        {
            playerFav = MaxValuesToString(playerValues, maxPlayerValue);
        }

        // Meistgespielte Hand des Computers
        string botFav;
        int[] botValues = { botRock, botPaper, botScissors };
        int maxBotValue = botValues.Max();
        int countBotMax = botValues.Count(v => v == maxBotValue);

        if (countBotMax == 3)
        {
            botFav = "Alle gleich";
        }
        else if (countBotMax == 2)
        {
            botFav = MaxValuesToString(botValues, maxBotValue);
        }
        else
        {
            botFav = MaxValuesToString(botValues, maxBotValue);
        }

        statisticText.text = 
            "Gespielte Runden:      " + numRounds + "\n"
            + "Spielstand (S:B):      " + playerPoints + ":" + botPoints + "\n"
            + "Meistgespielt Spieler: " + playerFav + "\n"
            + "Meistgespielt Bot:     " + botFav + "\n"
            + "Unentschieden:         " + tieCounter;

    }

    static string MaxValuesToString(int[] values, int maxValue) // Meistgepielte Hand
    {
        // Namen der Werte
        string[] names = { "Stein", "Papier", "Schere" };

        // Finde die Namen der größten Werte
        return string.Join(" ", names.Where((name, index) => values[index] == maxValue));
    }

    public void QuitGame()
    {
        // Im Editor wird das Spiel nicht beendet
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        
        // Beendet das Spiel in Build-Version
        #else
            Application.Quit(); // Beendet das Spiel in einer Build-Version
        #endif

        Debug.Log("Spiel wird beendet.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}