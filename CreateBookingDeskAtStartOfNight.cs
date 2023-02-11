using Kitchen;
using KitchenData;
using KitchenLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenIRegretIt
{
    public class CreateBookingDeskAtStartOfNight : StartOfNightSystem
    {
        EntityQuery appliancesQuery;
        protected override void Initialise()
        {
            base.Initialise();
            appliancesQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CAppliance)));
        }
        public bool FindTile(ref int placed_tile, List<Vector3> floor_tiles, out Vector3 candidate)
        {
            candidate = Vector3.zero;
            bool flag = false;
            while (!flag && placed_tile < floor_tiles.Count)
            {
                candidate = floor_tiles[placed_tile++];
                if (GetOccupant(candidate) == default(Entity))
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                return false;
            }
            return true;
        }

        protected override void OnUpdate()
        {
            if (PreferenceUtils.Get<KitchenLib.BoolPreference>(Main.MOD_GUID, Main.ENABLED_ID).Value)
            {
                Main.LogInfo("Checking if Booking Desk is missing");
                NativeArray<Entity> entities = appliancesQuery.ToEntityArray(Allocator.Temp);
                bool spawnBookingDesk = true;
                foreach (Entity entity in entities)
                {
                    if (Require(entity, out CAppliance appliance))
                    {
                        if (appliance.ID == AssetReference.BookingDesk)
                        {
                            Main.LogInfo("Checking if Booking Desk found. Skipping");
                            spawnBookingDesk = false;
                            break;
                        }
                    }
                }

                if (spawnBookingDesk)
                {
                    int level = GetOrCreate<SPlayerLevel>().Level;
                    Main.LogInfo($"Player level = {level}");
                    if (level < 1)
                    {
                        Main.LogInfo("Player level too low. Skipping");
                    }
                    else
                    {
                        List<Vector3> postTiles = GetPostTiles();
                        int placed_tile = 0;
                        if (!FindTile(ref placed_tile, postTiles, out var candidate))
                        {
                            candidate = GetFallbackTile();
                        }
                        if (PreferenceUtils.Get<KitchenLib.BoolPreference>(Main.MOD_GUID, Main.AS_PARCEL_ID).Value)
                        {
                            Main.LogInfo("Creating Booking Desk as parcel");
                            PostHelpers.CreateApplianceParcel(base.EntityManager, candidate, AssetReference.BookingDesk);
                        }
                        else
                        {
                            Main.LogInfo("Creating Booking Desk as letter");
                            PostHelpers.CreateBlueprintLetter(base.EntityManager, candidate, AssetReference.BookingDesk, 0f);
                        }
                    }
                }
                entities.Dispose();
            }
        }
    }
}
