using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;
using HWJBardHealer.Content.Projectiles.Bard;

namespace HWJBardHealer
{
    public class ThormwardPlayer : ModPlayer
    {
        public ItemWrapper accLifePedal;
        public int accLifePedalTimer;

        public override void ResetEffects()
        {
            accLifePedal.Reset();
        }

        public override void PostUpdateEquips()
        {
            if (accLifePedal.Active)
            {
                accLifePedalTimer++;

                int lifePetalType = ModContent.ProjectileType<LifePetalProj>();

                if (Player.ownedProjectileCounts[lifePetalType] < 1 && accLifePedalTimer > 20 && Main.myPlayer == Player.whoAmI)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Projectile.NewProjectile(
                            Player.GetSource_Accessory(accLifePedal.Item, null),
                            Player.Center.X,
                            Player.Center.Y,
                            0f,
                            0f,
                            lifePetalType,
                            100,
                            5f,  
                            Player.whoAmI,
                            i,   // ai
                            0f
                        );
                    }

                    accLifePedalTimer = 0;
                }
            }
            else
            {
                accLifePedalTimer = 0;
            }
        }

        public override void OnHitNPCWithProj(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            BardProjectile bardProj = projectile.ModProjectile as BardProjectile;
            if (bardProj == null)
                return;

            if (bardProj.InstrumentType == BardInstrumentType.Percussion)
            {
                if (Main.rand.NextFloat() < 0.25f && Main.myPlayer == Player.whoAmI && accLifePedal.Active)
                {
                    int petalProjType = ModContent.ProjectileType<LifePetalProj>();

                    List<Projectile> petals = new List<Projectile>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.active && proj.owner == Player.whoAmI && proj.type == petalProjType)
                            petals.Add(proj);
                    }

                    if (petals.Count > 0)
                    {
                        Projectile chosen = petals[Main.rand.Next(petals.Count)];
                        chosen.ai[1] = 1f;
                        chosen.friendly = true;
                        chosen.netUpdate = true;
                    }
                }
            }
        }

    }

    public struct ItemWrapper
    {
        public Item Item { readonly get; private set; }

        public bool Active => Item != null && !Item.IsAir;

        public void Reset() => Item = null;

        public void Set([NotNull] Item item) => Item = item;
    }
}
