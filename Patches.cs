using HarmonyLib;
using Klei;
using KMod;
using KSerialization;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Windows;
using YamlDotNet.Core.Tokens;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization;
using static DistributionPlatform;
using static ModInfo;
using static ResearchTypes;
using static Sim;
using static SimMessages;
using static STRINGS.RESEARCH.TECHS;

namespace ONI_Mod1
{
    public class Patches
    {
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                Debug.Log("I execute before Db.Initialize!");

                Strings.Add("STRINGS.BUILDINGS.PREFABS.TEST.NAME", STRINGS.UI.FormatAsLink("自定义火箭模块", "Test"));
                Strings.Add("STRINGS.BUILDINGS.PREFABS.TEST.DESC", "一个功能性火箭舱段。");
                Strings.Add("STRINGS.BUILDINGS.PREFABS.TEST.EFFECT", "提供你定义的功能。");

            }

            public static void Postfix()
            {
                Debug.Log("I execute after Db.Initialize!");

                ModUtil.AddBuildingToPlanScreen("Rocketry", "Test");
                Db.Get().Techs.Get("NuclearRefinement")?.unlockedItemIDs.Add("Test");
            }
        }
    }
}

public class TEST : IBuildingConfig
{
    public const string ID = "ScannerModule";

    public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

    public override BuildingDef CreateBuildingDef()
    {
        float[] construction_mass = new float[2] { 350f, 1000f };
        string[] construction_materials = new string[2]
        {
      SimHashes.Steel.ToString(),
      SimHashes.Polypropylene.ToString()
        };
        EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
        EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
        EffectorValues noise = tieR2;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Test", 5, 5, "rocket_scanner_module_kanim", 1000, 120f, construction_mass, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
        BuildingTemplates.CreateRocketBuildingDef(buildingDef);
        buildingDef.SceneLayer = Grid.SceneLayer.Building;
        buildingDef.OverheatTemperature = 2273.15f;
        buildingDef.Floodable = false;
        buildingDef.AttachmentSlotTag = GameTags.Rocket;
        buildingDef.ObjectLayer = ObjectLayer.Building;
        buildingDef.RequiresPowerInput = false;
        buildingDef.attachablePosition = new CellOffset(0, 0);
        buildingDef.CanMove = true;
        buildingDef.Cancellable = false;
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
        go.AddOrGet<LoopingSounds>();
        go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
        go.AddOrGetDef<ScannerModule.Def>().scanRadius = 0;
        go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
        {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
        };
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MINOR_PLUS);
    }
}
