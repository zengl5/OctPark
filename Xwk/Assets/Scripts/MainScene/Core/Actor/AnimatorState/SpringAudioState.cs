﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YB.XWK.MainScene
{
    public class SpringAudioState : ActorAnimancerState
    {

        public SpringAudioState(IActor actorAimMgr, AnimState animState) : base(animState)
        {
            Init(actorAimMgr);
            this.state = animState;
        }
        protected void Init(IActor actorAimMgr)
        {
            this.ActorStateName = "idlestate";
            this.HasInteractiveState = true;
            OnInit(actorAimMgr);
        }
        public SpringAudioState(IActor actorAimMgr) : base(null)
        {
            Init(actorAimMgr);
            Play(this, _ActorAnimancerResConfig, RequestNextState);
        }
        public override void RequestNextState()
        {
            int percent = Random.Range(0, 100);
            if (percent <= 10)
            {
                m_ActorMgr.EnterNextState(ActorStateEnum.ACTOR_STATE_ENUM_AI_TALK);
            }
            else if (percent > 10 && percent < 90)
            {
                int percent3 = Random.Range(0, 100);
                if (percent3 <= 5)
                {
                    m_ActorMgr.EnterNextState(ActorStateEnum.ACTOR_STATE_ENUM_WALKAROUND);
                }
                else
                {
                    int random = Random.Range(0, 5);
                    //2/3是春节语音，1/3是正常说话
                    if (random < 3)
                    {
                        m_ActorMgr.EnterNextState(ActorStateEnum.ACTOR_STATE_ENUM_SPRING_AUDIO);
                    }
                    else
                    {
                        m_ActorMgr.EnterNextState(ActorStateEnum.ACTOR_STATE_ENUM_IDLE);
                    }
                }
            }
            else
            {
                //分身术或者金箍棒
                if (Random.Range(0, 100) < 50)
                {
                    m_ActorMgr.EnterNextState(ActorStateEnum.ACTOR_STATE_ENUM_GC);
                }
                else
                {
                    m_ActorMgr.EnterNextState(ActorStateEnum.ACTOR_STATE_ENUM_SP);
                }
            }
        }
        public override void Stop()
        {
            base.Stop();
            AudioManager.Instance.StopPlayerSound();
        }
    }
}

