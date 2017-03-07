using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nme_blog.ExtensionMethods
{
    public static class MyExtensions
    {
        /// <summary>
        /// Split and Join on a StringBuilder 
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="separator">Character to split by</param>
        /// <param name="includeLastElement">Whether or not to include the last split element. True: Include, False: Discard, dont include </param>
        /// <param name="includeLastSeparator">Whether or not to include an empty character at the end. True: Include, False: Discard, dont include </param>
        /// <returns></returns>
        public static StringBuilder GetPartialString(this StringBuilder sb, char separator, bool includeLastElement = true, bool includeLastSeparator = false)
        {
            IList<StringBuilder> sbList = Split(sb, separator, includeLastElement);
            return Join(sbList, includeLastSeparator);
        }

        /// <summary>
        /// Split a StringBuilder by a special character into a StringBuilder List
        /// </summary>
        /// <param name="sb">StringBuilder</param>
        /// <param name="separator">Character to split by</param>
        /// <param name="includeLastElement">Whether or not to include the last split element. True: Include, False: Discard, dont include </param>
        /// <returns>StringBuilder List</returns>
        public static StringBuilder[] Split(StringBuilder sb, char separator, bool includeLastElement = true)
        {
            List<StringBuilder> results = new List<StringBuilder>();

            StringBuilder current = new StringBuilder();
            for (int i = 0; i < sb.Length; ++i)
            {
                if (sb[i] == separator)
                {
                    results.Add(current);
                    current = new StringBuilder();
                }
                else
                {
                    current.Append(sb[i]);
                }
            }

            if (includeLastElement && current.Length > 0)
            {
                results.Add(current);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Join a StringBuilder List into a StringBuilder separated by an empty character
        /// </summary>
        /// <param name="sbList">StringBuilder List</param>
        /// <param name="includeLastSeparator">Whether or not to include an empty character at the end. True: Include, False: Discard, dont include </param>
        /// <returns></returns>
        public static StringBuilder Join(IList<StringBuilder> sbList, bool includeLastSeparator = false)
        {
            StringBuilder sb1 = new StringBuilder();
            foreach (var sb2 in sbList)
            {
                sb1.Append(sb2);
                sb1.Append(' ');
            }
            if (!includeLastSeparator)
            {
                sb1.Remove(sb1.Length - 1, 1);
            }
            return sb1;
        }

    }
}