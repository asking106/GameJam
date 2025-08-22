using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	#region FIELD
	private Playermovement playerMv;
	private PlayerAnimation playerAnim;
	private Rigidbody2D myBody;
	private SpriteRenderer mySprite;
	private Animator myAnim;



	//Basic Attack
	[Header("Basic Attack")]
	public int noOfClick = 0;
	public float maxComboDelay = 1f;
	private float lastClickedTime = 0f;
	private bool basicAttackButtonPressed;


	//SkillAttack
	[Header("Skill Attack")]
	public bool skillAttack;
	public float attackPower = 50f;
	[HideInInspector] public bool skillAttackCharge;
	[HideInInspector] public bool skillAttacking;
	private bool skillAttackButtonPressed;
	private bool skillAttackButtonReleased;
    #endregion

    // Use this for initialization
    [Header("光剑设置")]
    public GameObject lightsaberPrefab;      // 光剑预制体
    public GameObject spawnEffectPrefab;     // 生成特效预制体
    public float formationTime = 1.5f;       // 光剑形成时间
    public float throwForce = 20f;           // 投掷力量
    public float returnDelay = 3f;           // 没有命中时的返回延迟
    public int PlayerDamage;
    private BoxCollider2D box;

    [Header("音频")]
    public AudioClip spawnSound;             // 生成音效
    public AudioClip throwSound;
    public AudioClip finishSound;// 投掷音效

    private GameObject currentLightsaber;    // 当前光剑实例
    private GameObject currentSpawnEffect;
    private bool isForming = false;          // 是否正在形成
    private bool isThrown = false;           // 是否已投掷
    private Vector3 formationScale;          // 光剑的目标尺寸
    public AudioSource audioSource;
    public float Rotatetime;
    private float timer;
    private bool isok;

    void Start()
    {
        timer = 0f;
        isok = false;
    }

    void Update()
    {
        
        // 鼠标左键按下时生成光剑
        if (Input.GetMouseButtonDown(0) && currentLightsaber == null)
        {
            StartFormation();
        }

        // 光剑形成过程
        if (isForming && currentLightsaber != null)
        {
            FormationProcess();
        }

        // 鼠标左键释放时投掷光剑
        if (Input.GetMouseButtonUp(0) && currentLightsaber != null && !isThrown)
        {
            Debug.Log("sssssssssssssss");
            ThrowLightsaber();
        }
    }
    Camera FindActiveCamera()
    {
        // 首先尝试获取主摄像机
        Camera mainCam = Camera.main;
        if (mainCam != null && mainCam.isActiveAndEnabled)
            return mainCam;

        // 如果没有主摄像机，查找所有激活的摄像机
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam != null && cam.isActiveAndEnabled && cam.gameObject.activeInHierarchy)
                return cam;
        }

        return null;
    }


    // 开始形成光剑
    void StartFormation()
    {
        // 获取鼠标在屏幕上的位置并转换为世界坐标
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -FindActiveCamera().transform.position.z;
        Vector3 spawnPosition = FindActiveCamera().ScreenToWorldPoint(mousePosition);

        // 实例化生成特效
        if (spawnEffectPrefab != null)
        {
            currentSpawnEffect= Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
        }

        // 实例化光剑
        currentLightsaber = Instantiate(lightsaberPrefab, spawnPosition, Quaternion.identity);
        currentLightsaber.GetComponent<Projectile2D>().damage =PlayerDamage;
        Debug.Log(PlayerDamage);

        // 设置初始尺寸为0
        formationScale = currentLightsaber.transform.localScale;
        currentLightsaber.transform.localScale = Vector3.zero;

        // 播放生成音效
        if (spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }

        isForming = true;
        isThrown = false;
    }

    // 光剑形成过程
    // 光剑形成过程
    void FormationProcess()
    {
        box = currentLightsaber.GetComponent<BoxCollider2D>();
        box.enabled = false;
        // 让光剑跟随鼠标移动
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -FindActiveCamera().transform.position.z;
        Vector3 targetPosition = FindActiveCamera().ScreenToWorldPoint(mousePosition);

        // 平滑移动到鼠标位置
        currentLightsaber.transform.position = Vector3.Lerp(
            currentLightsaber.transform.position,
            targetPosition,
            Time.deltaTime * 10f // 移动速度，可以调整
        );
        currentSpawnEffect.transform.position = currentLightsaber.transform.position;

        // 逐渐增大光剑尺寸直到达到目标尺寸
        currentLightsaber.transform.localScale = Vector3.Lerp(
            currentLightsaber.transform.localScale,
            formationScale,
            Time.deltaTime * (1f / formationTime)
        );

        timer += Time.deltaTime;
        if (timer >= formationTime && !isok)
        {
            isok = true;
            audioSource.PlayOneShot(finishSound);
        }

        // 检查是否完成形成
        if (Vector3.Distance(currentLightsaber.transform.localScale, formationScale) < 0.1f)
        {
            isForming = false;
            currentLightsaber.transform.localScale = formationScale;
        }

        // 计算目标旋转（朝向最近的敌人或鼠标方向）
        Vector2 throwDirection = FindThrowDirection();

        // 计算throwDirection与正上方向(Vector2.up)的夹角
        float targetAngle = Vector2.SignedAngle(Vector2.up, throwDirection);

        // 平滑旋转到目标角度
        float currentAngle = currentLightsaber.transform.rotation.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, Rotatetime * Time.deltaTime);

        currentLightsaber.transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }

    // 投掷光剑
    void ThrowLightsaber()
    {
        isForming = false;
        isThrown = true;
        timer = 0;
        
        box.enabled = true;

        // 添加刚体组件（如果不存在）
        Rigidbody2D rb = currentLightsaber.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = currentLightsaber.AddComponent<Rigidbody2D>();
        }

        // 设置刚体属性
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // 添加碰撞检测
       
        Projectile2D projectile = currentLightsaber.GetComponent<Projectile2D>();
        
        if (projectile == null)
        {
            projectile = currentLightsaber.AddComponent<Projectile2D>();
            
        }
        if(isok)
        {
            projectile.Initialize(returnDelay);
        }
        else
        {
            projectile.Initialize(0);
        }
       

        // 确定投掷方向（朝向最近的敌人或右方）
        Quaternion currentRotation = currentLightsaber.transform.rotation;

        // 将正上方向量(0,1)应用当前旋转
        Vector2 upDirection = Vector2.up;
        Vector2 rotatedDirection = currentRotation * upDirection;
      
        // 施加投掷力量
        rb.AddForce(rotatedDirection * throwForce, ForceMode2D.Impulse);

        // 播放投掷音效
        if (throwSound != null)
        {
            audioSource.Stop();
            if(isok)
            audioSource.PlayOneShot(throwSound);
        }

        // 重置当前光剑引用
        currentLightsaber = null;
        isok = false;
    }

    // 确定投掷方向
    Vector2 FindThrowDirection()
    {
        // 查找最近的敌人
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float nearestDistance = 20f;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(currentLightsaber.transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // 如果有敌人，朝向敌人；否则朝向右侧
        if (nearestEnemy != null)
        {
            return (nearestEnemy.transform.position - currentLightsaber.transform.position).normalized;
        }
        else
        {
            return Vector2.right;
        }
    }
}

// 光剑投射物脚本
 
 

