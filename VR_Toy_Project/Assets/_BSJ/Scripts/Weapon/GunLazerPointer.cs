using Meta.WitAi;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GunLazerPointer : MonoBehaviour
{
    // 어떤 손인지 판단
    public bool isLeftHand = default;

    // 크로스 헤어
    public Canvas crosshairCan;

    // 레이저 포인트를 발사할 라인 렌더러
    LineRenderer lineRenderer;

    // 레이저 포인터의 최대 거리
    [SerializeField]
    private float lrMaxDistance = 200f;

    // 레이저 포인터의 칼라
    public Color lazerColor;

    // 총구 쪽으로 살짝 이동하기 위한 Offset
    private float shotPointOffset = 0.15f;

    private void Awake()
    {
        // 라인 렌더러 셋팅
        lineRenderer = GetComponent<LineRenderer>();
        lazerColor.a = 0.5f;
        lineRenderer.startColor = lazerColor;
        lineRenderer.endColor = lazerColor;

        // 크로스헤어 색상 변경
        crosshairCan.GetComponentInChildren<Image>().color = lazerColor;
    }

    private void Update()
    {
        // Bullet과 UI 레이어 빼고 검출
        int layerMask = ((1 << LayerMask.NameToLayer("Bullet")) | (1 << LayerMask.NameToLayer("UI")) | 1 << LayerMask.NameToLayer("DetectArea") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("TurretBullet"));
        layerMask = ~layerMask;

        if (isLeftHand)
        {
            // 왼쪽 컨트롤러 기준으로 Ray를 만든다. (살짝 총구 쪽에서부터 시작하도록)
            Ray ray = new Ray(BSJVRInput.LHandPosition + BSJVRInput.LHand.forward * shotPointOffset, BSJVRInput.LHandDirection);
            RaycastHit hitInfo;

            // 충돌이 있다면?
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
            {
                // Ray가 부딪힌 지점에 라인 그리기
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, hitInfo.point);
            }

            // 충돌이 없다면?
            else
            {
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, ray.origin + BSJVRInput.LHandDirection * lrMaxDistance);
            }
        }       // if : 왼쪽 핸드 기준으로 레이저 포인터 만들기

        else
        {
            // 오른쪽 컨트롤러 기준으로 Ray를 만든다. (살짝 총구 쪽에서부터 시작하도록)
            Ray ray = new Ray(BSJVRInput.RHandPosition + BSJVRInput.RHand.forward * shotPointOffset, BSJVRInput.RHandDirection);
            RaycastHit hitInfo;

            // 충돌이 있다면?
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask))
            {
                // Ray가 부딪힌 지점에 라인 그리기
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, hitInfo.point);

                // 부딪힌 지점에 크로스 헤어 그리기
                crosshairCan.transform.position = hitInfo.point;
            }

            // 충돌이 없다면?
            else
            {
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, ray.origin + BSJVRInput.RHandDirection * lrMaxDistance);

                crosshairCan.transform.position = ray.origin + BSJVRInput.RHandDirection * lrMaxDistance;
            }
        }       // else : 오른쪽 핸드 기준으로 레이저 포인터 만들기
    }
}
