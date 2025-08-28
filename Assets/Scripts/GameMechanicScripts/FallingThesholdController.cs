using UnityEngine;

namespace Watermelon
{
    public class FallingThesholdController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            GameObject.Destroy(other.gameObject);
        }
    }
}
