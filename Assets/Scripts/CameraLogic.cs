using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraLogic : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    public bool camera_instage = false;
    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - this.transform.position;
    }
    public void SetStage(bool stage){
        camera_instage = stage;
    }
    // Update is called once per frame
    void Update()
    {
        if (!camera_instage)
            this.transform.position = new Vector3(target.position.x - offset.x,this.transform.position.y, this.transform.position.z);
        
    }
}
