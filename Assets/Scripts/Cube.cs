using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    Dictionary<Vector3, Piece> side = new Dictionary<Vector3, Piece>();

    private bool isBoom;
    private int values = 0;
    public bool IsBoom { get => isBoom; set { isBoom = value; values = -1; } }
    public int Values { get => values; set => values = value; }

    private void Awake()
    {
        foreach (Transform item in transform)
        {
            side.Add(item.transform.localPosition, item.GetComponent<Piece>());
        }
    }
    public void ActiveSide(int xDimention, int yDimention, int zDimention)
    {
        if (transform.localPosition.x == 0)
            if (side.ContainsKey(Vector3.left))
                side[Vector3.left].gameObject.SetActive(true);
        if (transform.localPosition.y == 0)
            if (side.ContainsKey(Vector3.down))
                side[Vector3.down].gameObject.SetActive(true);
        if (transform.localPosition.z == 0)
            if (side.ContainsKey(Vector3.back))
                side[Vector3.back].gameObject.SetActive(true);

        if (transform.localPosition.x == xDimention - 1)
            if (side.ContainsKey(Vector3.right))
                side[Vector3.right].gameObject.SetActive(true);
        if (transform.localPosition.y == yDimention - 1)
            if (side.ContainsKey(Vector3.up))
                side[Vector3.up].gameObject.SetActive(true);
        if (transform.localPosition.z == zDimention - 1)
            if (side.ContainsKey(Vector3.forward))
                side[Vector3.forward].gameObject.SetActive(true);
        DestroyEmptySide();
        SetBoom();
    }
    void SetBoom()
    {
        if (isBoom)
            foreach (Transform item in transform)
            {
                item.GetComponent<Piece>().SetBoom();
            }
    }
    void DestroyEmptySide()
    {
        foreach (Transform item in transform)
        {
            if (item.gameObject.activeSelf == false)
                Destroy(item.gameObject);
        }
    }
    public void SetValuesAllSide()
    {
        foreach (Transform item in transform)
        {
            Piece pieceItem = item.GetComponent<Piece>();
            pieceItem.SetNumber(Values);
        }
    }
}
