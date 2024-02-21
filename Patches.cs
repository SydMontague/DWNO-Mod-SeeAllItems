using HarmonyLib;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SeeAllItems
{
    class Patches
    {
        static bool activation_lock = false;

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        [HarmonyPrefix]
        static bool SetActive_Prefix(GameObject __instance)
        {
            return !activation_lock;
        }

        [HarmonyPatch(typeof(ItemPickPointManager), "UpdateNotUnity")]
        [HarmonyPrefix]
        static void UpdateNotUnity_Prefix(ItemPickPointManager __instance, float deltaTime, float time, float lastTime)
        {
            activation_lock = true;
        }

        [HarmonyPatch(typeof(ItemPickPointManager), "UpdateNotUnity")]
        [HarmonyPostfix]
        static void UpdateNotUnity_Postfix(ItemPickPointManager __instance, float deltaTime, float time, float lastTime)
        {
            activation_lock = false;
            for (int k = 0; k < __instance.m_itemPickPoints.Count; k++)
            {
                ItemPickPointBase itemPickPointBase = __instance.m_itemPickPoints[k];
                bool is_material = StorageData.m_itemPickPointData.GetMaterialPickPointData(__instance.m_itemPickPoints[k].id) != null;
                float distance = is_material ? Plugin.Instance.materialRenderDistance.Value : Plugin.Instance.itemRenderDistance.Value;

                if (Vector3.Distance(__instance.m_playerCtrl.transform.position, itemPickPointBase.transform.position) > distance)
                {
                    if (itemPickPointBase.gameObject.activeInHierarchy)
                    {
                        itemPickPointBase.gameObject.SetActive(false);
                    }
                }
                else if (!itemPickPointBase.gameObject.activeInHierarchy)
                {
                    itemPickPointBase.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(PartnerCtrl), "_UpdateItemPickPointsInRadar")]
        [HarmonyPostfix]
        static void UpdateItemRadar_Postfix(PartnerCtrl __instance)
        {
            Vector3 pos = __instance.transform.position;
            List<PartnerCtrl.ItemPickPointsRadar> toRemove = new List<PartnerCtrl.ItemPickPointsRadar>();
            foreach (var entry in __instance.m_ItemPickPointsRadar)
            {
                if (entry == null || entry.m_ItemPickPoint == null || entry.m_ItemPickPoint.gameObject == null)
                    continue;

                var itemPosition = entry.m_ItemPickPoint.gameObject.transform.position;
                var distance = Vector3.Distance(itemPosition, pos);
                if (distance > Plugin.Instance.itemRadarDistance.Value)
                    toRemove.Add(entry);
            }

            foreach (var entry in toRemove)
                __instance.m_ItemPickPointsRadar.Remove(entry);
        }
    }
}
