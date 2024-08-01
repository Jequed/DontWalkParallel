using UnityEngine;

public class PhobiaMeter : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer background;

    [SerializeField]
    private SpriteRenderer bar;

    private float barStartPositionY;

    void Start()
    {
        barStartPositionY = bar.transform.localPosition.y;
    }

    void Update()
    {
        float percentage = GlobalData.player.PhobiaLevel / GlobalData.player.MaximumPhobia;

        bar.transform.localScale = new Vector3(bar.transform.localScale.x, background.transform.localScale.y * percentage, bar.transform.localScale.z);

        bar.transform.localPosition = new Vector3(bar.transform.localPosition.x, barStartPositionY + (background.transform.localPosition.y - barStartPositionY) * percentage, bar.transform.localPosition.z);
    }
}