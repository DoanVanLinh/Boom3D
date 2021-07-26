using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] GameObject boomObj;
    [SerializeField] GameObject[] numbers;

    private int values = 0;

    public int Values { get => values; set => values = value; }
    private void Start()
    {
    }
    public void SetBoom()
    {
        values = -1;
        GameObject boomClone = Instantiate(boomObj, transform.position, Quaternion.identity);
        boomClone.transform.parent = transform;
        boomClone.SetActive(false);
    }
    public void SetNumber(int values)
    {
        this.values = values;
        if (values != -1)
        {
            GameObject numberClone = Instantiate(numbers[values], transform.position, Quaternion.identity);
            numberClone.transform.parent = transform;
            numberClone.SetActive(false);
        }
    }
    public void OpenBox()
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.transform.parent = transform.parent;
            child.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }
    private void OnMouseUp()
    {
        Cube parent = GetComponentInParent<Cube>();
        Debug.Log(parent.transform.position);
        if (parent.Values == 0&&!parent.IsBoom)
            GenatorCube.instance.OpenGroupOfEmpty(parent.KeyGroup);
        if(parent.Values != 0)
            parent.OpenAllBox();
    }
}
