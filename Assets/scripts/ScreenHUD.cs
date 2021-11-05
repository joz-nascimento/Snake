using System;
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

    string textStart = "A Snake Game...\n< Classic > \t Multiplayer \t Exit\n\nPress keys left or Right to choose and press ENTER...";
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
        PauseGame,
    }
    enum Options {
        classicGame,
        multiplayer,
        exit,
        continueGame,
        quitGame,
    }
    Options opt;
    screens currentScreen;
    // Gameplay teste
    //screens currentScreen = screens.InGame;


    void Start() {
        Camera.main.transform.position = new Vector3(8.09f, 6.8f, 12.41f);
        Camera.main.transform.eulerAngles = new Vector3(51.23f, 164.8f, 0);
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

        currentScreen = screens.Start;
        UIText.text = textStart;
        opt = Options.classicGame;
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
        Mytext.fontSize = 25;

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
                    Camera.main.transform.position = new Vector3(19.6f, 16.63f, 9);
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
                break;
            case screens.GameOver:
                UIText.enabled = true;
                UIText.text = textGameOver;
                if (Input.anyKeyDown) {
                    SetStartScreen();
                }
                break;
            case screens.Start:
                if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    switch (opt) {
                        case Options.classicGame:
                            UIText.text = textStart.Replace("< ", "").Replace(" >", "").Replace("Multiplayer", "< Multiplayer >");
                            opt = Options.multiplayer;
                            break;
                        case Options.multiplayer:
                            UIText.text = textStart.Replace("< ", "").Replace(" >", "").Replace("Exit", "< Exit >");
                            opt = Options.exit;
                            break;
                        case Options.exit:
                            UIText.text = textStart.Replace("< ", "").Replace(" >", "").Replace("Classic", "< Classic >");
                            opt = Options.classicGame;
                            break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    switch (opt) {
                        case Options.classicGame:
                            UIText.text = textStart.Replace("< ", "").Replace(" >", "").Replace("Exit", "< Exit >");
                            opt = Options.exit;
                            break;
                        case Options.multiplayer:
                            UIText.text = textStart.Replace("< ", "").Replace(" >", "").Replace("Classic", "< Classic >");
                            opt = Options.classicGame;
                            break;
                        case Options.exit:
                            UIText.text = textStart.Replace("< ", "").Replace(" >", "").Replace("Multiplayer", "< Multiplayer >");
                            opt = Options.multiplayer;
                            break;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return)) {
                    switch (opt) {
                        case Options.classicGame:
                            KeySkin p1 = new KeySkin();
                            p1.keyLeft = KeyCode.LeftArrow;
                            p1.keyRigh = KeyCode.RightArrow;
                            List<KeySkin> klist = new List<KeySkin>();
                            klist.Add(p1);
                            Camera.main.transform.position = new Vector3(19.6f, 16.63f, 9);
                            Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
                            Camera.main.orthographic = true;
                            UIText.enabled = false;
                            currentScreen = screens.InGame;
                            gameboard.StartGame(klist, false);
                            break;
                        case Options.multiplayer:
                            currentScreen = screens.FirstPlayer;
                            break;
                        case Options.exit:
                            Application.Quit();
                            break;
                    }

                }
                break;
            case screens.InGame:
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    Time.timeScale = 0;
                    opt = Options.continueGame;
                    UIText.text = "Game Paused\n\n< Continue >\tQuit";
                    UIText.enabled = true;
                    currentScreen = screens.PauseGame;
                }
                break;
            case screens.PauseGame:
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    Time.timeScale = 1;
                    UIText.enabled = false;
                    currentScreen = screens.InGame;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                    if (opt == Options.continueGame) {
                        opt = Options.quitGame;
                        UIText.text = "Game Paused\n\nContinue\t< Quit >";
                    }
                    else if (opt == Options.quitGame) {
                        opt = Options.continueGame;
                        UIText.text = "Game Paused\n\n< Continue >\tQuit";
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Return)) {
                    if (opt == Options.continueGame) {
                        Time.timeScale = 1;
                        UIText.enabled = false;
                        currentScreen = screens.InGame;
                    }
                    else if (opt == Options.quitGame) {
                        SetStartScreen();
                    }
                }
                break;
            default:
                break;
        }
    }

    private void SetStartScreen() {
        Time.timeScale = 1;
        gameboard.FinishGame();
        UIText.text = textStart;
        currentScreen = screens.Start;
        opt = Options.classicGame;
        keySkins = new List<KeySkin>();
        playerKeys = new List<KeyCode>();
        CreateModel();
        snake.SetActive(false);
    }

    public void SetGameOverScreen() {
        currentScreen = screens.GameOver;
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
        string playerId = (keySkins.Count).ToString();
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
