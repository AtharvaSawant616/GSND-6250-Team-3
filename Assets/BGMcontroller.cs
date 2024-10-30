using UnityEngine;

public class BGMcontroller : MonoBehaviour
{
    public AudioSource babyCryingSource;    // 改为 public
    public AudioSource soothingMusicSource; // 改为 public
    public AudioSource portalSoundSource;   // 改为 public

    void Start()
    {
        // 初始化并播放音频
        babyCryingSource.loop = true;
        soothingMusicSource.loop = true;

        babyCryingSource.Play();
        soothingMusicSource.Play();
    }
}
