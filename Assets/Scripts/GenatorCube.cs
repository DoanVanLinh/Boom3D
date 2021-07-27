using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GenatorCube : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject parentCube;
    [SerializeField] private int countBoom;
    [SerializeField] private GameObject XInput;
    [SerializeField] private GameObject YInput;
    [SerializeField] private GameObject ZInput;

    private List<Vector3> neighborCubeSdieZ = new List<Vector3>() { new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0), Vector3.up, Vector3.down, Vector3.right, Vector3.left };
    private List<Vector3> neighborCubeSdieX = new List<Vector3>() { new Vector3(0, 1, 1), new Vector3(0, 1, -1), new Vector3(0, -1, 1), new Vector3(0, -1, -1), Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
    private List<Vector3> neighborCubeSdieY = new List<Vector3>() { new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1), Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    private Dictionary<int, List<Cube>> groupOfEmpty;
    private Dictionary<Vector3, Cube> allCube;
    private List<Cube> allBoom;
    private int xDimention = 5;
    private int yDimention = 5;
    private int zDimention = 5;
    private GameObject parentClone;
    #region Singleton
    public static GenatorCube instance;
    private Camera mainCam;

    public int XDimention { get => xDimention; set => xDimention = value; }
    public int YDimention { get => yDimention; set => yDimention = value; }
    public int ZDimention { get => zDimention; set => zDimention = value; }

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
        mainCam = Camera.main;
        Create();
    }
    public void Create()
    {
        GameManager.Instance.IsEnd = false;
        SpawnCube();
        SetBoom();
        SetSideNummber();
        SetGroupOfEmpty();
        parentClone.transform.position = new Vector3(mainCam.transform.position.x, mainCam.transform.position.y, parentClone.transform.position.z);
    }
    void GetInput(ref int dimention, string text)
    {
        if (text != ""&& int.Parse(text)>0)
            dimention = int.Parse(text);
    }
    void SpawnCube()
    {
        
        GetInput(ref xDimention, XInput.GetComponent<TMP_InputField>().text);
        GetInput(ref yDimention, YInput.GetComponent<TMP_InputField>().text);
        GetInput(ref zDimention, ZInput.GetComponent<TMP_InputField>().text);

        countBoom = (xDimention * yDimention * zDimention - (xDimention - 1) * (yDimention - 1) * (zDimention - 1))/3;

        groupOfEmpty = new Dictionary<int, List<Cube>>();
        allCube = new Dictionary<Vector3, Cube>();
        allBoom = new List<Cube>();

        Destroy(parentClone);
        parentClone = Instantiate(parentCube, new Vector3((xDimention - 1) / 2f, (yDimention - 1) / 2f, (zDimention - 1) / 2f), Quaternion.identity);
        parentClone.AddComponent<RotateBox>();
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
                        cubeClone.transform.parent = parentClone.transform;
                    }
                }
            }
        }

    }
    void SetBoom()
    {
        while (countBoom != 0)
        {
            Cube randomPos = allCube.Values.ElementAt(Random.Range(0, allCube.Count));
            if (!allBoom.Contains(randomPos))
            {
                allBoom.Add(randomPos);
                countBoom--;
            }
        }
        foreach (Cube item in allBoom)
        {
            item.GetComponent<Cube>().SetBoom();
        }
    }
    public void OpenAllBoom()
    {
        foreach (Cube item in allBoom)
        {
            item.OpenAllBox();
        }
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
    List<Cube> GetNeighborCube(Vector3 currentCube)
    {
        List<Cube> neighbor = new List<Cube>();
        List<Vector3> neighborCube = new List<Vector3>();
        //Edge
        if (((currentCube.x == 0 || currentCube.x == xDimention - 1) && (currentCube.z == 0 || currentCube.z == zDimention - 1)) ||
            ((currentCube.z == 0 || currentCube.z == zDimention - 1) && (currentCube.y == 0 || currentCube.y == yDimention - 1)) ||
            ((currentCube.x == 0 || currentCube.x == xDimention - 1) && (currentCube.y == 0 || currentCube.y == yDimention - 1)))
        {
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
        HashSet<Vector3> DeleteDuplicate = new HashSet<Vector3>();
        foreach (var item in neighborCube)
        {
            if (DeleteDuplicate.Add(item))
            {
                Vector3 neighborPos = currentCube + item;
                if (allCube.ContainsKey(neighborPos))
                    neighbor.Add(allCube[neighborPos]);
            }
        }
        return neighbor;
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
        if (((currentCube.x == 0 || currentCube.x == xDimention - 1) && (currentCube.z == 0 || currentCube.z == zDimention - 1)) ||
            ((currentCube.z == 0 || currentCube.z == zDimention - 1) && (currentCube.y == 0 || currentCube.y == yDimention - 1)) ||
            ((currentCube.x == 0 || currentCube.x == xDimention - 1) && (currentCube.y == 0 || currentCube.y == yDimention - 1)))
        {
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
        HashSet<Vector3> DeleteDuplicate = new HashSet<Vector3>();
        foreach (Vector3 item in neighborCube)
        {
            if (DeleteDuplicate.Add(item))
            {
                Vector3 key = item + currentCube;
                if (allCube.ContainsKey(key) && allCube[key].KeyGroup == -1)
                    neighbor.Add(allCube[key]);
            }
        }
        return neighbor;
    }
    public void OpenGroupOfEmpty(int keyGroup)
    {
        List<Cube> group = groupOfEmpty[keyGroup];
        foreach (Cube item in group)
            item.OpenAllBox();
    }
}
