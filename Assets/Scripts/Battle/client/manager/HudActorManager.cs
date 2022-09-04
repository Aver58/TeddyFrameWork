#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    HudActorManager.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\1 星期二 23:44:31
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class HudActorManager : Singleton<HudActorManager>
{
    private const string path = "battle/component/HpBar";
    public Transform parentNode; 
    private RectTransform canvasTransform;
    private Camera uiCamera;
    private Dictionary<int, HudActor> m_actorMap;

    public void Init(RectTransform hudParent)
    {
        m_actorMap = new Dictionary<int, HudActor>();

        uiCamera = CameraManager.instance.uiCamera;
        canvasTransform = hudParent;
        parentNode = hudParent;

        GameMsg.instance.AddMessage<HeorHPUpdateEventArgs>(GameMsgDef.BattleEntity_HP_Updated, OnHeroHPUpdated);
    }

    ~HudActorManager()
    {
        GameMsg.instance.RemoveMessage(GameMsgDef.BattleEntity_HP_Updated, this);
    }

    public void OnHeroHPUpdated(HeorHPUpdateEventArgs args)
    {
        int actorID = args.id;

        HudActor actor;
        m_actorMap.TryGetValue(actorID, out actor);
        if(actor!=null)
        {
            float percent = (float)args.curHp / args.maxHp;

            actor.SetValue(percent);
        }
    }

    public void OnHeroActorCreated(HeroActor actor, bool isFriend)
    {
        // 创建一个血条
        LoadModule.LoadUI(path, delegate (AssetRequest data) {
            GameObject asset = data.asset as GameObject;

            GameObject go = Object.Instantiate(asset, parentNode, true);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            GameObject heroGO = actor.gameObject;
            HudController hudController = go.GetComponent<HudController>();
            Transform targetTransform = heroGO.transform.Find("Dummy OverHead");
            hudController.Init(canvasTransform, targetTransform, uiCamera, 0, isFriend);
            HudActor hudActor = new HudActor(go, hudController);
            float hpPercent = actor.battleUnit.GetHPPercent();
            hudActor.SetValue(hpPercent);

            m_actorMap.Add(actor.id, hudActor);
        });
    }
}