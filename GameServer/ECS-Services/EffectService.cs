using System;
using System.Collections.Generic;
using System.Linq;
using DOL.GS.PacketHandler;
using DOL.GS.PropertyCalc;
using DOL.GS.ServerProperties;
using DOL.GS.SpellEffects;
using DOL.GS.Spells;

namespace DOL.GS
{
    public static class EffectService
    {
        public static void Tick(long tick)
        {
            foreach (var e in EntityManager.GetAllEffects())
            {
                if (e.CancelEffect)
                {
                    if (tick > e.ExpireTick)
                        HandleCancelEffect(e);
                }
                else
                {
                    //switch (e.EffectType)
                    //{
                    //    //buffs
                    //    case eEffect.StrengthBuff:
                    //    case eEffect.DexterityBuff:
                    //    case eEffect.ConstitutionBuff:
                    //    case eEffect.QuicknessBuff:
                    //    case eEffect.AcuityBuff:
                    //    case eEffect.StrengthConBuff:
                    //    case eEffect.DexQuickBuff:
                    //    case eEffect.BaseAFBuff:
                    //    case eEffect.ArmorAbsorptionBuff:

                    //    case eEffect.BodyResistBuff:
                    //    case eEffect.SpiritResistBuff:
                    //    case eEffect.EnergyResistBuff:
                    //    case eEffect.HeatResistBuff:
                    //    case eEffect.ColdResistBuff:
                    //    case eEffect.MatterResistBuff:
                    //    case eEffect.BodySpiritEnergyBuff:
                    //    case eEffect.HeatColdMatterBuff:
                    //    case eEffect.AllMagicResistsBuff:
                    //    case eEffect.AllMeleeResistsBuff:
                    //    case eEffect.AllResistsBuff:

                    //    //debuffs
                    //    case eEffect.StrengthDebuff:
                    //    case eEffect.DexterityDebuff:
                    //    case eEffect.ConstitutionDebuff:
                    //    case eEffect.QuicknessDebuff:
                    //    case eEffect.AcuityDebuff:
                    //    case eEffect.StrConDebuff:
                    //    case eEffect.DexQuiDebuff:
                    //    case eEffect.ArmorAbsorptionDebuff:
                    //    case eEffect.ArmorFactorDebuff:

                    //    case eEffect.BodyResistDebuff:
                    //    case eEffect.ColdResistDebuff:
                    //    case eEffect.EnergyResistDebuff:
                    //    case eEffect.HeatResistDebuff:
                    //    case eEffect.MatterResistDebuff:
                    //    case eEffect.SpiritResistDebuff:
                    //    case eEffect.AllMeleeResistsDebuff:
                    //    case eEffect.NaturalResistDebuff:
                    //        HandlePropertyModification(e);
                    //        break;
                    //}
                    HandlePropertyModification(e);
                }

                EntityManager.RemoveEffect(e);
            }
        }

        private static void HandlePropertyModification(ECSGameEffect e)
        {

            if (e.Owner == null)
            {
                Console.WriteLine($"Invalid target for Effect {e}");
                return;
            }

            EffectListComponent effectList = e.Owner.effectListComponent;
            if (effectList == null)
            {
                Console.WriteLine($"No effect list found for {e.Owner}");
                return;
            }

            if (!effectList.AddEffect(e))
            {
                
                SendSpellResistAnimation(e);

            }
            else if (e.EffectType != eEffect.Pulse)
            {
                SendSpellAnimation(e);

                if (!e.RenewEffect)
                {
                    if (isDebuff(e.EffectType))
                    {
                        if (e.EffectType == eEffect.StrConDebuff || e.EffectType == eEffect.DexQuiDebuff)
                        {
                            foreach (var prop in getPropertyFromEffect(e.EffectType))
                            {
                                Console.WriteLine($"Debuffing {prop.ToString()}");
                                ApplyBonus(e.Owner, eBuffBonusCategory.SpecDebuff, prop, (int)e.SpellHandler.Spell.Value, true);
                            }
                        }
                        else
                        {
                            foreach (var prop in getPropertyFromEffect(e.EffectType))
                            {
                                Console.WriteLine($"Debuffing {prop.ToString()}");
                                ApplyBonus(e.Owner, eBuffBonusCategory.Debuff, prop, (int)e.SpellHandler.Spell.Value, true);
                            }
                        }

                    }
                    else
                    {
                        if (e.EffectType == eEffect.StrengthConBuff || e.EffectType == eEffect.DexQuickBuff || e.EffectType == eEffect.SpecAFBuff)
                        {
                            foreach (var prop in getPropertyFromEffect(e.EffectType))
                            {
                                Console.WriteLine($"Buffing {prop.ToString()}");
                                ApplyBonus(e.Owner, eBuffBonusCategory.SpecBuff, prop, (int)e.SpellHandler.Spell.Value, false);
                            }
                        }
                        else
                        {
                            foreach (var prop in getPropertyFromEffect(e.EffectType))
                            {
                                Console.WriteLine($"Buffing {prop.ToString()}");
                                ApplyBonus(e.Owner, eBuffBonusCategory.BaseBuff, prop, (int)e.SpellHandler.Spell.Value, false);
                            }
                        }
                    }
                }
                if (e.Owner is GamePlayer player)
                {
                    SendPlayerUpdates(player);
                }
            }
        }

        private static void HandleCancelEffect(ECSGameEffect e)
        {
            Console.WriteLine($"Handling Cancel Effect {e.SpellHandler.ToString()}");

            if (!e.Owner.effectListComponent.RemoveEffect(e))
            {
                Console.WriteLine("Unable to remove effect!");
                return;
            }
            if (e.EffectType != eEffect.Pulse)
            {
                if (isDebuff(e.EffectType))
                {
                    if (e.EffectType == eEffect.StrConDebuff || e.EffectType == eEffect.DexQuiDebuff)
                    {
                        foreach (var prop in getPropertyFromEffect(e.EffectType))
                        {
                            Console.WriteLine($"Canceling {prop.ToString()} on {e.Owner}.");
                            ApplyBonus(e.Owner, eBuffBonusCategory.SpecDebuff, prop, (int)e.SpellHandler.Spell.Value, false);
                        }
                    }
                    else
                    {
                        foreach (var prop in getPropertyFromEffect(e.EffectType))
                        {
                            Console.WriteLine($"Canceling {prop.ToString()} on {e.Owner}.");
                            ApplyBonus(e.Owner, eBuffBonusCategory.Debuff, prop, (int)e.SpellHandler.Spell.Value, false);
                        }
                    }
                }
                else
                {
                    if (e.EffectType == eEffect.StrengthConBuff || e.EffectType == eEffect.DexQuickBuff || e.EffectType == eEffect.SpecAFBuff)
                    {
                        foreach (var prop in getPropertyFromEffect(e.EffectType))
                        {
                            Console.WriteLine($"Canceling {prop.ToString()}");
                            ApplyBonus(e.Owner, eBuffBonusCategory.SpecBuff, prop, (int)e.SpellHandler.Spell.Value, true);
                        }
                    }
                    else
                    {
                        foreach (var prop in getPropertyFromEffect(e.EffectType))
                        {
                            Console.WriteLine($"Canceling {prop.ToString()}");
                            ApplyBonus(e.Owner, eBuffBonusCategory.BaseBuff, prop, (int)e.SpellHandler.Spell.Value, true);
                        }
                    }
                }
            }
            if (e.Owner is GamePlayer player)
            {
                SendPlayerUpdates(player);
                //Now update EffectList
                player.Out.SendUpdateIcons(e.Owner.effectListComponent.Effects.Values.Where(ef => ef.Icon != 0).ToList(), ref e.Owner.effectListComponent._lastUpdateEffectsCount);
            }
        }

        public static void SendSpellAnimation(ECSGameEffect e)
        {
            GameLiving target = e.SpellHandler.Target != null ? e.SpellHandler.Target : e.SpellHandler.Caster;

            //foreach (GamePlayer player in e.SpellHandler.Target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
            foreach (GamePlayer player in target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
            {
                player.Out.SendSpellEffectAnimation(e.SpellHandler.Caster, e.Owner, e.SpellHandler.Spell.ClientEffect, 0, false, 1);
            }

            if (e.Owner is GamePlayer player1)
            {
                player1.Out.SendUpdateIcons(player1.effectListComponent.Effects.Values.Where(ef => ef.Icon != 0).ToList(), ref player1.effectListComponent._lastUpdateEffectsCount);
            }
        }

        private static void SendSpellResistAnimation(ECSGameEffect e)
        {
            GameLiving target = e.SpellHandler.Target != null ? e.SpellHandler.Target : e.SpellHandler.Caster;
            //foreach (GamePlayer player in e.SpellHandler.Target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
            foreach (GamePlayer player in target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
            {
                player.Out.SendSpellEffectAnimation(e.SpellHandler.Caster, e.Owner, e.SpellHandler.Spell.ClientEffect, 0, false, 0);
            }
        }

        private static void SendPlayerUpdates(GamePlayer player)
        {
            player.Out.SendCharStatsUpdate();
            player.Out.SendCharResistsUpdate();
            player.UpdateEncumberance();
            player.UpdatePlayerStatus();
            player.Out.SendUpdatePlayer();
        }

        private static List<eProperty> getPropertyFromEffect(eEffect e)
        {
            List<eProperty> list = new List<eProperty>();
            switch (e)
            {
                //stats
                case eEffect.StrengthBuff:
                case eEffect.StrengthDebuff:
                    list.Add(eProperty.Strength);
                    return list;
                case eEffect.DexterityBuff:
                case eEffect.DexterityDebuff:
                    list.Add(eProperty.Dexterity);
                    return list;
                case eEffect.ConstitutionBuff:
                case eEffect.ConstitutionDebuff:
                    list.Add(eProperty.Constitution);
                    return list;
                case eEffect.AcuityBuff:
                case eEffect.AcuityDebuff:
                    list.Add(eProperty.Acuity);
                    return list;

                case eEffect.StrengthConBuff:
                case eEffect.StrConDebuff:
                    list.Add(eProperty.Strength);
                    list.Add(eProperty.Constitution);
                    return list;
                case eEffect.DexQuickBuff:
                case eEffect.DexQuiDebuff:
                    list.Add(eProperty.Dexterity);
                    list.Add(eProperty.Quickness);
                    return list;
                case eEffect.BaseAFBuff:
                case eEffect.SpecAFBuff:
                case eEffect.ArmorFactorDebuff:
                    list.Add(eProperty.ArmorFactor);
                    return list;
                case eEffect.ArmorAbsorptionBuff:
                case eEffect.ArmorAbsorptionDebuff:
                    list.Add(eProperty.ArmorAbsorption);
                    return list;

                //resists
                case eEffect.NaturalResistDebuff:
                    list.Add(eProperty.Resist_Natural);
                    return list;
                case eEffect.BodyResistBuff:
                case eEffect.BodyResistDebuff:
                    list.Add(eProperty.Resist_Body);
                    return list;
                case eEffect.SpiritResistBuff:
                case eEffect.SpiritResistDebuff:
                    list.Add(eProperty.Resist_Spirit);
                    return list;
                case eEffect.EnergyResistBuff:
                case eEffect.EnergyResistDebuff:
                    list.Add(eProperty.Resist_Energy);
                    return list;
                case eEffect.HeatResistBuff:
                case eEffect.HeatResistDebuff:
                    list.Add(eProperty.Resist_Heat);
                    return list;
                case eEffect.ColdResistBuff:
                case eEffect.ColdResistDebuff:
                    list.Add(eProperty.Resist_Cold);
                    return list;
                case eEffect.MatterResistBuff:
                case eEffect.MatterResistDebuff:
                    list.Add(eProperty.Resist_Matter);
                    return list;
                case eEffect.HeatColdMatterBuff:
                    list.Add(eProperty.Resist_Heat);
                    list.Add(eProperty.Resist_Cold);
                    list.Add(eProperty.Resist_Matter);
                    return list;
                case eEffect.BodySpiritEnergyBuff:
                    list.Add(eProperty.Resist_Body);
                    list.Add(eProperty.Resist_Spirit);
                    list.Add(eProperty.Resist_Energy);
                    return list;
                case eEffect.AllMagicResistsBuff:
                    list.Add(eProperty.Resist_Body);
                    list.Add(eProperty.Resist_Spirit);
                    list.Add(eProperty.Resist_Energy);
                    list.Add(eProperty.Resist_Heat);
                    list.Add(eProperty.Resist_Cold);
                    list.Add(eProperty.Resist_Matter);
                    return list;

                case eEffect.SlashResistBuff:
                case eEffect.SlashResistDebuff:
                    list.Add(eProperty.Resist_Slash);
                    return list;
                case eEffect.ThrustResistBuff:
                case eEffect.ThrustResistDebuff:
                    list.Add(eProperty.Resist_Thrust);
                    return list;
                case eEffect.CrushResistBuff:
                case eEffect.CrushResistDebuff:
                    list.Add(eProperty.Resist_Crush);
                    return list;
                case eEffect.AllMeleeResistsBuff:
                case eEffect.AllMeleeResistsDebuff:
                    list.Add(eProperty.Resist_Crush);
                    list.Add(eProperty.Resist_Thrust);
                    list.Add(eProperty.Resist_Slash);
                    return list;

                default:
                    Console.WriteLine($"Unable to find property mapping for: {e}");
                    return list;
            }
        }

        private static Boolean isDebuff(eEffect e)
        {
            switch (e)
            {
                case eEffect.Bladeturn:
                case eEffect.DamageAdd:
                case eEffect.DamageReturn:
                case eEffect.FocusShield:
                case eEffect.AblativeArmor:
                case eEffect.MeleeDamageBuff:
                case eEffect.MeleeHasteBuff:
                case eEffect.Celerity:
                case eEffect.MovementSpeedBuff:
                case eEffect.HealOverTime:
                case eEffect.StrengthBuff:
                case eEffect.DexterityBuff:
                case eEffect.ConstitutionBuff:
                case eEffect.AcuityBuff:
                case eEffect.StrengthConBuff:
                case eEffect.DexQuickBuff:
                case eEffect.BaseAFBuff:
                case eEffect.SpecAFBuff:
                case eEffect.PaladinAf:
                case eEffect.ArmorAbsorptionBuff:
                case eEffect.BodyResistBuff:
                case eEffect.SpiritResistBuff:
                case eEffect.EnergyResistBuff:
                case eEffect.HeatResistBuff:
                case eEffect.ColdResistBuff:
                case eEffect.MatterResistBuff:
                case eEffect.HealthRegenBuff:
                case eEffect.EnduranceRegenBuff:
                case eEffect.PowerRegenBuff:
                    return false;

                case eEffect.StrengthDebuff:
                case eEffect.DexterityDebuff:
                case eEffect.ConstitutionDebuff:
                case eEffect.AcuityDebuff:
                case eEffect.StrConDebuff:
                case eEffect.DexQuiDebuff:
                case eEffect.ArmorFactorDebuff:
                case eEffect.ArmorAbsorptionDebuff:
                case eEffect.BodyResistDebuff:
                case eEffect.SpiritResistDebuff:
                case eEffect.EnergyResistDebuff:
                case eEffect.HeatResistDebuff:
                case eEffect.ColdResistDebuff:
                case eEffect.MatterResistDebuff:
                    return true;
                default:
                    Console.WriteLine($"Unable to detect debuff status for {e}");
                    return false;
            }


        }

        

        /// <summary>
		/// Method used to apply bonuses
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="BonusCat"></param>
		/// <param name="Property"></param>
		/// <param name="Value"></param>
		/// <param name="IsSubstracted"></param>
		private static void ApplyBonus(GameLiving owner, eBuffBonusCategory BonusCat, eProperty Property, int Value, bool IsSubstracted)
        {
            IPropertyIndexer tblBonusCat;
            if (Property != eProperty.Undefined)
            {
                tblBonusCat = GetBonusCategory(owner, BonusCat);
                Console.WriteLine($"Value before: {tblBonusCat[(int)Property]}");
                if (IsSubstracted)
                    tblBonusCat[(int)Property] -= Value;
                else
                    tblBonusCat[(int)Property] += Value;
                Console.WriteLine($"Value after: {tblBonusCat[(int)Property]}");
            }
        }

        private static IPropertyIndexer GetBonusCategory(GameLiving target, eBuffBonusCategory categoryid)
        {
            IPropertyIndexer bonuscat = null;
            switch (categoryid)
            {
                case eBuffBonusCategory.BaseBuff:
                    bonuscat = target.BaseBuffBonusCategory;
                    break;
                case eBuffBonusCategory.SpecBuff:
                    bonuscat = target.SpecBuffBonusCategory;
                    break;
                case eBuffBonusCategory.Debuff:
                    bonuscat = target.DebuffCategory;
                    break;
                case eBuffBonusCategory.Other:
                    bonuscat = target.BuffBonusCategory4;
                    break;
                case eBuffBonusCategory.SpecDebuff:
                    bonuscat = target.SpecDebuffCategory;
                    break;
                case eBuffBonusCategory.AbilityBuff:
                    bonuscat = target.AbilityBonus;
                    break;
                default:
                    //if (log.IsErrorEnabled)
                    Console.WriteLine("BonusCategory not found " + categoryid + "!");
                    break;
            }
            return bonuscat;
        }
        #region DoT/Bleed

        // For DoT/Bleed functionality. Can be moved.
        public static void OnEffectPulse(ECSGameEffect effect)
        {

            if (effect.Owner.IsAlive == false)
            {
                effect.CancelEffect = true;
                EntityManager.AddEffect(effect);
            }

            if (effect.Owner.IsAlive)
            {
                if (effect.SpellHandler is DoTSpellHandler handler)
                {
                    // An acidic cloud surrounds you!
                    handler.MessageToLiving(effect.Owner, effect.SpellHandler.Spell.Message1, eChatType.CT_Spell);
                    // {0} is surrounded by an acidic cloud!
                    Message.SystemToArea(effect.Owner, Util.MakeSentence(effect.SpellHandler.Spell.Message2, effect.Owner.GetName(0, false)), eChatType.CT_YouHit, effect.Owner);
                    handler.OnDirectEffect(effect.Owner, effect.Effectiveness);
                }
                else if (effect.SpellHandler is StyleBleeding bleedHandler)
                {
                    if (effect.StartTick + effect.PulseFreq > GameLoop.GameLoopTime && effect.Owner.TempProperties.getProperty<int>(StyleBleeding.BLEED_VALUE_PROPERTY) == 0)
                    {
                        effect.Owner.TempProperties.setProperty(StyleBleeding.BLEED_VALUE_PROPERTY, (int)bleedHandler.Spell.Damage + (int)bleedHandler.Spell.Damage * Util.Random(25) / 100);  // + random max 25%

                    }
                    bleedHandler.MessageToLiving(effect.Owner, bleedHandler.Spell.Message1, eChatType.CT_YouWereHit);
                    Message.SystemToArea(effect.Owner, Util.MakeSentence(bleedHandler.Spell.Message2, effect.Owner.GetName(0, false)), eChatType.CT_YouHit, effect.Owner);

                    int bleedValue = effect.Owner.TempProperties.getProperty<int>(StyleBleeding.BLEED_VALUE_PROPERTY);

                    AttackData ad = bleedHandler.CalculateDamageToTarget(effect.Owner, 1.0);

                    bleedHandler.SendDamageMessages(ad);

                    // attacker must be null, attack result is 0x0A
                    foreach (GamePlayer player in ad.Target.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                    {
                        player.Out.SendCombatAnimation(null, ad.Target, 0, 0, 0, 0, 0x0A, ad.Target.HealthPercent);
                    }
                    // send animation before dealing damage else dead livings show no animation
                    ad.Target.OnAttackedByEnemy(ad);
                    ad.Attacker.DealDamage(ad);

                    if (--bleedValue <= 0 || !effect.Owner.IsAlive)
                    {
                        effect.ExpireTick = GameLoop.GameLoopTime - 1;
                    }
                    else effect.Owner.TempProperties.setProperty(StyleBleeding.BLEED_VALUE_PROPERTY, bleedValue);
                }
            }
        }

        #endregion
    }
}