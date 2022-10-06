using UnityEngine;

namespace ITMO.Scripts
{
    public class RotateIcon : MonoBehaviour
    {
        [SerializeField] private GameObject img;

        private void FixedUpdate()
        {
            var rot = img.transform.eulerAngles;
            img.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + Time.fixedDeltaTime * 100);
        }
    }
}