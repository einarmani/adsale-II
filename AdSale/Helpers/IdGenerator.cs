using System;
using System.Collections.Generic;

namespace AdSale.Helpers
{
    public static class IdGenerator
    {
        /// <summary>
        /// Create a unique id for item in a list that belongs to a list
        /// </summary>
        /// <param name="existingIds">A list of existing ids the new identifier will be part of (i.e. siblings)</param>
        /// <param name="prefix">A prefix for the new id</param>
        /// <returns>A unique id</returns>
        public static string Make(IList<string> existingIds, string prefix = null)
        {
            var newGuid = Guid.NewGuid().ToString();
            var newGuidSubstr = newGuid.Substring(0, newGuid.IndexOf('-'));

            // The id for a parent is the first part of a guid. The id for a child is the parentid + first part of a new guid
            var newId = string.IsNullOrEmpty(prefix)
                ? newGuidSubstr
                : string.Format("{0}-{1}", prefix, newGuidSubstr);

            if (existingIds.Contains(newId))
            {
                // call method again. The chances are 1 in a five gazillions ;-)
                return Make(existingIds, prefix);
            }

            return newId;
        }
    }
}
