namespace BepInEx {
    public class BepInPlugin : System.Attribute {
        public BepInPlugin(string guid, string name, string version) {}
    }
    public class BepInDependency : System.Attribute {
        public BepInDependency(string guid) {}
        public BepInDependency(string guid, DependencyFlags flags) {}
        public enum DependencyFlags { HardDependency = 1, SoftDependency = 2 }
    }
    public abstract class BaseUnityPlugin : UnityEngine.MonoBehaviour {
        // Logger MUST be a property (matches real BepInEx). Field would emit ldfld instead of call.
        public BepInEx.Logging.ManualLogSource Logger { get { return null; } }
    }
}
namespace BepInEx.Logging {
    public class ManualLogSource {
        public void LogInfo(object data) {}
        public void LogWarning(object data) {}
        public void LogError(object data) {}
        public void LogDebug(object data) {}
        public void LogFatal(object data) {}
        public void LogMessage(object data) {}
    }
}
