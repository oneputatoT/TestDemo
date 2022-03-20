using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerGhost : MonoBehaviour
{
    private PlayerMovement move;
    private AnimationManager anim;
    public Transform ghostsParent;     //Ѱ�Ҵ�Ż�Ӱ�ĸ���
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

        //��ȡ��Ӱ�Ӽ�Transform
        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            //�����ǻ�Ӱ��λ�õ���������ʱ����һ�̵�λ��
            s.AppendCallback(() => currentGhost.position = move.transform.position);
            //����ͬ��
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = anim.sr.flipX);
            //ģ��һ��
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().sprite = anim.sr.sprite);
            //��Ӱ��һ֡��ɫ
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
