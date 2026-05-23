using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Items;

namespace HWJBardHealer.Content.Accessories.Healer
{
    public class PhantomGrip : ThoriumItem
    {
        public override void SetDefaults()
        {
            this.isHealer = true;
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 10, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var mp = player.GetModPlayer<ThormwardPlayer>();
            mp.accPhantomGrip.Set(Item);
            mp.hasPhantomGrip = true;
        }
    }

    public class ScytheProGlobal : GlobalProjectile
    {
        private Vector2 originalPlayerPos;
        private bool alteredPosition = false;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.ModProjectile is ScythePro;
        public override bool InstancePerEntity => true;

        public override bool PreAI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            ThormwardPlayer mp = player.GetModPlayer<ThormwardPlayer>();

            if (mp.hasPhantomGrip && mp.phantomTransition > 0.01f)
            {
                originalPlayerPos = player.position;
                alteredPosition = true;

                Vector2 shoulder = player.Center + new Vector2(player.direction * -6f, -4f);
                Vector2 targetHandPos = player.Center + mp.phantomHandOffset;
                Vector2 currentHandPos = Vector2.Lerp(shoulder, targetHandPos, mp.phantomTransition);

                player.Center = currentHandPos;
            }
            return true;
        }

        public override void PostAI(Projectile projectile)
        {
            if (alteredPosition)
            {
                Player player = Main.player[projectile.owner];
                player.position = originalPlayerPos;
                alteredPosition = false;
            }
        }
    }

    public class MaterealizerHandLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Leggings);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.GetModPlayer<ThormwardPlayer>().phantomTransition > 0f
                && !drawInfo.drawPlayer.dead
                && !drawInfo.drawPlayer.invis;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            ThormwardPlayer mp = player.GetModPlayer<ThormwardPlayer>();

            Texture2D arm1 = ModContent.Request<Texture2D>("HWJBardHealer/Assets/MaterealizerHand1").Value;
            Texture2D arm2 = ModContent.Request<Texture2D>("HWJBardHealer/Assets/MaterealizerHand2").Value;

            Vector2 shoulder = player.Center + new Vector2(player.direction * -6f, -4f);

            Vector2 targetHandPos = player.Center + mp.phantomHandOffset;
            Vector2 handPos = Vector2.Lerp(shoulder, targetHandPos, mp.phantomTransition);

            float length1 = arm1.Height - 2;
            float length2 = arm2.Height - 2;
            Vector2 diff = handPos - shoulder;
            float dist = Math.Max(diff.Length(), 1f);

            float angle1, angle2;
            Vector2 elbow;

            if (dist >= length1 + length2)
            {
                angle1 = diff.ToRotation();
                angle2 = angle1;
                elbow = shoulder + diff.SafeNormalize(Vector2.UnitX) * length1;
            }
            else
            {
                float cosAngle = (length1 * length1 + dist * dist - length2 * length2) / (2 * length1 * dist);
                float alpha = (float)Math.Acos(MathHelper.Clamp(cosAngle, -1f, 1f));
                float thetaT = diff.ToRotation();

                angle1 = thetaT - alpha;
                elbow = shoulder + new Vector2(length1, 0).RotatedBy(angle1);
                angle2 = (handPos - elbow).ToRotation();
            }

            Vector2 origin = new Vector2(arm1.Width / 2f, arm1.Height);
            float spriteRot = MathHelper.PiOver2;

            Color drawColor = player.GetImmuneAlpha(Lighting.GetColor((int)shoulder.X / 16, (int)shoulder.Y / 16), drawInfo.shadow);

            drawInfo.DrawDataCache.Add(new DrawData(
                arm1,
                shoulder - Main.screenPosition,
                null,
                drawColor,
                angle1 + spriteRot,
                origin,
                1f,
                SpriteEffects.None,
                0
            ));

            drawInfo.DrawDataCache.Add(new DrawData(
                arm2,
                elbow - Main.screenPosition,
                null,
                drawColor,
                angle2 + spriteRot,
                origin,
                1f,
                SpriteEffects.None,
                0
            ));
        }
    }
}