using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenatorCube : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject parentCube;
    [SerializeField] private int xDimention;
    [SerializeField] private int yDimention;
    [SerializeField] private int zDimention;
    [SerializeField] private int countBoom;

    private Vector3[] neighborCube = new Vector3[8] { Vector3.up, new Vector3(1, 1, 0), Vector3.right, new Vector3(1, -1, 0), Vector3.down, new Vector3(-1, -1, 0), Vector3.left, new Vector3(-1, 1, 0) };
    private Dictionary<Vector3, Cube> allCube = new Dictionary<Vector3, Cube>();
    void Start()
    {
        SpawnCube();
        SetSideNummber();
        Debug.Log(allCube);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SpawnCube()
    {
        parentCube = Instantiate(parentCube, new Vector3(xDimention / 2, yDimention / 2, zDimention / 2), Quaternion.identity);
        for (int z = 0; z < zDimention; z++)
        {
            for (int y = 0; y < yDimention; y++)
            {
                for (int x = 0; x < xDimention; x++)
                {
                    if (x == 0 || y == 0 || z == 0 || x == xDimention - 1 || y == yDimention - 1 || z == zDimention - 1)
                    {
                        GameObject cubeClone = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
                        allCube.Add(new Vector3(x, y, z),cubeClone.GetComponent<Cube>());
                        SetSideAndBoom(cubeClone);
                        cubeClone.transform.parent = parentCube.transform;
                    }
                }
            }
        }

    }
    void SetSideAndBoom(GameObject cubeClone)
    {
        if (countBoom != 0 && Random.Range(0, 2) == 1)
        {
            cubeClone.GetComponent<Cube>().IsBoom = true;
            countBoom--;
        }
        cubeClone.GetComponent<Cube>().ActiveSide(xDimention, yDimention, zDimention);
    }
    List<Cube> GetNeighborCube(Vector3 currentCube)
    {
        List<Cube> neighbor = new List<Cube>();
        foreach (var item in neighborCube)
        {
            Vector3 neighborPos = currentCube + item;
            if (allCube.ContainsKey(neighborPos))
                neighbor.Add(allCube[neighborPos]);
        }
        return neighbor;
    }
    void SetSideNummber()
    {
        foreach (KeyValuePair<Vector3,Cube> item in allCube)
        {
            List<Cube> neighbor = GetNeighborCube(item.Key);
            foreach (Cube child in neighbor)
            {
                if (child.IsBoom)
                    item.Value.Values++;
            }
            item.Value.SetValuesAllSide();
        }
    }
}
