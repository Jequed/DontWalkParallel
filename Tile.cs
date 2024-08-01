using UnityEngine;

public abstract class Tile : DWPObject
{
    private BoxCollider boxCollider;

    protected override void GameStart()
    {
        base.GameStart();

        boxCollider = GetComponent<BoxCollider>();
    }

    protected override void GameUpdate()
    {
        base.GameUpdate();

        if (GlobalData.player.transform.position.x > transform.position.x + boxCollider.center.x - boxCollider.size.x * 0.5f * transform.localScale.x &&
            GlobalData.player.transform.position.x < transform.position.x + boxCollider.center.x + boxCollider.size.x * 0.5f * transform.localScale.x &&
            GlobalData.player.transform.position.y > transform.position.y + boxCollider.center.y - boxCollider.size.y * 0.5f * transform.localScale.y &&
            GlobalData.player.transform.position.y < transform.position.y + boxCollider.center.y + boxCollider.size.y * 0.5f * transform.localScale.y)
        {
            OnPlayerSteppedOn();
        }
    }

    protected abstract void OnPlayerSteppedOn();
}