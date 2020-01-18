﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MotionFramework;
using MotionFramework.Console;
using MotionFramework.Resource;
using MotionFramework.Event;
using MotionFramework.Config;
using MotionFramework.Audio;
using MotionFramework.Network;
using MotionFramework.Patch;
using MotionFramework.Scene;
using MotionFramework.Pool;

public class GameLauncher : MonoBehaviour
{
	void Awake()
	{
		// 不销毁游戏对象
		DontDestroyOnLoad(gameObject);

		// 注册日志系统
		MotionLog.RegisterCallback(HandleMotionFrameworkLog);

		// 初始化框架
		MotionEngine.Initialize(this);

		// 初始化控制台
		if (Application.isEditor || Debug.isDebugBuild)
			DeveloperConsole.Initialize();

		// 初始化应用
		InitAppliaction();
	}
	void Start()
	{
		CreateGameModules();
	}
	void Update()
	{
		MotionEngine.Update();
	}
	void OnGUI()
	{
		if (Application.isEditor || Debug.isDebugBuild)
			DeveloperConsole.DrawGUI();
	}

	private void InitAppliaction()
	{
		Application.runInBackground = true;
		Application.backgroundLoadingPriority = ThreadPriority.High;

		// 设置最大帧数
		Application.targetFrameRate = 60;

		// 屏幕不休眠
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	/// <summary>
	/// 监听框架日志
	/// </summary>
	private void HandleMotionFrameworkLog(ELogLevel logLevel, string log)
	{
		if (logLevel == ELogLevel.Log)
		{
			UnityEngine.Debug.Log(log);
		}
		else if (logLevel == ELogLevel.Error)
		{
			UnityEngine.Debug.LogError(log);
		}
		else if (logLevel == ELogLevel.Warning)
		{
			UnityEngine.Debug.LogWarning(log);
		}
		else if (logLevel == ELogLevel.Exception)
		{
			UnityEngine.Debug.LogError(log);
		}
		else
		{
			throw new NotImplementedException($"{logLevel}");
		}
	}

	private void CreateGameModules()
	{
		// 创建事件管理器
		MotionEngine.CreateModule<EventManager>();

		// 创建网络管理器
		var networkCreateParam = new NetworkManager.CreateParameters();
		networkCreateParam.PackageCoderType = typeof(ProtoPackageCoder);
		MotionEngine.CreateModule<NetworkManager>(networkCreateParam);

		// 创建资源管理器
		var resourceCreateParam = new ResourceManager.CreateParameters();
		resourceCreateParam.LocationRoot = GameDefine.AssetRootPath;
		resourceCreateParam.SimulationOnEditor = true;
		resourceCreateParam.BundleServices = null;
		resourceCreateParam.DecryptServices = null;
		resourceCreateParam.AutoReleaseInterval = 10f;
		MotionEngine.CreateModule<ResourceManager>(resourceCreateParam);

		// 创建音频管理器
		MotionEngine.CreateModule<AudioManager>();

		// 创建场景管理器
		MotionEngine.CreateModule<SceneManager>();

		// 创建对象池管理器
		MotionEngine.CreateModule<GameObjectPoolManager>();

		// 直接进入游戏
		MotionEngine.CreateModule<LuaManager>();	
	}
}