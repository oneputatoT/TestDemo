using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerGhost : MonoBehaviour
{
    private PlayerMovement move;
    private AnimationManager anim;
    public Transform ghostsParent;     //寻找存放幻影的父级
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;

    private void Start()
    {
        anim = FindObjectOfType<AnimationManager>();
        move = FindObjectOfType<PlayerMovement>();
    }

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        //获取幻影子集Transform
        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            //将我们幻影的位置等于人物冲刺时候那一刻的位置
            s.AppendCallback(() => currentGhost.position = move.transform.position);
            //方向同向
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = anim.sr.flipX);
            //模型一样
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().sprite = anim.sr.sprite);
            //幻影第一帧颜色
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendCallback(() =>FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(fadeColor, fadeTime);
    }
}
