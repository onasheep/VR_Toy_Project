using System.Collections;
using UnityEngine;
public class BossBombAttack : MonoBehaviour
{
    public float initialAngle = 30f;    // 처음 날라가는 각도
    public GameObject target;
    private float Shottime;
    private Rigidbody rb;               // Rigidbody

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Shottime = 0;


    }

    private void Start()
    {
        StartCoroutine(Firsttime());

    }


    IEnumerator Firsttime()
    {
        rb.useGravity = false;
        Vector3 velocity = new(15, 0, 0);
        rb.velocity = velocity;

        yield return new WaitForSeconds(1.5f);
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(1.5f);
        rb.useGravity = true;
        // 포물선 운동
        velocity = GetVelocity(transform.position, target.transform.position, initialAngle);
        rb.velocity = velocity;
    }







    public Vector3 GetVelocity(Vector3 startPos, Vector3 target, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 targetPos = new Vector3(target.x, 0, target.z); // y 좌표를 0으로 설정하여 y 값을 무시
        Vector3 shotPos = new Vector3(startPos.x, 0, startPos.z); // y 좌표를 0으로 설정하여 y 값을 무시

        float distance = Vector3.Distance(targetPos, shotPos);
        float yOffset = startPos.y - target.y; // yOffset을 0으로 설정하여 높이를 고려하지 않음

        if (distance <= 0 || yOffset <= 0)
        {
            return Vector3.zero;
        }

        float initialVelocity
           = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity
             = new Vector3(0f, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects
            = Vector3.Angle(Vector3.forward, targetPos - shotPos) * (target.x > startPos.x ? 1 : -1);
        Vector3 finalVelocity
            = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }

}