using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    private Dictionary<int, List<Cube>> groupOfEmpty = new Dictionary<int, List<Cube>>();
    private Cube currentCube;
    private Dictionary<Vector3, Cube> allCube = new Dictionary<Vector3, Cube>();
    #region Singleton
    public static GenatorCube instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    #endregion
    void Start()
    {
        SpawnCube();
        SetBoom();
        SetSideNummber();
        SetGroupOfEmpty();
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
                        cubeClone.GetComponent<Cube>().ActiveSide(xDimention, yDimention, zDimention);
                        cubeClone.transform.parent = parentCube.transform;
                    }
                }
            }
        }

    }
    void SetBoom()
    {
        List<Cube> randomBoom = new List<Cube>();
        while (countBoom != 0)
        {
            Cube randomPos = allCube.Values.ElementAt(Random.Range(0, allCube.Count));
            if (!randomBoom.Contains(randomPos))
            {
                randomBoom.Add(randomPos);
                countBoom--;
            }
        }
        foreach (Cube item in randomBoom)
        {
            item.GetComponent<Cube>().SetBoom();
        }
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
            if (!item.Value.IsBoom)
            {
                List<Cube> neighbor = GetNeighborCube(item.Key);
                foreach (Cube child in neighbor)
                {
                    if (child.IsBoom)
                        item.Value.Values++;
                }
            }
        }
        foreach (KeyValuePair<Vector3, Cube> item in allCube)
        {
            item.Value.SetValuesAllSide();
        }
    }
    void SetGroupOfEmpty()
    {
        int keyOfGroup = 0;
        foreach (KeyValuePair<Vector3, Cube> item in allCube)
        {
            if (item.Value.Values == 0 && item.Value.KeyGroup == -1 && !item.Value.IsBoom)
            {
                if (!groupOfEmpty.ContainsKey(keyOfGroup))
                    groupOfEmpty.Add(keyOfGroup, GetGroupOfEmpty(item.Value, ref keyOfGroup));
                keyOfGroup++;
            }
        }
    }
    private List<Cube> GetGroupOfEmpty(Cube currentCube, ref int keyOfGroup)
    {
        List<Cube> groupOfET = new List<Cube>();
        currentCube.KeyGroup = keyOfGroup;
        if (!groupOfET.Contains(currentCube))
            groupOfET.Add(currentCube);
        if (currentCube.Values > 0)
            return groupOfET;
        if (currentCube.Values == 0)//is EMPTY
        {
            List<Cube> neighbor = GetNeighborEmpty(currentCube.transform.position);
            foreach (Cube item in neighbor)
            {
                groupOfET.AddRange(GetGroupOfEmpty(item, ref keyOfGroup));
            }
        }
        return groupOfET;
    }

    private List<Cube> GetNeighborEmpty(Vector3 currentCube)
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
        foreach (Vector3 item in neighborCube)
        {
            Vector3 key = item + currentCube;
            if (allCube.ContainsKey(key) && allCube[key].KeyGroup == -1)
                neighbor.Add(allCube[key]);
        }
        return neighbor;
    }
    public void OpenGroupOfEmpty(int keyGroup)
    {
        List<Cube> group = groupOfEmpty[keyGroup];
        foreach (Cube item in group)
        {
            item.OpenAllBox();
        }
    }
}
