using UnityEngine;

public class ColliderFollowAnimation : MonoBehaviour
{
    public Animator bossAnimator; // 보스 애니메이션 컴포넌트
    public MeshCollider meshCollider; // Mesh Collider 컴포넌트


    private void Update()
    {
        // 보스 애니메이션의 현재 위치를 가져옵니다
        Vector3 animationPosition = bossAnimator.transform.position;

        // Mesh Collider의 위치를 애니메이션 위치로 업데이트합니다
        meshCollider.transform.position = animationPosition;
    }
}