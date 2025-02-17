using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraZoomScript : MonoBehaviour
{
    #region SINGLETON
    public static CameraZoomScript me;
    private void Awake()
    {
        me = this;
    }
    #endregion
    public bool enable = true;//Enable Zooming function
    public float zoomSpd;//Camera Zooming Speed
    [SerializeField]
    private float camMinSize;

    void Start()
    {

    }


    void Update()
    {
        if(enable)
        {
            FitCamera();
        }
    }
    public void FitCamera()
    {
        Bounds bound = new();
        //float zAxis = -10f;
        float[] bubblesZ = new float[BubbleMakerScript.me.bubbles.Count];
        for(int i = 0; i < bubblesZ.Length; i++)
        {
            bubblesZ.SetValue(BubbleMakerScript.me.bubbles[i].transform.position.z, i);
            if (BubbleMakerScript.me.bubbles[i] != null &&
                BubbleMakerScript.me.bubbles[i].GetComponentInChildren<BubbleScript>().active)
            {
                bound.Encapsulate(BubbleMakerScript.me.bubbles[i].GetComponentInChildren<Renderer>().bounds);
            }
        }
        if(bubblesZ.Length > 0)
        {
            //zAxis = bubblesZ.Min() - 1f;
        }
        Vector3 boundSize = bound.size;
        //float diagonal = Mathf.Sqrt((boundSize.x * boundSize.x) + (boundSize.y * boundSize.y) + (boundSize.z * boundSize.z));
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, (boundSize.y + 0.8f) / 2.0f, zoomSpd * Time.deltaTime);
        if(Camera.main.orthographicSize < camMinSize)
        {
            Camera.main.orthographicSize = camMinSize;
        }
        //transform.position = new Vector3(transform.position.x, transform.position.y, zAxis);
        Vector3 camCenter = new Vector3(bound.center.x, bound.center.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, camCenter, zoomSpd * Time.deltaTime);
    }
    public void SetCamZAxis()
    {
        float zAxis = -10f;
        float[] bubblesZ = new float[BubbleMasterScript.me.bubbles.Count];
        for (int i = 0; i < bubblesZ.Length; i++)
        {
            bubblesZ.SetValue(BubbleMasterScript.me.bubbles[i].transform.position.z, i);
        }
        if (bubblesZ.Length > 0)
        {
            zAxis = bubblesZ.Min() - 1f;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, zAxis);
    }
}
