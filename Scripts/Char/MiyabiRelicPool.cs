using BaseLib.Abstracts;
using Godot;

namespace Miyabists2.Scripts.Char
{
    internal class MiyabiRelicPool : CustomRelicPoolModel
    {
        public override Color LabOutlineColor => Miyabi.Color;

        public override string BigEnergyIconPath => "ironclad";
        //public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
    }
}
