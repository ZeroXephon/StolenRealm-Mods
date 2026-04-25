// StartingClasses v1.0.5
// Back to a simpler architecture like the user's working v1.0.0:
//   - PresetInjectPatch            (UpdateRoguelikeCharacterPresetTiles postfix)
//   - PresetDictPostfixPatch       (GetFromCharacterPresetFileDict postfix)
//   - GetCharacterPresetFilesPatch (PresetManager.GetCharacterPresetFiles(GameMode) - MP client fix)
//   - ApplyPassivesPatch           (SyncCharacterToPreset postfix)
//   - FinalizeCharacterCreationPatch (fortune flush)
//   - AcceptCharacterDiagnostic    (prefix - logs when Accept is actually clicked)
// The UpdatePresetButtonsPatch and SelectPresetLogPatch from v1.0.4 are GONE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Burst2Flame;

[BepInPlugin("com.modder.stolenrealm.startingclasses", "Starting Classes", "1.0.5")]
public class StartingClassesPlugin : BaseUnityPlugin
{
    public static ManualLogSource Log;
    public static List<CustomClassDef> ClassDefs = new List<CustomClassDef>();

    void Awake()
    {
        Log = base.Logger;
        Log.LogInfo("Starting Classes v1.0.5 loading...");
        DefineClassDefs();
        Log.LogInfo(string.Format("Starting Classes loaded - {0} class(es) defined.", ClassDefs.Count));
        try { new Harmony("com.modder.stolenrealm.startingclasses").PatchAll(); Log.LogInfo("PatchAll completed."); }
        catch (Exception ex) { Log.LogError("PatchAll failed: " + ex); }
    }

    void DefineClassDefs()
    {
        CustomClassDef def;

        def = new CustomClassDef { Id = "cls_dragon_kin", Name = "Dragon Kin", Description = "You're a dragon!" };
        def.StatBonuses["Might"] = 15; def.StatBonuses["Dexterity"] = 6; def.StatBonuses["Intelligence"] = 12;
        def.StatBonuses["Vitality"] = 12; def.StatBonuses["Reflex"] = 5;
        def.Equipment["Armor"]    = new Guid("1fe9d626-0244-4873-81d0-702b4efbf498");
        def.Equipment["MainHand"] = new Guid("f75f2f47-26df-42c9-bef8-7741621283ae");
        def.StartingSkillGuids.Add(new Guid("2446d269-e869-4479-b35e-c93d58a09386"));
        def.StartingSkillGuids.Add(new Guid("fd4cfa68-ad42-4f5b-9231-73f7ae16a806"));
        def.StartingSkillGuids.Add(new Guid("126bb043-6548-4075-bca8-77e5f90fe992")); // Hot Head I
        def.StartingFortuneGuids.Add(new Guid("fc095ea0-d01c-4179-927d-6e8bac27f68f")); // Dragon Claw Talisman
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_florist", Name = "Florist", Description = "Fight with flowers!" };
        def.StatBonuses["Might"] = 10; def.StatBonuses["Dexterity"] = 10; def.StatBonuses["Intelligence"] = 14;
        def.StatBonuses["Vitality"] = 10; def.StatBonuses["Reflex"] = 6;
        def.Equipment["Armor"]    = new Guid("4d405952-a3c9-482f-af8f-2d47fe2c337a");
        def.Equipment["MainHand"] = new Guid("a66fa447-7295-4017-ae92-25be38e4f8a7");
        def.StartingSkillGuids.Add(new Guid("27ad00f4-b115-4779-83e0-85781933df47"));
        def.StartingSkillGuids.Add(new Guid("908596ba-d2f0-486c-8974-bb648d9d7f2e"));
        def.StartingSkillGuids.Add(new Guid("ea930abb-1071-4f58-b667-1bde3627c5d0"));
        def.StartingSkillGuids.Add(new Guid("45041cfc-d8ba-4c87-8b9b-7ee36daab732"));
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_pyromaniac", Name = "Pyromaniac", Description = "Some people want to watch the world burn." };
        def.StatBonuses["Might"] = 12; def.StatBonuses["Dexterity"] = 10; def.StatBonuses["Intelligence"] = 12;
        def.StatBonuses["Vitality"] = 10; def.StatBonuses["Reflex"] = 6;
        def.Equipment["Armor"]    = new Guid("1733df1c-68ef-49b7-83bb-67f167338194");
        def.Equipment["MainHand"] = new Guid("ca69b54c-2b59-4a02-a24e-1cb46ec97edf"); // Emberstone Staff
        def.PassiveBonuses["ResistFire"] = 5f;
        def.StartingSkillGuids.Add(new Guid("6f8d77e8-aec2-49ee-8656-864e4ed013ea")); // Burning Ground
        def.StartingSkillGuids.Add(new Guid("4ded356a-facc-4408-87b0-99b00cb2fcc5")); // Fuel for the Flames II
        def.StartingSkillGuids.Add(new Guid("ac488fd4-a38f-4644-bb03-25c79513ae36")); // Avatar of Flame
        def.StartingSkillGuids.Add(new Guid("971efc39-f001-4564-b159-2584e105f04b")); // Flash Fire
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_bubbler", Name = "Bubbler", Description = "" };
        def.StatBonuses["Might"] = 10; def.StatBonuses["Dexterity"] = 10; def.StatBonuses["Intelligence"] = 15;
        def.StatBonuses["Vitality"] = 5; def.StatBonuses["Reflex"] = 10;
        def.Equipment["Armor"]    = new Guid("0890c75e-1ea7-472d-990d-27b08f4fa31a");
        def.Equipment["MainHand"] = new Guid("6331af41-dd52-4a25-9b24-e32c6c65e3c3");
        def.PassiveBonuses["ManaOnHit"] = 2f;
        def.StartingSkillGuids.Add(new Guid("f61e81b2-a830-4eb3-8ab9-2fe1fd39ab7c"));
        def.StartingSkillGuids.Add(new Guid("0a32ea87-3aa6-4bfa-86f8-da39a5b3aafd"));
        def.StartingSkillGuids.Add(new Guid("b6cb4d22-fa91-4b17-8c69-fdd47bed405f"));
        def.StartingSkillGuids.Add(new Guid("6794f244-500f-48b8-8cad-ea53e9a69c9f"));
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_dire_lycan", Name = "Dire Lycan", Description = "The Lycan's angrier leader." };
        def.StatBonuses["Might"] = 12; def.StatBonuses["Dexterity"] = 8; def.StatBonuses["Intelligence"] = 8;
        def.StatBonuses["Vitality"] = 10; def.StatBonuses["Reflex"] = 12;
        def.Equipment["Head"]     = new Guid("18c27ca7-77fa-4686-9c05-ec4f03c6a370");
        def.Equipment["Armor"]    = new Guid("e0a3fb0e-b63f-4dcf-b96d-2190eee3e30e");
        def.Equipment["MainHand"] = new Guid("61f5cb3e-15a8-4f82-92a5-6d4ddf1df875");
        def.StartingSkillGuids.Add(new Guid("b2d332e5-4980-4a00-af0c-3958e4e22ca3"));
        def.StartingSkillGuids.Add(new Guid("fd4cfa68-ad42-4f5b-9231-73f7ae16a806"));
        def.StartingSkillGuids.Add(new Guid("85a9b32a-7c27-45fa-b38f-5da5e5c9a7b8"));
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_druid", Name = "Druid", Description = "Nature always wins." };
        def.StatBonuses["Might"] = 12; def.StatBonuses["Dexterity"] = 8; def.StatBonuses["Intelligence"] = 14;
        def.StatBonuses["Vitality"] = 10; def.StatBonuses["Reflex"] = 6;
        def.Equipment["Armor"]    = new Guid("0890c75e-1ea7-472d-990d-27b08f4fa31a");
        def.Equipment["MainHand"] = new Guid("7e0aa4d1-6886-443e-b03c-ac7348df65c1");
        def.StartingSkillGuids.Add(new Guid("d2b61bb2-f017-4234-bf38-b67d20e7bbbc"));
        def.StartingSkillGuids.Add(new Guid("3096d08b-a4d2-4954-8930-c202fa3b0699"));
        def.StartingSkillGuids.Add(new Guid("2112128e-960f-4661-aebb-a50dc4d7760c"));
        def.StartingSkillGuids.Add(new Guid("0e87e9e4-69a1-4ef4-84d8-e42a2af3017b"));
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_plague_bringer", Name = "Plague Bringer", Description = "The end is nigh. " };
        def.StatBonuses["Might"] = 14; def.StatBonuses["Dexterity"] = 14; def.StatBonuses["Intelligence"] = 10;
        def.StatBonuses["Vitality"] = 5; def.StatBonuses["Reflex"] = 7;
        def.Equipment["Head"]     = new Guid("ee477b9f-3860-4636-8c09-10cdeab3f2d6");
        def.Equipment["Armor"]    = new Guid("5ca03df9-2676-4785-8907-8597fda0eada");
        def.Equipment["MainHand"] = new Guid("8219f947-72a8-441f-8315-dac233cb7e15");
        def.PassiveBonuses["LifeOnHit"] = 2f;
        def.StartingSkillGuids.Add(new Guid("f1f19d21-64b5-4e2a-80f9-3e7a29525945"));
        def.StartingSkillGuids.Add(new Guid("51c5c7ac-97f4-43b5-a811-6b98f7c17f89"));
        def.StartingSkillGuids.Add(new Guid("3954ff4b-0a05-4c10-a5d8-e39650662535"));
        def.StartingSkillGuids.Add(new Guid("2577c321-48a4-47c3-8fc9-a6716523e670"));
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_mitosis", Name = "Mitosis", Description = "You are your own best friend." };
        def.StatBonuses["Might"] = 10; def.StatBonuses["Dexterity"] = 10; def.StatBonuses["Intelligence"] = 14;
        def.StatBonuses["Vitality"] = 6; def.StatBonuses["Reflex"] = 10;
        def.Equipment["Armor"]    = new Guid("348f434f-0e26-4782-ae76-70e81f262116");
        def.Equipment["MainHand"] = new Guid("5080ac8c-d02e-456d-aadd-f4be1511995e");
        def.StartingSkillGuids.Add(new Guid("aba3d603-6b30-464a-b07f-9ae52a59d914"));
        def.StartingFortuneGuids.Add(new Guid("5f0973c7-b19f-4229-9f42-2acb3bc1eb40")); // Mirror Shard
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_holy_dragon", Name = "Holy Dragon", Description = "Holiest of draggos. " };
        def.StatBonuses["Might"] = 10; def.StatBonuses["Dexterity"] = 8; def.StatBonuses["Intelligence"] = 13;
        def.StatBonuses["Vitality"] = 13; def.StatBonuses["Reflex"] = 6;
        def.Equipment["Armor"]    = new Guid("e0a3fb0e-b63f-4dcf-b96d-2190eee3e30e"); // Pupil's Attire
        def.Equipment["MainHand"] = new Guid("af234de2-dad2-4f07-aa21-1f1f9064a8fc"); // Gemstone Ritual Staff
        def.StartingSkillGuids.Add(new Guid("3c3533f9-21cf-46c9-957d-c6b957b7716e")); // Wrath of the Righteous
        def.StartingSkillGuids.Add(new Guid("6734cfa8-5699-4014-a4d7-7ae2b359d1e8")); // Empowered Light I
        def.StartingSkillGuids.Add(new Guid("83ce3706-dea3-404f-9648-3b43b4ff4737")); // Empowered Light II
        def.StartingSkillGuids.Add(new Guid("9cb9aac5-3cc5-4765-a967-11f5720e549f")); // Mass Cure
        def.StartingSkillGuids.Add(new Guid("fd4cfa68-ad42-4f5b-9231-73f7ae16a806")); // Skin Walker
        def.StartingFortuneGuids.Add(new Guid("b903a0b1-b9c6-4ee4-94f4-fbd3bbdbec52")); // Ivory Dragon Scales
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_shadow_dragon", Name = "Shadow Dragon", Description = "Wings in the darkness." };
        def.StatBonuses["Might"] = 10; def.StatBonuses["Dexterity"] = 10; def.StatBonuses["Intelligence"] = 12;
        def.StatBonuses["Vitality"] = 12; def.StatBonuses["Reflex"] = 6;
        def.Equipment["Armor"]    = new Guid("79e1f0fc-2766-42f1-8436-8076ee1b9b12"); // Assassin's Garb
        def.Equipment["MainHand"] = new Guid("51e7b9f9-447a-429c-a515-a33f9b7f0525"); // Boot Blade
        def.Equipment["OffHand"]  = new Guid("9fdd0bb3-e160-4e84-84ac-57a1072e8c1f"); // Broken Blade
        def.StartingSkillGuids.Add(new Guid("39380e38-ea1c-4916-8d0f-b8836076a6fd")); // Endless Night
        def.StartingSkillGuids.Add(new Guid("fd4cfa68-ad42-4f5b-9231-73f7ae16a806")); // Skin Walker
        def.StartingSkillGuids.Add(new Guid("8812a8fd-f3ef-4557-819b-e2fd553cab52")); // Vengeful Shadows
        def.StartingFortuneGuids.Add(new Guid("05f92026-c7ec-4a55-a7f1-45a7a3894dfd")); // Ebony Dragon Scales
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_harmful_blapper", Name = "Harmful Blapper", Description = "Blam! Blam! Blam! Blam!" };
        def.StatBonuses["Might"] = 10; def.StatBonuses["Dexterity"] = 10; def.StatBonuses["Intelligence"] = 10;
        def.StatBonuses["Vitality"] = 10; def.StatBonuses["Reflex"] = 10;
        def.Equipment["Head"]     = new Guid("154e0f8c-198a-4aab-95b7-d50d717a90b1"); // Golden Valkyrie Helm
        def.Equipment["Armor"]    = new Guid("a231a488-cd2f-45a3-90d0-2f4481626b36"); // Golden Valkyrie Plate
        def.Equipment["MainHand"] = new Guid("c0347ea4-2c2d-49a1-bfdf-080ec6844daf"); // Last Word
        def.Equipment["OffHand"]  = new Guid("c0347ea4-2c2d-49a1-bfdf-080ec6844daf"); // Last Word
        def.StartingSkillGuids.Add(new Guid("adca2a3d-dfb9-496a-9937-345c8fd286e7")); // Harm
        ClassDefs.Add(def);

        def = new CustomClassDef { Id = "cls_one_punch_man", Name = "One Punch Man", Description = "ONE PUNCH MAAAAAN!" };
        def.StatBonuses["Might"] = 14; def.StatBonuses["Dexterity"] = 12; def.StatBonuses["Intelligence"] = 4;
        def.StatBonuses["Vitality"] = 10; def.StatBonuses["Reflex"] = 10;
        def.Equipment["Armor"]    = new Guid("352e9ddc-4370-4b25-b72a-371b28c02a70"); // Canary Coat
        def.Equipment["MainHand"] = new Guid("38b4a977-74d1-4649-a6b5-d0da5fae5650"); // Dragon Fist
        def.StartingSkillGuids.Add(new Guid("1e089acc-84fc-4d8f-8432-915e5daa7d9d")); // One Punch Monk
        def.StartingSkillGuids.Add(new Guid("72730fff-2170-46e2-9a5a-3c26c1baee98")); // Strength
        def.StartingSkillGuids.Add(new Guid("eedc261a-8bc6-48f8-91b2-e1e0587b022e")); // Focused Strike
        def.StartingSkillGuids.Add(new Guid("126bb043-6548-4075-bca8-77e5f90fe992")); // Hot Head I
        def.StartingSkillGuids.Add(new Guid("72a6afb2-c7e6-4918-a1d3-4f08680e2b73")); // Hot Head II
        def.StartingSkillGuids.Add(new Guid("a151ffba-bc8a-4e8c-95c2-a209e2e3ac98")); // Warmonger I
        def.StartingSkillGuids.Add(new Guid("7f5cbda1-f1fd-4f71-9347-e00ffbf5e96b")); // Warmonger II
        def.StartingSkillGuids.Add(new Guid("b22fceef-4110-46ee-a9d2-12e9dd9f8d82")); // Light's Strength
        ClassDefs.Add(def);

        // ─────────────────────────────────────────────────────────────
        // The Oracle — contributed by Claude.
        // Theme: a diviner who sees patterns across the battlefield,
        // links fates, and makes targets unravel together. Oculus Stone
        // grants Teleport Self (the Oracle steps between what others see
        // as distance), Soul Link couples enemy fates, Spectral Mark
        // amplifies the next strike, Chain Lightning cascades through
        // those connections. Int/Reflex-leaning caster with scholar's
        // attire to fit the archivist aesthetic.
        // ─────────────────────────────────────────────────────────────
        def = new CustomClassDef { Id = "cls_oracle", Name = "The Oracle", Description = "Sees the threads. Pulls them taut." };
        def.StatBonuses["Might"] = 6; def.StatBonuses["Dexterity"] = 10; def.StatBonuses["Intelligence"] = 14;
        def.StatBonuses["Vitality"] = 10; def.StatBonuses["Reflex"] = 12;
        def.Equipment["Head"]     = new Guid("4c167516-478a-49ab-bf17-1afdee99ef07"); // Scholar's Hat
        def.Equipment["Armor"]    = new Guid("05853776-7d91-46c1-b09f-182ecf304536"); // Scholar's Attire
        def.Equipment["MainHand"] = new Guid("5ed3483d-4507-4277-994f-6f9a41defd95"); // Oculus Stone (grants Teleport Self)
        def.StartingSkillGuids.Add(new Guid("455db553-430b-47c6-8f6f-a5576c034b0a")); // Soul Link
        def.StartingSkillGuids.Add(new Guid("043b48ce-e6b3-4ba0-9258-e4d06f46b82b")); // Stalker's Mark
        def.StartingSkillGuids.Add(new Guid("eb59a98e-0811-40fd-ad20-482c66ea821a")); // Chain Lightning
        ClassDefs.Add(def);
    }

    public static Game GetGameInstance()
    {
        try {
            var prop = typeof(Game).GetProperty("Instance",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (prop != null) return prop.GetValue(null) as Game;
        } catch { }
        try {
            var field = typeof(Game).GetField("_instance",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (field != null) return field.GetValue(null) as Game;
        } catch { }
        return null;
    }

    public static CharacterAttribute FindAttribute(string name)
    {
        try {
            var game = GetGameInstance();
            if (game == null) return null;
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            string[] candidates = { "CharacterAttributes", "Attributes", "CharacterVariableAttributes", "VariableAttributes" };
            foreach (var propName in candidates) {
                var prop = typeof(Game).GetProperty(propName, flags);
                if (prop == null) continue;
                var list = prop.GetValue(game) as IEnumerable;
                if (list == null) continue;
                foreach (var item in list) {
                    var attr = item as CharacterAttribute;
                    if (attr == null) continue;
                    var asObj = (UnityEngine.Object)(object)attr;
                    if (asObj != null && asObj.name == name) return attr;
                }
            }
        } catch { }
        return null;
    }
}

public class CustomClassDef
{
    public string Id;
    public string Name;
    public string Description;
    public Dictionary<string, int>   StatBonuses      = new Dictionary<string, int>();
    public Dictionary<string, Guid>  Equipment        = new Dictionary<string, Guid>();
    public Dictionary<string, float> PassiveBonuses   = new Dictionary<string, float>();
    public List<Guid> StartingSkillGuids   = new List<Guid>();
    public List<Guid> StartingFortuneGuids = new List<Guid>();

    public Guid GeneratedGuid {
        get {
            using (var md5 = MD5.Create()) {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes("sc:" + Id));
                return new Guid(hash);
            }
        }
    }
}

// ============================================================================
// PATCH 1: PresetManager.UpdateRoguelikeCharacterPresetTiles (postfix) - build + inject tiles
// ============================================================================
[HarmonyPatch(typeof(PresetManager), "UpdateRoguelikeCharacterPresetTiles")]
public static class PresetInjectPatch
{
    private static Dictionary<string, CharacterPresetFile> _runtimePresets = new Dictionary<string, CharacterPresetFile>();
    private static FieldInfo _fMight, _fDex, _fVit, _fInt, _fRef, _fTotal;
    private static FieldInfo _fPresetName, _fPresetDesc, _fTier, _fPresetType, _fNeedsUnlock, _fSpecial, _fSkills;
    private static FieldInfo _fHead, _fArmor, _fMainHand, _fOffHand;
    private static FieldInfo _fHolder, _fPrefab, _fTileList, _fTilePreset;
    private static MethodInfo _mTileSetPreset, _mGetFromPresetDict;
    private static FieldInfo _fGamePresetArr;
    private static readonly string[] AppearanceFields = {
        "Gender", "SkinColorIndex", "HairColorIndex", "HairTypeIndex", "HeadTypeIndex",
        "EyebrowTypeIndex", "FacialHairTypeIndex", "BodyArtColorIndex", "EyeColorIndex", "StubbleColorIndex"
    };
    private static bool _resolved;

    [HarmonyPostfix]
    public static void Postfix(PresetManager __instance)
    {
        try {
            if (!_resolved) Resolve();
            var game = StartingClassesPlugin.GetGameInstance();
            RegisterOffDictSkills(game);
            foreach (var def in StartingClassesPlugin.ClassDefs) {
                var preset = EnsurePreset(def, game, true);
                if (preset != null) AppendTile(__instance, preset);
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError("PresetInject error: " + ex);
        }
    }

    // Some skills (e.g. "Ignite" granted by the Apocalypse weapon) exist in the
    // game data but are NOT registered in Game._SkillInfo_Dict, so Game.GetFromSkillDict
    // returns null for their GUID and our BuildSkills silently skips them. Walk every
    // item's GrantedSkills / SkillTriggers.Actions once per session and inject any
    // missing SkillInfo into the dict so starting-skill resolution works normally.
    //
    // NOTE: The items dict may be empty very early (e.g. when PresetDictPostfixPatch fires
    // on the initial preset screen load, before item data is loaded). In that case this
    // method does nothing and we leave the guard flag false so a later call retries.
    private static bool _offDictSkillsRegistered;
    private static void RegisterOffDictSkills(Game game)
    {
        if (_offDictSkillsRegistered || game == null) return;
        try {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var fSkillDict = typeof(Game).GetField("_SkillInfo_Dict", flags);
            var fItemsDict = typeof(Game).GetField("_Items_Dict", flags);
            if (fSkillDict == null || fItemsDict == null) return;
            var skillDict = fSkillDict.GetValue(game) as IDictionary;
            var itemsDict = fItemsDict.GetValue(game) as IDictionary;
            if (skillDict == null || itemsDict == null || itemsDict.Count == 0) return;

            var fGuid          = typeof(SkillInfo).GetField("Guid");
            var fGranted       = typeof(ItemInfo).GetField("GrantedSkills");
            var fSkillTriggers = typeof(ItemInfo).GetField("SkillTriggers");
            var fActions       = typeof(SkillTrigger).GetField("Actions");

            int added = 0;
            foreach (DictionaryEntry kv in itemsDict) {
                var item = kv.Value as ItemInfo;
                if (item == null) continue;

                var candidates = new List<SkillInfo>();
                var gs = fGranted != null ? fGranted.GetValue(item) as IEnumerable : null;
                if (gs != null) foreach (var s in gs) { var si = s as SkillInfo; if (si != null) candidates.Add(si); }

                var trigs = fSkillTriggers != null ? fSkillTriggers.GetValue(item) as IEnumerable : null;
                if (trigs != null) foreach (var trig in trigs) {
                    if (trig == null) continue;
                    var acts = fActions != null ? fActions.GetValue(trig) as IEnumerable : null;
                    if (acts == null) continue;
                    foreach (var a in acts) { var si = a as SkillInfo; if (si != null) candidates.Add(si); }
                }

                foreach (var si in candidates) {
                    var g = fGuid != null ? (Guid)fGuid.GetValue(si) : Guid.Empty;
                    if (g == Guid.Empty || skillDict.Contains(g)) continue;
                    skillDict[g] = si;
                    added++;
                }
            }
            _offDictSkillsRegistered = true;
            if (added > 0) StartingClassesPlugin.Log.LogInfo(string.Format(
                "Registered {0} off-dict item skill(s) into _SkillInfo_Dict.", added));
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogWarning("RegisterOffDictSkills: " + ex.Message);
        }
    }

    public static bool HasCached(string id) { return _runtimePresets.ContainsKey(id); }
    public static CharacterPresetFile GetCachedPreset(string id) {
        CharacterPresetFile p; return _runtimePresets.TryGetValue(id, out p) ? p : null;
    }

    public static CharacterPresetFile EnsurePreset(CustomClassDef def, Game game, bool registerInGame)
    {
        RegisterOffDictSkills(game); // ensure item-only skills are resolvable before we build anything
        CharacterPresetFile existing;
        if (_runtimePresets.TryGetValue(def.Id, out existing)) {
            // If this preset was built before off-dict skills were available, some starting
            // skills may have been silently skipped. Re-run BuildSkills so newly-registered
            // skills (e.g. Ignite, available only after _Items_Dict is populated) get picked up.
            try {
                int wantedSkills = def.StartingSkillGuids.Count(g => g != Guid.Empty);
                var skillsArr = _fSkills != null ? _fSkills.GetValue(existing) as Array : null;
                int currentSkills = skillsArr != null ? skillsArr.Length : 0;
                if (wantedSkills > currentSkills) {
                    BuildSkills(existing, def, game);
                }
            } catch { }
            if (registerInGame) RegisterPreset(existing, game);
            return existing;
        }
        if (!_resolved) Resolve();
        if (game == null) return null;
        try {
            CharacterPresetFile[] pool = null;
            if (_fGamePresetArr != null) pool = _fGamePresetArr.GetValue(game) as CharacterPresetFile[];
            if (pool == null) {
                var prop = typeof(Game).GetProperty("CharacterPresetFiles",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                pool = (prop != null ? prop.GetValue(game) : null) as CharacterPresetFile[];
            }
            if (pool == null || pool.Length == 0) return null;

            var template = Array.Find(pool, p => {
                if (p == null || _fPresetType == null) return false;
                var v = _fPresetType.GetValue(p);
                return (int)Convert.ChangeType(v, typeof(int)) == 1;
            }) ?? pool[0];

            var preset = (CharacterPresetFile)(object)UnityEngine.Object.Instantiate((UnityEngine.Object)(object)template);
            preset.Guid = def.GeneratedGuid;
            SF(preset, _fPresetName, def.Name);
            SF(preset, _fPresetDesc, def.Description);
            SF(preset, _fNeedsUnlock, false);
            if (_fPresetType != null)
                SF(preset, _fPresetType, Enum.ToObject(_fPresetType.FieldType, 1));

            RandomizeAppearance(preset, pool, def.Id.GetHashCode());

            SF(preset, _fHead, null);
            SF(preset, _fArmor, null);
            SF(preset, _fMainHand, null);
            SF(preset, _fOffHand, null);
            SF(preset, _fSkills, new SkillInfo[0]);
            SF(preset, _fSpecial, new SpecialPresetInfo[0]);

            int mig=0, dex=0, vit=0, intl=0, ref_=0;
            def.StatBonuses.TryGetValue("Might", out mig);
            def.StatBonuses.TryGetValue("Dexterity", out dex);
            def.StatBonuses.TryGetValue("Vitality", out vit);
            def.StatBonuses.TryGetValue("Intelligence", out intl);
            def.StatBonuses.TryGetValue("Reflex", out ref_);
            SF(preset, _fMight, mig); SF(preset, _fDex, dex); SF(preset, _fVit, vit);
            SF(preset, _fInt, intl); SF(preset, _fRef, ref_); SF(preset, _fTotal, mig+dex+vit+intl+ref_);

            BuildEquipment(preset, def, game);
            BuildPassives(preset, def);
            BuildSkills(preset, def, game);

            _runtimePresets[def.Id] = preset;
            StartingClassesPlugin.Log.LogInfo(string.Format(
                "  Created preset: {0} M{1}/D{2}/V{3}/I{4}/R{5}", def.Name, mig, dex, vit, intl, ref_));

            if (registerInGame) RegisterPreset(preset, game);
            return preset;
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError(string.Format("EnsurePreset({0}): ", def.Name) + ex);
            return null;
        }
    }

    private static void RandomizeAppearance(CharacterPresetFile preset, CharacterPresetFile[] pool, int seed)
    {
        try {
            var customGuids = new HashSet<Guid>(StartingClassesPlugin.ClassDefs.Select(d => d.GeneratedGuid));
            var vanilla = pool.Where(p => p != null && !customGuids.Contains(p.Guid)).ToArray();
            if (vanilla.Length == 0) return;
            var rng = new System.Random(seed);
            var donor = vanilla[rng.Next(vanilla.Length)];
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var name in AppearanceFields) {
                var f = typeof(CharacterPresetFile).GetField(name, flags);
                if (f != null) f.SetValue(preset, f.GetValue(donor));
            }
        } catch { }
    }

    private static void BuildEquipment(CharacterPresetFile preset, CustomClassDef def, Game game)
    {
        if (def.Equipment.Count == 0) return;
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var getItem = typeof(Game).GetMethods(flags).FirstOrDefault(m =>
            (m.Name.Contains("ItemInfo") || m.Name == "GetFromItemDict") &&
            m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(Guid));
        if (getItem == null) return;
        var slotMap = new Dictionary<string, FieldInfo> {
            { "Head", _fHead }, { "Armor", _fArmor },
            { "MainHand", _fMainHand }, { "OffHand", _fOffHand }
        };
        foreach (var kv in def.Equipment) {
            FieldInfo slot;
            if (!slotMap.TryGetValue(kv.Key, out slot) || slot == null) continue;
            try {
                var item = getItem.Invoke(game, new object[] { kv.Value });
                if (item != null) slot.SetValue(preset, item);
            } catch { }
        }
    }

    private static void BuildPassives(CharacterPresetFile preset, CustomClassDef def)
    {
        if (_fSpecial == null || def.PassiveBonuses.Count == 0) return;
        var variableAttrs = new HashSet<string> { "MaxHealth", "MaxMana", "TurnManaPerRecovery" };
        var effects = new List<CharacterEffectInfo>();
        foreach (var kv in def.PassiveBonuses) {
            var attr = StartingClassesPlugin.FindAttribute(kv.Key);
            if (attr == null) continue;
            var eff = new CharacterEffectInfo {
                EffectTarget = default(EffectTarget),
                CharacterAttribute = attr,
                CharacterEffectMethod = variableAttrs.Contains(kv.Key) ? (CharacterEffectMethod)1 : (CharacterEffectMethod)0,
                Amount = kv.Value.ToString(),
                Infinite = true,
            };
            effects.Add(eff);
        }
        if (effects.Count == 0) return;
        var special = new SpecialPresetInfo {
            Title = def.Name + " passives",
            Description = def.Description,
            SkillTriggers = new SkillTrigger[0],
            CharacterEffects = effects.ToArray(),
        };
        SF(preset, _fSpecial, new[] { special });
    }

    private static void BuildSkills(CharacterPresetFile preset, CustomClassDef def, Game game)
    {
        if (_fSkills == null || def.StartingSkillGuids.Count == 0 || game == null) return;
        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var getSkill = typeof(Game).GetMethods(flags).FirstOrDefault(m =>
            m.Name == "GetFromSkillDict" && m.GetParameters().Length == 1 &&
            m.GetParameters()[0].ParameterType == typeof(Guid));
        if (getSkill == null) return;
        var skills = new List<SkillInfo>();
        var missing = new List<Guid>();
        foreach (var g in def.StartingSkillGuids.Where(x => x != Guid.Empty)) {
            try {
                var s = getSkill.Invoke(game, new object[] { g }) as SkillInfo;
                if (s != null) skills.Add(s);
                else missing.Add(g);
            } catch { missing.Add(g); }
        }
        if (skills.Count > 0) {
            SF(preset, _fSkills, skills.ToArray());
            var fSkillName = typeof(SkillInfo).GetField("SkillName");
            var names = skills.Select(s => fSkillName != null ? fSkillName.GetValue(s) as string : "?").ToArray();
            StartingClassesPlugin.Log.LogInfo(string.Format(
                "  {0} starting skills: [{1}]{2}",
                def.Name, string.Join(", ", names),
                missing.Count > 0 ? " (missing: " + string.Join(", ", missing.Select(x => x.ToString()).ToArray()) + ")" : ""));
        }
    }

    private static void AppendTile(PresetManager mgr, CharacterPresetFile preset)
    {
        if (_fHolder == null || _fPrefab == null) return;
        var holder = _fHolder.GetValue(mgr) as Transform;
        var prefab = _fPrefab.GetValue(mgr) as CharacterPresetTile;
        if (holder == null || prefab == null) return;

        if (_fTileList != null) {
            var list = _fTileList.GetValue(mgr) as IList;
            if (list != null) {
                foreach (var t in list) {
                    if (t == null) continue;
                    object existing = null;
                    if (_fTilePreset != null) { try { existing = _fTilePreset.GetValue(t); } catch { } }
                    if (existing != null && ReferenceEquals(existing, preset)) return;
                }
            }
        }

        try {
            var instWithParent = typeof(UnityEngine.Object).GetMethods().FirstOrDefault(m =>
                m.Name == "Instantiate" && m.IsGenericMethodDefinition
                && m.GetParameters().Length == 2
                && m.GetParameters()[1].ParameterType == typeof(Transform));
            CharacterPresetTile tile = null;
            if (instWithParent != null) {
                var closed = instWithParent.MakeGenericMethod(typeof(CharacterPresetTile));
                tile = closed.Invoke(null, new object[] { prefab, holder }) as CharacterPresetTile;
            }
            if (tile == null) return;

            if (_mTileSetPreset != null) {
                try { _mTileSetPreset.Invoke(tile, new object[] { preset }); }
                catch (Exception ex) { StartingClassesPlugin.Log.LogWarning("  Tile setter failed: " + ex.Message); }
            } else if (_fTilePreset != null) {
                _fTilePreset.SetValue(tile, preset);
            }
            if (_fTileList != null) {
                var list = _fTileList.GetValue(mgr) as IList;
                if (list != null) list.Add(tile);
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError("AppendTile: " + ex);
        }
    }

    private static string def_name(CharacterPresetFile p) {
        try { return (_fPresetName != null ? _fPresetName.GetValue(p) as string : null) ?? "?"; }
        catch { return "?"; }
    }

    private static void RegisterPreset(CharacterPresetFile preset, Game game)
    {
        if (game == null) return;
        try {
            var fDict = typeof(Game).GetField("_CharacterPresetFile_Dict",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fDict != null) {
                var dict = fDict.GetValue(game) as IDictionary;
                if (dict == null && _mGetFromPresetDict != null) {
                    try { _mGetFromPresetDict.Invoke(game, new object[] { Guid.Empty }); } catch { }
                    dict = fDict.GetValue(game) as IDictionary;
                }
                if (dict != null && !dict.Contains(preset.Guid)) {
                    dict[preset.Guid] = preset;
                    StartingClassesPlugin.Log.LogInfo(string.Format("  Dict registered: {0}", def_name(preset)));
                }
            }
            if (_fGamePresetArr != null) {
                var arr = _fGamePresetArr.GetValue(game) as CharacterPresetFile[];
                if (arr != null && Array.IndexOf(arr, preset) < 0) {
                    var newArr = new CharacterPresetFile[arr.Length + 1];
                    Array.Copy(arr, newArr, arr.Length);
                    newArr[arr.Length] = preset;
                    _fGamePresetArr.SetValue(game, newArr);
                    StartingClassesPlugin.Log.LogInfo(string.Format(
                        "  Array registered: {0} (index {1})", def_name(preset), arr.Length));
                }
            }
            if (_mGetFromPresetDict != null) {
                try { _mGetFromPresetDict.Invoke(game, new object[] { preset.Guid }); } catch { }
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError("RegisterPreset: " + ex);
        }
    }

    public static void Resolve()
    {
        _resolved = true;
        var pub = BindingFlags.Public | BindingFlags.Instance;
        var all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var tPreset = typeof(CharacterPresetFile);
        var tMgr = typeof(PresetManager);
        var tTile = typeof(CharacterPresetTile);
        var tGame = typeof(Game);

        _fMight = tPreset.GetField("Might", pub);
        _fDex = tPreset.GetField("Dexterity", pub);
        _fVit = tPreset.GetField("Vitality", pub);
        _fInt = tPreset.GetField("Intelligence", pub);
        _fRef = tPreset.GetField("Reflex", pub);
        _fTotal = tPreset.GetField("TotalStatPoints", pub);
        _fPresetName = tPreset.GetField("PresetName", pub);
        _fPresetDesc = tPreset.GetField("PresetDescription", pub);
        _fTier = tPreset.GetField("Tier", pub);
        _fPresetType = tPreset.GetField("PresetType", pub);
        _fNeedsUnlock = tPreset.GetField("NeedsUnlock", pub);
        _fSpecial = tPreset.GetField("SpecialPresetInfo", pub);
        _fSkills = tPreset.GetField("StartingSkills", pub);
        _fHead = tPreset.GetField("Head", pub);
        _fArmor = tPreset.GetField("Armor", pub);
        _fMainHand = tPreset.GetField("MainHand", pub);
        _fOffHand = tPreset.GetField("OffHand", pub);

        _fHolder = tMgr.GetField("CharacterPresetTileContainer", all);
        _fPrefab = tMgr.GetField("CharacterPresetTilePrefab", all);
        _fTileList = tMgr.GetField("_CharacterPresetTiles", all);
        _fTilePreset = tTile.GetField("_CharacterPresetFile", all);
        var ptp = tTile.GetProperty("CharacterPresetFile", all);
        _mTileSetPreset = ptp != null ? ptp.GetSetMethod(true) : tTile.GetMethod("set_CharacterPresetFile", all);

        _fGamePresetArr = tGame.GetField("_CharacterPresetFiles",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        _mGetFromPresetDict = tGame.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "GetFromCharacterPresetFileDict"
                              && m.GetParameters().Length == 1
                              && m.GetParameters()[0].ParameterType == typeof(Guid));

        StartingClassesPlugin.Log.LogInfo(string.Format(
            "PresetInject resolve\n  stats:{0} name:{1} skills:{2} equip:{3} holder:{4} prefab:{5} list:{6} tile:{7} setter:{8} gameArr:{9} dictGetter:{10}",
            _fMight != null, _fPresetName != null, _fSkills != null, _fArmor != null, _fHolder != null,
            _fPrefab != null, _fTileList != null, _fTilePreset != null,
            _mTileSetPreset != null, _fGamePresetArr != null, _mGetFromPresetDict != null));
    }

    private static void SF(object o, FieldInfo f, object v)
    {
        if (f == null || o == null) return;
        try { f.SetValue(o, v); } catch { }
    }
}

// ============================================================================
// PATCH 2: Game.GetFromCharacterPresetFileDict postfix
// ============================================================================
[HarmonyPatch(typeof(Game), "GetFromCharacterPresetFileDict")]
public static class PresetDictPostfixPatch
{
    private static bool _busy;

    [HarmonyPostfix]
    public static void Postfix(Game __instance, Guid guid, ref CharacterPresetFile __result)
    {
        if (__result != null || _busy) return;
        var def = StartingClassesPlugin.ClassDefs.FirstOrDefault(d => d.GeneratedGuid == guid);
        if (def == null) return;
        try {
            _busy = true;
            if (!PresetInjectPatch.HasCached(def.Id)) {
                __result = PresetInjectPatch.EnsurePreset(def, __instance, false);
                if (__result != null)
                    StartingClassesPlugin.Log.LogInfo(string.Format(
                        "  Dict postfix (early): built {0} without registering", def.Name));
            } else {
                __result = PresetInjectPatch.EnsurePreset(def, __instance, true);
                if (__result != null)
                    StartingClassesPlugin.Log.LogInfo(string.Format("  Dict postfix: loaded {0}", def.Name));
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError("PresetDictPostfixPatch: " + ex);
        } finally {
            _busy = false;
        }
    }
}

// ============================================================================
// PATCH 3: PresetManager.GetCharacterPresetFiles(GameMode) - MP client fix
// ============================================================================
[HarmonyPatch(typeof(PresetManager), "GetCharacterPresetFiles", new Type[] { typeof(GameMode) })]
public static class GetCharacterPresetFilesPatch
{
    [HarmonyPostfix]
    public static void Postfix(GameMode gameMode, ref List<CharacterPresetFile> __result)
    {
        if (__result == null || gameMode != GameMode.Roguelike) return;
        try {
            foreach (var def in StartingClassesPlugin.ClassDefs) {
                var preset = PresetInjectPatch.GetCachedPreset(def.Id);
                if (preset != null && !__result.Contains(preset))
                    __result.Add(preset);
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError("GetCharacterPresetFilesPatch: " + ex);
        }
    }
}

// ============================================================================
// PATCH 4: Character.SyncCharacterToPreset postfix - passives + queue fortunes
// ============================================================================
[HarmonyPatch(typeof(Character), "SyncCharacterToPreset")]
public static class ApplyPassivesPatch
{
    private static readonly Dictionary<Character, List<Guid>> _pendingFortunes = new Dictionary<Character, List<Guid>>();

    [HarmonyPostfix]
    public static void Postfix(Character __instance, CharacterPresetFile preset)
    {
        try {
            if (preset == null) return;
            _pendingFortunes.Remove(__instance);

            var def = StartingClassesPlugin.ClassDefs.FirstOrDefault(d => d.GeneratedGuid == preset.Guid);
            if (def == null) return;
            if (def.PassiveBonuses.Count == 0 && def.StartingFortuneGuids.Count == 0) return;

            var mAddAttr = typeof(Character).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == "AddAttribute" && m.GetParameters().Length == 3);
            if (mAddAttr == null) {
                StartingClassesPlugin.Log.LogWarning("ApplyPassives: AddAttribute not found!");
                return;
            }

            foreach (var kv in def.PassiveBonuses) {
                var attr = StartingClassesPlugin.FindAttribute(kv.Key);
                if (attr == null) {
                    StartingClassesPlugin.Log.LogWarning(string.Format("ApplyPassives: attribute '{0}' not found", kv.Key));
                    continue;
                }
                mAddAttr.Invoke(__instance, new object[] { attr, kv.Value, 0f });
                StartingClassesPlugin.Log.LogInfo(string.Format("  Applied passive: {0} +{1} to {2}", kv.Key, kv.Value, def.Name));
            }

            if (def.StartingFortuneGuids.Count > 0) {
                _pendingFortunes[__instance] = new List<Guid>(def.StartingFortuneGuids);
                StartingClassesPlugin.Log.LogInfo(string.Format(
                    "  Queued {0} fortune(s) for {1} (will apply in FinalizeCharacterCreation)",
                    def.StartingFortuneGuids.Count, def.Name));
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError("ApplyPassivesPatch: " + ex);
        }
    }

    public static void FlushPendingFortunes()
    {
        if (_pendingFortunes.Count == 0) return;
        var mAddFortune = typeof(Character).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "AddFortune" && m.GetParameters().Length == 2);
        if (mAddFortune == null) {
            StartingClassesPlugin.Log.LogWarning("FlushFortunes: AddFortune(Guid,int) not found - fortunes skipped.");
            _pendingFortunes.Clear();
            return;
        }
        var snapshot = new Dictionary<Character, List<Guid>>(_pendingFortunes);
        _pendingFortunes.Clear();
        foreach (var kv in snapshot) {
            foreach (var guid in kv.Value) {
                try {
                    mAddFortune.Invoke(kv.Key, new object[] { guid.ToString(), 1 });
                    StartingClassesPlugin.Log.LogInfo(string.Format("  Applied fortune (deferred): {0}", guid));
                    TryAutoEquipFortune(kv.Key, guid.ToString());
                } catch (Exception ex) {
                    StartingClassesPlugin.Log.LogError(string.Format("  Fortune {0} failed: {1}", guid, ex.Message));
                }
            }
        }
    }

    private static void TryAutoEquipFortune(Character character, string guidStr)
    {
        try {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var pData = typeof(Character).GetProperty("FortuneData", flags);
            var list = (pData != null ? pData.GetValue(character) : null) as IList;
            if (list == null) return;
            foreach (var fsd in list) {
                if (fsd == null) continue;
                var t = fsd.GetType();
                object got = null;
                var fGuid = t.GetField("Guid", flags);
                if (fGuid != null) got = fGuid.GetValue(fsd);
                else {
                    var pGuid = t.GetProperty("Guid", flags);
                    if (pGuid != null) got = pGuid.GetValue(fsd);
                }
                if (got == null || got.ToString() != guidStr) continue;
                var fIdx = t.GetField("EquippedSlotIndex", flags);
                if (fIdx != null) {
                    fIdx.SetValue(fsd, 0);
                    var mLoad = typeof(Character).GetMethods(flags)
                        .FirstOrDefault(m => m.Name == "LoadFortunes" && m.GetParameters().Length == 0);
                    if (mLoad != null) mLoad.Invoke(character, null);
                }
                break;
            }
        } catch { }
    }
}

// ============================================================================
// PATCH 5: GameLogic.FinalizeCharacterCreation - flush queued fortunes + catch + log exceptions
// ============================================================================
[HarmonyPatch(typeof(GameLogic), "FinalizeCharacterCreation")]
public static class FinalizeCharacterCreationPatch
{
    [HarmonyPrefix]
    public static void Prefix(string name)
    {
        StartingClassesPlugin.Log.LogInfo(string.Format("=== FinalizeCharacterCreation() entered, name='{0}'", name));
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
        StartingClassesPlugin.Log.LogInfo("=== FinalizeCharacterCreation() completed normally");
        try { ApplyPassivesPatch.FlushPendingFortunes(); }
        catch (Exception ex) { StartingClassesPlugin.Log.LogError("FinalizeCharacterCreationPatch: " + ex); }
    }

    [HarmonyFinalizer]
    public static Exception Finalizer(Exception __exception)
    {
        if (__exception != null) {
            StartingClassesPlugin.Log.LogError(string.Format(
                "=== FCC THREW {0}: {1}", __exception.GetType().Name, __exception.Message));
            StartingClassesPlugin.Log.LogError("=== Stack: " + __exception.StackTrace);
            if (__exception.InnerException != null)
                StartingClassesPlugin.Log.LogError("=== Inner: " + __exception.InnerException);
        }
        return __exception; // re-throw so we don't mask game issues
    }
}

// ============================================================================
// PROPER FIX: Extra Meta Progression (EMP) only expands RoguelikePowerup.PowerupLevels
// when the powerup WINDOW opens, which happens AFTER character creation. So the first
// time the game calculates stats for your saved roguelike powerups, PowerupLevels is
// still the vanilla tiny array, and any saved Level > that length crashes.
//
// We force EMP's expansion to run early, the first time the game touches the powerup
// dictionary (which is well before FinalizeCharacterCreation calls GetAttributeValueByMethod).
// If EMP isn't installed or its API fails, we silently no-op and the band-aid below
// still keeps things functional.
// ============================================================================
[HarmonyPatch(typeof(Game), "GetFromRoguelikePowerupDict")]
public static class ExpandPowerupsEarly
{
    private static bool _done;

    [HarmonyPrefix]
    public static void Prefix()
    {
        if (_done) return;
        _done = true;
        try { ForceEmpExpansion(); }
        catch (Exception ex) {
            StartingClassesPlugin.Log.LogWarning("Early EMP expansion failed: " + ex.Message);
        }
    }

    private static void ForceEmpExpansion()
    {
        const BindingFlags anyStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        const BindingFlags anyInst   = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        // Find EMP plugin type across loaded assemblies.
        Type empType = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            try {
                empType = asm.GetType("ExtraMetaProgression.ExtraMetaProgressionPlugin");
                if (empType != null) break;
            } catch { }
        }
        if (empType == null) return; // EMP not installed, nothing to do.

        var mGetVanilla  = empType.GetMethod("GetVanillaPowerups",  anyStatic);
        var mBuildLevels = empType.GetMethod("BuildVanillaLevels",  anyStatic);
        var mGetAttrRef  = empType.GetMethod("GetVanillaAttrRef",   anyStatic);
        var mGetAttrName = empType.GetMethod("GetVanillaAttrName",  anyStatic);
        var fVanillaDefs = empType.GetField ("VanillaDefs",         anyStatic);
        if (mGetVanilla == null || mBuildLevels == null || mGetAttrRef == null ||
            mGetAttrName == null || fVanillaDefs == null) {
            StartingClassesPlugin.Log.LogWarning("Early EMP expansion: API shape has changed, skipping.");
            return;
        }

        // Build TWO lookup dicts matching EMP's own logic:
        //   VanillaName   (e.g. "Might", "Critical Chance") -> def
        //   AttributeName (e.g. "m_maxhealth", "m_critchance") -> def
        // Powerups match via either path.
        var defs = fVanillaDefs.GetValue(null) as IEnumerable;
        if (defs == null) return;
        var byName = new Dictionary<string, object>();
        var byAttr = new Dictionary<string, object>();
        foreach (var def in defs) {
            if (def == null) continue;
            var tDef = def.GetType();
            var vName = tDef.GetField("VanillaName");
            var aName = tDef.GetField("AttributeName");
            var vn = vName != null ? vName.GetValue(def) as string : null;
            var an = aName != null ? aName.GetValue(def) as string : null;
            if (!string.IsNullOrEmpty(vn)) byName[vn] = def;
            if (!string.IsNullOrEmpty(an)) byAttr[an] = def;
        }

        // Enumerate live RoguelikePowerup objects.
        var powerups = mGetVanilla.Invoke(null, null) as IEnumerable;
        if (powerups == null) return;

        var tPw = typeof(RoguelikePowerup);
        var fName2   = tPw.GetField("Name",          anyInst);
        var fLevels  = tPw.GetField("PowerupLevels", anyInst);
        if (fName2 == null || fLevels == null) return;

        int matched = 0, skipped = 0;
        foreach (var pw in powerups) {
            if (pw == null) continue;
            var pwName = fName2.GetValue(pw) as string ?? "";

            // Try name match first, then fall back to attribute-name match.
            object def;
            if (!byName.TryGetValue(pwName, out def)) {
                string attrName = null;
                try { attrName = mGetAttrName.Invoke(null, new object[] { pw }) as string; } catch { }
                if (string.IsNullOrEmpty(attrName) || !byAttr.TryGetValue(attrName, out def)) {
                    skipped++;
                    continue;
                }
            }
            try {
                var existingAttr = mGetAttrRef.Invoke(null, new object[] { pw });
                var newLevels = mBuildLevels.Invoke(null, new object[] { def, existingAttr });
                if (newLevels != null) {
                    fLevels.SetValue(pw, newLevels);
                    matched++;
                }
            } catch (Exception ex) {
                StartingClassesPlugin.Log.LogWarning("  expand " + pwName + ": " + ex.Message);
            }
        }
        StartingClassesPlugin.Log.LogInfo(string.Format(
            "Force-expanded {0} vanilla roguelike powerup(s) ({1} had no matching def).",
            matched, skipped));

        // Also register EMP's custom (Append) powerups early. Otherwise any saved
        // meta-progression levels on them won't apply to stats on the first
        // character created in a session (user saw e.g. 110 HP first time, 114 HP
        // after opening the powerup window once). Replicates steps 1-5 of EMP's
        // PopulatePatch.Postfix; the UI-tile step (6) is left to EMP when the
        // window actually opens.
        try {
            var fAppendDefs = empType.GetField("AppendDefs", anyStatic);
            var fRuntimePowerupObjects = empType.GetField("RuntimePowerupObjects", anyStatic);
            var mEnsureSave = empType.GetMethod("EnsurePowerupsInSaveData", anyStatic);
            var mBuildCustomLevels = empType.GetMethod("BuildLevels", anyStatic);
            var mRegisterInGame = empType.GetMethod("RegisterInGameDict", anyStatic);
            if (fAppendDefs == null || mBuildCustomLevels == null) return;

            if (mEnsureSave != null)
                try { mEnsureSave.Invoke(null, null); } catch (Exception ex) {
                    StartingClassesPlugin.Log.LogWarning("EnsureSave: " + ex.Message);
                }

            var appendDefs = fAppendDefs.GetValue(null) as IEnumerable;
            if (appendDefs == null) return;

            // The runtime dict is Dictionary<string, RoguelikePowerup> keyed by def.Id.
            var runtimeDict = fRuntimePowerupObjects != null
                ? fRuntimePowerupObjects.GetValue(null) as IDictionary
                : null;

            var tPowerup = typeof(RoguelikePowerup);
            var fGuid = tPowerup.GetField("Guid", anyInst);
            var fPwName = tPowerup.GetField("Name", anyInst);
            var fPwLevels = tPowerup.GetField("PowerupLevels", anyInst);

            int created = 0, already = 0;
            foreach (var def in appendDefs) {
                if (def == null) continue;
                var tDef = def.GetType();
                var fId = tDef.GetField("Id");
                var fDefName = tDef.GetField("Name");
                var pGenGuid = tDef.GetProperty("GeneratedGuid");
                if (fId == null || fDefName == null || pGenGuid == null) continue;

                var id = fId.GetValue(def) as string;
                if (string.IsNullOrEmpty(id)) continue;

                // Skip if already registered.
                if (runtimeDict != null && runtimeDict.Contains(id)) { already++; continue; }

                try {
                    // ScriptableObject.CreateInstance<RoguelikePowerup>() via reflection
                    // (ScriptableObject isn't in our compile-time stubs).
                    var tSO = Type.GetType("UnityEngine.ScriptableObject, UnityEngine.CoreModule")
                           ?? Type.GetType("UnityEngine.ScriptableObject, UnityEngine");
                    if (tSO == null) break;
                    var mCI = tSO.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(mm => mm.Name == "CreateInstance"
                                           && mm.IsGenericMethodDefinition
                                           && mm.GetParameters().Length == 0);
                    if (mCI == null) break;
                    var pw = mCI.MakeGenericMethod(typeof(RoguelikePowerup))
                        .Invoke(null, null) as RoguelikePowerup;
                    if (pw == null) break;

                    var genGuid = (Guid)pGenGuid.GetValue(def, null);
                    if (fGuid != null) fGuid.SetValue(pw, genGuid);
                    if (fPwName != null) fPwName.SetValue(pw, fDefName.GetValue(def) as string ?? id);
                    var levels = mBuildCustomLevels.Invoke(null, new object[] { def });
                    if (fPwLevels != null && levels != null) fPwLevels.SetValue(pw, levels);

                    if (runtimeDict != null) runtimeDict[id] = pw;
                    if (mRegisterInGame != null) {
                        try { mRegisterInGame.Invoke(null, new object[] { pw }); } catch { }
                    }
                    created++;
                } catch (Exception ex) {
                    StartingClassesPlugin.Log.LogWarning("  custom powerup " + id + ": " + ex.Message);
                }
            }
            StartingClassesPlugin.Log.LogInfo(string.Format(
                "Registered {0} custom EMP powerup(s) ({1} already present).", created, already));
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogWarning("Custom powerup pre-registration: " + ex.Message);
        }
    }
}
// caused by stale/corrupt roguelike meta-progression data (e.g. a saved powerup
// Level that exceeds its current PowerupLevels.Length after a game or Extra-Meta-
// Progression update). Returns 0 for that attribute calc and lets FCC finish so
// the user can actually create characters. Only swallows IndexOutOfRangeException;
// all other exceptions propagate normally.
// ============================================================================
[HarmonyPatch(typeof(Character), "GetAttributeValueByMethod")]
public static class GetAttributeValueByMethodSafeguard
{
    private static int _loggedCount;

    [HarmonyFinalizer]
    public static Exception Finalizer(Exception __exception, ref float __result)
    {
        if (__exception is IndexOutOfRangeException) {
            if (_loggedCount < 5) {
                StartingClassesPlugin.Log.LogWarning(string.Format(
                    "GetAttributeValueByMethod IndexOutOfRange swallowed ({0}); returning 0. " +
                    "Cause is usually stale roguelike meta-progression save data.",
                    ++_loggedCount));
            }
            __result = 0f;
            return null; // swallow
        }
        return __exception;
    }
}

// ============================================================================
// DIAGNOSTIC: Log when AcceptCharacter is actually invoked (i.e. Accept button clicked)
// ============================================================================
[HarmonyPatch(typeof(PresetManager), "AcceptCharacter")]
public static class AcceptCharacterDiagnostic
{
    [HarmonyPrefix]
    public static void Prefix(PresetManager __instance)
    {
        try {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            string presetName = "(no preset)";
            var curProp = typeof(PresetManager).GetProperty("CharacterPreset", flags);
            var cur = curProp != null ? curProp.GetValue(__instance) as CharacterPresetFile : null;
            if (cur != null) presetName = cur.PresetName ?? "(unnamed)";

            string entered = "(no name field)";
            var nf = typeof(PresetManager).GetField("nameFieldRoguelike", flags);
            if (nf != null) {
                var nfVal = nf.GetValue(__instance);
                if (nfVal != null) {
                    var tp = nfVal.GetType().GetProperty("text");
                    if (tp != null) entered = (tp.GetValue(nfVal) as string) ?? "(empty)";
                }
            }
            StartingClassesPlugin.Log.LogInfo(string.Format(
                "=== AcceptCharacter() called: preset='{0}' nameFieldText='{1}'", presetName, entered));

            // One-shot skill + item + fortune dump for modding reference.
            SkillDumper.DumpOnce();
            ItemDumper.DumpOnce();
            FortuneDumper.DumpOnce();
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogError("AcceptCharacterDiagnostic: " + ex);
        }
    }
}

// ============================================================================
// One-shot skill dump to BepInEx/plugins/stolenrealm_skills.txt. Triggered the
// first time the user clicks Accept on a character. Gives name, GUID, type, and
// player-usable flag for every skill in the game's dictionary so you can pick
// GUIDs to add to StartingSkillGuids in DefineClassDefs.
// ============================================================================
public static class SkillDumper
{
    private static bool _done;

    public static void DumpOnce()
    {
        if (_done) return;
        _done = true;
        try { Dump(); }
        catch (Exception ex) { StartingClassesPlugin.Log.LogWarning("SkillDumper: " + ex.Message); }
    }

    private static void Dump()
    {
        var game = StartingClassesPlugin.GetGameInstance();
        if (game == null) return;

        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var fDict = typeof(Game).GetField("_SkillInfo_Dict", flags);
        if (fDict == null) { StartingClassesPlugin.Log.LogWarning("SkillDumper: _SkillInfo_Dict not found."); return; }
        var dict = fDict.GetValue(game) as IDictionary;
        if (dict == null) { StartingClassesPlugin.Log.LogWarning("SkillDumper: _SkillInfo_Dict is null."); return; }

        var tSkill = typeof(SkillInfo);
        var fName = tSkill.GetField("SkillName");
        var fDesc = tSkill.GetField("Description");
        var fGuid = tSkill.GetField("Guid");
        var fType = tSkill.GetField("SkillType");
        var fDmgType = tSkill.GetField("DamageType");
        var fHidden = tSkill.GetField("DontIncludeInTree");
        var fDisabled = tSkill.GetField("Disabled");

        var rows = new List<string>();
        foreach (DictionaryEntry kv in dict) {
            var skill = kv.Value as SkillInfo;
            if (skill == null) continue;
            string name = fName != null ? fName.GetValue(skill) as string : "?";
            var guidObj = fGuid != null ? fGuid.GetValue(skill) : null;
            string guid = guidObj != null ? guidObj.ToString() : "?";
            string stype = fType != null ? fType.GetValue(skill).ToString() : "?";
            string dtype = fDmgType != null ? fDmgType.GetValue(skill).ToString() : "?";
            bool hidden = fHidden != null && (bool)fHidden.GetValue(skill);
            bool disabled = fDisabled != null && (bool)fDisabled.GetValue(skill);
            bool player = !hidden && !disabled;
            string desc = fDesc != null ? (fDesc.GetValue(skill) as string ?? "") : "";
            desc = desc.Replace("\n", " ").Replace("\r", " ");
            if (desc.Length > 120) desc = desc.Substring(0, 117) + "...";

            rows.Add(string.Format("{0,-30}  {1}  type={2,-10} dmg={3,-10} player={4}  |  {5}",
                name ?? "(unnamed)", guid, stype, dtype, player ? "Y" : "n", desc));
        }
        rows.Sort();

        var path = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "stolenrealm_skills.txt");
        var header = string.Format(
            "Stolen Realm skill dump ({0} skills)\n" +
            "=============================================\n" +
            "Columns: name  guid  type  dmg  player  |  description\n" +
            "'player=Y' means the skill is not hidden or disabled (likely usable in tree/starting skills).\n\n",
            rows.Count);
        System.IO.File.WriteAllText(path, header + string.Join("\n", rows.ToArray()));
        StartingClassesPlugin.Log.LogInfo(string.Format(
            "SkillDumper wrote {0} skills to {1}", rows.Count, path));

        // Second pass: find skills reachable through items but NOT in the main dict
        // (weapon proc skills, etc.) and append them to the file.
        try {
            var fItems = typeof(Game).GetField("_Items_Dict", flags);
            if (fItems == null) return;
            var items = fItems.GetValue(game) as IDictionary;
            if (items == null) return;

            // Collect all SkillInfo objects referenced from items.
            var byGuid = new Dictionary<Guid, SkillInfo>();
            foreach (DictionaryEntry kv in dict) {
                var s = kv.Value as SkillInfo;
                if (s != null) byGuid[fGuid != null ? (Guid)fGuid.GetValue(s) : Guid.Empty] = s;
            }

            var extras = new List<SkillInfo>();
            var seen = new HashSet<Guid>(byGuid.Keys);
            var fGranted       = typeof(ItemInfo).GetField("GrantedSkills");
            var fSkillTriggers = typeof(ItemInfo).GetField("SkillTriggers");
            var fActions       = typeof(SkillTrigger).GetField("Actions");
            foreach (DictionaryEntry kv in items) {
                var item = kv.Value as ItemInfo;
                if (item == null) continue;
                var gs = fGranted != null ? fGranted.GetValue(item) as IEnumerable : null;
                if (gs != null) foreach (var x in gs) {
                    var si = x as SkillInfo;
                    if (si == null) continue;
                    var g = fGuid != null ? (Guid)fGuid.GetValue(si) : Guid.Empty;
                    if (g != Guid.Empty && seen.Add(g)) extras.Add(si);
                }
                var trigs = fSkillTriggers != null ? fSkillTriggers.GetValue(item) as IEnumerable : null;
                if (trigs != null) foreach (var trig in trigs) {
                    if (trig == null) continue;
                    var acts = fActions != null ? fActions.GetValue(trig) as IEnumerable : null;
                    if (acts == null) continue;
                    foreach (var x in acts) {
                        var si = x as SkillInfo;
                        if (si == null) continue;
                        var g = fGuid != null ? (Guid)fGuid.GetValue(si) : Guid.Empty;
                        if (g != Guid.Empty && seen.Add(g)) extras.Add(si);
                    }
                }
            }

            if (extras.Count > 0) {
                var extraRows = new List<string>();
                foreach (var skill in extras) {
                    string name  = fName    != null ? fName.GetValue(skill)    as string : "?";
                    string guid  = fGuid    != null ? fGuid.GetValue(skill).ToString()   : "?";
                    string stype = fType    != null ? fType.GetValue(skill).ToString()   : "?";
                    string dtype = fDmgType != null ? fDmgType.GetValue(skill).ToString(): "?";
                    string desc  = fDesc    != null ? (fDesc.GetValue(skill) as string ?? "") : "";
                    desc = desc.Replace("\n", " ").Replace("\r", " ");
                    if (desc.Length > 200) desc = desc.Substring(0, 197) + "...";
                    extraRows.Add(string.Format("{0,-30}  {1}  type={2,-10} dmg={3,-10}  |  {4}",
                        name ?? "(unnamed)", guid, stype, dtype, desc));
                }
                extraRows.Sort();
                System.IO.File.AppendAllText(path,
                    "\n\n=== OFF-DICT SKILLS (reachable only via items; not shown in skill tree) ===\n" +
                    string.Join("\n", extraRows.ToArray()));
                StartingClassesPlugin.Log.LogInfo(string.Format(
                    "SkillDumper appended {0} off-dict skills", extraRows.Count));
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogWarning("SkillDumper extras: " + ex.Message);
        }
    }
}

// ============================================================================
// Dumps fortunes to stolenrealm_fortunes.txt. Runtime-reflective because the
// Fortune type doesn't appear as a straightforward public TypeDef, but the
// collection sits on Game._Fortunes and we just iterate + reflect.
// ============================================================================
public static class FortuneDumper
{
    private static bool _done;

    public static void DumpOnce()
    {
        if (_done) return;
        _done = true;
        try { Dump(); }
        catch (Exception ex) { StartingClassesPlugin.Log.LogWarning("FortuneDumper: " + ex.Message); }
    }

    private static void Dump()
    {
        var game = StartingClassesPlugin.GetGameInstance();
        if (game == null) return;

        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var fFortunes = typeof(Game).GetField("_Fortunes", flags);
        if (fFortunes == null) { StartingClassesPlugin.Log.LogWarning("FortuneDumper: _Fortunes not found."); return; }
        var list = fFortunes.GetValue(game) as IEnumerable;
        if (list == null) { StartingClassesPlugin.Log.LogWarning("FortuneDumper: _Fortunes is null."); return; }

        var rows = new List<string>();
        foreach (var f in list) {
            if (f == null) continue;
            var t = f.GetType();
            // Try common fields/props
            object guid = null, name = null, desc = null, rarity = null;
            var fGuid   = t.GetField("Guid")   ?? t.GetField("Id");
            var fName   = t.GetField("Name")   ?? t.GetField("DisplayName");
            var fDesc   = t.GetField("Description");
            var fRarity = t.GetField("Rarity");
            if (fGuid   != null) guid   = fGuid.GetValue(f);
            if (fName   != null) name   = fName.GetValue(f);
            if (fDesc   != null) desc   = fDesc.GetValue(f);
            if (fRarity != null) rarity = fRarity.GetValue(f);
            // Fall back to asset name if Name field missing
            if (name == null) {
                try { var asObj = (UnityEngine.Object)(object)f; if (asObj != null) name = asObj.name; } catch { }
            }
            string descStr = desc != null ? desc.ToString() : "";
            descStr = descStr.Replace("\n", " ").Replace("\r", " ");
            if (descStr.Length > 120) descStr = descStr.Substring(0, 117) + "...";

            rows.Add(string.Format("{0,-30}  {1}  {2,-10}  |  {3}",
                name ?? "(unnamed)", guid ?? "?", rarity ?? "?", descStr));
        }
        rows.Sort();

        var path = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "stolenrealm_fortunes.txt");
        System.IO.File.WriteAllText(path,
            string.Format("Stolen Realm fortune dump ({0} fortunes)\n=============================================\n\n", rows.Count)
            + string.Join("\n", rows.ToArray()));
        StartingClassesPlugin.Log.LogInfo(string.Format(
            "FortuneDumper wrote {0} fortunes to {1}", rows.Count, path));
    }
}
// Useful for identifying weapon procs. Each row:
//   <item asset-name>  <guid>  |  GrantedSkills: [names]  |  TriggerActions: [names]
// ============================================================================
public static class ItemDumper
{
    private static bool _done;

    public static void DumpOnce()
    {
        if (_done) return;
        _done = true;
        try { Dump(); }
        catch (Exception ex) { StartingClassesPlugin.Log.LogWarning("ItemDumper: " + ex.Message); }
    }

    private static void Dump()
    {
        var game = StartingClassesPlugin.GetGameInstance();
        if (game == null) return;

        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var fSkills = typeof(Game).GetField("_SkillInfo_Dict", flags);
        var skillDict = fSkills != null ? fSkills.GetValue(game) as IDictionary : null;

        var tItemInfo = typeof(ItemInfo);
        var fItemGuidEarly = tItemInfo.GetField("Guid");

        // Aggregate items from every known collection and dedupe by GUID. `_Items_Dict` is
        // the primary runtime dict but it misses quest / reward items that only live in the
        // `Items` array, `_UnassignedItems`, or `_WorldLootItems` (e.g. Valler's Pocket Watch).
        var allItems = new Dictionary<Guid, ItemInfo>();
        Action<object, string> absorb = (src, label) => {
            if (src == null) { StartingClassesPlugin.Log.LogInfo("  absorb " + label + ": source=null"); return; }
            int before = allItems.Count, seen = 0;
            if (src is IDictionary d) {
                foreach (DictionaryEntry kv in d) {
                    seen++;
                    var it = kv.Value as ItemInfo;
                    if (it == null) continue;
                    try {
                        var g = fItemGuidEarly != null ? (Guid)fItemGuidEarly.GetValue(it) : Guid.Empty;
                        if (g != Guid.Empty && !allItems.ContainsKey(g)) allItems[g] = it;
                    } catch { }
                }
            } else if (src is IEnumerable e) {
                foreach (var x in e) {
                    seen++;
                    var it = x as ItemInfo;
                    if (it == null) continue;
                    try {
                        var g = fItemGuidEarly != null ? (Guid)fItemGuidEarly.GetValue(it) : Guid.Empty;
                        if (g != Guid.Empty && !allItems.ContainsKey(g)) allItems[g] = it;
                    } catch { }
                }
            } else {
                StartingClassesPlugin.Log.LogInfo("  absorb " + label + ": src is " + src.GetType().FullName);
                return;
            }
            int added = allItems.Count - before;
            StartingClassesPlugin.Log.LogInfo(string.Format("  absorb {0,-24}: seen={1} added={2}", label, seen, added));
        };

        string[] fieldNames = { "_Items_Dict", "Items", "_UnassignedItems", "_WorldLootItems" };
        foreach (var fname in fieldNames) {
            var f = typeof(Game).GetField(fname, flags);
            if (f != null) { try { absorb(f.GetValue(game), fname); } catch { } }
        }
        // Also try the Items property (backing field is <Items>k__BackingField)
        try {
            var p = typeof(Game).GetProperty("Items", flags);
            if (p != null) absorb(p.GetValue(game), "Items(prop)");
        } catch { }

        // Last resort: ask Unity for every loaded ItemInfo asset. This picks up prefabs / asset
        // bundle content for items that don't appear in any runtime dict (e.g. quest reward items
        // like Valler's Pocket Watch that only exist as Resource assets).
        try {
            var tResources = Type.GetType("UnityEngine.Resources, UnityEngine.CoreModule")
                          ?? Type.GetType("UnityEngine.Resources, UnityEngine");
            if (tResources != null) {
                var mFind = tResources.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == "FindObjectsOfTypeAll"
                                      && !m.IsGenericMethod
                                      && m.GetParameters().Length == 1);
                if (mFind != null) {
                    var arr = mFind.Invoke(null, new object[] { typeof(ItemInfo) }) as IEnumerable;
                    absorb(arr, "FindObjectsOfTypeAll");
                }
            }
        } catch (Exception ex) {
            StartingClassesPlugin.Log.LogWarning("ItemDumper FindObjectsOfTypeAll failed: " + ex.Message);
        }

        if (allItems.Count == 0) { StartingClassesPlugin.Log.LogWarning("ItemDumper: no items found in any collection."); return; }

        // Build skill guid -> name lookup.
        var skillNames = new Dictionary<Guid, string>();
        if (skillDict != null) {
            var fSkillName = typeof(SkillInfo).GetField("SkillName");
            var fSkillGuid = typeof(SkillInfo).GetField("Guid");
            foreach (DictionaryEntry kv in skillDict) {
                var s = kv.Value as SkillInfo;
                if (s == null) continue;
                var g = fSkillGuid != null ? (Guid)fSkillGuid.GetValue(s) : Guid.Empty;
                var n = fSkillName != null ? fSkillName.GetValue(s) as string : null;
                if (g != Guid.Empty) skillNames[g] = n ?? "(unnamed)";
            }
        }

        var fItemGuid       = fItemGuidEarly;
        var fGranted        = tItemInfo.GetField("GrantedSkills");
        var fSkillTriggers  = tItemInfo.GetField("SkillTriggers");
        var fItemType       = tItemInfo.GetField("ItemType");
        var fRarity         = tItemInfo.GetField("Rarity");

        // SkillTrigger.Actions and .GeneralEffects reference skills.
        var tTrigger = typeof(SkillTrigger);
        var fActions = tTrigger.GetField("Actions");
        var fGeneralEffects = tTrigger.GetField("GeneralEffects");

        var rows = new List<string>();
        foreach (var kv in allItems) {
            var item = kv.Value;
            if (item == null) continue;

            var grantedGuids = new List<Guid>();
            try {
                var gs = fGranted != null ? fGranted.GetValue(item) as IEnumerable : null;
                if (gs != null) foreach (var s in gs) {
                    var si = s as SkillInfo;
                    if (si != null) {
                        var fg = typeof(SkillInfo).GetField("Guid");
                        var g = fg != null ? (Guid)fg.GetValue(si) : Guid.Empty;
                        if (g != Guid.Empty) grantedGuids.Add(g);
                    }
                }
            } catch { }

            var triggerGuids = new List<Guid>();
            try {
                var triggers = fSkillTriggers != null ? fSkillTriggers.GetValue(item) as IEnumerable : null;
                if (triggers != null) foreach (var trig in triggers) {
                    if (trig == null) continue;
                    // Actions contains SkillInfo references
                    var actions = fActions != null ? fActions.GetValue(trig) as IEnumerable : null;
                    if (actions != null) foreach (var a in actions) {
                        var si = a as SkillInfo;
                        if (si != null) {
                            var fg = typeof(SkillInfo).GetField("Guid");
                            var g = fg != null ? (Guid)fg.GetValue(si) : Guid.Empty;
                            if (g != Guid.Empty) triggerGuids.Add(g);
                        }
                    }
                }
            } catch { }

            // Read the item's in-game description (the flavor text that shows in tooltips).
            string optDesc = "";
            try {
                var fOptDesc = tItemInfo.GetField("OptionalDescription");
                if (fOptDesc != null) optDesc = (fOptDesc.GetValue(item) as string) ?? "";
                optDesc = optDesc.Replace("\r", " ").Replace("\n", " ").Trim();
            } catch { }

            // Read AttributeEffects - these are the "Cooldowns -1", "Mana Costs -20%" style
            // entries that the UI renders for items which have no OptionalDescription but
            // still show effect text in the tooltip (e.g. Valler's Pocket Watch).
            var effectLines = new List<string>();
            try {
                var fAttrEffects = tItemInfo.GetField("AttributeEffects");
                var effs = fAttrEffects != null ? fAttrEffects.GetValue(item) as IEnumerable : null;
                if (effs != null) {
                    foreach (var ef in effs) {
                        if (ef == null) continue;
                        var et = ef.GetType();
                        string attrName = "?", method = "?", amount = "?";
                        try {
                            var attr = et.GetField("CharacterAttribute").GetValue(ef);
                            if (attr != null) {
                                var asObj = (UnityEngine.Object)(object)attr;
                                attrName = asObj != null ? asObj.name : attr.ToString();
                            }
                        } catch { }
                        try { method = et.GetField("CharacterEffectMethod").GetValue(ef).ToString(); } catch { }
                        try { amount = (et.GetField("Amount").GetValue(ef) as string) ?? "?"; } catch { }
                        effectLines.Add(string.Format("{0} {1} {2}", attrName, method, amount));
                    }
                }
            } catch { }

            // Emit every item, even if it has no skills or description. Previous filter hid
            // ~480 items (weak/common ones) but also sometimes hid items the user cared about.
            // We'd rather over-dump and let the user filter.

            // Item display name: use the ScriptableObject's asset name.
            string name = "(unnamed)";
            try { var asObj = (UnityEngine.Object)(object)item; if (asObj != null) name = asObj.name ?? "(unnamed)"; } catch { }

            string guid = "?";
            try { var g = fItemGuid != null ? (Guid)fItemGuid.GetValue(item) : Guid.Empty; guid = g.ToString(); } catch { }

            string rarity = "?", itype = "?";
            try { if (fRarity   != null) rarity = fRarity.GetValue(item).ToString(); } catch { }
            try { if (fItemType != null) itype  = fItemType.GetValue(item).ToString(); } catch { }

            var grantedNames  = grantedGuids.Select(g => {
                string n; return skillNames.TryGetValue(g, out n) ? n + "[" + g + "]" : g.ToString();
            }).ToArray();
            var triggerNames  = triggerGuids.Select(g => {
                string n; return skillNames.TryGetValue(g, out n) ? n + "[" + g + "]" : g.ToString();
            }).ToArray();

            rows.Add(string.Format(
                "{0,-36}  {1}  {2,-10} {3,-10}\n    Desc:     {4}\n    Effects:  {5}\n    Granted:  {6}\n    Triggers: {7}",
                name, guid, itype, rarity,
                string.IsNullOrEmpty(optDesc) ? "(none)" : optDesc,
                effectLines.Count == 0 ? "(none)" : string.Join(" · ", effectLines.ToArray()),
                grantedNames.Length == 0 ? "(none)" : string.Join(", ", grantedNames),
                triggerNames.Length == 0 ? "(none)" : string.Join(", ", triggerNames)));
        }
        rows.Sort();

        var path = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "stolenrealm_items.txt");
        var header = string.Format(
            "Stolen Realm item dump ({0} items with skills, triggers, or descriptions)\n" +
            "=============================================\n\n",
            rows.Count);
        System.IO.File.WriteAllText(path, header + string.Join("\n", rows.ToArray()));
        StartingClassesPlugin.Log.LogInfo(string.Format(
            "ItemDumper wrote {0} items to {1}", rows.Count, path));

        // Also emit a CSV in exactly the format the class planner expects, sourced from
        // the game's own item data (direct ItemInfo stat fields + AttributeEffects values).
        // This lets the planner show EVERY item (905) instead of just a hand-curated subset.
        try { WriteCsv(allItems); }
        catch (Exception ex) { StartingClassesPlugin.Log.LogWarning("ItemDumper CSV: " + ex.Message); }
    }

    // CSV column headers — mirror the class_planner's expected format exactly.
    // The planner parses on ';' with a blank field meaning "no value for this stat".
    private static readonly string[] CSV_COLUMNS = {
        "Name","Rarity","Slot","GUID",
        "Might","Dexterity","Vitality","Intelligence","Reflex",
        "ManaPower","CounterAttack","GenericDamage",
        "FlatHealth","HealthModifier","FlatMana","ManaModifier",
        "SummonDamage","SummonHealth","SummonResistance","SummonLimit",
        "Armour","MagicArmour","DamageType","WpnDamage",
        "FlatPhysical","PhysicalModifier",
        "FlatFire","FireModifier",
        "FlatLightning","LightningModifier",
        "FlatCold","ColdModifier",
        "FlatShadow","ShadowModifier",
        "FlatHoly","HolyModifier",
        "ReturnFire","ReturnLightning","ReturnCold","ReturnShadow",
        "LifeSteal","ManaSteal",
        "FireResistance","LightningResistance","ColdResistance","PhysicalResistance",
        "DamageReduction","Range",
    };

    // Map an AttributeEffect's CharacterAttribute name to its CSV column key.
    // When the same concept has multiple in-game names (DamageModFire vs FireModifier,
    // ResistFire vs FireResistance, etc.), we route them to the planner's column name.
    private static readonly Dictionary<string, string> ATTR_TO_CSV = new Dictionary<string, string> {
        { "DamageModFire",       "FireModifier" },
        { "DamageModLightning",  "LightningModifier" },
        { "DamageModCold",       "ColdModifier" },
        { "DamageModShadow",     "ShadowModifier" },
        { "DamageModHoly",       "HolyModifier" },
        { "DamageModPhysical",   "PhysicalModifier" },
        { "DamageMod",           "GenericDamage" },
        { "DamageFlatFire",      "FlatFire" },
        { "DamageFlatLightning", "FlatLightning" },
        { "DamageFlatCold",      "FlatCold" },
        { "DamageFlatShadow",    "FlatShadow" },
        { "DamageFlatHoly",      "FlatHoly" },
        { "DamageFlatPhysical",  "FlatPhysical" },
        { "ResistFire",          "FireResistance" },
        { "ResistLightning",     "LightningResistance" },
        { "ResistCold",          "ColdResistance" },
        { "ResistPhysical",      "PhysicalResistance" },
        { "ResistHoly",          "PhysicalResistance" }, // no separate HolyResistance column
        { "ManaPowerMod",        "ManaPower" },
        { "CounterAttack",       "CounterAttack" },
        { "MaxHealth",           "FlatHealth" },
        { "HealthMod",           "HealthModifier" },
        { "MaxMana",             "FlatMana" },
        { "ManaMod",             "ManaModifier" },
        { "SummonDamage",        "SummonDamage" },
        { "SummonHealth",        "SummonHealth" },
        { "SummonResistance",    "SummonResistance" },
        { "SummonLimit",         "SummonLimit" },
        { "LifeSteal",           "LifeSteal" },
        { "ManaSteal",           "ManaSteal" },
        { "DamageReturnedFlatFire",      "ReturnFire" },
        { "DamageReturnedFlatLightning", "ReturnLightning" },
        { "DamageReturnedFlatCold",      "ReturnCold" },
        { "DamageReturnedFlatShadow",    "ReturnShadow" },
        { "DamageReduction",     "DamageReduction" },
        { "Range",               "Range" },
    };

    private static void WriteCsv(Dictionary<Guid, ItemInfo> items)
    {
        var t = typeof(ItemInfo);
        var fGuid           = t.GetField("Guid");
        var fItemType       = t.GetField("ItemType");
        var fRarity         = t.GetField("Rarity");
        var fMight          = t.GetField("Might");
        var fDex            = t.GetField("Dexterity");
        var fInt            = t.GetField("Intelligence");
        var fVit            = t.GetField("Vitality");
        var fReflex         = t.GetField("Reflex");
        var fArmorRatio     = t.GetField("ArmorRatio");
        var fMagicArmorRatio= t.GetField("MagicArmorRatio");
        var fArmorRatioShield     = t.GetField("ArmorRatioShield");
        var fMagicArmorRatioShield= t.GetField("MagicArmorRatioShield");
        var fAttrEffects    = t.GetField("AttributeEffects");

        var tEff = typeof(CharacterEffectInfo);
        var fEffAttr   = tEff.GetField("CharacterAttribute");
        var fEffMethod = tEff.GetField("CharacterEffectMethod");
        var fEffAmt    = tEff.GetField("Amount");

        var rowLines = new List<string>();
        rowLines.Add(string.Join(";", CSV_COLUMNS));

        foreach (var kv in items) {
            var item = kv.Value;
            if (item == null) continue;

            // Accumulate values into a dict keyed by CSV column name.
            var cols = new Dictionary<string, string>();
            foreach (var c in CSV_COLUMNS) cols[c] = "";

            // Name + identifiers
            string iname = "(unnamed)";
            try { var o = (UnityEngine.Object)(object)item; if (o != null) iname = o.name ?? "(unnamed)"; } catch { }
            cols["Name"] = iname;
            try { cols["Rarity"] = fRarity.GetValue(item).ToString(); } catch { }
            try { cols["Slot"]   = fItemType.GetValue(item).ToString(); } catch { }
            try { cols["GUID"]   = fGuid.GetValue(item).ToString(); } catch { }

            // Direct stat fields on ItemInfo — all are `float` in the game code.
            float mi = 0f, dx = 0f, ii = 0f, vi = 0f, rf = 0f;
            try { mi = (float)fMight.GetValue(item);  } catch { }
            try { dx = (float)fDex.GetValue(item);    } catch { }
            try { ii = (float)fInt.GetValue(item);    } catch { }
            try { vi = (float)fVit.GetValue(item);    } catch { }
            try { rf = (float)fReflex.GetValue(item); } catch { }
            if (mi != 0f) cols["Might"]        = ((int)(mi + (mi >= 0 ? 0.5f : -0.5f))).ToString();
            if (dx != 0f) cols["Dexterity"]    = ((int)(dx + (dx >= 0 ? 0.5f : -0.5f))).ToString();
            if (ii != 0f) cols["Intelligence"] = ((int)(ii + (ii >= 0 ? 0.5f : -0.5f))).ToString();
            if (vi != 0f) cols["Vitality"]     = ((int)(vi + (vi >= 0 ? 0.5f : -0.5f))).ToString();
            if (rf != 0f) cols["Reflex"]       = ((int)(rf + (rf >= 0 ? 0.5f : -0.5f))).ToString();

            // Armor columns: game uses ratios that get multiplied by a base value per slot.
            // The existing CSV stored the computed max value. We don't know the multiplier
            // at dump time, but the ratios themselves are informative. Emit raw ratio *100
            // as an integer approximation (0..100 range roughly).
            float ar = 0f, mar = 0f, ars = 0f, mars = 0f;
            try { ar  = (float)fArmorRatio.GetValue(item);          } catch { }
            try { mar = (float)fMagicArmorRatio.GetValue(item);     } catch { }
            try { ars = (float)fArmorRatioShield.GetValue(item);    } catch { }
            try { mars= (float)fMagicArmorRatioShield.GetValue(item); } catch { }
            // If the item is a shield, prefer shield ratio. Otherwise regular.
            string slotStr = cols["Slot"] ?? "";
            float useAr  = slotStr == "Shield" ? ars : ar;
            float useMar = slotStr == "Shield" ? mars: mar;
            if (useAr  > 0f) cols["Armour"]      = ((int)(useAr  * 100f + 0.5f)).ToString();
            if (useMar > 0f) cols["MagicArmour"] = ((int)(useMar * 100f + 0.5f)).ToString();

            // AttributeEffects -> routed CSV columns. Use MAX of range.
            try {
                var effs = fAttrEffects.GetValue(item) as IEnumerable;
                if (effs != null) foreach (var ef in effs) {
                    if (ef == null) continue;
                    string attrName = null;
                    try {
                        var attr = fEffAttr.GetValue(ef);
                        if (attr != null) {
                            var o = (UnityEngine.Object)(object)attr;
                            attrName = o != null ? o.name : attr.ToString();
                        }
                    } catch { }
                    if (string.IsNullOrEmpty(attrName)) continue;

                    string csvCol;
                    if (!ATTR_TO_CSV.TryGetValue(attrName, out csvCol)) continue; // skip attrs that don't map to a CSV column

                    string amt = "";
                    try { amt = (fEffAmt.GetValue(ef) as string) ?? ""; } catch { }
                    // Parse amount: plain number, or "{a,b}" range. Take max.
                    float val = 0f;
                    var m = System.Text.RegularExpressions.Regex.Match(amt, @"^\{\s*([-\d.]+)\s*,\s*([-\d.]+)\s*\}$");
                    if (m.Success) {
                        float a, b;
                        if (float.TryParse(m.Groups[1].Value, out a) && float.TryParse(m.Groups[2].Value, out b))
                            val = Math.Max(Math.Abs(a), Math.Abs(b)) * (b < 0 || a < 0 ? -1 : 1);
                    } else if (!float.TryParse(amt, out val)) {
                        continue; // formula-style value (Item.GetFlatDamageValue etc.) — skip for CSV
                    }
                    if (val == 0f) continue;
                    cols[csvCol] = ((int)(val + (val >= 0 ? 0.5f : -0.5f))).ToString();
                }
            } catch { }

            // Build row
            var vals = new List<string>(CSV_COLUMNS.Length);
            foreach (var c in CSV_COLUMNS) vals.Add(cols[c] ?? "");
            rowLines.Add(string.Join(";", vals.ToArray()));
        }

        var csvPath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "stolenrealm_items.csv");
        System.IO.File.WriteAllText(csvPath, string.Join("\n", rowLines.ToArray()));
        StartingClassesPlugin.Log.LogInfo(string.Format(
            "ItemDumper wrote CSV with {0} rows to {1}", rowLines.Count - 1, csvPath));
    }
}

