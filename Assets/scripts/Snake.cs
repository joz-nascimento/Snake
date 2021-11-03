using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public bool IAPilot = false;
    public float currentSpeed;
    public float minimumSpeed;
    public float maximumSpeed;
    public float speedBoostValue;
    public int snakesInitialLength;
    public int sizeY;
    public int sizeX;
    public int score;
    public Vector2 spawnPos;
    public int foodPoints;
    private float startTime = 0;
    public GameObject section;
    public Board snakeGame;
    public GameObject PlayerInfo;
    public GameObject stats;
    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public string name;
    public Vector3 foodPosition;

    bool gameEnded = false;
    public bool batteringRamBlock;
    public bool timeTravelBlock;
    private bool foodCollected;

    public List<GameObject> snakeList;

    public enum Direction { 
        left,
        right,
        forward,
    }
    public enum PowerUps { 
        speedUp,
        ramBlock,
        timeTravel,
        snakePart,
    }
    public Direction direction;
    public LayerMask layer;
    public int skinNumber = 0;

    public void Start() {
        if (snakeList.Count < 1) {
            Debug.Log("snakeList.Count < 1");
            Setup();
        }
    }

    public void Setup() {
        gameEnded = false;
        for (int i = 0; i < snakeList.Count; i++) {
            Destroy(snakeList[i]);
        }
        if (stats == null) {
            stats = Instantiate(PlayerInfo);
            stats.GetComponent<Stats>().SetPlayerId(name);
            Canvas myCanvas = (Canvas)FindObjectOfType(typeof(Canvas));
            if (myCanvas != null) {
                stats.transform.SetParent(myCanvas.transform);
            }
        }
        if (snakeGame == null) {
            snakeGame = (Board)FindObjectOfType(typeof(Board));
        }
        else {
            currentSpeed = snakeGame.startSpeed;
        }
        spawnPos = new Vector2(Random.Range(snakesInitialLength, sizeX), Random.Range(snakesInitialLength, sizeY));
        snakeList = new List<GameObject>();
        startTime = Time.time;
        score = 0;
        CreateSnake(spawnPos.x, spawnPos.y);

        ChangeSkin(skinNumber);
    }

    void CreateSnake(float x = 10, float y = 9) {
        Vector3 pos = new Vector3(x, 0, y);

        for (int i = 0; i < snakeList.Count - 1; i++) {
            Destroy(snakeList[i]);
        }
        snakeList = new List<GameObject>();

        snakeList.Add(Instantiate(section, pos, Quaternion.identity));
        snakeList[0].GetComponent<Body>().SetSection(Body.Section.head);
        snakeList[0].GetComponent<Rigidbody>().detectCollisions = false;
        snakeList[0].transform.parent = gameObject.transform;
        for (int i = 0; i < snakesInitialLength-1; i++) {
            pos = snakeList[i].transform.position - snakeList[i].transform.forward;
            snakeList.Add(Instantiate(section, pos, snakeList[i].transform.rotation));
            if (i == snakesInitialLength - 2) {
                snakeList[snakeList.Count - 1].GetComponent<Body>().SetSection(Body.Section.tail);
            }
            else {
                snakeList[snakeList.Count - 1].GetComponent<Body>().SetSection(Body.Section.body);
            }
            snakeList[snakeList.Count-1].transform.parent = gameObject.transform;
        }

        snakeList[0].GetComponent<Rigidbody>().detectCollisions = true;
        stats.GetComponent<Stats>().SetRamBlock(false);
        stats.GetComponent<Stats>().SetTimeBlock(false);
        stats.GetComponent<Stats>().UpdateScore(score);
        UpdateSpeed(0);
    }

    void Update() {
        if (IAPilot) {
            IADrive();
        } else {
            if (Input.GetKeyDown(leftKey)) {
                //snakeList[1].GetComponent<Body>().Move(Body.Movement.turnLeft);
                direction = Direction.left;
                //snakeList[0].transform.Rotate(0, -90, 0);
            }
            else if (Input.GetKeyDown(rightKey)) {
                direction = Direction.right;
                //snakeList[1].GetComponent<Body>().Move(Body.Movement.turnRight);
                //snakeList[0].transform.Rotate(0, 90, 0);
            }

            else if (Input.GetKey(KeyCode.Return) & gameEnded) {
                Setup();
            }

            else if (Input.GetKeyDown(KeyCode.P)) {
                skinNumber = (skinNumber + 1) % 16;
                foreach (var item in snakeList) {
                    item.GetComponent<Body>().ChangeSkin(skinNumber);
                }
            }
        }

        float currentTime = Time.time;
        if (currentTime - startTime >= 1 / currentSpeed * 10) {
            MovingForward();
            startTime = currentTime;

            // cruzando o cenário
            Vector3 pos = snakeList[0].transform.position;
            if (Mathf.Round(pos.x) > sizeX-1) {
                snakeList[0].transform.position = new Vector3(0, pos.y, pos.z);
            }
            else if (Mathf.Round(pos.x) < 0) {
                snakeList[0].transform.position = new Vector3(sizeX-1, pos.y, pos.z);
            }

            if (Mathf.Round(pos.z) > sizeY-1) {
                snakeList[0].transform.position = new Vector3(pos.x, pos.y, 0);
            }
            else if (Mathf.Round(pos.z) < 0) {
                snakeList[0].transform.position = new Vector3(pos.x, pos.y, sizeY-1);
            }
            //TestEat();
        }
    }

    public void ChangeSkin(int skinIndex) {
        skinNumber = skinIndex;
        foreach (var item in snakeList) {
            item.GetComponent<Body>().ChangeSkin(skinNumber);
        }
    }

    private void IADrive() {
        DirectionToFood();
        HitBlock.BlockStyle hitBlock = CheckHit(direction);
        if (hitBlock == HitBlock.BlockStyle.snakePart) {
            if (direction == Direction.left) {
                hitBlock = CheckHit(Direction.forward);
                if (hitBlock == HitBlock.BlockStyle.snakePart) {
                    direction = Direction.right;
                }
                else {
                    direction = Direction.forward;
                }
            }
            else if (direction == Direction.right) {
                hitBlock = CheckHit(Direction.forward);
                if (hitBlock == HitBlock.BlockStyle.snakePart) {
                    direction = Direction.left;
                }
                else {
                    direction = Direction.forward;
                }
            }
            else {
                hitBlock = CheckHit(Direction.left);
                if (hitBlock == HitBlock.BlockStyle.snakePart) {
                    direction = Direction.right;
                }
                else {
                    direction = Direction.left;
                }
            }
        }
    }

    void DirectionToFood() {
        if (snakeGame == null) {
            return;
        }
        Vector3 food = foodPosition;
        foreach (var item in snakeGame.goodsList) {
            if (item.GetComponent<HitBlock>().style == HitBlock.BlockStyle.food) {
                food = item.transform.position;
            }
        }
        Vector3 targetDir = food - snakeList[0].transform.position;
        Vector3 forward = snakeList[0].transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        if (angle >= 90) {
            direction = Direction.left;
        }
        else if (angle <= -90) {
            direction = Direction.right;
        }
    }

    Direction GetDirection(GameObject from, GameObject to) {
        Vector3 targetDir = to.transform.position - from.transform.position;
        Vector3 forward = from.transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);
        Debug.Log("angle: " + angle);
        if (angle < -5) {
            return Direction.right;
        }
        else if (angle > 5) {
            return Direction.left;
        }
        else {
            return Direction.forward;
        }
    }

    void MovingForward() {
        Vector3 lastPos = snakeList[0].transform.position;
        Quaternion lastRot = snakeList[0].transform.rotation;
        Vector3 currentPos;
        Quaternion currentRot;

        // Rotate head
        if (direction == Direction.left) {
            snakeList[0].transform.Rotate(0, -90, 0);
        }
        else if (direction == Direction.right) {
            snakeList[0].transform.Rotate(0, 90, 0);
        }

        HitBlock.BlockStyle hitBlock = CheckHit();
        if (foodCollected) {
            snakeList.Insert(1, Instantiate(section, lastPos, lastRot));
            snakeList[1].transform.parent = gameObject.transform;
            snakeList[1].GetComponent<Body>().SetSection(Body.Section.body);
            snakeList[1].GetComponent<Body>().ChangeSkin(skinNumber);
        }

        // Move head forward
        snakeList[0].transform.Translate(Vector3.forward);

        if (!foodCollected) {
            // update position after head
            for (int i = 1; i < snakeList.Count; i++) {
                currentPos = snakeList[i].transform.position;
                currentRot = snakeList[i].transform.rotation;
                snakeList[i].transform.position = lastPos;
                snakeList[i].transform.rotation = lastRot;
                lastPos = currentPos;
                lastRot = currentRot;
            }
            // Align tail
            snakeList[snakeList.Count - 1].transform.rotation = snakeList[snakeList.Count - 2].transform.rotation;

            // update sections
            for (int i = snakeList.Count - 2; i > 0; i--) {
                snakeList[i].GetComponent<Body>().eatFood = snakeList[i - 1].GetComponent<Body>().eatFood;
                snakeList[i].GetComponent<Body>().Move(snakeList[i - 1].GetComponent<Body>().movement);
            }
        }

        // follow head curve
        if (direction == Direction.left) {
                snakeList[1].GetComponent<Body>().Move(Body.Movement.turnLeft);
            }
        else if (direction == Direction.right) {
            snakeList[1].GetComponent<Body>().Move(Body.Movement.turnRight);
            }
        snakeList[0].GetComponent<Body>().eatFood = foodCollected;
        foodCollected = false;
        direction = Direction.forward;

        if (hitBlock == HitBlock.BlockStyle.snakePart) {
            CollisionDetected();
        }
    }

    /*public bool CheckObstacle() {
        var ray = new Ray(snakeList[0].transform.position, snakeList[0].transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.5f, -1)) {
            if (hit.transform.gameObject.CompareTag("Goods")) {
                //hit.transform.gameObject.GetComponent<Goods>().style;
            }
            return true;
        }
        return false;
    }*/

    public HitBlock.BlockStyle CheckHit(Direction dir = Direction.forward) {
        Vector3 rotation;
        if (dir == Direction.forward) {
            rotation = snakeList[0].transform.forward;
        }
        else if (dir == Direction.left) {
            rotation = -snakeList[0].transform.right;
        }
        else {
            rotation = snakeList[0].transform.right;
        }


        Vector3 testPos = snakeList[0].transform.position;

        Vector3 MoveTowards = snakeList[0].transform.position + rotation;
        if (Mathf.Round(MoveTowards.x) < 0) {
            testPos = new Vector3(sizeX, testPos.y, testPos.z);
        }
        else if (Mathf.Round(MoveTowards.x) > sizeX - 1) {
            testPos = new Vector3(-1, testPos.y, testPos.z);
        }
        else if (Mathf.Round(MoveTowards.z) < 0) {
            testPos = new Vector3(testPos.x, testPos.y, sizeY);
        }
        else if (Mathf.Round(MoveTowards.z) > sizeY-1) {
            testPos = new Vector3(testPos.x, testPos.y, -1);
        }

        var ray = new Ray(testPos, rotation);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.5f, -1)) {
            if (hit.transform.gameObject.CompareTag("Goods")) {
                HitBlock.BlockStyle block = hit.transform.gameObject.GetComponent<HitBlock>().style;
                switch (block) {
                    case HitBlock.BlockStyle.food:
                        UpdateSpeed(-1);
                        UpdateScore();
                        foodCollected = true;
                        break;
                    case HitBlock.BlockStyle.enginePower:
                        UpdateSpeed(speedBoostValue);
                        break;
                    case HitBlock.BlockStyle.batteringRam:
                        batteringRamBlock = true;
                        stats.GetComponent<Stats>().SetRamBlock(true);
                        break;
                    case HitBlock.BlockStyle.timeTravel:
                        timeTravelBlock = true;
                        snakeGame.SaveGame(gameObject);
                        stats.GetComponent<Stats>().SetTimeBlock(true);
                        break;
                }
                int index = snakeGame.goodsList.IndexOf(hit.transform.gameObject);
                snakeGame.RemoveGoods(index);
                return block;
            }
            else {
                return HitBlock.BlockStyle.snakePart;
            }
        }
        return HitBlock.BlockStyle.none;
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
        score += foodPoints;
        stats.GetComponent<Stats>().UpdateScore(score);
    }

    public void CollisionDetected() {
        if (batteringRamBlock) {
            batteringRamBlock = false;
            stats.GetComponent<Stats>().SetRamBlock(false);
        }
        else if (timeTravelBlock) {
            timeTravelBlock = false;
            snakeGame.LoadGame(gameObject);
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
        Blink(2);
    }

    void Blink(float waitTime) {
        foreach (var item in snakeList) {
            StartCoroutine(item.GetComponent<Body>().Blink(waitTime));
        }
    }
}
