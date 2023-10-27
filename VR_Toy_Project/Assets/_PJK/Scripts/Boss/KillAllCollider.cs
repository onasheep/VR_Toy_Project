using UnityEngine;

public class KillAllCollider : MonoBehaviour
{
    //터렛
    [SerializeField]
    private GameObject turret;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Turret"))
        {
            turret = other.gameObject;
            Destroy(turret);
        }
    }
}
