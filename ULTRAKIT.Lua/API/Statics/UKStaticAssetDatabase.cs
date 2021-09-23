﻿using System.Collections.Generic;
using System;
using UnityEngine;
using ULTRAKIT.Lua.API.Abstract;
using MoonSharp.Interpreter;

namespace ULTRAKIT.Data
{
    public class UKStaticAssetDatabase : UKStatic
    {
        public override string name => "AssetDatabase";

        private Dictionary<string, GameObject> assetDict = new Dictionary<string, GameObject>();
        public static readonly string[] assetNames = {
            "RevolverBeam",
            "RevolverBeamSuper",
            "MaliciousBeam",
            "ShotgunProjectile",
            "Grenade",
            "NailHeated",
            "Harpoon",
            "NailFodder",
            "Nail",
            "RevolverBeamAlt",
            "RevolverBeamSuperAlt",
            "Coin",
            "RailcannonBeamMalicious",
            "RailcannonBeam",
            "HarpoonMalicious",
            "Projectile",
            "ProjectileHoming",
            "ProjectileExplosiveHH",
            "ProjectileExplosive",
            "ShotgunProjectileEnemy",
            "ProjectileMinosPrime",
            "ProjectileSpread",
            "Explosion",
            "ExplosionPrime",
            "ExplosionBig",
            "MindflayerExplosion",
            "ExplosionMaliciousRail",
            "ExplosionSuper",
            "ExplosionSand",
            "VirtueShatterExplosionless",
            "VirtueShatterExplosionlessHuge",
            "BlackHoleExplosion",
            "ExplosionWaveEnemy",
            "ExplosionWaveSisyphus",
            "ExplosionWave",
        };


        [MoonSharpHidden]
        public UKStaticAssetDatabase()
        {
            foreach (string Name in assetNames)
            {
                assetDict.Add(Name, AssetFind(Name));
            }
        }

        public GameObject Create(string name)
        {
            try
            {
                return GameObject.Instantiate(assetDict[name]);
            }
            catch (KeyNotFoundException)
            {
                Debug.LogWarning($"Could Not Find Asset {name}, please check the asset list");
                return new GameObject();
            }
        }

        //TODO: create asset position

        private GameObject AssetFind(string name)
        {
            //Find set Object in the prefabs
            GameObject[]
                Pool = Resources.FindObjectsOfTypeAll<GameObject>();
            GameObject
                Original = new GameObject();

            foreach (GameObject obj in Pool)
            {
                if (obj.gameObject.name == name)
                {
                    Original = obj;
                }
            }
            return Original;
        }

    }
}