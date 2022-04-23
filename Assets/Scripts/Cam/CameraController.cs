using General;
using UnityEngine;

namespace Cam
{
    public class CameraController : MonoBehaviour
    {
        private void Awake()
        {
            Hub.Register(this);
        }
    }
}