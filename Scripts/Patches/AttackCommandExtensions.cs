using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Miyabists2.Scripts.Patches
{
    public static class AttackCommandExtensions
    {
        // 提前缓存 PropertyInfo 提升性能
        private static readonly PropertyInfo DamagePropsProperty =
            typeof(AttackCommand).GetProperty("DamageProps", BindingFlags.Public | BindingFlags.Instance);

        public static AttackCommand Unblockable(this AttackCommand command)
        {
            // 1. 先把原本的 DamageProps 读出来
            ValueProp currentProps = command.DamageProps;

            // 2. 进行位运算叠加（假设原版枚举里包含 Unblockable）
            ValueProp newProps = currentProps | ValueProp.Unblockable;

            // 3. 通过反射绕过 private set 强行写入
            DamagePropsProperty?.SetValue(command, newProps);

            return command;
        }
    }
}
