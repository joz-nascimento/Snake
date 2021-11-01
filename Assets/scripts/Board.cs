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
    private float currentSpeed;
    private int score;
    private bool gameEnded = false;
    private List<GameObject> playersList;
    public List<Material> matList;
    AudioSource audio;
    public SfxManager sfxManager;
    public AudioClip collect_sound;

    private void Start() {
        Camera.main.GetComponent<ScreenHUD>().gameboard = this.GetComponent<Board>();
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(sizeX/10.0f, 1, sizeY/10.0f);
        plane.transform.position = new Vector3(sizeX / 2.0f -0.5f, -0.5f, sizeY / 2.0f - 0.5f);
        boardTile.SetTextureScale("_MainTex", new Vector2(sizeX, sizeY));
        plane.GetComponent<MeshRenderer>().material = boardTile;
        audio = GetComponent<AudioSource>();


        // Gameplay test
        /*KeyColor p1 = new KeyColor();
        p1.keyLeft = KeyCode.LeftArrow;
        p1.keyRigh = KeyCode.RightArrow;
        List<KeyColor> klist = new List<KeyColor>();
        klist.Add(p1);
        StartGame(klist);*/
    }

    public void StartGame(List<KeySkin> keySkin) {
        if (snakesInitialLength < 3) {
            snakesInitialLength = 3;
        }
        playersList = new List<GameObject>();

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
                //snk.ChangeSkin(keySkin[i].skinNumber);
                playersList.Add(obj);
            }
        }
        Camera.main.GetComponent<ScreenHUD>().AlignPlayerInfo(playersList);
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.food, 0));
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.enginePower, 5));
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.timeTravel, 8));
        StartCoroutine(CreateGoods(HitBlock.BlockStyle.batteringRam, 10));

    }

    public void FinishGame() {
    
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
