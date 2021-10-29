using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private GameObject body_obj;
    private GameObject body_fat_obj;
    private GameObject turn_left_obj;
    private GameObject turn_left_fat_obj;
    private GameObject turn_right_obj;
    private GameObject turn_right_fat_obj;
    private GameObject tail_obj;
    private List<GameObject> body_list;
    public bool isFat;
    public bool eatFood;
    public enum Movement{
        straight,
        turnLeft,
        turnRight,
        last,
    }
    public Movement movement;

    // Start is called before the first frame update
    void Start()
    {
        body_obj = transform.Find("body_low").gameObject;
        body_fat_obj = transform.Find("body_fat_low").gameObject;
        turn_left_obj = transform.Find("turn_left_low").gameObject;
        turn_left_fat_obj = transform.Find("turn_left_fat_low").gameObject;
        turn_right_obj = transform.Find("turn_right_low").gameObject;
        turn_right_fat_obj = transform.Find("turn_right_fat_low").gameObject;
        tail_obj = transform.Find("tail_low").gameObject;

        body_list = new List<GameObject> { body_obj, body_fat_obj, turn_left_obj, turn_left_fat_obj, turn_right_obj, turn_right_fat_obj, tail_obj };
        isFat = false;
        eatFood = false;
        movement = Movement.straight;
        Move(movement);
    }

    public void Setup() {
        Start();
    }

    public void Move(Movement mov) {
        HideAll();
        movement = mov;
        switch (mov) {
            case Movement.straight:
                if (isFat) {
                    body_fat_obj.SetActive(true);
                }
                else {
                    body_obj.SetActive(true);
                }
                break;
            case Movement.turnLeft:
                if (isFat) {
                    turn_left_fat_obj.SetActive(true);
                }
                else {
                    turn_left_obj.SetActive(true);
                }
                break;
            case Movement.turnRight:
                if (isFat) {
                    turn_right_fat_obj.SetActive(true);
                }
                else {
                    turn_right_obj.SetActive(true);
                }
                break;
            case Movement.last:
                tail_obj.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void HideAll() {
        foreach (GameObject go in body_list) {
            if (go.activeSelf) {
                go.SetActive(false);
            }
        }
    }

    public void SetRenderer(bool value) {
        foreach (GameObject go in body_list) {
            go.GetComponent<MeshRenderer>().enabled = value;
        }
    }

}
