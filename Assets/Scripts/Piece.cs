using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] GameObject boomObj;
    [SerializeField] GameObject[] numbers;

    private int values = 0;

    public int Values { get => values; set => values = value; }

    public void SetBoom()
    {
        values = -1;
        GameObject boomClone = Instantiate(boomObj, transform.position, Quaternion.identity);
        boomClone.transform.parent = transform;
    }
    public void SetNumber(int values)
    {
        this.values = values;
        if (values != -1)
        {
            GameObject numberClone = Instantiate(numbers[values], transform.position, Quaternion.identity);
            numberClone.transform.parent = transform;
        }
    }
    private void OnMouseDown()
    {
        if (transform.childCount > 0)
            transform.GetChild(0).transform.parent = transform.parent;
        Destroy(gameObject);
    }
}
