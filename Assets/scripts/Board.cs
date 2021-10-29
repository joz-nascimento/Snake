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
    public GameObject head;
    public GameObject section;
    public Material boardTile;

    public List<GameObject> goodsList;
    private float currentSpeed;
    private int score;
    private bool gameEnded = false;
    private List<Snake> playersList;
    public List<Material> matList;
    //Snake snake1;
    //Snake snake2;


    private void Start() {
        Camera.main.GetComponent<ScreenHUD>().gameboard = this.GetComponent<Board>();
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(sizeX/10.0f, 1, sizeY/10.0f);
        plane.transform.position = new Vector3(sizeX / 2.0f -0.5f, -0.5f, sizeY / 2.0f - 0.5f);
        boardTile.SetTextureScale("_MainTex", new Vector2(sizeX, sizeY));
        plane.GetComponent<MeshRenderer>().material = boardTile;
    }

    public void StartGame(List<KeyColor> keyColor) {
        if (snakesInitialLength < 3) {
            snakesInitialLength = 3;
        }
        playersList = new List<Snake>();

        for (int i = 0; i < keyColor.Count; i++) {
            GameObject obj = new GameObject("snake" + i);
            Snake snake = obj.AddComponent<Snake>();
            snake.Setup(GetComponent<Board>());
            snake.IAPilot = false;
            snake.leftKey = keyColor[i].keyLeft;
            snake.rightKey = keyColor[i].keyRigh;

            snake.stats.GetComponent<Stats>().SetPlayerId("P" + (i + 1).ToString());
            playersList.Add(snake);
            snake.SetColor(keyColor[i].color);
        }

        for (int i = 0; i < keyColor.Count; i++) {
            GameObject obj = new GameObject("cpu" + i);
            Snake snake = obj.AddComponent<Snake>();
            snake.Setup(GetComponent<Board>());
            snake.IAPilot = true;
            snake.stats.GetComponent<Stats>().SetPlayerId("CPU");
            playersList.Add(snake);
        }

        StartCoroutine(CreateGoods(Goods.GoodsStyle.foodBlock, 0));
        StartCoroutine(CreateGoods(Goods.GoodsStyle.enginePowerBlock, 5));
        StartCoroutine(CreateGoods(Goods.GoodsStyle.timeTravelBlock, 8));
        StartCoroutine(CreateGoods(Goods.GoodsStyle.batteringRamBlock, 10));

    }

    public void StartGame(List<KeyCode> keys) {
        if (snakesInitialLength < 3) {
            snakesInitialLength = 3;
        }
        playersList = new List<Snake>();

        for (int i = 0; i < keys.Count/2; i++) {
            GameObject obj = new GameObject("snake" + i);
            Snake snake = obj.AddComponent<Snake>();
            snake.Setup(GetComponent<Board>());
            snake.IAPilot = false;
            snake.leftKey = keys[i * 2];
            snake.rightKey = keys[i * 2 + 1];
            snake.stats.GetComponent<Stats>().SetPlayerId("P" + (i + 1).ToString());
            playersList.Add(snake);
            switch (i) {
                case 0:
                    snake.SetColor(new Color32(0, 201, 254, 1));
                    break;
                case 1:
                    snake.SetColor(Color.blue);
                    break;
                case 2:
                    snake.SetColor(Color.green);
                    break;
                case 3:
                    snake.SetColor(Color.yellow);
                    break;
                default:
                    break;
            }
        }

        for (int i = 0; i < keys.Count / 2; i++) {
            GameObject obj = new GameObject("cpu" + i);
            Snake snake = obj.AddComponent<Snake>();
            snake.Setup(GetComponent<Board>());
            snake.IAPilot = true;
            snake.stats.GetComponent<Stats>().SetPlayerId("CPU");
            playersList.Add(snake);
        }

        StartCoroutine(CreateGoods(Goods.GoodsStyle.foodBlock, 0));
        StartCoroutine(CreateGoods(Goods.GoodsStyle.enginePowerBlock, 5));
        StartCoroutine(CreateGoods(Goods.GoodsStyle.timeTravelBlock, 8));
        StartCoroutine(CreateGoods(Goods.GoodsStyle.batteringRamBlock, 10));
    }

    Vector2 GetRandomPos() {
        int posX = Random.Range(0, sizeX);
        int posY = Random.Range(0, sizeY);
        return new Vector2(posX, posY);
    }

    public void RemoveGoods(int index) {
        float time;
        GameObject itemToDestroy = goodsList[index];
        switch (itemToDestroy.GetComponent<Goods>().style) {
            case Goods.GoodsStyle.foodBlock:
                StartCoroutine(CreateGoods(Goods.GoodsStyle.foodBlock, 0));
                break;
            case Goods.GoodsStyle.enginePowerBlock:
                time = Random.Range(5.0f, 10.0f);
                StartCoroutine(CreateGoods(Goods.GoodsStyle.enginePowerBlock, time));
                break;
            case Goods.GoodsStyle.batteringRamBlock:
                time = Random.Range(5.0f, 10.0f);
                StartCoroutine(CreateGoods(Goods.GoodsStyle.batteringRamBlock, time));
                break;
            case Goods.GoodsStyle.timeTravelBlock:
                time = Random.Range(5.0f, 10.0f);
                StartCoroutine(CreateGoods(Goods.GoodsStyle.timeTravelBlock, time));
                break;
        }
        goodsList.RemoveAt(index);
        Destroy(itemToDestroy);
    }

    public IEnumerator CreateGoods(Goods.GoodsStyle style, float time) {
        yield return new WaitForSeconds(time);
        Vector2 pos = GetRandomPos();
        bool invalidPos = true;
        while (invalidPos) {
            bool isInside = false;
            foreach (var player in playersList) {
                for (int i = 0; i < player.snakeList.Count; i++) {
                    if (pos.x == (int)Mathf.Round(player.snakeList[0].transform.position.x)) {
                        if (pos.y == (int)Mathf.Round(player.snakeList[0].transform.position.z)) {
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
        go.GetComponent<Goods>().SetStyle(style);
        goodsList.Add(go);
    }


}
