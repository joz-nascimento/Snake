using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHUD : MonoBehaviour
{
    public GameObject groupUi;
    public List<GameObject> playersInfoList;
    public List<KeySkin> keySkins;
    Canvas myCanvas;
    GameObject myGO;
    int index = 0;
    public List<KeyCode> ellegibleKeys;
    public List<KeyCode> availableKeys;
    public List<KeyCode> playerKeys;
    public List<Color> playerColors;
    public List<Color> availableColors;
    public int currentSkin;
    public int indexColor;
    public int indexSkin;

    public Text UIText;
    public GameObject snake_PF;
    GameObject snake;

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

    // Gameplay teste
    //screens currentScreen = screens.InGame;


    void Start() {
        Camera.main.transform.position = new Vector3(5.9f, 5.75f, 9.31f);
        Camera.main.transform.eulerAngles = new Vector3(58.8f, -195.2f, 0);
        Camera.main.orthographic = false;
        playersInfoList = new List<GameObject>();
        //indexColor = 0;
        indexSkin = 0;

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
        keySkins = new List<KeySkin>();
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
        CreateModel();
        snake.SetActive(false);
    }

    void CreateModel() {
        snake = Instantiate(snake_PF);
        Snake snk = snake.GetComponent<Snake>();
        snk.currentSpeed = 0;
        snk.minimumSpeed = 0;
        snk.snakesInitialLength = 7;
        snk.sizeX = 10;
        snk.sizeY = 10;
        snk.IAPilot = false;
        snk.name = "Model";
        snk.Setup();
        Vector3 pos = snk.snakeList[0].transform.position;
        snk.snakeList[1].transform.Rotate(0, -90, 0);
        snk.snakeList[1].GetComponent<Body>().SetSection(Body.Section.body, Body.Movement.turnRight);
        snk.snakeList[2].transform.position = new Vector3(pos.x+1, pos.y, pos.z-1);
        snk.snakeList[2].GetComponent<Body>().SetSection(Body.Section.body, Body.Movement.turnLeft);
        snk.snakeList[3].transform.position = new Vector3(pos.x+1, pos.y, pos.z-2);
        snk.snakeList[4].transform.position = new Vector3(pos.x+1, pos.y, pos.z-3);
        snk.snakeList[4].transform.Rotate(0, -90, 0);
        snk.snakeList[4].GetComponent<Body>().SetSection(Body.Section.body, Body.Movement.turnRight);
        snk.snakeList[5].transform.position = new Vector3(pos.x+2, pos.y, pos.z-3);
        snk.snakeList[5].transform.Rotate(0, -90, 0);
        snk.snakeList[6].transform.position = new Vector3(pos.x + 3, pos.y, pos.z - 3);
        snk.snakeList[6].transform.Rotate(0, -90, 0);
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
        //Mytext.color = availableColors[1];
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
                    Camera.main.transform.position = new Vector3(20, 16.63f, 9);
                    Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
                    Camera.main.orthographic = true;
                    Destroy(snake);
                    gameboard.StartGame(keySkins);
                }
                ScanKeys();
                break;
            case screens.RegisteredPlayer:
                if (Input.GetKeyDown(KeyCode.Return)) {
                    KeySkin temp = new KeySkin();
                    temp.Setup(ellegibleKeys[0], ellegibleKeys[1], indexSkin);
                    keySkins.Add(temp);
                    indexSkin = 0;
                    snake.GetComponent<Snake>().ChangeSkin(indexSkin);
                    currentScreen = screens.AnotherPlayer;
                    snake.SetActive(false);
                }
                if (Input.GetKeyDown(ellegibleKeys[0])) {
                    indexSkin -= 1;
                    if (indexSkin < 0) {
                        indexSkin = 15;
                    }
                    snake.GetComponent<Snake>().ChangeSkin(indexSkin);
                }
                else if (Input.GetKeyDown(ellegibleKeys[1])) {
                    indexSkin += 1;
                    if (indexSkin >= 15) {
                        indexSkin = 0;
                    }
                    snake.GetComponent<Snake>().ChangeSkin(indexSkin);
                }
                //UIText.color = currentSkin;
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

    public void AlignPlayerInfo(List<GameObject> objs) {
        int count = 0;
        foreach (var obj in objs) {
            obj.GetComponent<Snake>().stats.transform.SetParent(myCanvas.transform);
            obj.GetComponent<Snake>().stats.GetComponent<RectTransform>().anchoredPosition = new Vector3(5 + 140 * count, -5, 0);
            count += 1;
        }
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
        string playerId = (keySkins.Count / 2).ToString();
        string key1 = ellegibleKeys[0].ToString();
        string key2 = ellegibleKeys[1].ToString();
        string textRegistered = $"Player {playerId} registered to game.\n" +
            $"Key {key1}: Turn left\nKey {key2}: Turn Right\n\n" +
            $"Use these keys to choose your snake color\nand Press ENTER to continue...";
        UIText.text = textRegistered;
        currentScreen = screens.RegisteredPlayer;
        snake.SetActive(true);
    }
}

public class KeySkin
{
    public KeyCode keyLeft;
    public KeyCode keyRigh;
    public int skinNumber;

    public void Setup(KeyCode kl, KeyCode kr, int c) {
        keyLeft = kl;
        keyRigh = kr;
        skinNumber = c;
    }
}
