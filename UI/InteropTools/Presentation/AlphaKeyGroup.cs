using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using Windows.Globalization.Collation;

namespace InteropTools.Presentation
{
    public class AlphaKeyGroup<T> : List<T>
    {
        /// <summary>
        ///     The delegate that is used to get the key information.
        /// </summary>
        /// <param name="item">An object of type T</param>
        /// <returns>The key value to use for this object</returns>
        public delegate string GetKeyDelegate(T item);

        /// <summary>
        ///     Public constructor.
        /// </summary>
        /// <param name="key">The key for this group.</param>
        private AlphaKeyGroup(string key)
        {
            Key = key;
        }

        /// <summary>
        ///     The Key of this group.
        /// </summary>
        public string Key { get; }

        /// <summary>
        ///     Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The </param>
        /// <returns>Theitems source for a LongListSelector</returns>
        private static List<AlphaKeyGroup<T>> CreateGroups(CharacterGroupings slg)
        {
            return (from key in slg where string.IsNullOrWhiteSpace(key.Label) == false select new AlphaKeyGroup<T>(key.Label)).ToList();
        }

        /// <summary>
        ///     Create a list of AlphaGroup<T> with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, GetKeyDelegate getKey,
            bool sort)
        {
            CharacterGroupings slg = new();
            List<AlphaKeyGroup<T>> list = CreateGroups(slg);

            foreach (T item in items)
            {
                string index = "";
                index = slg.Lookup(getKey(item));

                if (string.IsNullOrEmpty(index) == false)
                {
                    try
                    {
                        list.Find(a => a.Key == index).Add(item);
                    }

                    catch
                    {
                        list.First().Add(item);
                    }
                }
            }

            if (!sort)
            {
                return list;
            }

            foreach (AlphaKeyGroup<T> group in list)
            {
                group.Sort((c0, c1) => ci.CompareInfo.Compare(getKey(c0), getKey(c1)));
            }

            return list;
        }
    }
}