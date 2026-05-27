using System.Collections.Generic;
using UnityEngine;

public interface IGameSystem
{
	public void OnAdded();
	public void OnGameStarted();
}

public static class G
{
	public static bool IsPaused
	{
		get => GameTime.IsPaused;
		set => GameTime.IsPaused = value;
	}
	public static float dt => Time.deltaTime * Time.timeScale;
	
	public static VampireMain vamp => Get<VampireMain>();

	private static List<IGameSystem> generic = new List<IGameSystem>();

	public static void Init()
	{
		generic.Clear();
		GameTime.Reset();
	}
	
	public static void Add(IGameSystem system)
	{
		generic.Add(system);
	}
	
	public static void Add<T>() where T : IGameSystem, new()
	{
		generic.Add(new T());
	}

	public static T Get<T>()
	{
		foreach ( var gn in generic )
		{
			if (gn is T gnt)
				return gnt;
		}
		
		Debug.LogError($"Could not find dependency {typeof(T).FullName}");
		
		return default(T);
	}

	public static void Start()
	{
		foreach ( var gn in generic )
		{
			gn.OnGameStarted();
		}
	}
}
