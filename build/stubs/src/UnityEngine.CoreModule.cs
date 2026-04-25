namespace UnityEngine {
    public class Object {
        public string name;
        public static UnityEngine.Object Instantiate(UnityEngine.Object original) { return null; }
        public static T Instantiate<T>(T original) where T : UnityEngine.Object { return null; }
        public static void DontDestroyOnLoad(UnityEngine.Object o) {}
    }
    public class GameObject : Object { 
        public bool activeSelf { get { return false; } }
        public void SetActive(bool b) {}
        public T GetComponent<T>() where T : Component { return null; }
        public Component GetComponent(System.Type t) { return null; }
        public Transform transform { get { return null; } }
    }
    public class Component : Object {
        public Transform transform { get { return null; } }
        public GameObject gameObject { get { return null; } }
        public T GetComponent<T>() where T : Component { return null; }
        public Component GetComponent(System.Type t) { return null; }
    }
    public class Transform : Component {
        public Transform parent { get { return null; } }
        public int childCount { get { return 0; } }
        public Transform GetChild(int i) { return null; }
        public Vector3 localScale { get; set; }
        public Vector3 localPosition { get; set; }
    }
    public class RectTransform : Transform {
        public Vector2 sizeDelta { get; set; }
    }
    public class MonoBehaviour : Component {}
    public class ScriptableObject : Object {
        public static T CreateInstance<T>() where T : ScriptableObject { return null; }
        public static ScriptableObject CreateInstance(System.Type t) { return null; }
    }
    public class Sprite : Object {}
    public class Texture : Object {}
    public class Resources {
        public static UnityEngine.Object[] FindObjectsOfTypeAll(System.Type t) { return null; }
    }
    public struct Color {
        public float r, g, b, a;
        public Color(float r, float g, float b, float a) { this.r=r; this.g=g; this.b=b; this.a=a; }
    }
    public struct Vector2 {
        public float x, y;
        public Vector2(float x, float y) { this.x = x; this.y = y; }
    }
    public struct Vector3 {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public static Vector3 one { get { return new Vector3(1, 1, 1); } }
    }
    public class Debug {
        public static void Log(object msg) {}
        public static void LogWarning(object msg) {}
        public static void LogError(object msg) {}
    }
}
