using UnityEngine;

public class MotionTester : MonoBehaviour
{
    [Header("Motion Testing")]
    public Animator animator;
    
    [Header("Test Controls")]
    [SerializeField] private string motionNameToTest = "daiji";
    
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    
    [ContextMenu("Play Daiji Motion")]
    public void PlayDaijiMotion()
    {
        PlayAnimationClip("daiji");
    }
    
    [ContextMenu("Play Zs1 Motion")]
    public void PlayZs1Motion()
    {
        PlayAnimationClip("zs1");
    }
    
    [ContextMenu("Play Custom Motion")]
    public void PlayCustomMotion()
    {
        PlayAnimationClip(motionNameToTest);
    }
    
    private void PlayAnimationClip(string clipName)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // AnimationClip을 직접 재생
            var clips = animator.runtimeAnimatorController.animationClips;
            
            foreach (var clip in clips)
            {
                if (clip.name.Contains(clipName))
                {
                    // Animator.Play로 클립 재생
                    animator.Play(clip.name);
                    Debug.Log($"모션 재생: {clip.name}");
                    return;
                }
            }
            
            Debug.LogError($"모션을 찾을 수 없음: {clipName}");
            Debug.Log("사용 가능한 모션들:");
            foreach (var clip in clips)
            {
                Debug.Log($"- {clip.name}");
            }
        }
        else
        {
            Debug.LogError("Animator 또는 AnimatorController를 찾을 수 없습니다!");
        }
    }
    
    [ContextMenu("List Available Animation Clips")]
    public void ListAvailableAnimationClips()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            Debug.Log($"사용 가능한 AnimationClip 개수: {clips.Length}");
            
            for (int i = 0; i < clips.Length; i++)
            {
                Debug.Log($"클립 {i}: {clips[i].name} (길이: {clips[i].length}초)");
            }
        }
        else
        {
            Debug.LogError("Animator 또는 AnimatorController를 찾을 수 없습니다!");
        }
    }
    
    [ContextMenu("Check Animator Status")]
    public void CheckAnimatorStatus()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        if (animator != null)
        {
            Debug.Log($"Animator 찾음: {animator.name}");
            Debug.Log($"AnimatorController 존재: {animator.runtimeAnimatorController != null}");
            Debug.Log($"현재 재생 중인 상태: {animator.GetCurrentAnimatorStateInfo(0).IsName("daiji")}");
            
            if (animator.runtimeAnimatorController != null)
            {
                Debug.Log($"클립 개수: {animator.runtimeAnimatorController.animationClips.Length}");
            }
        }
        else
        {
            Debug.LogError("Animator를 찾을 수 없습니다!");
        }
    }
    
    [ContextMenu("Stop All Motions")]
    public void StopAllMotions()
    {
        if (animator != null)
        {
            animator.Play("Default"); // 기본 상태로 돌아가기
            Debug.Log("모든 모션 정지");
        }
    }
}