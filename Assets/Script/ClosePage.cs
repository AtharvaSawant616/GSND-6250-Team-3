using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePage : MonoBehaviour
{
    public Canvas Page;
    public Player2 player;

    void Start()
    {
        // 设定延迟时间，5秒后自动关闭界面
        Invoke("CloseCurrentPage", 5f);
    }

    public void CloseCurrentPage()
    {
        Page.gameObject.SetActive(false);
        player.EnableDamage();
    }
}
