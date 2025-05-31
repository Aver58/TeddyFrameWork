public class GameWorld {
    private int worldId;
    private string warId;
    private bool isDispose;
    private GameWorldFeatures baseFeatures;

    public GameWorld() {
        worldId = -1;
        warId = string.Empty;
    }

    public void Init() {
        OnInit();

        var instance = UpdateRegister.Instance;// mono逻辑初始化
        baseFeatures = new GameWorldFeatures(this);
        // AddBaseFeature<LocalizationFeature>();
        LocalizationFeature.Init();
    }

    private void OnInit() {
        isDispose = false;

    }

    public void Clear() {
        if (isDispose) {
            return;
        }

        Dispose();
    }

    private void Dispose() {
        if (isDispose) {
            return;
        }

        isDispose = true;
    }

    public void AddBaseFeature<T>() where T : AbsBaseGameWorldFeature, new() {
        if (isDispose) {
            return;
        }
        baseFeatures.AddFeature<T>();
    }
}