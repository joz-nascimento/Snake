using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHUD : MonoBehaviour
{
    public GameObject groupUi;
    public List<GameObject> playersInfoList;
    public List<KeyColor> keyColors;
    Canvas myCanvas;
    GameObject myGO;
    int index = 0;
    public List<KeyCode> ellegibleKeys;
    public List<KeyCode> availableKeys;
    public List<KeyCode> playerKeys;
    public List<Color> playerColors;
    public List<Color> availableColors;
    public Color currentColor;
    public int indexColor;

    public Text UIText;

    public float downTime, upTime, pressTime = 0;
    public float countDown = 1.0f;
    public bool ready = false;
    public Board gameboard;

    string textStart = "Press any key to start....";
    string textInitial = "Press two keys for 1 sec\nto enter in the game";
    string textNext = "Press ENTER to start game\nor press another two keys to another player";
    string textGameOver = "Game Over\n\nPress any key to start screen...";

    enum screens {
        Start,
        FirstPlayer,
        AnotherPlayer,
        RegisteredPlayer,
        InGame,
        GameOver,
    }

    screens currentScreen = screens.FirstPlayer;


    void Start() {
        playersInfoList = new List<GameObject>();
        ListBasicColors();
        indexColor = 0;
        currentColor = availableColors[indexColor];

        // Canvas
        myGO = new GameObject();
        myGO.name = "Canvas";
        myGO.AddComponent<Canvas>();

        myCanvas = myGO.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        myGO.AddComponent<CanvasScaler>();
        myGO.AddComponent<GraphicRaycaster>();

        UIText = CreateText();
        ellegibleKeys = new List<KeyCode> { KeyCode.Space, KeyCode.Space };
        availableKeys = new List<KeyCode>();
        keyColors = new List<KeyColor>();
        playerKeys = new List<KeyCode>();
        string keys = "1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
        for (int i = 0; i < keys.Length; i++) {
            string key = keys[i].ToString();
            if (i < 10) {
                key = "Alpha" + key;
            }
            KeyCode thisKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);
            availableKeys.Add(thisKeyCode);
        }

    }

    Text CreateText() {
        GameObject GOText;
        RectTransform rectTransform;
        Text Mytext;

        // Text
        GOText = new GameObject("Main Text");
        GOText.transform.SetParent(myGO.transform);

        Mytext = GOText.AddComponent<Text>();
        Mytext.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        Mytext.horizontalOverflow = HorizontalWrapMode.Overflow;
        Mytext.verticalOverflow = VerticalWrapMode.Overflow;
        Mytext.alignment = TextAnchor.MiddleCenter;
        Mytext.fontSize = 30;

        // Text position
        rectTransform = Mytext.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.sizeDelta = new Vector2(400, 200);
        Mytext.color = availableColors[1];
        return Mytext;
    }

    void Update() {
        switch (currentScreen) {
            case screens.FirstPlayer:
                UIText.text = textInitial;
                ScanKeys();
                break;
            case screens.AnotherPlayer:
                UIText.text = textNext;
                if (Input.GetKeyDown(KeyCode.Return)) {
                    UIText.enabled = false;
                    currentScreen = screens.InGame;
                    //gameboard.StartGame(playerKeys);
                    gameboard.StartGame(keyColors);
                }
                ScanKeys();
                break;
            case screens.RegisteredPlayer:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    KeyColor temp = new KeyColor();
                    temp.Setup(ellegibleKeys[0], ellegibleKeys[1], currentColor);
                    keyColors.Add(temp);
                    currentColor = availableColors[0];
                    currentScreen = screens.AnotherPlayer;
                }
                if (Input.GetKeyDown(ellegibleKeys[0])) {
                    indexColor -= 1;
                    if (indexColor < 0) {
                        indexColor = availableColors.Count - 1;
                    }
                    currentColor = availableColors[indexColor];
                }
                else if (Input.GetKeyDown(ellegibleKeys[1])) {
                    indexColor += 1;
                    if (indexColor >= availableColors.Count) {
                        indexColor = 0;
                    }
                    currentColor = availableColors[indexColor];
                }
                UIText.color = currentColor;
                break;
            case screens.GameOver:
                UIText.enabled = true;
                UIText.text = textGameOver;
                if (Input.anyKeyDown) {
                    currentScreen = screens.Start;
                }
                break;
            case screens.Start:
                UIText.text = textStart;
                if (Input.anyKeyDown) {
                    currentScreen = screens.FirstPlayer;
                }
                break;
            default:
                break;
        }
    }

    void ScanKeys() {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
            if (Input.GetKey(vKey)) {
                if (availableKeys.Contains(vKey) && playerKeys.Contains(vKey) == false) {
                    if (!ellegibleKeys.Contains(vKey)) {
                        ellegibleKeys[index] = vKey;
                        index = (index + 1) % 2;
                    }
                }
            }
        }

        if (Input.GetKey(ellegibleKeys[0]) && Input.GetKey(ellegibleKeys[1]) && ready == false) {
            downTime = Time.time;
            pressTime = downTime + countDown;
            ready = true;
        }
        if (Input.GetKeyUp(ellegibleKeys[0]) || Input.GetKeyUp(ellegibleKeys[1])) {
            ready = false;
        }
        if (Time.time >= pressTime && ready == true) {
            ready = false;
            SortKeys();
            playerKeys.AddRange(ellegibleKeys);
            RegisterPlayer();
        }
    }

    public GameObject CreatePlayerStats() {
        GameObject obj = Instantiate(groupUi);
        obj.transform.SetParent(myGO.transform);
        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(5 + 140 * playersInfoList.Count , -5, 0);
        playersInfoList.Add(obj);
        return obj;
    }

    void SortKeys() {
        float keyA = UpdateValue(availableKeys.IndexOf(ellegibleKeys[0]));
        float keyB = UpdateValue(availableKeys.IndexOf(ellegibleKeys[1]));
        if (keyA > keyB) {
            KeyCode temp = ellegibleKeys[0];
            ellegibleKeys[0] = ellegibleKeys[1];
            ellegibleKeys[1] = temp;
        }
    }

    float UpdateValue(float i) {
        if (i < 10) {
            return i;
        }
        else if (i < 20) {
            i = -10 + 0.125f;
        }
        else if (i < 29) {
            i = i - 20 + 0.75f;
        }
        else {
            i = i - 29 + 1.25f;
        }
        return i;
    }

    void RegisterPlayer() {
        //string playerId = (playerKeys.Count / 2).ToString();
        string playerId = (keyColors.Count / 2).ToString();
        string key1 = ellegibleKeys[0].ToString();
        string key2 = ellegibleKeys[1].ToString();
        string textRegistered = $"Player {playerId} registered to game.\n" +
            $"Key {key1}: Turn left\nKey {key2}: Turn Right\n\n" +
            $"Use these keys to choose your snake color\nand Press ENTER to continue...";
        UIText.text = textRegistered;
        UIText.color = availableColors[0];
        currentScreen = screens.RegisteredPlayer;
    }

    void ListBasicColors() {
        availableColors = new List<Color>();
        availableColors.Add(Color.white);
        availableColors.Add(Color.green);
        availableColors.Add(Color.red);
        availableColors.Add(Color.blue);
        availableColors.Add(Color.yellow);
        availableColors.Add(Color.cyan);
        availableColors.Add(Color.gray);
        availableColors.Add(Color.magenta);
        availableColors.Add(Color.black);
        availableColors.Add(new Color32(0, 201, 254, 255));   // aqua
        availableColors.Add(new Color32(232, 0, 254, 255));   // pink
        availableColors.Add(new Color32( 254 , 161 , 0, 255 )); // orange
        availableColors.Add(new Color32(143, 0, 254, 255));  // purple
        availableColors.Add(new Color32(166, 254, 0, 255));  // lime
        availableColors.Add(new Color32(60, 0, 254, 255));   // navy
        availableColors.Add(new Color32(165, 42, 42, 255));  // brown
    }
}

public class KeyColor
{
    public KeyCode keyLeft;
    public KeyCode keyRigh;
    public Color color;

    public void Setup(KeyCode kl, KeyCode kr, Color c) {
        keyLeft = kl;
        keyRigh = kr;
        color = c;
    }
}
