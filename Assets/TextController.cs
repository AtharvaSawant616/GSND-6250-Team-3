using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
public Text uiText; // 关联的UI Text组件
    public string[] messages; // 想要显示的文本内容数组
    public float displayDuration = 3f; // 每个文本显示的时间

    private int currentMessageIndex = 0;

    void Start()
    {
        Invoke("ShowTextDelay", 5f);
    }

    IEnumerator ShowTextSequence()
    {
        while (currentMessageIndex < messages.Length)
        {
            uiText.text = messages[currentMessageIndex];
            uiText.gameObject.SetActive(true);
            yield return new WaitForSeconds(displayDuration);

            uiText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f); // 可选：让文本消失一小段时间后再显示下一个文本

            currentMessageIndex++;
        }
    }
    private void ShowTextDelay(){
                if (messages.Length > 0)
        {
            StartCoroutine(ShowTextSequence());
        }

    }
}
