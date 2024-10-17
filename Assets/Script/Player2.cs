using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class Player2 : MonoBehaviour
{
    public ClosePage page;
    public Text interactText;
    private bool nearMedKit = false;
    private GameObject currentMedKit;

    public Text medNum;
    public int medCount = 1;
    public Text ComputerinteractText;
    public Text Computer1interactText;
    private bool nearComputer = false;
    private bool nearComputer1 = false;
    private GameObject currentFiles;
    private GameObject currentFiles1;
    public Text FileNum;
    public int fileCount;


    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public float damagePerSec = 5;
    public float moveSpeed = 6.0f;  // 移动速度
    public float jumpHeight = 2.0f; // 跳跃高度
    public float mouseSensitivity = 100f;  // 鼠标灵敏度
    public Transform playerCamera;  // 摄像机

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private float gravity = -9.8f;
    private bool isGrounded;
    private bool getFile = false;
    private bool getFile1 = false;
    private float damageTimer = 0f;
    private bool isTakingDamage = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        interactText.gameObject.SetActive(false);
        ComputerinteractText.gameObject.SetActive(false);
        Computer1interactText.gameObject.SetActive(false);
        LockAndHideCursor();
        isTakingDamage = false;

    }

    private void FixedUpdate()
    {
        if (isTakingDamage) // 仅在启用扣血时进行扣血
        {
            damageTimer += Time.fixedDeltaTime;
            if (damageTimer >= 1.0f)
            {
                ContinueTakeDamage(damagePerSec);
                damageTimer = 0f; // 重置计时器
            }
        }
    }
        public void EnableDamage()
    {
        isTakingDamage = true; // 激活扣血逻辑
    }

    void Update()
    {

        if (nearMedKit && Input.GetKeyDown(KeyCode.E))
        {
            CollectMedKit();
        }
        if (nearComputer && Input.GetKeyDown(KeyCode.E))
        {
            CollectFiles();
        }
        if (nearComputer1 && Input.GetKeyDown(KeyCode.E))
        {
            CollectFiles1(); // 新增从电脑1收集文件的方法
        }

        // if (PageIsActive())
        // {
        //     UnlockAndShowCursor(); // 解锁并显示鼠标
        //     return; // 如果 UI 界面显示，停止执行后续代码
        // }
        // else
        // {
        //     LockAndHideCursor(); // 界面关闭时锁定并隐藏鼠标
        // }
        // 处理鼠标移动
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 限制垂直视角范围

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);

        // 检测是否在地面
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 保持玩家紧贴地面
        }

        // 获取WASD输入
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 应用重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        RecoveryHealth();

    }

    // public void EnablePlayerControl()
    // {
    //     LockAndHideCursor();
    // }
    private void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // private void UnlockAndShowCursor()
    // {
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    // }

    // private bool PageIsActive()
    // {
    //     return page.gameObject.activeSelf; // 检查 UI 是否显示
    // }
    private void CollectMedKit()
    {
        if (currentMedKit != null)
        {
            Destroy(currentMedKit); // 销毁 medkit
            medCount++;
            medNum.text = "X " + medCount.ToString();
            interactText.gameObject.SetActive(false);
            nearMedKit = false; // 关闭靠近 medkit 状态
            currentMedKit = null; // 清空当前 medkit 引用
        }
    }
    private void CollectFiles()
    {
        if (currentFiles != null)
        {
            fileCount++;
            getFile = true;
            FileNum.text = fileCount.ToString() + " / " + "2";
            ComputerinteractText.gameObject.SetActive(false);
            nearComputer = false; // 关闭靠近 medkit 状态
            currentFiles = null; // 清空当前 medkit 引用

        }
    }

    private void CollectFiles1()
    {
        if (currentFiles1 != null)
        {
            fileCount++;
            getFile1 = true;
            FileNum.text = fileCount.ToString() + " / " + "2";
            Computer1interactText.gameObject.SetActive(false); // 隐藏电脑1的交互文本
            nearComputer1 = false;
            currentFiles1 = null;
        }
    }

    private void ContinueTakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - (int)damage, 0);
        healthBar.setHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MedKit"))
        {
            nearMedKit = true; // 标记玩家已靠近 medkit
            currentMedKit = other.gameObject; // 记录当前的 medkit 对象
        }
        if (other.CompareTag("Computer") && !getFile)
        {//检测玩家碰到此电脑以及已经获得文件
            nearComputer = true;
            currentFiles = other.gameObject;
            ComputerinteractText.gameObject.SetActive(true);
        }
        if (other.CompareTag("Computer1") && !getFile1)
        {
            nearComputer1 = true;
            currentFiles1 = other.gameObject;
            Computer1interactText.gameObject.SetActive(true); // 显示电脑1的交互文本
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MedKit"))
        {
            nearMedKit = false; // 标记玩家离开 medkit
            currentMedKit = null; // 清空当前 medkit 引用
            interactText.gameObject.SetActive(false); // 隐藏交互提示
        }
        if (other.CompareTag("Computer"))
        {
            nearComputer = false;
            currentFiles = null;
            ComputerinteractText.gameObject.SetActive(false);
        }
        if (other.CompareTag("Computer1"))
        {
            nearComputer1 = false;
            currentFiles1 = null;
            Computer1interactText.gameObject.SetActive(false); // 隐藏电脑1的交互文本
        }
    }

    private void RecoveryHealth()
    {
        if (Input.GetKeyDown(KeyCode.Q) && medCount > 0)
        {
            if (currentHealth + 20 > 100)
            {
                currentHealth = 100;
            }
            else
            {
                currentHealth += 20;
            }
            medCount--;
            medNum.text = medCount.ToString();
        }
    }
}
