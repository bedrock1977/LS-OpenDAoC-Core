using DOL.GS.PacketHandler;

namespace DOL.GS.Effects
{
    public class AtlasOF_RainOfIceECSEffect : DamageAddECSEffect
    {
        public AtlasOF_RainOfIceECSEffect(ECSGameEffectInitParams initParams)
            : base(initParams)
        {
        }
        
        public override ushort Icon { get { return 4236; } }
        public override string Name { get { return "Rain Of Ice"; } }
        public override bool HasPositiveEffect { get { return true; } }
    }
}