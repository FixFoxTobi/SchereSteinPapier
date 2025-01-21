using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pv;
using Pv.Unity;
using Leap.PhysicalHands;
using Unity.VisualScripting;

public class AccessKeyInput : MonoBehaviour
{
    public TMP_InputField AccessKey;
    public GameObject uiOverlay_Key;
    public GameObject uiOverlay_Menu;
    public GameObject uiOverlay_Game;
    public TMP_Text statusText;
    public SSPGameplay gameScript;
    public Light screenLight;

    PorcupineManager porcupine;

    private static string basePath = Application.streamingAssetsPath;
    private static List<string> keywordPaths = new List<string>() { basePath + "/Schere-Stein-Papier_de_windows_v3_0_0.ppn" }; // Liste mit Pfad zu allen erkennbaren Keywords/Wakewords
    private string modelPath = basePath + "/porcupine_params_de.pv"; // Pfad zum Deutschsprachigen Model

    void Start()
    {
        uiOverlay_Game.SetActive(false); // Game Overlay ausblenden
        uiOverlay_Menu.SetActive(false); // Menu Overlay ausblenden
    }

    public void ButtonPress()
    {
        try
        {
            porcupine = PorcupineManager.FromKeywordPaths(
                                                    AccessKey.text,
                                                    keywordPaths,
                                                    OnWakeWordDetected,
                                                    modelPath: modelPath);
            if (porcupine != null)
            {
                Debug.Log("Access Key ist gültig!");
                uiOverlay_Key.SetActive(false); // AccessKey Overlay ausblenden
                // noch gegen MenuOverlay austauschen
                uiOverlay_Menu.SetActive(true); // Game Overlay einblenden
                gameScript.StartGame(AccessKey.text);
                screenLight.color = new Color(0.149f, 0.769f, 0.576f);
                screenLight.intensity = 1.5f;
            }
        }
        catch (PorcupineInvalidArgumentException e)
        {
            Debug.LogError($"Ungültiger Access Key: {e.Message}");
            statusText.text = "Access Key ist ungültig!";
            Invoke("TextReset", 2f);
        }
    }

    private void OnWakeWordDetected(int keywordIndex) { }

    private void TextReset() // Text nach abgelaufener Zeit wieder zurücksetzen
    {
        statusText.text = " ";
    }
}
