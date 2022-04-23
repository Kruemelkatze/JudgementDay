using System.Collections;
using System.Collections.Generic;
using General;
using UnityEngine;

[ExecuteAlways]
public class ScreenRenderer : MonoBehaviour
{
    private Camera _camera;
    private RenderTexture _renderTexture;

    [SerializeField] private Vector2 screenAspect = new Vector2(25f, 32f);


    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.aspect = screenAspect.x / screenAspect.y;
        _renderTexture = _camera.targetTexture;
        //_renderTexture.width = Screen.height;
        //_renderTexture.height = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
    }
}