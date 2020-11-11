﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using XBL.Core;
using Assets.Scripts.C_Framework;
using XWK.Common.UI_Reward;

namespace XWK.VITALITY.MOUDLE {
    public class VitalityMoudleMgr : MonoBehaviour
    {
        private GameObject _BaiYinSpiritRole;
        private GameObject _XwkRole;
        private GameObject _BaiYinHeadEffect;
        private GameObject _XwkBodyEffect;
        private GameObject _PowerEffect;
        private Vector3 _BaiYinRolePos;
        private Vector3 _XwkRolePos;
        private BoxCollider _BYEffectBoxcollider;

        private float _CurrentTime=0f;
        private float _TotalTime = 8f;
        private bool _TouchOver = false;
        private bool _StartChangeColor = false;

        private Material _PowerEffect_Noise3_Mat;
        private Material _PowerEffect_Noise3_2_Mat;
        private Material _PowerEffect_Noise3_4_Mat;

        private bool _FirstFlag = true;
        private float _Progress = 0f;
        private Camera _CurrentCamera;
        private Animator _XwkAnimator;
        private Animator _BaiYinAnimator;
        private Tween DelayStop;
        private bool _CanPlaySound = true;
        private float _XwkEffectSceleFactor = 0.5f;
        private bool _TouchFlag = false;
        private GameObject _TouchEffect;
        private string _InvokeMethod = "PlayTaskAudio";
        private string _InvokeTouchMethod = "ShowEffect";
      //  private string _PressUIName = "UI_effect_CA";
        private GameObject _GuideHand;
        // Use this for initialization
        void Start()
        {
            _CanPlaySound = true;
            _TouchOver = false;
            _StartChangeColor = false;
            InitRole();
            PauseUIMoudleMgr.Instance.mGameGoMainCityAction -= QuitGame;
            PauseUIMoudleMgr.Instance.mGameGoMainCityAction += QuitGame;
            PlayTaskAudio();
            // C_UIMgr.Instance.OpenUI(_PressUIName, new Vector3(989.0f, 9.7f, 0.0f));
            // C_UIMgr.Instance.OpenUI(_PressUIName, new Vector3(1060.0f, 12.8f, 0.0f),8f);
            RewardUIManager.GetInstance().RegisterStory(MotionType.Click, SourceType.Interaction, 5, (b) => {
                if (!_TouchOver)
                {
                    TouchOverEvent();
                }
            });
        }
        public void PlayTaskAudio()
        {
            AudioManager.Instance.PlayerSound("story/byjl/sound/byjl_031.ogg",false,()=> {
                Invoke(_InvokeMethod, 5);
            });
        }
        private void PlaySound()
        {
            AudioManager.Instance.PlayerSound("public/sound/common_150.ogg", false, () =>
            {
                DelayStop = DOVirtual.DelayedCall(3f, () =>
                {
                    PlaySound();
                });
            });
        }
        private void InitRole()
        {
            _BaiYinSpiritRole = GameResMgr.Instance.LoadResource<GameObject>("public/mesh/jl_00002/public_model_jl00002_1#mesh.prefab", true);
            _BaiYinRolePos= _BaiYinSpiritRole.transform.position = new Vector3(11.29f, 0.02f, 0.834f);
            _BaiYinSpiritRole.transform.localRotation = Quaternion.Euler(new Vector3(0f, -39.9f, 0f));
            _BaiYinSpiritRole.gameObject.SetActive(true);

            _XwkRole = GameResMgr.Instance.LoadResource<GameObject>("public/mesh/wukong/public_model_wukong#mesh.prefab", true);
            _XwkRolePos = _XwkRole.transform.position = new Vector3(9.46f, 0.0223f, 0.7f);
            _XwkRole.transform.localRotation = Quaternion.Euler(new Vector3(0f,48.7f,0f));
            _XwkRole.gameObject.SetActive(true);

            _XwkBodyEffect = GameResMgr.Instance.LoadResource<GameObject>("public/Hero_Effect/prefab/public_effect_nlq_big.prefab", true);
            _XwkBodyEffect.transform.SetParent(_XwkRole.transform);
            _XwkBodyEffect.transform.localPosition = new Vector3(0,55f,0);
            _XwkBodyEffect.transform.localScale = new Vector3(50f,50f,50f);
            _XwkBodyEffect.SetActive(false);

            _BaiYinHeadEffect = GameResMgr.Instance.LoadResource<GameObject>("public/Hero_Effect/prefab/public_effect_nlq_small.prefab", true);
            _BaiYinHeadEffect.transform.position = new Vector3(_BaiYinRolePos.x, _BaiYinRolePos.y+1.5f, _BaiYinRolePos.z);
            GameObject BYEffectHeadClone = new GameObject("public_effect_nlq_small_collider");
            BYEffectHeadClone.transform.position = _BaiYinHeadEffect.transform.position;
            _BYEffectBoxcollider = BYEffectHeadClone.GetAddComponent<BoxCollider>();
           _BYEffectBoxcollider.size = new Vector3(1.5f, 2f, 1.5f);
           _BYEffectBoxcollider.center = new Vector3(0, 1f, 0);
            _TouchEffect = GameResMgr.Instance.LoadResource<GameObject>("public/hero_effect/prefab/public_effect_dianji.prefab", true);
            _TouchEffect.transform.position = new Vector3(11.24f,1.46f,1.235f);
            _TouchEffect.gameObject.SetActive(false);
            Invoke(_InvokeTouchMethod, 3f);


            _PowerEffect = GameResMgr.Instance.LoadResource<GameObject>("public/Hero_Effect/prefab/public_effect_nlcs.prefab", true);
            _PowerEffect.transform.position = _BaiYinHeadEffect.transform.position;
            _PowerEffect.SetActive(false);
            _PowerEffect_Noise3_Mat = Utility.FindChild(_PowerEffect.transform, "pd_wk_lg2_s_wk_noise3").GetComponent<MeshRenderer>().material;
            _PowerEffect_Noise3_2_Mat = Utility.FindChild(_PowerEffect.transform, "Plane003_wk_lg3_s_wk_noise3_2").GetComponent<MeshRenderer>().material;
            _PowerEffect_Noise3_4_Mat = Utility.FindChild(_PowerEffect.transform, "Plane004_wk_lg3_s_wk_noise3_2").GetComponent<MeshRenderer>().material;

            _CurrentCamera = GameObject.FindGameObjectWithTag("ActorCamera").GetComponent<Camera>();
            _CurrentCamera.transform.position = new Vector3(10.7f,0.8f,3.9f);
            _CurrentCamera.transform.rotation = Quaternion.Euler(new Vector3(-1.74f,184.4f,0f));

            _XwkAnimator = _XwkRole.GetAddComponent<Animator>();
            _XwkAnimator.runtimeAnimatorController = GameResMgr.Instance.LoadResource<RuntimeAnimatorController>("public/animatorcontroller/public_model_wukong1#mesh_animatorcontoller");
            _BaiYinAnimator = _BaiYinSpiritRole.GetAddComponent<Animator>();
            _BaiYinAnimator.runtimeAnimatorController = GameResMgr.Instance.LoadResource<RuntimeAnimatorController>("public/animatorcontroller/public_model_jl00002_1#mesh_animatorcontoller");

            _GuideHand = GameResMgr.Instance.LoadResource<GameObject>("public/Hero_Effect/prefab/public_effect_shoudianji.prefab", true);
            _GuideHand.transform.position = new Vector3(11.01f, 1.28f, 1.71f);
            _GuideHand.transform.rotation =Quaternion.Euler( new Vector3(0f,180f,0f));
            _GuideHand.gameObject.SetActive(true);
        }
        public void ShowEffect()
        {
            if (_TouchEffect!=null)
            {
                _TouchEffect.gameObject.SetActive(true);
            }
        }
        void PlayXwkUpAnimation()
        {
            _XwkAnimator.SetTrigger("wukong_xz_fukong01#anim");
        }
        // Update is called once per frame
        void Update()
        {
            TouchEvent();
            TowardXwk();
        }
        private void TowardXwk()
        {
            if (_PowerEffect!=null && _PowerEffect.activeSelf)
            {
                Vector3 powerToXwk = (_XwkRole.transform.position - _PowerEffect.transform.position);
                Quaternion lookAtRot = Quaternion.LookRotation(powerToXwk);
                _PowerEffect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 80 + lookAtRot.eulerAngles.x));
            }
        }
        private void TouchEvent()
        {
            if (_TouchOver)
            {
                ClickPowerEffect();
                return;
            }
            //碰到元气圈
            if (TouchManager.Instance.IsTouchValid(0))
            {
                TouchPhaseEnum phase = (TouchPhaseEnum)TouchManager.Instance.GetTouchPhase(0);
                switch (phase)
                {
                    case TouchPhaseEnum.BEGAN:
                        {
                            _TouchFlag = true;

                            if (_CanPlaySound)
                            {
                              //  _TouchEffect.gameObject.SetActive(false);
                                AudioManager.Instance.StopPlayerSound();
                                CancelInvoke(_InvokeMethod);
                                _CanPlaySound = false;
                                PlaySound();
                            }
                            Vector2 startTouchpos;

                            TouchManager.Instance.GetTouchPos(0, out startTouchpos);
                            RaycastHit hit;
                            Ray ray;
                            ray = _CurrentCamera.ScreenPointToRay(startTouchpos);
                            if (Physics.Raycast(ray, out hit, 1000) && hit.collider != null)
                            {
                                GameObject obj = hit.collider.gameObject;
                                if (obj != null && obj.name.Equals("public_effect_nlq_small_collider"))
                                {
                                    TouchOverEvent();

                                    RewardUIManager.GetInstance().SetSuccess();

                                    //PlayEndAction();
                                    //C_EventHandler.SendEvent(C_EnumEventChannel.Global, "UI_Press_Bar");

                                    //PlayXwkUpAnimation();

                                    //ClickPowerEffect();
                                }
                            }
                        }
                        break;
                    case TouchPhaseEnum.STATIONARY:
                        {
                            
                        }
                        break;
                    case TouchPhaseEnum.ENDED:
                        {
                            //if (!_TouchmanagerEnd)
                            //{
                            //    return;
                            //}
                            //_TouchmanagerEnd = false;
                            //if (_Progress < 1 && !_FirstFlag)
                            //{
                            //    _PowerEffect_Noise3_Mat.DOKill();
                            //    _PowerEffect_Noise3_2_Mat.DOKill();
                            //    _PowerEffect_Noise3_4_Mat.DOKill();
                            //    _FirstFlag = true;
                            //}
                            //if (_TouchFlag)
                            //{
                            //    _TouchFlag = false;
                            //    AudioManager.Instance.StopAllEffect();
                            //}
                        }
                        break;
                }
            }
        }
        private void TouchOverEvent()
        {
            _GuideHand.gameObject.SetActive(false);

            CancelInvoke(_InvokeTouchMethod);

            _BYEffectBoxcollider.enabled = false;

            _TouchOver = true;

            _TouchEffect.gameObject.SetActive(false);

            PlayTouchEffectSound();

            PlayEndAction();
        }
        private void PlayEndAction()
        {
            C_EventHandler.SendEvent(C_EnumEventChannel.Global, "UI_Press_Bar");

            PlayXwkUpAnimation();

            ClickPowerEffect();
        }
        public void PlayTouchEffectSound()
        {
            AudioManager.Instance.PlayEffectSound("public/sound_effect/public_xwkyx_034.ogg", true);
        }
        protected void ClickPowerEffect()
        {
            if (_PowerEffect == null) return;
            _CurrentTime += Time.deltaTime;
            if (_CurrentTime >= _TotalTime)
            {
                _CurrentTime = _TotalTime;
            }

            _Progress = _CurrentTime / _TotalTime;
            if (_Progress < 1)
            {
                _PowerEffect.SetActive(true);
                _XwkBodyEffect.SetActive(true);
                //基础缩放比例
                float scalefactor = (1.4f + 1) / 1.4f;
                _PowerEffect.transform.localScale = new Vector3(1, (Mathf.Abs(_XwkRole.transform.position.x - _PowerEffect.transform.position.x)) / scalefactor, 1);
                _XwkRole.transform.position = new Vector3(_XwkRole.transform.position.x, (0.0223f + (2.61f - 0.0223f) * _Progress), _XwkRole.transform.position.z);
                _XwkRole.transform.localRotation = Quaternion.Euler(0,48.7f+(6.55f-48.7f)*_Progress,0);
                _BaiYinHeadEffect.transform.localScale = new Vector3(1 - _Progress, 1 - _Progress, 1 - _Progress);
               /// _XwkEffectSceleFactor *= 0.9f;
                _XwkBodyEffect.transform.localScale = new Vector3(40+(_Progress * 100)/2, 40 + (_Progress * 100) / 2, 40 + (_Progress * 100) / 2);
              //  _XwkBodyEffect.transform.localScale = new Vector3((_XwkEffectSceleFactor+_Progress)/2 * 100, (_XwkEffectSceleFactor + _Progress)/2 * 100, (_XwkEffectSceleFactor + _Progress)/2 * 100);
                _CurrentCamera.transform.position = new Vector3(_CurrentCamera.transform.position.x, _CurrentCamera.transform.position.y,(3.9f+(6.7f- 3.9f)*_Progress));
                _CurrentCamera.transform.localRotation = Quaternion.Euler(-1.74f+(-11.11f+1.74f)*_Progress, 184.36f, 0);
                if (_Progress > 0.5f && !_StartChangeColor)
                {
                    //PlayTouchEffectSound();
                    _StartChangeColor = true;
                    _PowerEffect_Noise3_Mat.DOColor(new Color(107f / 255f, 114f / 255f, 255f / 255f, 0f), _TotalTime- _CurrentTime);
                    _PowerEffect_Noise3_2_Mat.DOColor(new Color(62f / 255f, 175f / 255f, 255f / 255f, 0f), _TotalTime - _CurrentTime);
                    _PowerEffect_Noise3_4_Mat.DOColor(new Color(83f / 255f, 205f / 255f, 255f / 255f, 0f), _TotalTime - _CurrentTime);

                    //_PowerEffect_Noise3.GetComponent<MeshRenderer>().material.SetFloat("_MMultiplier", 4f* Time.deltaTime);
                    //_PowerEffect_Noise3_2.GetComponent<MeshRenderer>().material.SetFloat("_MMultiplier", 0.465f * Time.deltaTime);
                    //_PowerEffect_Noise3_4.GetComponent<MeshRenderer>().material.SetFloat("_MMultiplier", 0.38f *Time.deltaTime);
                }
            }
            else
            {
                _PowerEffect.SetActive(false);
                QuitGame();
                if (Slate.CutsceneSequencePlayer._CurrentCutScene!=null)
                    Slate.CutsceneSequencePlayer._CurrentCutScene.Stop();
            }
        }
        public void QuitGame()
        {
            AudioManager.Instance.StopAllEffect();
            AudioManager.Instance.StopPlayerSound();
        //    C_UIMgr.Instance.CloseUI(_PressUIName);
            //CancelInvoke(_InvokeTouchMethod);
            //CancelInvoke(_InvokeMethod);
            if (!string.IsNullOrEmpty(_InvokeTouchMethod))
            {
                CancelInvoke(_InvokeTouchMethod);
                _InvokeTouchMethod = "";
            }
            if (!string.IsNullOrEmpty(_InvokeMethod))
            {
                CancelInvoke(_InvokeMethod);
                _InvokeMethod = "";
            }
            if (_TouchEffect!=null)
            {
                GameObject.DestroyImmediate(_TouchEffect);
                _TouchEffect = null;
            }
            if (_BaiYinSpiritRole != null)
            {
                GameObject.DestroyImmediate(_BaiYinSpiritRole);
                _BaiYinSpiritRole = null;
            }
            if (_PowerEffect != null)
            {
                GameObject.DestroyImmediate(_PowerEffect);
                _PowerEffect = null;
            }
            if (_CurrentCamera != null)
            {
                GameObject.DestroyImmediate(_CurrentCamera.gameObject);
                _CurrentCamera = null;
            }
            if (_XwkBodyEffect != null)
            {
                GameObject.DestroyImmediate(_XwkBodyEffect.gameObject);
                _XwkBodyEffect = null;
            }
            if (_BaiYinHeadEffect != null)
            {
                GameObject.DestroyImmediate(_BaiYinHeadEffect.gameObject);
                _BaiYinHeadEffect = null;
            }
            if (_XwkRole != null)
            {
                GameObject.DestroyImmediate(_XwkRole.gameObject);
                _XwkRole = null;
            }
            if (TouchManager.Instance!=null)
            {
                GameObject.DestroyObject(TouchManager.Instance);
            }
            if (_BYEffectBoxcollider != null)
            {
                GameObject.DestroyObject(_BYEffectBoxcollider.gameObject);
                _BYEffectBoxcollider = null;
            }
            if (_GuideHand!=null)
            {
                GameObject.DestroyObject(_GuideHand);
                _GuideHand = null;
            }
            if (DelayStop!=null)
            {
                DelayStop.Kill();
                DelayStop = null;
            }
        }
    }

}
