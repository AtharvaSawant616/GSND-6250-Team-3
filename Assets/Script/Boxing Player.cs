using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoxingPlayer : MonoBehaviour
{
    public GameObject EndTextManage;
    public float speed = 6.0f;
    public float gravity = -9.8f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;
    public Image depressionReport;
    public Text PressEtoOpen;
    public Text AliPoster;
    public Text GloveText;
    public Text ChampionText;
    public Text MarryText;
    public Text SoItMeText;
    public Text WifeDepressionText;
    public Text LucyDiary;
    public Text ACText;
    public Text WifeDead;
    public Text Table;
    public Image DiaryImage;
    public Image DeathReport;
    public Image Trophy2D;
    public Image AccidentalReport;
    public BGMcontroller bGMcontroller;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    private bool isNearReport = false; // 标记玩家是否靠近 Depression report
    private bool isNearDiary = false;
    private bool isNearDeathReport = false;
    private bool isNearTvPoster = false;
    private bool isNearChampion = false;
    private bool isNearAR = false;
    private bool isNearAi = false;
    private bool isNearGlove = false;
    private bool isnearWinPicture = false;
    private bool isnearMarryPicture = false;
    private bool isnearTable = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (up and down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        // playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (left and right)
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);

        // Movement input
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.W)) z = 1f;    // Forward
        if (Input.GetKey(KeyCode.S)) z = -1f;   // Backward
        if (Input.GetKey(KeyCode.A)) x = -1f;   // Left
        if (Input.GetKey(KeyCode.D)) x = 1f;    // Right

        // Move based on local forward/right direction
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 检测玩家是否在 Depression report 附近并按下 E 键
        if (isNearReport && Input.GetKeyDown(KeyCode.E))
        {
            PressEtoOpen.gameObject.SetActive(false); // 隐藏提示
            depressionReport.gameObject.SetActive(true); // 显示报告
            StartCoroutine(WifeDreText());
        }
        if (isNearDiary && Input.GetKeyDown(KeyCode.E))
        {
            PressEtoOpen.gameObject.SetActive(false);
            DiaryImage.gameObject.SetActive(true);
            StartCoroutine(LucyDiaryText());
        }
        if (isNearDeathReport && Input.GetKeyDown(KeyCode.E))
        {
            PressEtoOpen.gameObject.SetActive(false);
            DeathReport.gameObject.SetActive(true);
            StartCoroutine(WifePassAway());
        }
        if (isNearTvPoster && Input.GetKeyDown(KeyCode.E))
        {
            bGMcontroller.AliTvSource.Play();
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (isNearChampion && Input.GetKeyDown(KeyCode.E))
        {
            bGMcontroller.ChampionWin.Play();
            Trophy2D.gameObject.SetActive(true);
            StartCoroutine(ItsMeText());


            PressEtoOpen.gameObject.SetActive(false);
        }
        if (isNearAR && Input.GetKeyDown(KeyCode.E))
        {
            AccidentalReport.gameObject.SetActive(true);
            PressEtoOpen.gameObject.SetActive(false);
            StartCoroutine(ACTextMethod());
        }
        if (isNearAi && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ShowAliPosterText());
        }
        if (isnearTable && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TableText());
        }
        if (isNearAi && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ShowAliPosterText());
        }
        if (isNearAi && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ShowAliPosterText());
        }
        if (isNearGlove && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ShowGloveText());
        }
        if (isnearWinPicture && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ShowWinningText());
        }
        if (isnearMarryPicture && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ShowMarryText());
        }
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            //depressionReport.gameObject.SetActive(false);
            //DiaryImage.gameObject.SetActive(false);
            //DeathReport.gameObject.SetActive(false);
            //Trophy2D.gameObject.SetActive(false);
            //AccidentalReport.gameObject.SetActive(false);
            Debug.Log("Pressing the left mouse button");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Depression report"))
        {
            isNearReport = true; // 玩家靠近报告
            PressEtoOpen.gameObject.SetActive(true); // 显示提示
        }
        if (other.CompareTag("Diary"))
        {
            isNearDiary = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if (other.CompareTag("DeathReport"))
        {
            isNearDeathReport = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if(other.CompareTag("BGM")){
            bGMcontroller.BGM.Play();
        }
        if (other.CompareTag("ExitGate"))
        {
            bGMcontroller.babyCryingSource.Stop();
            bGMcontroller.soothingMusicSource.Stop();
            bGMcontroller.portalSoundSource.Stop();
            bGMcontroller.AliTvSource.Stop();
            bGMcontroller.ChampionWin.Stop();
        }
        if (other.CompareTag("AliTv"))
        {
            isNearTvPoster = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if (other.CompareTag("ChampionWin"))
        {
            isNearChampion = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if (other.CompareTag("AccidentalReport"))
        {
            isNearAR = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if (other.CompareTag("Ali"))
        {
            isNearAi = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if (other.CompareTag("Glove"))
        {
            isNearGlove = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if (other.CompareTag("WinPicture"))
        {
            isnearWinPicture = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if (other.CompareTag("MarryPicture"))
        {
            isnearMarryPicture = true;
            PressEtoOpen.gameObject.SetActive(true);
        }
        if(other.CompareTag("EndTextTrigger")){
            EndTextManage.gameObject.SetActive(true);
        }
        if(other.CompareTag("Table"))
        {
            PressEtoOpen.gameObject.SetActive(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Depression report"))
        {
            isNearReport = false; // 玩家离开报告区域
            PressEtoOpen.gameObject.SetActive(false); // 隐藏提示
        }
        if (other.CompareTag("Diary"))
        {
            isNearDiary = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("DeathReport"))
        {
            isNearDeathReport = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("AliTv"))
        {
            isNearTvPoster = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("ChampionWin"))
        {
            isNearChampion = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("AccidentalReport"))
        {
            isNearAR = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("Ali"))
        {
            isNearAi = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("Glove"))
        {
            isNearGlove = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("WinPicture"))
        {
            isnearWinPicture = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if (other.CompareTag("MarryPicture"))
        {
            isnearMarryPicture = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
        if(other.CompareTag("Table"))
        {
            isnearTable = false;
            PressEtoOpen.gameObject.SetActive(false);
        }
    }
    private IEnumerator ShowAliPosterText()
    {
        yield return new WaitForSeconds(1f);
        AliPoster.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        AliPoster.gameObject.SetActive(false);
    }

    private IEnumerator ShowGloveText()
    {
        yield return new WaitForSeconds(1.5f);
        GloveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        GloveText.gameObject.SetActive(false);
    }
    private IEnumerator ShowWinningText()
    {
        yield return new WaitForSeconds(1f);
        ChampionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        ChampionText.gameObject.SetActive(false);
    }
    private IEnumerator ShowMarryText()
    {
        MarryText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        MarryText.gameObject.SetActive(false);
    }
    private IEnumerator ItsMeText()
    {
        yield return new WaitForSeconds(1f);
        SoItMeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(6f);
        SoItMeText.gameObject.SetActive(false);
    }
    private IEnumerator WifeDreText()
    {
        yield return new WaitForSeconds(4f);
        WifeDepressionText.gameObject.SetActive(true);
        yield return new WaitForSeconds(6f);
        WifeDepressionText.gameObject.SetActive(false);
    }
    private IEnumerator LucyDiaryText()
    {
        yield return new WaitForSeconds(3.5f);
        LucyDiary.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        LucyDiary.gameObject.SetActive(false);
    }
    private IEnumerator ACTextMethod()
    {   yield return new WaitForSeconds(3.5f);
        ACText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        ACText.gameObject.SetActive(false);
    }
    private IEnumerator WifePassAway()
    {
        yield return new WaitForSeconds(1.5f);
        WifeDead.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        WifeDead.gameObject.SetActive(false);
    }

    private IEnumerator TableText()
    {
        yield return new WaitForSeconds(1f);
        Table.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        Table.gameObject.SetActive(false);
    }


}
