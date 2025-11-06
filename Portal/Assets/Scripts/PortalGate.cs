using System.Collections;
using UnityEngine;

public class PortalGate : MonoBehaviour
{
    public Animation m_Animation;
    public AnimationClip m_OpenAnimationClip;
    public AnimationClip m_OpenedAnimationClip;
    public AnimationClip m_CloseAnimationClip;
    public AnimationClip m_ClosedAnimationClip;

    public bool m_IsOpened;

    public enum TStates
    {
        OPEN,
        OPENED,
        CLOSE,
        CLOSED
    }

    TStates m_States;
    private void Start()
    {
        if (m_IsOpened)
            SetOpenedState();
        else
            SetClosedState();
    }

    void SetOpenedState()
    {
        m_States = TStates.OPENED;
        m_Animation.Play(m_OpenedAnimationClip.name);
    }
    void SetClosedState()
    {
        m_States = TStates.CLOSED;
        m_Animation.Play(m_ClosedAnimationClip.name);
    }

    public void Open()
    {
        if (m_States == TStates.CLOSED)
        {
            m_States = TStates.OPEN;
            m_Animation.Play(m_OpenAnimationClip.name);
            StartCoroutine(SetState(m_OpenAnimationClip.length, TStates.OPENED));
        }
    }
    public void Close()
    {
        if (m_States == TStates.OPENED)
        {
            m_States = TStates.CLOSE;
            m_Animation.Play(m_CloseAnimationClip.name);
            StartCoroutine(SetState(m_CloseAnimationClip.length, TStates.CLOSED));
        }
    }

    IEnumerator SetState(float AnimationTime, TStates State)
    {
        yield return new WaitForSeconds(AnimationTime);
        m_States=State;
    }
}
