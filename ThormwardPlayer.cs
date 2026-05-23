using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Items.HealerItems;
using HWJBardHealer.Content.Projectiles.Bard;

namespace HWJBardHealer
{
    public class ThormwardPlayer : ModPlayer
    {
        public ItemWrapper accLifePedal;
        public ItemWrapper accPopeCross;
        public ItemWrapper accGenesisCore;
        public ItemWrapper accPhantomGrip;

        public int accLifePedalTimer;
        private int popeCrossHitCounter;
        private int popeCrossDefenseTimer;

        public bool hasPhantomGrip;
        public bool accDarkPoetry;

        public Vector2 phantomHandOffset;
        public float phantomTransition = 0f;

        public override void ResetEffects()
        {
            accLifePedal.Reset();
            accPopeCross.Reset();
            accDarkPoetry = false;

            accPhantomGrip.Reset();
            hasPhantomGrip = false;

            if (popeCrossDefenseTimer > 0)
            {
                popeCrossDefenseTimer--;
                Player.statDefense += 20;
            }
        }

        public override void OnEnterWorld()
        {
            phantomHandOffset = new Vector2(45f * Player.direction, 15f);
        }

        public override void PostUpdateEquips()
        {
            if (accLifePedal.Active)
            {
                accLifePedalTimer++;

                int lifePetalType = ModContent.ProjectileType<LifePetalProj>();
                if (Player.ownedProjectileCounts[lifePetalType] < 1 &&
                    accLifePedalTimer > 20 &&
                    Main.myPlayer == Player.whoAmI)
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
                            i, // ai
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

        public override void PostUpdate()
        {
            bool hideHand = false;
            if (!Player.HeldItem.IsAir && Player.HeldItem.ModItem is ScytheItem)
            {
                if (ItemLoader.AltFunctionUse(Player.HeldItem, Player))
                {
                    hideHand = true;
                }
            }

            if (hasPhantomGrip && !hideHand)
                phantomTransition = MathHelper.Clamp(phantomTransition + 0.08f, 0f, 1f);
            else
                phantomTransition = MathHelper.Clamp(phantomTransition - 0.08f, 0f, 1f);

            if (hasPhantomGrip && Main.myPlayer == Player.whoAmI)
            {
                if (Main.mouseRight && !hideHand)
                {
                    Vector2 shoulderPos = Player.Center + new Vector2(Player.direction * -6f, -4f);
                    Vector2 diff = Main.MouseWorld - shoulderPos;

                    if (diff.Length() > 110f) // radius
                    {
                        diff.Normalize();
                        diff *= 110f;
                    }
                    phantomHandOffset = diff;
                }
            }
        }

        public void IncrementPopeCrossProgress()
        {
            popeCrossHitCounter++;
            if (popeCrossHitCounter >= 12)
            {
                popeCrossHitCounter = 0;
                ActivatePopeCrossBlessing();
            }
        }

        private void ActivatePopeCrossBlessing()
        {
            popeCrossDefenseTimer = 900;

            bool healedAnyone = false;

            // Heal
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player target = Main.player[i];
                if (target.active && target.team == Player.team && target.team != 0 && target.whoAmI != Player.whoAmI)
                {
                    int healAmount = 8;
                    target.statLife = Utils.Clamp(target.statLife + healAmount, 0, target.statLifeMax2);
                    target.HealEffect(healAmount, true);
                    healedAnyone = true;

                    // glow 
                    for (int j = 0; j < 8; j++)
                    {
                        int crossDust = Dust.NewDust(
                            target.Center - new Vector2(4, 16),
                            8, 8,
                            DustID.GoldCoin,
                            Main.rand.NextFloat(-1f, 1f),
                            Main.rand.NextFloat(-2f, 0f),
                            120,
                            default,
                            1.3f
                        );
                        Main.dust[crossDust].noGravity = true;
                    }
                }
            }

            // sound 
            if (healedAnyone)
            {
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 1.2f }, Player.Center);

                for (int i = 0; i < 25; i++)
                {
                    int dust = Dust.NewDust(
                        Player.Center - new Vector2(10, 10),
                        20, 20,
                        DustID.GoldFlame,
                        Main.rand.NextFloat(-2f, 2f),
                        Main.rand.NextFloat(-3f, 0f),
                        150,
                        default,
                        1.6f
                    );
                    Main.dust[dust].noGravity = true;
                }
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
        public void Set(Item item) => Item = item;
    }
}
