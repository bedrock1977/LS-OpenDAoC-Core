﻿using DOL.AI.Brain;
using DOL.GS;

namespace DOL.GS
{
	public class PygmyGoblinTangler : GameNPC
	{
		public PygmyGoblinTangler() : base() { }

		public override bool AddToWorld()
		{
			INpcTemplate npcTemplate = NpcTemplateMgr.GetTemplate(12979);
			LoadTemplate(npcTemplate);
			//RespawnInterval = Util.Random(3600000, 7200000);

			PygmyGoblinTanglerBrain sbrain = new PygmyGoblinTanglerBrain();
			if (NPCTemplate != null)
			{
				sbrain.AggroLevel = NPCTemplate.AggroLevel;
				sbrain.AggroRange = NPCTemplate.AggroRange;
			}
			BodyType = 0;
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
	public class PygmyGoblinTanglerBrain : StandardMobBrain
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public PygmyGoblinTanglerBrain() : base()
		{
			ThinkInterval = 1500;
		}
		private bool BringGoblins = false;
        public override void Think()
		{
			if (!CheckProximityAggro())
				BringGoblins = false;

			if(HasAggro && Body.TargetObject != null)
            {
				if(!BringGoblins)
                {
					foreach(GameNPC npc in Body.GetNPCsInRadius(500))
                    {
						GameLiving target = Body.TargetObject as GameLiving;
						if(npc != null && npc.IsAlive && npc.Name.ToLower() == "pygmy goblin" && npc.Brain is not ControlledMobBrain && npc.Brain is StandardMobBrain brain)
                        {
							if (target != null && target.IsAlive && brain != null && !brain.HasAggro)
								brain.AddToAggroList(target, 10);
						}
                    }
					BringGoblins = true;
                }
            }
			base.Think();
		}
	}
}



