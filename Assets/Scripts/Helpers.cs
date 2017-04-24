using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts {
	public static class Helpers {

		public static Dictionary<K, V> Add<K, V>(this Dictionary<K, V> dict, K key, V value) {
			dict.Add(key, value);
			return dict;
		}
	}
}
