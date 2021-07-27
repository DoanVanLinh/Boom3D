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
        GameObject boomClone = Instantiate(boomObj, transform.position, transform.rotation);
        boomClone.transform.parent = transform;
        boomClone.SetActive(false);
    }
    public void SetNumber(int values)
    {
        this.values = values;
        if (values != -1)
        {
            GameObject numberClone = Instantiate(numbers[values], transform.position, transform.rotation);
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
        if (!GameManager.Instance.IsEnd)
        {
            Cube parent = GetComponentInParent<Cube>();
            if (parent.Values == 0 && !parent.IsBoom)
                GenatorCube.instance.OpenGroupOfEmpty(parent.KeyGroup);
            if (parent.Values != 0)
                parent.OpenAllBox();
            if (parent.IsBoom)
            {
                GenatorCube.instance.OpenAllBoom();
                GameManager.Instance.IsEnd = true;
            }
        }
    }
}
