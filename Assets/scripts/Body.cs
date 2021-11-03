using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private GameObject head_obj;
    private GameObject body_obj;
    private GameObject body_fat_obj;
    private GameObject turn_left_obj;
    private GameObject turn_left_fat_obj;
    private GameObject turn_right_obj;
    private GameObject turn_right_fat_obj;
    private GameObject tail_obj;
    private List<GameObject> body_list;
    public LayerMask layer;
    public bool eatFood;
    //public bool eatFood;
    public int skinNumber;
    public enum Direction { Front, Left, Right };
    public enum Movement{
        straight,
        turnLeft,
        turnRight,
        last,
    }
    public enum Section {
        head,
        body,
        tail,
    }
    public Movement movement;

    // Start is called before the first frame update
    void Awake()
    {
        head_obj = transform.Find("head_low").gameObject;
        body_obj = transform.Find("body_low").gameObject;
        body_fat_obj = transform.Find("body_fat_low").gameObject;
        turn_left_obj = transform.Find("turn_left_low").gameObject;
        turn_left_fat_obj = transform.Find("turn_left_fat_low").gameObject;
        turn_right_obj = transform.Find("turn_right_low").gameObject;
        turn_right_fat_obj = transform.Find("turn_right_fat_low").gameObject;
        tail_obj = transform.Find("tail_low").gameObject;

        body_list = new List<GameObject> {
            head_obj, 
            body_obj, 
            body_fat_obj, 
            turn_left_obj, 
            turn_left_fat_obj, 
            turn_right_obj, 
            turn_right_fat_obj, 
            tail_obj
        };
        eatFood = false;
        //eatFood = false;
        movement = Movement.straight;
        foreach (GameObject go in body_list) {
            go.SetActive(false);
        }
    }

    public void Setup() {
        //Start();
    }

    public void SetSection(Section sec, Movement mov = Movement.straight) {
        HideAll();
        switch (sec) {
            case Section.head:
                head_obj.SetActive(true);
                head_obj.transform.parent.gameObject.AddComponent<Rigidbody>();
                head_obj.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                head_obj.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
                break;
            case Section.body:
                Move(mov);
                break;
            case Section.tail:
                tail_obj.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void Move(Movement mov) {
        HideAll();
        movement = mov;
        switch (mov) {
            case Movement.straight:
                if (eatFood) {
                    body_fat_obj.SetActive(true);
                }
                else {
                    body_obj.SetActive(true);
                }
                break;
            case Movement.turnLeft:
                if (eatFood) {
                    turn_left_fat_obj.SetActive(true);
                }
                else {
                    turn_left_obj.SetActive(true);
                }
                break;
            case Movement.turnRight:
                if (eatFood) {
                    turn_right_fat_obj.SetActive(true);
                }
                else {
                    turn_right_obj.SetActive(true);
                }
                break;
            default:
                break;
        }
    }

    private void HideAll() {
        foreach (GameObject go in body_list) {
            if (go.activeSelf) {
                go.SetActive(false);
                return;
            }
        }
    }

    public void SetRenderer(bool value) {
        foreach (GameObject go in body_list) {
            go.GetComponent<MeshRenderer>().enabled = value;
        }
    }

    public bool CheckObstacle(Direction direction) {
        Vector3 dir = transform.forward;
        if (direction == Direction.Left) {
            dir = -transform.right;
        }
        else if (direction == Direction.Right) {
            dir = transform.right;
        }
        var ray = new Ray(transform.position, dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.4f, layer)) {
            return true;
        }
        return false;
    }

    public void ChangeSkin(int number) {
        Renderer rend;
        int y = number / 4;
        int x = number % 4;
        foreach (var item in body_list) {
            rend = item.GetComponent<Renderer>();
            rend.material.SetTextureOffset("_MainTex", new Vector2(x * 0.25f, 1 - (y * 0.25f)));
        }
    }

    public IEnumerator Blink(float waitTime) {
        var endTime = Time.time + waitTime;
        if (head_obj.activeSelf) {
            GetComponent<Rigidbody>().detectCollisions = false;
        }
        while (Time.time < endTime) {
            foreach (GameObject section in body_list) {
                if (section != null) {
                    section.GetComponent<Renderer>().enabled = false;
                }
            }
            yield return new WaitForSeconds(0.2f);
            foreach (GameObject section in body_list) {
                if (section != null) {
                    section.GetComponent<Renderer>().enabled = true;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
        if (head_obj != null && head_obj.activeSelf) {
            GetComponent<Rigidbody>().detectCollisions = true;
        }
    }
}
