using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenatorCube))]
public class GameManager : MonoBehaviour
{
    [SerializeField] float smoothRotateBox;

    float touchesPrevPosDifference, touchesCurPosDifference, zoomModifier;

    Vector2 firstTouchPrevPos, secondTouchPrevPos;

    [SerializeField]
    float zoomModifierSpeed = 0.1f;

    private Camera mainCam;
    private float timer;
    private bool isHold;

    private static GameManager instance;

    public static GameManager Instance { get => instance; set => instance = value; }
    public float SmoothRotateBox { get => smoothRotateBox; set => smoothRotateBox = value; }

    void Start()
    {
        mainCam = Camera.main;
        Singleton();
    }

    // Update is called once per frame
    void Update()
    {
        Zoom();
        timer += isHold == true ? Time.deltaTime : 0;
    }
    void Singleton()
    {
        if (instance == null)
        {
            instance = this;
        }else
            Destroy(gameObject);
    }
    void Zoom()
    {
        if (Input.touchCount == 2)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
            secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

            touchesPrevPosDifference = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
            touchesCurPosDifference = (firstTouch.position - secondTouch.position).magnitude;

            zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;

            if (touchesPrevPosDifference > touchesCurPosDifference)
                mainCam.fieldOfView += zoomModifier;
            if (touchesPrevPosDifference < touchesCurPosDifference)
                mainCam.fieldOfView -= zoomModifier;

        }

        mainCam.orthographicSize = Mathf.Clamp(mainCam.fieldOfView, 2f, 300f);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Test()
    {
        
    }
}
