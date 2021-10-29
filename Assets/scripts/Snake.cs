using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public bool IAPilot = false;
    public int score;
    public float currentSpeed;
    private float minimumSpeed;
    private float maximumSpeed;
    private float speedBoostValue;
    public Vector2 spawnPos;
    public int foodPoints;
    private float startTime = 0;

    public GameObject head;
    public GameObject section;
    public Board snakeGame;
    public GameObject stats;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public Color color = Color.white;

    bool gameEnded = false;
    public bool batteringRamBlock;
    public bool timeTravelBlock;
    private bool foodCollected;

    private List<GameObject> goodsList;
    public List<GameObject> snakeList;

    public void Setup(Board board, Vector2? pos = null) {
        stats = Camera.main.GetComponent<ScreenHUD>().CreatePlayerStats();
        snakeGame = board;
        currentSpeed = snakeGame.startSpeed;
        maximumSpeed = snakeGame.maximumSpeed;
        minimumSpeed = snakeGame.minimumSpeed;
        speedBoostValue = snakeGame.speedBoostValue;
        if (pos == null) {
            float x = 1 + snakeGame.snakesInitialLength;
            float y = Random.Range(1, snakeGame.sizeY-1);
            pos =  new Vector2(x, y); 
        }
        spawnPos = (Vector2) pos;

        snakeList = new List<GameObject>();
        goodsList = new List<GameObject>();
        Setup();
    }

    void Setup() {
        gameEnded = false;
        for (int i = 0; i < snakeList.Count; i++) {
            Destroy(snakeList[i]);
        }
        snakeList = new List<GameObject>();
        for (int i = 0; i < goodsList.Count; i++) {
            Destroy(goodsList[i]);
        }
        goodsList = new List<GameObject>();
        startTime = Time.time;

        currentSpeed = snakeGame.startSpeed;

        score = 0;

        CreateSnake(spawnPos.x, spawnPos.y);
    }

    void CreateSnake(float x = 10, float y = 9) {
        Vector3 pos = new Vector3(x, 0, y);

        for (int i = 0; i < snakeList.Count - 1; i++) {
            Destroy(snakeList[i]);
        }
        snakeList = new List<GameObject>();

        snakeList.Add(Instantiate(snakeGame.head, pos, Quaternion.identity));
        snakeList[0].GetComponent<Renderer>().material.color = color;
        snakeList[0].transform.Rotate(0, 90, 0);
        snakeList[0].GetComponent<Rigidbody>().detectCollisions = false;
        snakeList[0].transform.parent = gameObject.transform;
        for (int i = 0; i < snakeGame.snakesInitialLength-1; i++) {
            pos = snakeList[i].transform.position - snakeList[i].transform.forward;
            snakeList.Add(Instantiate(snakeGame.section, pos, snakeList[i].transform.rotation));
            snakeList[snakeList.Count-1].transform.parent = gameObject.transform;
            snakeList[snakeList.Count - 1].GetComponent<Renderer>().material.color = color;
        }
        snakeList[0].GetComponent<Rigidbody>().detectCollisions = true;
        stats.GetComponent<Stats>().SetRamBlock(false);
        stats.GetComponent<Stats>().SetTimeBlock(false);
        stats.GetComponent<Stats>().UpdateScore(score);
        UpdateSpeed(0);
    }

    public void SetColor(Color newColor) {
        color = newColor;
        foreach (var item in snakeList) {
            item.GetComponent<Renderer>().material.color = color;
        }
    }

    void Update() {

        if (IAPilot) {
            IADrive();
        } else {
            if (Input.GetKeyDown(leftKey)) {
                snakeList[0].transform.Rotate(0, -90, 0);
            }
            else if (Input.GetKeyDown(rightKey)) {
                snakeList[0].transform.Rotate(0, 90, 0);
            }

            else if (Input.GetKey(KeyCode.Return) & gameEnded) {
                Setup();
            }
        }

        float currentTime = Time.time;
        if (currentTime - startTime >= 1 / currentSpeed * 10) {
            MovingForward();
            startTime = currentTime;

            // cruzando o cenário
            Vector3 pos = snakeList[0].transform.position;
            if (Mathf.Round(pos.x) > snakeGame.sizeX-1) {
                snakeList[0].transform.position = new Vector3(0, pos.y, pos.z);
            }
            else if (Mathf.Round(pos.x) < 0) {
                snakeList[0].transform.position = new Vector3(snakeGame.sizeX-1, pos.y, pos.z);
            }

            if (Mathf.Round(pos.z) > snakeGame.sizeY-1) {
                snakeList[0].transform.position = new Vector3(pos.x, pos.y, 0);
            }
            else if (Mathf.Round(pos.z) < 0) {
                snakeList[0].transform.position = new Vector3(pos.x, pos.y, snakeGame.sizeY-1);
            }
            TestEat();
        }
    }

    private void IADrive() {
        DirectionToFood(snakeList[0].transform);
        if (snakeList[0].GetComponent<Head>().CheckObstacle(Head.Direction.Front)) {
            if (snakeList[0].GetComponent<Head>().CheckObstacle(Head.Direction.Right)) {
                snakeList[0].transform.Rotate(0, -90, 0);
            }
            else {
                snakeList[0].transform.Rotate(0, 90, 0);
            }
        }
    }

    void DirectionToFood(Transform head) {
        Vector3 food = Vector3.zero;
        foreach (var item in snakeGame.goodsList) {
            if (item.GetComponent<Goods>().style == Goods.GoodsStyle.foodBlock) {
                food = item.transform.position;
            }
        }
        Vector3 targetDir = food - head.position;
        Vector3 forward = head.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        if (angle >= 90) {
            if (!snakeList[0].GetComponent<Head>().CheckObstacle(Head.Direction.Left)) {
                head.transform.Rotate(0, -90, 0);
            }
        }
        else if (angle <= -90) {
            if (!snakeList[0].GetComponent<Head>().CheckObstacle(Head.Direction.Right)) {
                head.transform.Rotate(0, 90, 0);
            }
        }

    }

    void MovingForward() {
        if (foodCollected) {
            snakeList[0].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            snakeList.Add(Instantiate(snakeGame.section, snakeList[1].transform.position, Quaternion.identity));
            snakeList[snakeList.Count - 1].transform.parent = gameObject.transform;
            snakeList[snakeList.Count - 1].GetComponent<Renderer>().material.color = color;
            foodCollected = false;
        }
        Vector3 lastPos = snakeList[0].transform.position;
        Quaternion lastRot = snakeList[0].transform.rotation;
        Vector3 currentPos;
        Quaternion currentRot;
        snakeList[0].transform.Translate(Vector3.forward);
        for (int i = 1; i < snakeList.Count; i++) {
            currentPos = snakeList[i].transform.position;
            currentRot = snakeList[i].transform.rotation;
            snakeList[i].transform.position = lastPos;
            snakeList[i].transform.rotation = lastRot;
            lastPos = currentPos;
            lastRot = currentRot;
        }

        for (int i = snakeList.Count - 1; i > 0; i--) {
            if (snakeList[i - 1].transform.localScale.x > 1) {
                if (i != snakeList.Count - 1) {
                    snakeList[i].transform.localScale = snakeList[i - 1].transform.localScale;
                }
                snakeList[i - 1].transform.localScale = Vector3.one;
            }
        }

    }

    void TestEat() {
        if (snakeGame.goodsList.Count == 0) {
            return;
        }
        for (int i = 0; i < snakeGame.goodsList.Count; i++) {
            if (Mathf.Round(snakeGame.goodsList[i].transform.position.x) == Mathf.Round(snakeList[0].transform.position.x)) {
                if (Mathf.Round(snakeGame.goodsList[i].transform.position.z) == Mathf.Round(snakeList[0].transform.position.z)) {
                    switch (snakeGame.goodsList[i].GetComponent<Goods>().style) {
                        case Goods.GoodsStyle.foodBlock:
                            UpdateSpeed(-1);
                            UpdateScore();
                            foodCollected = true;
                            break;
                        case Goods.GoodsStyle.enginePowerBlock:
                            UpdateSpeed(speedBoostValue);
                            break;
                        case Goods.GoodsStyle.batteringRamBlock:
                            batteringRamBlock = true;
                            stats.GetComponent<Stats>().SetRamBlock(true);
                            break;
                        case Goods.GoodsStyle.timeTravelBlock:
                            timeTravelBlock = true;
                            stats.GetComponent<Stats>().SetTimeBlock(true);
                            break;
                    }
                    snakeGame.RemoveGoods(i);
                    break;
                }
            }
        }
    }

    void UpdateSpeed(float power) {
        currentSpeed += power;
        if (currentSpeed < minimumSpeed) {
            currentSpeed = minimumSpeed;
        }
        else if (currentSpeed > maximumSpeed) {
            currentSpeed = maximumSpeed;
        }
        float barValue = currentSpeed / maximumSpeed;
        stats.GetComponent<Stats>().UpdateBar(barValue);
    }

    void UpdateScore() {
        score += snakeGame.foodPoints;
        stats.GetComponent<Stats>().UpdateScore(score);
    }

    public void CollisionDetected() {
        if (batteringRamBlock) {
            batteringRamBlock = false;
            stats.GetComponent<Stats>().SetRamBlock(false);
        }
        else {
            currentSpeed = 0;
            if (IAPilot) {
                Invoke(nameof(Setup), 1.0f);
            }
            else {
                Invoke(nameof(Setup), 1.0f);
            }
        }
        StartCoroutine(Blink(2));
    }

    IEnumerator Blink(float waitTime) {
        var endTime = Time.time + waitTime;
        snakeList[0].GetComponent<Rigidbody>().detectCollisions = false;
        while (Time.time < endTime) {
            foreach (GameObject section in snakeList) {
                section.GetComponent<Renderer>().enabled = false;
            }
            yield return new WaitForSeconds(0.2f);
            foreach (GameObject section in snakeList) {
                section.GetComponent<Renderer>().enabled = true;
            }
            yield return new WaitForSeconds(0.2f);
        }
        snakeList[0].GetComponent<Rigidbody>().detectCollisions = true;
    }
}
