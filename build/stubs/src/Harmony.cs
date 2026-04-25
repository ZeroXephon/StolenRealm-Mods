using System;
using System.Reflection;
using System.Collections.Generic;
namespace HarmonyLib {
    public class Harmony {
        public Harmony(string id) {}
        public void PatchAll() {}
        public void PatchAll(System.Type t) {}
        public static Harmony CreateAndPatchAll(System.Type t, string id = null) { return null; }
    }
    public class HarmonyPatch : System.Attribute {
        public HarmonyPatch() {}
        public HarmonyPatch(System.Type t) {}
        public HarmonyPatch(System.Type t, string method) {}
        public HarmonyPatch(System.Type t, string method, params System.Type[] args) {}
        public HarmonyPatch(string typeName, string method) {}
    }
    public class HarmonyPrefix  : System.Attribute {}
    public class HarmonyPostfix : System.Attribute {}
    public class HarmonyFinalizer : System.Attribute {}
    public class HarmonyTargetMethod : System.Attribute {}
    public class HarmonyPriority : System.Attribute { public HarmonyPriority(int p) {} }
    public static class AccessTools {
        public static System.Type TypeByName(string name) { return null; }
        public static FieldInfo Field(System.Type t, string name) { return null; }
        public static PropertyInfo Property(System.Type t, string name) { return null; }
        public static MethodInfo Method(System.Type t, string name) { return null; }
        public static MethodInfo Method(System.Type t, string name, System.Type[] args) { return null; }
        public static IEnumerable<MethodInfo> GetDeclaredMethods(System.Type t) { return null; }
    }
    public class Traverse {
        public static Traverse Create(object root) { return null; }
        public Traverse Field(string name) { return null; }
        public Traverse Property(string name) { return null; }
        public T GetValue<T>() { return default(T); }
        public Traverse SetValue(object value) { return null; }
    }
}
