using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class VideoTrigger : MonoBehaviour
{
	[Header("Video")]
	public VideoPlayer videoPlayer;                    // 视频播放器组件
	public GameObject videoPanel;                      // 视频UI面板（初始隐藏）
	public RawImage videoDisplay;                      // 显示视频的RawImage

	[Header("Trigger Settings")]
	public string playerTag = "Player";               // 触发的对象 Tag
	public bool triggerOnce = true;                   // 只触发一次
	public float retriggerCooldown = 1.0f;            // 可重复触发时的冷却

	[Header("Player Control")]
	public bool disablePlayerMovementDuringVideo = true; // 视频播放期间禁用玩家移动

	private bool hasTriggered = false;
	private float nextAllowedTime = 0f;
	private Playermovement cachedPlayerMove = null;
	private bool isVideoPlaying = false;

	private void Reset()
	{
		var col = GetComponent<Collider2D>();
		if (col != null) col.isTrigger = true;
	}

	private void Start()
	{
		// 初始化视频播放器
		if (videoPlayer == null)
		{
			videoPlayer = GetComponent<VideoPlayer>();
		}

		// 设置视频播放完成事件
		if (videoPlayer != null)
		{
			videoPlayer.loopPointReached += OnVideoFinished;
		}

		// 初始隐藏视频面板
		if (videoPanel != null)
		{
			videoPanel.SetActive(false);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag(playerTag)) return;
		if (videoPlayer == null || videoPanel == null) return;
		if (triggerOnce && hasTriggered) return;
		if (Time.unscaledTime < nextAllowedTime) return;
		if (isVideoPlaying) return;

		cachedPlayerMove = other.GetComponent<Playermovement>();
		if (disablePlayerMovementDuringVideo && cachedPlayerMove != null)
			cachedPlayerMove.enabled = false;

		PlayVideo();

		hasTriggered = true;
		nextAllowedTime = Time.unscaledTime + retriggerCooldown;
	}

	private void PlayVideo()
	{
		if (videoPlayer == null || videoPanel == null) return;

		isVideoPlaying = true;
		videoPanel.SetActive(true);

		// 设置视频到RawImage显示
		if (videoDisplay != null)
		{
			videoPlayer.targetTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 24);
			videoDisplay.texture = videoPlayer.targetTexture;
		}

		// 开始播放
		videoPlayer.Play();
	}

	private void OnVideoFinished(VideoPlayer vp)
	{
		isVideoPlaying = false;
		
		// 隐藏视频面板
		if (videoPanel != null)
		{
			videoPanel.SetActive(false);
		}

		// 恢复玩家移动
		if (disablePlayerMovementDuringVideo && cachedPlayerMove != null)
		{
			cachedPlayerMove.enabled = true;
			cachedPlayerMove = null;
		}

		// 重置视频播放器
		if (videoPlayer != null)
		{
			videoPlayer.Stop();
			videoPlayer.frame = 0;
		}
	}

	private void OnDestroy()
	{
		if (videoPlayer != null)
		{
			videoPlayer.loopPointReached -= OnVideoFinished;
		}
	}
}
