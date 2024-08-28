using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.DTO.Firebase.RemoteConfig
{
    public static class RemoteConfigKeys
    {
        public readonly static RemoteConfigKey<double> PopularityDateWeight = new("popularity", "popularity_date_weight", 3);
        public readonly static RemoteConfigKey<int> PopularityDateMonths = new("popularity", "popularity_date_monts", 12);
        public readonly static RemoteConfigKey<double> PopularityCriticWeight = new("popularity", "popularity_critic_weight", 5);
        public readonly static RemoteConfigKey<double> PopularityPopularWeight = new("popularity", "popularity_popular_weight", 1);
        public readonly static RemoteConfigKey<double> PopularityFavoriteWeight = new("popularity", "popularity_favorite_weight", 0.5);
    }
}
