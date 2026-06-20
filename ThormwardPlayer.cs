using HWJBardHealer.Content.Projectiles.Bard;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Projectiles.Thrower;

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
        public bool accHarmonyHeadgear;
        public bool throwGuide4;

        public Vector2 phantomHandOffset;
        public float phantomTransition = 0f;

        private static readonly FieldInfo ThoriumEmpField = typeof(ThoriumPlayer).GetField("Empowerments", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        private static readonly PropertyInfo ThoriumEmpProp = typeof(ThoriumPlayer).GetProperty("Empowerments", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);


        public override void ResetEffects()
        {
            accLifePedal.Reset();
            accPopeCross.Reset();
            accDarkPoetry = false;
            accHarmonyHeadgear = false;
            throwGuide4 = false;

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
            ThoriumPlayer thoriumPlayer = Player.GetModPlayer<ThoriumPlayer>();
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
            if (accHarmonyHeadgear)
            {
                int uniqueEmpowerments = GetUniqueEmpowermentCount(thoriumPlayer);
                if (uniqueEmpowerments > 0)
                {
                    Player.moveSpeed += uniqueEmpowerments * 0.02f;
                    Player.GetAttackSpeed(BardDamage.Instance) += uniqueEmpowerments * 0.02f;
                    Player.GetDamage(BardDamage.Instance) += uniqueEmpowerments * 0.04f;
                    thoriumPlayer.inspirationRegenBonus += uniqueEmpowerments * 0.02f;
                }
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


        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            IEntitySource source = proj.GetSource_OnHit(target, null);
            if (throwGuide4)
            {
                int guideDamage = 0;
                if (throwGuide4)
                {
                    guideDamage = (int)((double)proj.damage * 0.3);
                }
                if (guideDamage > 0)
                {
                    Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<ThrowingGuideFollowup>(), guideDamage, 0f, proj.owner, (float)target.whoAmI, 0f, 0f);
                }
            }

            BardProjectile bardProj = proj.ModProjectile as BardProjectile;
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
                        Projectile proj2 = Main.projectile[i];
                        if (proj2.active && proj2.owner == Player.whoAmI && proj2.type == petalProjType)
                            petals.Add(proj2);
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

        private int GetUniqueEmpowermentCount(ThoriumPlayer thoriumPlayer)
        {
            object rawEmps = ThoriumEmpField?.GetValue(thoriumPlayer) ?? ThoriumEmpProp?.GetValue(thoriumPlayer);
            if (rawEmps is ThoriumMod.Empowerments.EmpowermentData emps)
            {
                return emps.ActiveEmpowerments.Count;
            }
            return 0;
        }

        public struct ItemWrapper
        {
            public Item Item { readonly get; private set; }
            public bool Active => Item != null && !Item.IsAir;
            public void Reset() => Item = null;
            public void Set(Item item) => Item = item;
        }
    }
}
