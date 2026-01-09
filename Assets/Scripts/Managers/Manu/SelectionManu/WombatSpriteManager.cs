using System.Collections.Generic;
using UnityEngine;

public class WombatSpriteManager : MonoBehaviour
{
    public List<GameObject> sprites;
    public Transform leftPos;
    public Transform rightPos;
    public GameObject wr;
    public GameObject wl;


    private void Awake()
    {
        wr = SpawnSprite(0);
        wl = SpawnSprite(0, true);
    }

    private GameObject SpawnSprite(int index, bool right = false)
    {
        GameObject obj = Instantiate(sprites[index], right ? rightPos.position : leftPos.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(2, 2, 2);
        if (right) { obj.transform.localScale = new Vector3(-2, 2, 2); }
        return obj;
    }

    private void DeleteSprite(GameObject obj)
    {
        Destroy(obj);
    }

    public void ChangeSprite(int w, int index, bool right = false)
    {

        if (w == 1)
        {
            DeleteSprite(wl);
            wl = SpawnSprite(index, true);
        }
        else
        {
            DeleteSprite(wr);
            wr = SpawnSprite(index, right);
        }
    }
}
