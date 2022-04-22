using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ScreenRenderer : MonoBehaviour
{
    private Camera _camera;
    private RenderTexture _renderTexture;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.aspect = 1;
        _renderTexture = _camera.targetTexture;
        //_renderTexture.width = Screen.height;
        //_renderTexture.height = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
