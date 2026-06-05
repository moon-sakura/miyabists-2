using STS2RitsuLib.Interop.AutoRegistration;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class YixuanPotionPool : TypeListPotionPoolModel
    {
        public override Color LabOutlineColor => Yixuan.Color;

        public override string EnergyColorName => "regent";

        public override string BigEnergyIconPath => "regent";
        //public override string BigEnergyIconPath => "res://images/charui/big_energy.png";
        //public override string TextEnergyIconPath => "res://images/charui/text_energy.png";
    }
}