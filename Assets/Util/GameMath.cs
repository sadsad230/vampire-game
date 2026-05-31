using System.Collections.Generic;
using UnityEngine;

public static class GameMath
{
	public static bool IsColliding(Vector3 v1, Vector3 v2, float f)
	{
		return Vector3.Distance( v1, v2 ) < f;
	}
	
	public static bool IsOnScreen(Vector3 position)
	{
		Vector3 viewportPoint = Camera.main.WorldToViewportPoint(position);
		
		bool onScreen = viewportPoint.x >= 0 && viewportPoint.x <= 1
		                                     && viewportPoint.y >= 0 && viewportPoint.y <= 1
		                                     && viewportPoint.z > 0; 

		return onScreen;
	}
	
	public static (Transform, float) FindClosest<T>(List<T> transforms, Vector2 pos) where T : Component
	{
		Transform closestTransform = null;
		float minDistance = float.MaxValue; 

		foreach (T t in transforms)
		{
			float distance = Vector2.Distance(t.transform.position, pos);
			if (distance < minDistance)
			{
				minDistance = distance;
				closestTransform = t.transform;
			}
		}

		return (closestTransform, minDistance);
	}
	
	public static float AngleToPos2D(Vector2 from, Vector2 to)
	{
		var direction = Direction(from, to);
		float angleRadians = Mathf.Atan2(direction.y, direction.x);
		float angleDegrees = angleRadians * Mathf.Rad2Deg;
		return (angleDegrees + 360) % 360;
	}
	
	public static Vector3 Direction(Vector3 origin, Vector3 target)
	{
		return (target - origin).normalized;
	}
	
	public static Vector2 AngleToVector2(float angleDegrees)
	{
		float angleRadians = angleDegrees * Mathf.Deg2Rad;
		return new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
	}
	
	public static Vector3 CanvasToWorld(RectTransform target)
	{
		var componentInParent = target.GetComponentInParent<Canvas>();
		var rectTransform = componentInParent.GetComponent<RectTransform>();

		Vector2 screenPosition = RectTransformUtility.PixelAdjustPoint(target.position, target, componentInParent);
		
		Vector3 worldPosition;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPosition, Camera.main, out worldPosition);
		return worldPosition;
	}
	
	public static Vector3 GetInputAxis2D()
	{
		var inputAxis2D = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		return inputAxis2D.normalized;
	}
	
	public static List<Vector2> Arc(Vector2 origin, float radius, Vector2 direction, int numberOfPoints)
	{
		List<Vector2> points = new List<Vector2>();
		
		direction.Normalize();
		
		float angle = Mathf.Atan2(direction.y, direction.x);

		float verticalRange = radius / 2;
		
		float startAngle = angle - Mathf.Asin(verticalRange / radius);
		float endAngle = angle + Mathf.Asin(verticalRange / radius);
		
		float angleIncrement = (endAngle - startAngle) / (numberOfPoints - 1);

		for (int i = 0; i < numberOfPoints; i++)
		{
			float currentAngle = startAngle + angleIncrement * i;
			float x = origin.x + radius * Mathf.Cos(currentAngle);
			float y = origin.y + radius * Mathf.Sin(currentAngle);
			points.Add(new Vector2(x, y));
		}

		return points;
	}
	
	public static float Inverse01(float damage)
	{
		return 1f - Mathf.Clamp01(damage);
	}
}
