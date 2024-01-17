using System;
using UnityGameFramework.Runtime;

public static class EntityExtension {
    private static int s_SerialId = 0;
    public static int GenerateSerialId(this EntityComponent entityComponent)
    {
        return --s_SerialId;
    }
    
    // public static void ShowEffect(this EntityComponent entityComponent, EffectData data)
    // {
    //     entityComponent.ShowEntity(typeof(Effect), "Effect", Constant.AssetPriority.EffectAsset, data);
    // }
    
    // private static void ShowEntity(this EntityComponent entityComponent, Type logicType, string entityGroup, int priority, EntityData data)
    // {
    //     // if (data == null)
    //     // {
    //     //     Log.Warning("Data is invalid.");
    //     //     return;
    //     // }
    //
    //     // IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
    //     // DREntity drEntity = dtEntity.GetDataRow(data.TypeId);
    //     // if (drEntity == null)
    //     // {
    //     //     Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
    //     //     return;
    //     // }
    //     //
    //     // entityComponent.ShowEntity(data.Id, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), entityGroup, priority, data);
    // }
    
    public static void HideEntity(this EntityComponent entityComponent, Entity entity)
    {
        entityComponent.HideEntity(entity.Entity);
    }
}