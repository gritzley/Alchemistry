using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.guinealion.animatedBook;

public class Cookbook : MonoBehaviour
{

    [SerializeField] private BookPageCollider LeftPage;
    [SerializeField] private BookPageCollider RightPage;
    [SerializeField] private float FoldSpeed = 0.2f;
    private LightweightBookHelper bookHelper;

    void Start()
    {
        bookHelper = GetComponent<LightweightBookHelper>();
        LeftPage.OnClick = LeftPageClick;
        RightPage.OnClick = RightPageClick;
    }

    void LeftPageClick()
    {
        Debug.Log("left clicked");
        if (bookHelper.Progress == 0)
        {
            bookHelper.Orientation = -1;
            ToggleOpen();
        }
        else
        {
            bookHelper.PrevPage();
        }
    }

    void RightPageClick()
    {
        Debug.Log("right clicked");
        if (bookHelper.Progress == bookHelper.PageAmmount - 1)
        {
            bookHelper.Orientation = 1;
            ToggleOpen();
        }
        else
        {
            bookHelper.NextPage();
        }
    }

    void ToggleOpen()
    {
        if (bookHelper.OpenAmmount == 0)
        {
            bookHelper.Open();
        }
        if (bookHelper.OpenAmmount == 1)
        {
            bookHelper.Close();
        }
    }
}
