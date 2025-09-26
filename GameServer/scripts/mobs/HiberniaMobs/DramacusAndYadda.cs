﻿using DOL.AI.Brain;
using DOL.GS;

namespace DOL.GS
{
	public class Dramacus : GameNPC
	{
		public Dramacus() : base() { }

		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60160118);
			LoadTemplate(npcTemplate);

			DramacusBrain sbrain = new DramacusBrain();
			SetOwnBrain(sbrain);
			LoadedFromScript = false;//load from database
			SaveIntoDatabase();
			base.AddToWorld();
			return true;
		}
	}
}
namespace DOL.AI.Brain
{
	public class DramacusBrain : StandardMobBrain
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public DramacusBrain() : base()
		{
			AggroLevel = 100;
			AggroRange = 400;
			ThinkInterval = 1500;
		}
		public override void Think()
		{
			if (HasAggro && Body.TargetObject != null)
			{
				GameLiving target = Body.TargetObject as GameLiving;
				foreach (GameNPC npc in Body.GetNPCsInRadius(4000))
				{
					if (npc != null && npc.IsAlive && npc.Brain is YaddaBrain brian)
					{
						if (!brian.HasAggro && brian != null && target != null && target.IsAlive)
							brian.AddToAggroList(target, 10);
					}
				}
			}
			base.Think();
		}
	}
}

namespace DOL.GS
{
	public class Yadda : GameNPC
	{
		public Yadda() : base() { }

		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(60168085);
			LoadTemplate(npcTemplate);

			YaddaBrain sbrain = new YaddaBrain();
			SetOwnBrain(sbrain);
			LoadedFromScript = false;//load from database
			SaveIntoDatabase();
			base.AddToWorld();
			return true;
		}
	}
}
namespace DOL.AI.Brain
{
	public class YaddaBrain : StandardMobBrain
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public YaddaBrain() : base()
		{
			AggroLevel = 100;
			AggroRange = 400;
			ThinkInterval = 1500;
		}
		public override void Think()
		{
			if (HasAggro && Body.TargetObject != null)
			{
				GameLiving target = Body.TargetObject as GameLiving;
				foreach (GameNPC npc in Body.GetNPCsInRadius(4000))
				{
					if (npc != null && npc.IsAlive && npc.Brain is DramacusBrain brian)
					{
						if (!brian.HasAggro && brian != null && target != null && target.IsAlive)
							brian.AddToAggroList(target, 10);
					}
				}
			}
			base.Think();
		}
	}
}
