using UnityEngine;
using com.guinealion.animatedBook;
using UnityEngine.UI;

public class Cookbook : MonoBehaviour
{

    [SerializeField] private BookPageCollider LeftPage;
    [SerializeField] private BookPageCollider RightPage;
    [SerializeField] private AudioClip PageTurnSound;
    [SerializeField] private GameObject PotionsTab;
    [SerializeField] private GameObject IngredientsTab;
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
        if (bookHelper.Progress <= 11)
        {
            PotionsTab.GetComponent<Button>().interactable = false;
            PotionsTab.GetComponent<TabFade>().SetActive(false);
            IngredientsTab.GetComponent<Button>().interactable = true;
            IngredientsTab.GetComponent<TabFade>().SetActive(true);
        }
    }

    void RightPageClick()
    {
        if (bookHelper.Progress < bookHelper.PageAmmount -1)
        {
            bookHelper.NextPage();
            AudioManager.PlaySoundAtLocationOnBeat(PageTurnSound, transform.position);
        }
        if (bookHelper.Progress >= 10)
        {
            PotionsTab.GetComponent<Button>().interactable = true;
            PotionsTab.GetComponent<TabFade>().SetActive(true);
            IngredientsTab.GetComponent<Button>().interactable = false;
            IngredientsTab.GetComponent<TabFade>().SetActive(false);
        }
    }

    public void JumpToIngredients() => bookHelper.GoToPage(11, true, 0.1f);
    public void JumpToPotions() => bookHelper.GoToPage(0, true, 0.1f);
}
