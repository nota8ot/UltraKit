﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data.ScriptableObjects.Registry;
using ULTRAKIT.Loader;
using UnityEngine;

namespace ULTRAKIT.Core.BossSpawns
{
    public static class BossSpawnsInjector
    {
        static string[] List ={
            "MinosPrime",
            "V2",
            "Gabriel",
            "Flesh Prison",
            "DroneFlesh",
            "DroneSkull Variant",
            "MinosBoss",
            "Wicked"
        };

        public static void Initialize()
        {
            Loader.Addon b = new Loader.Addon();

            b.Data = new Data.UKAddonData();
            b.Data.ModName = "Bosses";
            b.Data.Author = "UltraKit";
            b.Data.ModDesc = "Contains Enemies that cannot be spawned by the spawner arm";

            b.Bundle = CoreContent.UIBundle;///SINCE EVERYTHING WILL BE REGISTERED OUTSIDE OF THE ADDON I'LL JUST USE THE UI BUNDLE

            b.Path = "Internal";

            b.enabled = true;

            Loader.AddonLoader.registry.Add(b, new List<UKContent>());
            Loader.AddonLoader.registry[b].AddRange(Enemies());
        }

        public static List<UKContentSpawnable> Enemies()
        {
            List<UKContentSpawnable> a = new List<UKContentSpawnable>();
            

            foreach (string item in List) a.Add(EnemySpawnable(item));

            return a;
        }
        public static UKContentSpawnable EnemySpawnable(string Enemy)
        {

            UKContentSpawnable a = ScriptableObject.CreateInstance<UKContentSpawnable>();

            a.Name = Enemy;
            a.type = Data.ScriptableObjects.Registry.Type.Enemy;

            a.Prefab = PrefabFind(Enemy);
            a.Icon = CoreContent.UIBundle.LoadAsset<Sprite>($"{Enemy}");

            return a;
        }
        public static GameObject PrefabFind(string name)
        {
            //Find set Object in the prefabs
            GameObject[] Pool = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject a = null;
            foreach (GameObject obj in Pool)
            {
                if (obj.gameObject.name == name)
                {
                    if (obj.gameObject.tag == "Enemy" || name == "Wicked")
                    {
                        if (obj.activeSelf != true) obj.SetActive(true);
                        a = obj;

                        // Fix lighting
                        var smrs = a.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                        foreach (var item in smrs)
                        {
                            item.gameObject.layer = LayerMask.NameToLayer("Outdoors");
                        }
                    }
                }
            }

            var bhb = a.GetComponentInChildren<BossHealthBar>();
            if (bhb == null)
            {
                bhb = a.GetComponentInChildren<EnemyIdentifier>(true).gameObject.AddComponent<BossHealthBar>();
            }

            bhb.bossName = "";

            var cust = bhb?.gameObject.AddComponent<CustomHealthbarPos>();
            if (cust)
            {
                cust.offset = Vector3.up * 6;
                cust.enabled = false;
            }
            return a;
        }
    }

    [HarmonyPatch(typeof(BossHealthBar))]
    public static class BossHealthBarPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(BossHealthBar __instance, EnemyIdentifier ___eid, GameObject ___bossBar)
        {
            var cust = __instance.gameObject.GetComponent<CustomHealthbarPos>();
            if (cust)
            {
                cust.barObj = ___bossBar;
                cust.enemy = ___eid.gameObject;
                cust.enabled = true;
            }
        }
    }

    [HarmonyPatch(typeof(MinosBoss))]
    public static class MinosPatch
    {
        static float minosHeight = 600, minosOffset = 200;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix(MinosBoss __instance)
        {
            var cust = __instance.GetComponentInChildren<CustomHealthbarPos>(true);
            if (cust)
            {
                cust.offset = Vector3.up * minosHeight * 1.5f;
                cust.size = 0.25f;
                var plr = MonoSingleton<NewMovement>.Instance.transform;
                __instance.transform.position = plr.position + Vector3.down * minosHeight;
                __instance.transform.position += plr.forward * minosOffset;
                __instance.transform.forward = Vector3.ProjectOnPlane((plr.position - __instance.transform.position).normalized, Vector3.up);
            }
        }
    }

    [HarmonyPatch(typeof(Wicked))]
    public static class WickedPatch
    {
        [HarmonyPatch("GetHit")]
        [HarmonyPrefix]
        public static bool GetHitPrefix(Wicked __instance)
        {
            if (__instance.patrolPoints[0] == null)
            {
                var oldAud = __instance.hitSound.GetComponent<AudioSource>();

                var newAudObj = new GameObject();
                newAudObj.transform.position = __instance.transform.position;

                var newAud = newAudObj.AddComponent<AudioSource>();
                newAud.playOnAwake = false;
                newAud.clip = oldAud.clip;
                newAud.volume = oldAud.volume;
                newAud.pitch = oldAud.pitch;
                newAud.spatialBlend = oldAud.spatialBlend;
                newAud.Play();

                GameObject.Destroy(newAudObj, newAud.clip.length);

                GameObject.Destroy(__instance.gameObject);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(V2))]
    public static class V2Patch
    {
        [HarmonyPatch("BeginEscape")]
        [HarmonyPrefix]
        public static void BeginEscapePrefix(V2 __instance)
        {
            if(__instance.escapeTarget == null)
            {
                __instance.escapeTarget = new GameObject("Escape").transform;
                __instance.escapeTarget.parent = __instance.transform;
                __instance.escapeTarget.localPosition = Vector3.zero;
                GameObject.Destroy(__instance.gameObject, 2);
            }
        }
    }
}