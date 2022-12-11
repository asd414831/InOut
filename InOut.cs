using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//使用此腳本前請先在工作檔放入DOTween插件
//在2022時還活著的連結：https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

//使用方式：把此腳本丟到你想讓他用簡單動畫出現或消失的物件上，然後依你的需求調整參數

public class InOut : MonoBehaviour
{
    //用來處理物件出現或消失的簡單動畫的程式
    [Header("曲線模式")]
    [Tooltip("決定這物體的緩入緩出曲線")]
    public Ease TheEase;

    public enum Type
    {
        LocalScale,
        LocalMove,
        LocalRotate,
    }
    [Header("動畫模式")]
    [Tooltip("決定這物體的動畫行為")]
    public Type TheAnime;

    public enum Type2
    {
        Absolute,
        Relative,
    }
    [Header("絕對相對")]
    [Tooltip("決定起點數值設置的是絕對的還是相對的\n絕對Absolute\n相對Relative")]
    public Type2 TheBegin;

    public enum Type3
    {
        Nothing,
        Loop,
        Close,
        Destory,
    }
    [Header("收回之後")]
    [Tooltip("決定收回後是放置、循環、關閉、還是刪除")]
    public Type3 TheBackMode;

    [Header("起始數值")]
    [Tooltip("依照各動畫模式代表其物件的起始變數為這數值")]
    public Vector3 Begin;

    [Header("動畫時間")]
    [Tooltip("決定執行一次的時間")]
    [Range(0f, 360f)]
    public float Time = 0.1f;

    [Header("啟用延遲時間")]
    [Tooltip("可以讓所有動畫延遲幾秒後再執行")]
    [Range(0f, 60f)]
    public float Delay_time = 0f;

    [Header("自動收回時間")]
    [Tooltip("可以讓出現的動畫幾秒後自動收回")]
    [Range(0f, 360f)]
    public float Autoback_time = 0f;

    [Header("停在起始嗎")]
    [Tooltip("決定是否停在起始位置")]
    public bool Stopin = false;

    [Header("預設倒帶嗎")]
    [Tooltip("決定是否初次啟用改成收回動畫")]
    public bool Backin = false;

    [Header("預設啟用嗎")]
    [Tooltip("決定是否一開始就要播放動畫，如果啟用收回後就會把物件關閉")]
    public bool Startin = true;

    Vector3 Now_sca;//目前大小
    Vector3 Now_pos;//目前位置
    Vector3 Now_rot;//目前角度
    bool BackIng = false;//確保BackIn後還可以Back
    void Awake()//定義目前狀態
    {
        Now_pos = transform.localPosition;
        Now_rot = transform.localEulerAngles;
        Now_sca = transform.localScale;
    }
    void OnEnable()//當他被打開時觸發
    {
        Enable();
    }
    void Enable()//觸發動畫
    {
        if (Startin)
        {
            if (!Backin)
            {
                if (TheBegin == Type2.Absolute)
                {
                    if (TheAnime == Type.LocalMove)
                        transform.localPosition = Begin;
                    if (TheAnime == Type.LocalRotate)
                        transform.localEulerAngles = Begin;
                    if (TheAnime == Type.LocalScale)
                        transform.localScale = Begin;
                }
                else
                {
                    if (TheAnime == Type.LocalMove)
                        transform.localPosition = Begin + Now_pos;
                    if (TheAnime == Type.LocalRotate)
                        transform.localEulerAngles = Begin + Now_rot;
                    if (TheAnime == Type.LocalScale)
                        transform.localScale = Begin + Now_sca;
                }

                Invoke("Anime", Delay_time);
            }
            else
            {
                if (TheBegin == Type2.Absolute)
                {
                    if (TheAnime == Type.LocalMove)
                        transform.localPosition = Now_pos;
                    if (TheAnime == Type.LocalRotate)
                        transform.localEulerAngles = Now_rot;
                    if (TheAnime == Type.LocalScale)
                        transform.localScale = Now_sca;
                }
                else
                {
                    if (TheAnime == Type.LocalMove)
                        transform.localPosition = Now_pos + Begin;
                    if (TheAnime == Type.LocalRotate)
                        transform.localEulerAngles = Now_rot + Begin;
                    if (TheAnime == Type.LocalScale)
                        transform.localScale = Now_sca + Begin;
                }
                BackIng = true;
                Invoke("Back", Delay_time);
            }

            if (Autoback_time > 0)
            {
                if (!Backin)
                    Invoke("Back", Delay_time + Autoback_time);
                else
                    Invoke("Anime", Delay_time + Autoback_time);
            }

        }
    }
    void Anime()//正常動畫
    {
        Sequence ani = DOTween.Sequence();
        ani.SetEase(TheEase);

        if (TheAnime == Type.LocalMove)
        {
            if (!Stopin)
            {
                ani.Join(transform.DOLocalMove(Now_pos, Time));
            }
        }
        if (TheAnime == Type.LocalRotate)
        {
            if (!Stopin)
            {
                ani.Join(transform.DOLocalRotate(Now_rot, Time, RotateMode.FastBeyond360));
            }
        }
        if (TheAnime == Type.LocalScale)
        {
            if (!Stopin)
            {
                ani.Join(transform.DOScale(Now_sca, Time));
            }
        }

        ani.Restart();

        if (Autoback_time > 0 && Backin)
            Invoke("BackEnd", Time);
    }
    void Back()//收回動畫
    {
        Sequence ani = DOTween.Sequence();
        ani.SetEase(TheEase);

        if (TheAnime == Type.LocalMove)
        {
            transform.localPosition = Now_pos;
            if (!Stopin)
            {
                if (TheBegin == Type2.Absolute)
                    ani.Join(transform.DOLocalMove(Begin, Time));
                else
                    ani.Join(transform.DOLocalMove(Begin + Now_pos, Time));
            }
        }
        if (TheAnime == Type.LocalRotate)
        {
            transform.localEulerAngles = Now_rot;
            if (!Stopin)
            {
                if (TheBegin == Type2.Absolute)
                    ani.Join(transform.DOLocalRotate(Begin, Time, RotateMode.FastBeyond360));
                else
                    ani.Join(transform.DOLocalRotate(Begin + Now_rot, Time, RotateMode.FastBeyond360));
            }

        }
        if (TheAnime == Type.LocalScale)
        {
            transform.localScale = Now_sca;
            if (!Stopin)
            {
                if (TheBegin == Type2.Absolute)
                    ani.Join(transform.DOScale(Begin, Time));
                else
                    ani.Join(transform.DOScale(Begin + Now_sca, Time));
            }
        }

        ani.Restart();

        if (BackIng)
        {
            BackIng = false;
            return;
        }
        Invoke("BackEnd", Time);
    }
    void BackEnd()//收回完畢
    {
        if (TheBackMode == Type3.Destory)
            Destroy(gameObject);
        if (TheBackMode == Type3.Close)
            gameObject.SetActive(false);
        if (TheBackMode == Type3.Loop)
            Enable();
    }
}
