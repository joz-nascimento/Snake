using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int sizeX;
    public int sizeY;
    public int snakesInitialLength;
    public float startSpeed;
    public float minimumSpeed;
    public float maximumSpeed;
    public float speedBoostValue;
    public int foodPoints;
    public GameObject goods;
    public GameObject snake;
    public Material boardTile;

    public List<GameObject> goodsList;
    private bool gameEnded = false;
    private List<GameObject> playersList;
    new AudioSource audio;
    public SfxManager sfxManager;
    public AudioClip startMusic;
    public AudioClip inGameMusic;
    public AudioClip endMusic;

    public List<List<SnakeData>> snkData;

    private void Start() {
        Camera.main.GetComponent<ScreenHUD>().gameboard = this.GetComponent<Board>();
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(sizeX/10.0f, 1, sizeY/10.0f);
        plane.transform.position = new Vector3(sizeX / 2.0f -0.5f, -0.5f, sizeY / 2.0f - 0.5f);
        boardTile.SetTextureScale("_MainTex", new Vector2(sizeX, sizeY));
        plane.GetComponent<MeshRenderer>().material = boardTile;
        audio = GetComponent<AudioSource>();
        audio.clip = startMusic;
        audio.Play();

        // Gameplay test
        /*KeySkin p1 = new KeySkin();
        p1.keyLeft = KeyCode.LeftArrow;
        p1.keyRigh = KeyCode.RightArrow;
        List<KeySkin> klist = new List<KeySkin>();
        klist.Add(p1);
        StartGame(klist);
        Camera.main.transform.position = new Vector3(19.6f, 16.63f, 9);
        Camera.main.transform.eulerAngles = new Vector3(90, 0, 0);
        Camera.main.orthographic = true;*/
    }

    public void StartGame(List<KeySkin> keySkin) {
        //coroutines = new List<Coroutine>();
        if (snakesInitialLength < 3) {
            snakesInitialLength = 3;
        }
        playersList = new List<GameObject>();
        snkData = new List<List<SnakeData>>();

        for (int i = 0; i < keySkin.Count; i++) {
            for (int j = 0; j < 2; j++) {
                GameObject obj = Instantiate(snake);
                Snake snk = obj.GetComponent<Snake>();
                snk.currentSpeed = startSpeed;
                snk.maximumSpeed = maximumSpeed;
                snk.minimumSpeed = minimumSpeed;
                snk.speedBoostValue = speedBoostValue;
                snk.snakesInitialLength = snakesInitialLength;
                snk.foodPoints = foodPoints;
                snk.sizeX = sizeX;
                snk.sizeY = sizeY;
                if (j == 0) {
                    snk.IAPilot = false;
                    snk.name = "P" + (i + 1).ToString();
                    snk.leftKey = keySkin[i].keyLeft;
                    snk.rightKey = keySkin[i].keyRigh;
                    snk.skinNumber = keySkin[i].skinNumber;
                }
                else {
                    snk.skinNumber = Random.Range(0,15);
                    snk.IAPilot = true;
                    snk.name = "CPU";
                }
                snk.Setup();
                playersList.Add(obj);
                snkData.Add(new List<SnakeData>());
            }
        }
        Camera.main.GetComponent<ScreenHUD>().AlignPlayerInfo(playersList);
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.food, 0));
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.enginePower, 5));
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.timeTravel, 8));
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.batteringRam, 10));
        audio.clip = inGameMusic;
        audio.Play();
    }

    public void FinishGame() {
    
    }

    public void SaveGame(GameObject obj) {
        int index = playersList.IndexOf(obj);
        List<SnakeData> snakeDatas = new List<SnakeData>();
            for (int j = 0; j < playersList.Count; j++) {
                snakeDatas.Add(new SnakeData());
                snakeDatas[snakeDatas.Count-1].Save(playersList[j].GetComponent<Snake>());
            }
        snkData[index] = snakeDatas;
        Debug.Log("SaveGame");
    }

    public void LoadGame(GameObject obj) {
        int index = playersList.IndexOf(obj);
        StopAllCoroutines();
        for (int i = 0; i < playersList.Count; i++) {
            playersList[i].GetComponent<Snake>().StopAllCoroutines();
            Destroy(playersList[i].GetComponent<Snake>().stats.gameObject);
            Destroy(playersList[i]);
        }
        playersList = new List<GameObject>();
        for (int i = 0; i < snkData[index].Count; i++) {
            GameObject newObj = Instantiate(snake);
            playersList.Add(snkData[index][i].LoadGame(newObj));
        }
        Debug.Log("index: " + index);
        playersList[index].GetComponent<Snake>().timeTravelBlock = false;
        playersList[index].GetComponent<Snake>().stats.GetComponent<Stats>().SetTimeBlock(false);
        Camera.main.GetComponent<ScreenHUD>().AlignPlayerInfo(playersList);
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.timeTravel, 8));
        Debug.Log("LoadGame");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            //SaveGame();
            
        }
        else if (Input.GetKeyDown(KeyCode.L)) {
            //LoadGame();
        }
    }

    Vector2 GetRandomPos() {
        int posX = Random.Range(0, sizeX);
        int posY = Random.Range(0, sizeY);
        return new Vector2(posX, posY);
    }

    public void RemoveGoods(int index) {
        float time;
        GameObject itemToDestroy = goodsList[index];
        switch (itemToDestroy.GetComponent<HitBlock>().style) {
            case HitBlock.BlockStyle.food:
                sfxManager.PlaySound(SfxManager.Clip.collect);
                StartCoroutine(CreateGoods(HitBlock.BlockStyle.food, 0));
                break;
            case HitBlock.BlockStyle.enginePower:
                sfxManager.PlaySound(SfxManager.Clip.speedUp);
                time = Random.Range(5.0f, 10.0f);
                StartCoroutine(CreateGoods(HitBlock.BlockStyle.enginePower, time));
                break;
            case HitBlock.BlockStyle.batteringRam:
                sfxManager.PlaySound(SfxManager.Clip.ramBlock);
                time = Random.Range(5.0f, 10.0f);
                StartCoroutine(CreateGoods(HitBlock.BlockStyle.batteringRam, time));
                break;
            case HitBlock.BlockStyle.timeTravel:
                sfxManager.PlaySound(SfxManager.Clip.timeTravel);
                time = Random.Range(5.0f, 10.0f);
                StartCoroutine(CreateGoods(HitBlock.BlockStyle.timeTravel, time));
                break;
        }
        goodsList.RemoveAt(index);
        Destroy(itemToDestroy);
    }

    public IEnumerator CreateGoods(HitBlock.BlockStyle style, float time) {
        yield return new WaitForSeconds(time);
        Vector2 pos = GetRandomPos();
        bool invalidPos = true;
        while (invalidPos) {
            bool isInside = false;
            foreach (var player in playersList) {
                for (int i = 0; i < player.GetComponent<Snake>().snakeList.Count; i++) {
                    if (pos.x == (int)Mathf.Round(player.GetComponent<Snake>().snakeList[0].transform.position.x)) {
                        if (pos.y == (int)Mathf.Round(player.GetComponent<Snake>().snakeList[0].transform.position.z)) {
                            isInside = true;
                        }
                    }

                }
            }
            if (isInside == false) {
                invalidPos = false;
            }
            else {
                pos = GetRandomPos();
            }
        }
        GameObject go = Instantiate(goods, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        go.GetComponent<HitBlock>().SetStyle(style);
        goodsList.Add(go);
    }


}

public class SnakeData {
    public string name;
    public bool IAPilot;
    public float currentSpeed;
    public int score;
    public int skinNumber;
    public KeyCode leftKey;
    public KeyCode rightKey;

    public List<Vector3> partPos;
    public List<Quaternion> partRot;
    public List<bool> eatFood;
    public Snake.Direction direction;
    internal float maximumSpeed;
    internal float minimumSpeed;
    internal float speedBoostValue;
    internal int foodPoints;
    internal int sizeX;
    internal int sizeY;
    internal int snakesInitialLength;
    private bool batteringRamBlock;
    private bool timeTravelBlock;

    public void Save(Snake snake) {
        partPos = new List<Vector3>();
        partRot = new List<Quaternion>();
        eatFood = new List<bool>();
        name = snake.name;
        IAPilot = snake.IAPilot;
        currentSpeed = snake.currentSpeed;
        score = snake.score;
        skinNumber = snake.skinNumber;
        leftKey = snake.leftKey;
        rightKey = snake.rightKey;
        maximumSpeed = snake.maximumSpeed;
        minimumSpeed = snake.minimumSpeed;
        speedBoostValue = snake.speedBoostValue;
        foodPoints = snake.foodPoints;
        sizeX = snake.sizeX;
        sizeY = snake.sizeY;
        snakesInitialLength = snake.snakesInitialLength;
        batteringRamBlock = snake.batteringRamBlock;
        timeTravelBlock = snake.timeTravelBlock;

        foreach (var item in snake.snakeList) {
            partPos.Add(item.transform.position);
            partRot.Add(item.transform.rotation);
            eatFood.Add(item.GetComponent<Body>().eatFood);
        }
        //direction = snake.direction;
        direction = Snake.Direction.forward;
    }

    public GameObject LoadGame(GameObject obj) {
        Snake snk = obj.GetComponent<Snake>();
        snk.currentSpeed = currentSpeed;
        snk.maximumSpeed = maximumSpeed;
        snk.minimumSpeed = minimumSpeed;
        snk.speedBoostValue = speedBoostValue;
        snk.snakesInitialLength = partPos.Count;
        snk.foodPoints = foodPoints;
        snk.sizeX = sizeX;
        snk.sizeY = sizeY;
        snk.IAPilot = IAPilot;
        snk.name = name;
        snk.leftKey = leftKey;
        snk.rightKey = rightKey;
        snk.skinNumber = skinNumber;
        snk.direction = direction;
        snk.Setup();
        snk.batteringRamBlock = batteringRamBlock;
        snk.timeTravelBlock = timeTravelBlock;
        snk.snakesInitialLength = snakesInitialLength;
        snk.ChangeSkin(skinNumber);
        snk.score = score;
        snk.stats.GetComponent<Stats>().UpdateScore(snk.score);
        snk.stats.GetComponent<Stats>().SetTimeBlock(timeTravelBlock);
        snk.stats.GetComponent<Stats>().SetRamBlock(batteringRamBlock);

        for (int i = 0; i < obj.GetComponent<Snake>().snakeList.Count; i++) {
            obj.GetComponent<Snake>().snakeList[i].transform.SetPositionAndRotation(partPos[i], partRot[i]);
            obj.GetComponent<Snake>().snakeList[i].GetComponent<Body>().eatFood = eatFood[i];
        }
        for (int i = obj.GetComponent<Snake>().snakeList.Count - 2; i > 1; i--) {
            Snake.Direction dir = GetDirection(obj.GetComponent<Snake>().snakeList[i], obj.GetComponent<Snake>().snakeList[i - 1]);
            switch (dir) {
                case Snake.Direction.left:
                    obj.GetComponent<Snake>().snakeList[i].GetComponent<Body>().Move(Body.Movement.turnLeft);
                    break;
                case Snake.Direction.right:
                    obj.GetComponent<Snake>().snakeList[i].GetComponent<Body>().Move(Body.Movement.turnRight);
                    break;
                case Snake.Direction.forward:
                    obj.GetComponent<Snake>().snakeList[i].GetComponent<Body>().Move(Body.Movement.straight);
                    break;
            }
        }
        return obj;
    }

    Snake.Direction GetDirection(GameObject from, GameObject to) {
        Vector3 targetDir = to.transform.position - from.transform.position;
        Vector3 forward = from.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        if (angle < -5) {
            return Snake.Direction.right;
        }
        else if (angle > 5) {
            return Snake.Direction.left;
        }
        else {
            return Snake.Direction.forward;
        }
    }
}
