using System.Collections.Generic;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MVVMToolkit.UI
{
    public static class LocalizeStringEventExtensions
    {
        public static void Set(this LocalizeStringEvent local, string localizationKey, string table = null, params (string, string)[] dynamicValues)
        {
            if (!string.IsNullOrEmpty(table))
            {
                local.StringReference.TableReference = table;
            }

            Dictionary<string, IVariable> variables = new();
            foreach ((string key, string value) in dynamicValues)
            {
                variables[key] = new StringVariable { Value = value };
            }

            local.StringReference.Arguments = new object[] { variables };
            local.SetEntry(localizationKey);
            local.RefreshString();
        }

        public static void Set(this LocalizeStringEvent local, string localizationKey, params (string, string)[] dynamicValues)
        {
            local.Set(localizationKey, null, dynamicValues);
        }
    }
}
