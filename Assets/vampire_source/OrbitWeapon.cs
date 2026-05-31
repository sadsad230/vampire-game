using DG.Tweening;
using UnityEngine;

public class OrbitWeapon : ManagedBehaviour
{
    public float OrbitRadius = 2f;
    public float RotationDuration = 2f;  
    
    private Tween rotationTween;
    
    public void InitOrbitSystem(int count, GameObject prefab)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        
        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            float radians = angle * Mathf.Deg2Rad;
            
            GameObject projectile = Instantiate(prefab, transform);
            
            float x = Mathf.Cos(radians) * OrbitRadius;
            float y = Mathf.Sin(radians) * OrbitRadius;
            projectile.transform.localPosition = new Vector3(x, y, 0f);
        }
        rotationTween = transform.DORotate(new Vector3(0f, 0f, 360f), RotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void Update()
    {
        if (rotationTween == null || !rotationTween.IsActive()) return;
        
        if (G.IsPaused && rotationTween.IsPlaying())
        {
            rotationTween.Pause();
        }
        else if (!G.IsPaused && !rotationTween.IsPlaying())
        {
            rotationTween.Play();
        }
    }

    private void OnDestroy()
    {
        rotationTween?.Kill();
    }
}