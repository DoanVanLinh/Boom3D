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

    private List<Vector3> neighborCubeSdieZ = new List<Vector3>() { new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0) };
    private List<Vector3> neighborCubeSdieX = new List<Vector3>() { new Vector3(0, 1, 1), new Vector3(0, 1, -1), new Vector3(0, -1, 1), new Vector3(0, -1, -1) };
    private List<Vector3> neighborCubeSdieY = new List<Vector3>() { new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1) };
    private List<Vector3> neighborCubeEdge = new List<Vector3>() { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

    private Dictionary<Vector3, Cube> allCube = new Dictionary<Vector3, Cube>();
    void Start()
    {
        SpawnCube();
        SetSideNummber();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void SpawnCube()
    {
        parentCube = Instantiate(parentCube, new Vector3((xDimention - 1) / 2f, (yDimention - 1) / 2f, (zDimention - 1) / 2f), Quaternion.identity);

        parentCube.AddComponent<RotateBox>();
        for (int z = 0; z < zDimention; z++)
        {
            for (int y = 0; y < yDimention; y++)
            {
                for (int x = 0; x < xDimention; x++)
                {
                    if (x == 0 || y == 0 || z == 0 || x == xDimention - 1 || y == yDimention - 1 || z == zDimention - 1)
                    {
                        GameObject cubeClone = Instantiate(cube, new Vector3(x, y, z), Quaternion.identity);
                        allCube.Add(new Vector3(x, y, z), cubeClone.GetComponent<Cube>());
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
        List<Vector3> neighborCube = new List<Vector3>();
        //Edge
        if (((currentCube.z == 0 || currentCube.z == xDimention - 1) && (currentCube.z == 0 || currentCube.z == zDimention - 1)) ||
            ((currentCube.z == 0 || currentCube.z == xDimention - 1) && (currentCube.y == 0 || currentCube.y == zDimention - 1)) ||
            ((currentCube.z == 0 || currentCube.z == zDimention - 1) && (currentCube.y == 0 || currentCube.y == zDimention - 1)))
        {
            neighborCube.AddRange(neighborCubeEdge);
            neighborCube.AddRange(neighborCubeSdieX);
            neighborCube.AddRange(neighborCubeSdieY);
            neighborCube.AddRange(neighborCubeSdieZ);
        }
        else//Side
        {
            if (currentCube.x == 0 || currentCube.x == xDimention - 1)
                neighborCube.AddRange(neighborCubeSdieX);
            if (currentCube.y == 0 || currentCube.y == yDimention - 1)
                neighborCube.AddRange(neighborCubeSdieY);
            if (currentCube.z == 0 || currentCube.z == zDimention - 1)
                neighborCube.AddRange(neighborCubeSdieZ);
        }
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
        foreach (KeyValuePair<Vector3, Cube> item in allCube)
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
