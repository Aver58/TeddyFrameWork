using System;
using System.Collections.Generic;

public interface IGameWorldFeature {
    void Init(GameWorld gameWorld);
    void Clear();
}

public abstract class AbsGameWorldFeature : IGameWorldFeature {
    protected GameWorld gameWorld;
    public void Init(GameWorld world) {
        gameWorld = world;
        OnInit();
    }

    public void Clear() {
        OnRemove();
    }

    protected virtual void OnInit() { }
    protected virtual void OnRemove() { }
}

public abstract class AbsBaseGameWorldFeature : AbsGameWorldFeature { }

public class GameWorldFeatures : IDisposable {
    private GameWorld gameWorld;
    private List<IGameWorldFeature> features;
    private Dictionary<Type, IGameWorldFeature> featureDict;

    public GameWorldFeatures(GameWorld world) {
        gameWorld = world;
        features = new List<IGameWorldFeature>(32);
        featureDict = new Dictionary<Type, IGameWorldFeature>(32);
    }

    public void Dispose() {
        int length = features.Count;
        for (int i = 0; i < length; ++i) {
            features[i].Clear();
        }
        features.Clear();
        featureDict.Clear();
        features = null;
        featureDict = null;
        gameWorld = null;
    }

    public void AddFeature<T>() where T : IGameWorldFeature, new() {
        if (FindFeatureIndex<T>() == -1) {
            IGameWorldFeature feature = new T();
            features.Add(feature);
            featureDict.Add(feature.GetType(), feature);
            feature.Init(gameWorld);
        }
    }

    public void RemoveFeature<T>() where T : IGameWorldFeature {
        int index = FindFeatureIndex<T>();
        if (index >= 0) {
            IGameWorldFeature feature = features[index];
            features.RemoveAt(index);
            featureDict.Remove(feature.GetType());
            feature.Clear();
        }
    }

    public T GetFeature<T>() where T : IGameWorldFeature {
        if (featureDict.TryGetValue(typeof(T), out IGameWorldFeature target)) {
            return (T) target;
        }
        return default(T);
    }

    private int FindFeatureIndex<T>() {
        Type t = typeof(T);
        int length = features.Count;
        for (int i = 0; i < length; ++i) {
            if (t == features[i].GetType()) {
                return i;
            }
        }
        return -1;
    }
}
