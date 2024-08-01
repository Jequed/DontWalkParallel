using UnityEngine;

public class PhobiaRed : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    bool startedPhobia = false;

    private float lastPhobiaTime = 0.0f;

    protected void Update()
    {
        if (GlobalData.playMode)
        {
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, GlobalData.player.PhobiaLevel / GlobalData.player.MaximumPhobia);// Mathf.Sin(Time.time - lastPhobiaTime));
        }
        else
        {
            spriteRenderer.gameObject.SetActive(false);
        }
    }
}