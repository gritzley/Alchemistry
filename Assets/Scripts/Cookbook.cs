using UnityEngine;
using com.guinealion.animatedBook;

public class Cookbook : MonoBehaviour
{

    [SerializeField] private BookPageCollider LeftPage;
    [SerializeField] private BookPageCollider RightPage;
    [SerializeField] private AudioClip PageTurnSound;
    private LightweightBookHelper bookHelper;

    void Start()
    {
        bookHelper = GetComponent<LightweightBookHelper>();
        LeftPage.OnClick = LeftPageClick;
        RightPage.OnClick = RightPageClick;
    }

    void LeftPageClick()
    {
        if (bookHelper.Progress > 0)
        {
            bookHelper.PrevPage();
            AudioManager.PlaySoundAtLocationOnBeat(PageTurnSound, transform.position);
        }
    }

    void RightPageClick()
    {
        if (bookHelper.Progress < bookHelper.PageAmmount -1)
        {
            bookHelper.NextPage();
            AudioManager.PlaySoundAtLocationOnBeat(PageTurnSound, transform.position);
        }
    }
}
