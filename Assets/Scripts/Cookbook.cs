using UnityEngine;
using com.guinealion.animatedBook;

public class Cookbook : MonoBehaviour
{

    [SerializeField] private BookPageCollider LeftPage;
    [SerializeField] private BookPageCollider RightPage;
    private LightweightBookHelper bookHelper;

    void Start()
    {
        bookHelper = GetComponent<LightweightBookHelper>();
        LeftPage.OnClick = () => bookHelper.PrevPage();
        RightPage.OnClick = () => bookHelper.NextPage();
    }
}
