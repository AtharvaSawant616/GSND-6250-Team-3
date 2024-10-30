using UnityEngine;

public class BGMcontroller : MonoBehaviour
{
    public AudioSource babyCryingSource;    // 改为 public
    public AudioSource soothingMusicSource; // 改为 public
    public AudioSource portalSoundSource;   // 改为 public
    public Transform playerTransform;       // 玩家位置的 Transform
    public Transform referencePoint;        // 起始点或参考点的 Transform

    public float maxVolume = 1.0f;          // 音量最大值
    public float maxDistance = 50.0f;       // 当距离超过此值时音量达到最大

    void Start()
    {
        Invoke("PlayMusic", 3f);
    }

    void Update()
    {
        AdjustVolumeBasedOnDistance();
    }

    private void PlayMusic(){
                // 初始化并播放音频
        babyCryingSource.loop = true;
        soothingMusicSource.loop = true;

        babyCryingSource.Play();
        soothingMusicSource.Play();
    }

    void AdjustVolumeBasedOnDistance()
    {
        float distance = Vector3.Distance(playerTransform.position, referencePoint.position);

        // 根据距离计算音量，距离越大音量越接近 maxVolume
        float volume = Mathf.Clamp(distance / maxDistance, 0, maxVolume);
        babyCryingSource.volume = volume;
    }
}
