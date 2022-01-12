using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OuterWario
{
    public static class Util
    {
        public static GameObject[] FindObjectsWithName(string name)
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name.Equals(name)).ToArray();
        }

        public static Transform SearchInChildren(Transform parent, string target)
        {
            if (parent.name.Equals(target)) return parent;

            foreach (Transform child in parent)
            {
                var search = SearchInChildren(child, target);
                if (search != null) return search;
            }

            return null;
        }

        public static void Log(string msg)
        {
            OuterWario.Instance.ModHelper.Console.WriteLine(msg, OWML.Common.MessageType.Info);
        }

        public static void LogError(string msg)
        {
            OuterWario.Instance.ModHelper.Console.WriteLine("Error: " + msg, OWML.Common.MessageType.Error);
        }
    }
}
