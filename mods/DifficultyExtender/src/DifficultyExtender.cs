// DifficultyExtender — Stolen Realm BepInEx plugin
// Extends roguelike difficulties from 6 to 10, dumps vanilla values, exposes
// a class-unlock-by-difficulty registry for other mods (e.g. StartingClasses).
//
// v1.0.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DifficultyExtender
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class DifficultyExtenderPlugin : BaseUnityPlugin
    {
        public const string PluginGuid    = "com.user.stolenrealm.difficultyextender";
        public const string PluginName    = "DifficultyExtender";
        public const string PluginVersion = "1.0.15";

        // Safe method resolver — avoids Harmony version-specific AccessTools.Method(string) overload
        // which is missing in the Harmony version bundled with this BepInEx build.
        public static MethodBase FindMethod(string typeName, string methodName)
        {
            var t = AccessTools.TypeByName(typeName);
            if (t == null) return null;
            return t.GetMethod(methodName,
                BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static);
        }

        public static DifficultyExtenderPlugin Instance;
        public static ManualLogSource Log;

        // ---------------- Public API: class unlock gating ----------------
        // Other mods register: "preset GUID X requires HighestRoguelikeDifficultyIndexUnlocked >= N"
        // Accessible via reflection from other plugins (no compile-time dep needed):
        //   var t = Type.GetType("DifficultyExtender.DifficultyExtenderPlugin, DifficultyExtender");
        //   t.GetMethod("RegisterRequirement").Invoke(null, new object[] { presetGuid, minIndex });
        public static readonly Dictionary<Guid, int> Requirements = new Dictionary<Guid, int>();

        public static void RegisterRequirement(Guid presetGuid, int minHighestUnlockedIndex)
        {
            Requirements[presetGuid] = minHighestUnlockedIndex;
            Log?.LogInfo($"[gate] Registered: preset {presetGuid} requires HighestUnlocked >= {minHighestUnlockedIndex}");
        }

        public static int GetRequirement(Guid presetGuid)
        {
            int v;
            return Requirements.TryGetValue(presetGuid, out v) ? v : 0;
        }

        // D7-D10 multipliers (tuned from real D6 vanilla values: HP=350% Dmg=350% BP=+100 EnMods=+100 BossDmg=+45 BossHP=+100).
        // Float fields: SCALE the template's (D6's) value.
        // Int fields:   ADD to the template's value.
        // Names follow vanilla Roman-numeral convention.
        // Each row: { name, desc, hpMul, dmgMul, goldMul, lootMul, xpMul, addBP, addEnemyMods, addBossDmg, addBossHp }
        public static readonly object[][] NewDifficulties = new object[][]
        {
            // D7 — first step beyond vanilla. Real teeth, but a fair fight.
            new object[] { "Difficulty VII", "Beyond mortal challenge.",
                1.50f, 1.25f, 1.0f, 1.0f, 1.0f,   25,  25,  30,  50 },
            // D8 — definitive break point for most builds.
            new object[] { "Difficulty VIII", "Where heroes break.",
                2.00f, 1.55f, 1.0f, 1.0f, 1.0f,   50,  50,  60, 100 },
            // D9 — only well-tuned parties survive consistently.
            new object[] { "Difficulty IX", "The threshold of madness.",
                2.75f, 1.85f, 1.0f, 1.0f, 1.0f,   75,  75,  95, 175 },
            // D10 — the flag-planting tier. Every fight matters.
            new object[] { "Difficulty X", "Impossible. Or so they said.",
                4.00f, 2.25f, 1.0f, 1.0f, 1.0f,  100, 100, 135, 275 },
        };

        // ---------------- State ----------------
        static bool _vanillaDumped;
        static bool _arrayExtended;

        // Resolved types (cached after first lookup)
        static Type _tGameLogic, _tDifficultySettings, _tDifficultySetting;
        static Type _tCharacterPresetFile, _tPresetManager, _tCharacterPresetTile;
        static Type _tGlobalSaveData, _tRoot;

        static bool ResolveTypes()
        {
            if (_tGameLogic != null) return true;
            _tGameLogic           = AccessTools.TypeByName("GameLogic");
            _tDifficultySettings  = AccessTools.TypeByName("DifficultySettings");
            _tDifficultySetting   = AccessTools.TypeByName("DifficultySetting");
            _tCharacterPresetFile = AccessTools.TypeByName("CharacterPresetFile");
            _tPresetManager       = AccessTools.TypeByName("PresetManager");
            _tCharacterPresetTile = AccessTools.TypeByName("CharacterPresetTile");
            _tGlobalSaveData      = AccessTools.TypeByName("Burst2Flame.GlobalSaveData")
                                  ?? AccessTools.TypeByName("GlobalSaveData");
            _tRoot                = AccessTools.TypeByName("Root");
            return _tGameLogic != null && _tDifficultySettings != null && _tDifficultySetting != null;
        }

        public void Awake()
        {
            Log = Logger;
            Instance = this;
            try
            {
                new Harmony(PluginGuid).PatchAll();
                Log.LogInfo(PluginName + " v" + PluginVersion + " loaded; Harmony patches applied");
            }
            catch (Exception e)
            {
                Log.LogError("[Awake] " + e);
            }
        }

        // ---------------- Core: extend the array (idempotent, lazy) ----------------
        // Resolves a singleton instance across common naming conventions. 
        // In this game: GameLogic exposes `get_instance` (lowercase property) + `_instance` (field).
        static object GetSingleton(Type t)
        {
            if (t == null) return null;
            // Property, try both cases
            foreach (var name in new[] { "instance", "Instance" })
            {
                var p = t.GetProperty(name,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                if (p != null) return p.GetValue(null, null);
            }
            // Field fallback
            foreach (var name in new[] { "_instance", "instance", "Instance", "s_instance" })
            {
                var f = t.GetField(name,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                if (f != null) return f.GetValue(null);
            }
            return null;
        }

        public static void EnsureExtended()
        {
            if (_arrayExtended) return;
            if (!ResolveTypes()) { Log.LogWarning("[extend] types not resolved yet"); return; }

            try
            {
                var glInstance = GetSingleton(_tGameLogic);
                if (glInstance == null) { Log.LogWarning("[extend] GameLogic singleton not resolved yet (will retry)"); return; }

                var pSettings = _tGameLogic.GetProperty("DifficultySettings",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (pSettings == null) { Log.LogError("[extend] no DifficultySettings property on GameLogic"); return; }
                var settingsObj = pSettings.GetValue(glInstance, null);
                if (settingsObj == null) { Log.LogWarning("[extend] DifficultySettings is null (singleton not ready)"); return; }

                var fRogue = _tDifficultySettings.GetField("DifficultiesRoguelike");
                if (fRogue == null) { Log.LogError("[extend] no DifficultiesRoguelike field"); return; }

                // DifficultiesRoguelike is List<DifficultySetting> in this build — NOT an array.
                // Use IList to work with either shape. For List<T>, we can Add() in place.
                var listObj = fRogue.GetValue(settingsObj);
                if (listObj == null) { Log.LogError("[extend] DifficultiesRoguelike is null"); return; }

                var ilist = listObj as System.Collections.IList;
                if (ilist == null)
                {
                    Log.LogError($"[extend] DifficultiesRoguelike is not an IList (type={listObj.GetType().FullName})");
                    return;
                }

                if (!_vanillaDumped)
                {
                    DumpVanilla(ilist);
                    _vanillaDumped = true;
                }

                int oldLen = ilist.Count;
                if (oldLen >= 6 + NewDifficulties.Length)
                {
                    Log.LogInfo($"[extend] DifficultiesRoguelike already length {oldLen}; skipping");
                    _arrayExtended = true;
                    return;
                }

                if (oldLen == 0)
                {
                    Log.LogWarning("[extend] DifficultiesRoguelike is empty; cannot template from D-last");
                    return;
                }

                var template = ilist[oldLen - 1];

                for (int i = 0; i < NewDifficulties.Length; i++)
                {
                    var spec = NewDifficulties[i];
                    var clone = CloneDifficultyFromTemplate(template, spec);
                    if (ilist.IsFixedSize)
                    {
                        // Fallback for array-backed shapes (shouldn't happen here, but safe)
                        Log.LogWarning("[extend] Collection is fixed-size; cannot Add. Aborting.");
                        return;
                    }
                    ilist.Add(clone);
                }

                _arrayExtended = true;

                Log.LogInfo($"[extend] DifficultiesRoguelike extended {oldLen} -> {ilist.Count}");
                for (int i = oldLen; i < ilist.Count; i++)
                {
                    LogEntry($"D{i + 1}", ilist[i]);
                }
            }
            catch (Exception e)
            {
                Log.LogError($"[extend] {e}");
            }
        }

        public static bool ArrayExtended { get { return _arrayExtended; } }

        static void DumpVanilla(System.Collections.IList list)
        {
            Log.LogInfo($"=== VANILLA DifficultiesRoguelike (length={list.Count}) ===");
            for (int i = 0; i < list.Count; i++)
            {
                LogEntry($"D{i + 1}", list[i]);
            }
            try
            {
                int h = GetHighestUnlocked();
                Log.LogInfo($"  Player HighestRoguelikeDifficultyIndexUnlocked = {h}");
            }
            catch { }
            Log.LogInfo("=== END VANILLA DUMP ===");
        }

        static string GF(object d, string fname)
        {
            if (d == null) return "?";
            var f = d.GetType().GetField(fname);
            if (f == null) return "?";
            var v = f.GetValue(d);
            return v == null ? "null" : v.ToString();
        }

        static void LogEntry(string label, object d)
        {
            if (d == null) { Log.LogInfo("  " + label + ": NULL"); return; }
            Log.LogInfo("  " + label
                + ": name='" + GF(d, "DifficultyName") + "'"
                + " HP=" + GF(d, "HealthModifierPerc")
                + " Dmg=" + GF(d, "DamageModifierPerc")
                + " Gold=" + GF(d, "GoldModifierPerc")
                + " Loot=" + GF(d, "LootModifierPerc")
                + " XP=" + GF(d, "ExperienceModifierPerc")
                + " BP=+" + GF(d, "AdditionalBattlePointsPerc")
                + " EnMods=+" + GF(d, "AdditionalEnemyModsPerc")
                + " BossDmg=+" + GF(d, "BossDamageModifierPerc")
                + " BossHP=+" + GF(d, "BossHealthModifierPerc"));
        }

        static object CloneDifficultyFromTemplate(object template, object[] spec)
        {
            // DifficultySetting is a plain System.Object-derived POCO — not a Unity Object,
            // not a ScriptableObject. Use Activator.CreateInstance for the default ctor.
            object clone;
            try
            {
                clone = Activator.CreateInstance(_tDifficultySetting);
            }
            catch (Exception e)
            {
                Log.LogError($"[clone] Activator.CreateInstance({_tDifficultySetting.FullName}) failed: {e.Message}");
                // Final fallback: bypass ctor entirely
                clone = System.Runtime.Serialization.FormatterServices
                    .GetUninitializedObject(_tDifficultySetting);
            }

            var t = _tDifficultySetting;

            // Copy Icon reference from template (Sprite ref, doesn't need cloning)
            var fIcon = t.GetField("DifficultyIcon");
            if (fIcon != null) fIcon.SetValue(clone, fIcon.GetValue(template));

            // Strings
            t.GetField("DifficultyName").SetValue(clone, (string)spec[0]);
            t.GetField("DifficultyDesc").SetValue(clone, (string)spec[1]);

            // Floats: scaled from template
            ScaleFloat(t, template, clone, "HealthModifierPerc",     (float)spec[2]);
            ScaleFloat(t, template, clone, "DamageModifierPerc",     (float)spec[3]);
            ScaleFloat(t, template, clone, "GoldModifierPerc",       (float)spec[4]);
            ScaleFloat(t, template, clone, "LootModifierPerc",       (float)spec[5]);
            ScaleFloat(t, template, clone, "ExperienceModifierPerc", (float)spec[6]);

            // Ints: additive over template
            AddInt(t, template, clone, "AdditionalBattlePointsPerc", (int)spec[7]);
            AddInt(t, template, clone, "AdditionalEnemyModsPerc",    (int)spec[8]);
            AddInt(t, template, clone, "BossDamageModifierPerc",     (int)spec[9]);
            AddInt(t, template, clone, "BossHealthModifierPerc",     (int)spec[10]);

            return clone;
        }

        static void ScaleFloat(Type t, object src, object dst, string name, float mul)
        {
            var f = t.GetField(name);
            if (f == null) return;
            var v = f.GetValue(src);
            if (v is float) f.SetValue(dst, (float)v * mul);
        }
        static void AddInt(Type t, object src, object dst, string name, int add)
        {
            var f = t.GetField(name);
            if (f == null) return;
            var v = f.GetValue(src);
            if (v is int) f.SetValue(dst, (int)v + add);
        }

        // ---------------- Class unlock gate (postfix) ----------------
        // Reads PresetManager.CurrentPreset; if it has a registered requirement,
        // and player's HighestRoguelikeDifficultyIndexUnlocked is below it,
        // disable RoguelikeAcceptButton and activate any LockedByDifficulty UI on the tile.
        public static void ApplyGate(object presetManagerInstance)
        {
            try
            {
                if (Requirements.Count == 0) return;          // nothing registered, fast out
                if (!ResolveTypes()) return;
                if (presetManagerInstance == null) return;

                // Only gate in roguelike. Burst2Flame.Game.RoguelikeModeActive is the canonical check.
                if (!IsRoguelikeMode()) return;

                // Get current preset
                var pCur = _tPresetManager.GetProperty("CurrentPreset",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (pCur == null) return;
                var preset = pCur.GetValue(presetManagerInstance, null);
                if (preset == null) return;

                // Get its Guid
                var fGuid = _tCharacterPresetFile.GetField("Guid");
                if (fGuid == null) return;
                var gObj = fGuid.GetValue(preset);
                if (!(gObj is Guid)) return;
                Guid g = (Guid)gObj;

                int requiredIdx;
                if (!Requirements.TryGetValue(g, out requiredIdx)) return;

                int highest = GetHighestUnlocked();
                if (highest >= requiredIdx) return; // unlocked already

                // Disable Accept (Roguelike variant — char creation in roguelike mode uses this)
                var fAccept = _tPresetManager.GetField("RoguelikeAcceptButton");
                if (fAccept != null)
                {
                    var btn = fAccept.GetValue(presetManagerInstance);
                    SetInteractable(btn, false);
                }

                // Activate the tile's LockedByDifficultyObj overlay (and label, if present)
                ApplyTileLockUi(presetManagerInstance, preset, requiredIdx);
            }
            catch (Exception e)
            {
                Log.LogError($"[gate] {e}");
            }
        }

        static bool IsRoguelikeMode()
        {
            try
            {
                var tGame = AccessTools.TypeByName("Burst2Flame.Game");
                if (tGame == null) return false;
                var pInst = tGame.GetProperty("Instance",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                var inst = pInst?.GetValue(null, null);
                if (inst == null) return false;
                var pRogue = tGame.GetProperty("RoguelikeModeActive");
                if (pRogue == null) return false;
                var v = pRogue.GetValue(inst, null);
                return (v is bool) && (bool)v;
            }
            catch { return false; }
        }

        static void ApplyTileLockUi(object presetManagerInstance, object preset, int requiredIdx)
        {
            try
            {
                // Find the CharacterPresetTile whose _CharacterPresetFile matches `preset`
                var pTiles = _tPresetManager.GetProperty("CharacterPresetTiles");
                if (pTiles == null) return;
                var tilesObj = pTiles.GetValue(presetManagerInstance, null);
                if (tilesObj == null) return;

                var enumerable = tilesObj as System.Collections.IEnumerable;
                if (enumerable == null) return;

                var tType = _tCharacterPresetTile;
                if (tType == null) return;
                var fTilePreset = tType.GetField("_CharacterPresetFile",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var fLockObj    = tType.GetField("LockedByDifficultyObj");
                var fLockText   = tType.GetField("LockedByDifficultyText");
                if (fTilePreset == null || fLockObj == null) return;

                foreach (var tile in enumerable)
                {
                    if (tile == null) continue;
                    var tilePreset = fTilePreset.GetValue(tile);
                    if (!ReferenceEquals(tilePreset, preset)) continue;

                    var go = fLockObj.GetValue(tile);
                    SetGameObjectActive(go, true);

                    if (fLockText != null)
                    {
                        var txt = fLockText.GetValue(tile);
                        SetTextString(txt, "Complete Difficulty " + requiredIdx);
                    }
                    break;
                }
            }
            catch (Exception e)
            {
                Log.LogWarning($"[gate] tile UI: {e.Message}");
            }
        }

        static void SetGameObjectActive(object go, bool v)
        {
            if (go == null) return;
            var m = go.GetType().GetMethod("SetActive", new Type[] { typeof(bool) });
            if (m != null) m.Invoke(go, new object[] { v });
        }

        static void SetTextString(object txt, string s)
        {
            if (txt == null) return;
            // Try .text property (UnityEngine.UI.Text or TMP_Text both have this)
            var p = txt.GetType().GetProperty("text");
            if (p != null) p.SetValue(txt, s, null);
        }

        static int GetHighestUnlocked()
        {
            try
            {
                if (_tGlobalSaveData == null) return 0;
                var pInst = _tGlobalSaveData.GetProperty("Instance",
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                if (pInst == null) return 0;
                var gs = pInst.GetValue(null, null);
                if (gs == null) return 0;
                var pGet = _tGlobalSaveData.GetProperty("HighestRoguelikeDifficultyIndexUnlocked");
                if (pGet == null) return 0;
                var v = pGet.GetValue(gs, null);
                return (v is int) ? (int)v : 0;
            }
            catch (Exception e)
            {
                Log.LogWarning($"[gate] GetHighestUnlocked: {e.Message}");
                return 0;
            }
        }

        static void SetInteractable(object button, bool v)
        {
            if (button == null) return;
            var p = button.GetType().GetProperty("interactable");
            if (p != null) p.SetValue(button, v, null);
        }
    }

    // Lazy: extend before the first time the difficulty UI populates
    [HarmonyPatch]
    static class PopulateDifficultiesPatch
    {
        static MethodBase TargetMethod()
        {
            return DifficultyExtenderPlugin.FindMethod("DifficultySettingManager", "PopulateDifficulties");
        }
        static void Prefix() => DifficultyExtenderPlugin.EnsureExtended();
    }

    // Also extend on game logic awake (if reached first)
    [HarmonyPatch]
    static class GameLogicAwakeExtend
    {
        static MethodBase TargetMethod()
        {
            return DifficultyExtenderPlugin.FindMethod("GameLogic", "Start")
                ?? DifficultyExtenderPlugin.FindMethod("GameLogic", "Awake");
        }
        static void Postfix() => DifficultyExtenderPlugin.EnsureExtended();
    }

    // Class-unlock gate
    [HarmonyPatch]
    static class UpdatePresetButtonsGate
    {
        static MethodBase TargetMethod()
        {
            return DifficultyExtenderPlugin.FindMethod("PresetManager", "UpdatePresetButtons");
        }
        static void Postfix(object __instance) => DifficultyExtenderPlugin.ApplyGate(__instance);
    }
    // Tile layout fix — when there are >6 tiles, scale each tile down so all fit in vanilla's
    // existing horizontal layout. We touch ONLY the tiles themselves, never their parents
    // or any shared UI components — that keeps the changes self-contained to this window.
    [HarmonyPatch]
    static class TileLayoutFixPostfix
    {
        static MethodBase TargetMethod()
        {
            return DifficultyExtenderPlugin.FindMethod("DifficultySettingManager", "PopulateDifficulties");
        }

        static bool _applied;

        static void Postfix(object __instance)
        {
            try
            {
                if (__instance == null) return;
                if (!DifficultyExtenderPlugin.ArrayExtended) return;
                if (_applied) return;

                var t = __instance.GetType();
                var fItems = t.GetField("DifficultyItems",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fItems == null) { Log.LogWarning("[layout] no DifficultyItems"); return; }
                var items = fItems.GetValue(__instance) as System.Collections.IList;
                if (items == null) return;
                int n = items.Count;
                if (n <= 6) return;

                // Scale each tile so all N fit. Floor at 0.55 so text is still legible.
                float scale = 6.0f / (float)n;
                if (scale < 0.55f) scale = 0.55f;
                ApplyTileScale(items, scale);

                // Tile text cleanup — runs after scaling so simplified lock text fits cleanly.
                CleanUpTileText(items);

                _applied = true;
                Log.LogInfo("[layout] Scaled " + n + " tiles to " + scale.ToString("0.00") + "; text cleaned up");
            }
            catch (Exception e)
            {
                Log.LogWarning("[layout] " + e.Message);
            }
        }

        static void ApplyTileScale(System.Collections.IList items, float factor)
        {
            var tVec3 = Type.GetType("UnityEngine.Vector3, UnityEngine.CoreModule")
                     ?? Type.GetType("UnityEngine.Vector3, UnityEngine");
            if (tVec3 == null) return;
            var ctor = tVec3.GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float) });
            if (ctor == null) return;
            object scaleVec = ctor.Invoke(new object[] { factor, factor, 1f });

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i]; if (item == null) continue;
                var pTr = item.GetType().GetProperty("transform");
                var tr = pTr?.GetValue(item, null);
                if (tr == null) continue;
                var pScale = tr.GetType().GetProperty("localScale");
                pScale?.SetValue(tr, scaleVec, null);
            }
        }

        // Hide DifficultyTitle on every tile, simplify LockedText to a short non-wrapping form.
        static void CleanUpTileText(System.Collections.IList items)
        {
            try
            {
                int hidTitle = 0, fixedLock = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    var tile = items[i];
                    if (tile == null) continue;
                    var tt = tile.GetType();

                    // Hide the redundant title (the big Roman numeral on the sprite shows it)
                    var fTitle = tt.GetField("DifficultyTitle",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var titleObj = fTitle?.GetValue(tile);
                    if (titleObj != null && DeactivateGameObject(titleObj)) hidTitle++;

                    // Simplify lock text. Tile at index i is difficulty (i+1); requires clearing i.
                    // i==0 (Difficulty I) is always unlocked.
                    var fLock = tt.GetField("LockedText",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    var lockObj = fLock?.GetValue(tile);
                    if (lockObj != null)
                    {
                        SetTextString(lockObj, (i > 0) ? ("Clear " + ToRoman(i)) : "");
                        fixedLock++;
                    }
                }
                Log.LogInfo("[layout] Tile text: hid " + hidTitle + " titles, simplified " + fixedLock + " lock texts");
            }
            catch (Exception e)
            {
                Log.LogWarning("[layout] tile text cleanup: " + e.Message);
            }
        }

        // ---- helpers ----

        static bool DeactivateGameObject(object component)
        {
            if (component == null) return false;
            var pGo = component.GetType().GetProperty("gameObject");
            var go = pGo?.GetValue(component, null);
            if (go == null) return false;
            var mSetActive = go.GetType().GetMethod("SetActive", new Type[] { typeof(bool) });
            if (mSetActive == null) return false;
            mSetActive.Invoke(go, new object[] { false });
            return true;
        }

        static void SetTextString(object textComponent, string s)
        {
            if (textComponent == null) return;
            var p = textComponent.GetType().GetProperty("text");
            if (p != null) p.SetValue(textComponent, s, null);
        }

        static string ToRoman(int n)
        {
            switch (n)
            {
                case 1: return "I";
                case 2: return "II";
                case 3: return "III";
                case 4: return "IV";
                case 5: return "V";
                case 6: return "VI";
                case 7: return "VII";
                case 8: return "VIII";
                case 9: return "IX";
                case 10: return "X";
                default: return n.ToString();
            }
        }

        static ManualLogSource Log
        {
            get { return DifficultyExtenderPlugin.Log; }
        }
    }
}
