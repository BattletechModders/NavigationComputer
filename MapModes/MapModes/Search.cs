using BattleTech;
using Harmony;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace MapModes
{
    public class Search : IMapMode
    {
        private readonly static Dictionary<string, string> tagIDToFriendlyName = new Dictionary<string, string>()
        {
            { "planet_industry_agriculture", "agriculture" },
            { "planet_industry_aquaculture", "aquaculture" },
            { "planet_industry_manufacturing", "manufacturing" },
            { "planet_industry_mining", "mining" },
            { "planet_industry_poor", "poor" },
            { "planet_industry_recreation", "recreation" },
            { "planet_industry_research", "research" },
            { "planet_industry_rich", "rich" },

            { "planet_size_large", "high gravity planet" },
            { "planet_size_medium", "medium gravity planet" },
            { "planet_size_small", "low gravity planet" },

            { "planet_civ_innersphere", "inner sphere-level civilization" },
            { "planet_civ_periphery", "periphery-level civilization" },
            { "planet_civ_primitive", "primitive civilization" },

            { "planet_climate_arctic", "arctic world" },
            { "planet_climate_arid", "arid world" },
            { "planet_climate_desert", "desert world" },
            { "planet_climate_ice", "ice world" },
            { "planet_climate_mars", "martian world" },
            { "planet_climate_lunar", "lunar world" },
            { "planet_climate_moon", "barren world" },
            { "planet_climate_rocky", "rocky world" },
            { "planet_climate_terran", "terran world" },
            { "planet_climate_tropical", "tropical world" },
            { "planet_climate_water", "water world" },

            { "planet_other_alienvegetation", "alien vegetation" },
            { "planet_other_battlefield", "battlefield" },
            { "planet_other_blackmarket", "black market" },
            { "planet_other_boreholes", "geothermal boreholes" },
            { "planet_other_capital", "regional capital" },
            { "planet_other_comstar", "comstar presence" },
            { "planet_other_empty", "uninhabited" },
            { "planet_other_floatingworld", "dense cloud layer" },
            { "planet_other_fungus", "dominant fungus" },
            { "planet_other_gasgiant", "gas giant moon" },
            { "planet_other_hub", "travel hub" },
            { "planet_other_megacity", "megacity" },
            { "planet_other_megaforest", "planetwide forest" },
            { "planet_other_mudflats", "planetwide mudflats" },
            { "planet_other_newcolony", "recently colonized" },
            { "planet_other_pirate", "pirate presence" },
            { "planet_other_plague", "plague quarantine" },
            { "planet_other_prison", "prison planet" },
            { "planet_other_ruins", "ruins" },
            { "planet_other_starleague", "former star league presence" },
            { "planet_other_stonedcaribou", "hallucinatory vegetation" },
            { "planet_other_storms", "planetwide storms" },
            { "planet_other_taintedair", "tainted atmosphere" },
            { "planet_other_volcanic", "extensive vulcanism" },
            { "planet_other_moon", "moons" },

            { "planet_pop_large", "large population" },
            { "planet_pop_medium", "moderate population" },
            { "planet_pop_none", "token population" },
            { "planet_pop_small", "small population" }
        };
        private static Dictionary<Faction, FactionDef> factionEnumToDef;

        public string Name { get; set; } = "System Search";
        private float DimLevel;

        public Search(float dimLevel = 10f)
        {
            DimLevel = dimLevel;
        }

        private bool DoesFactionMatchSearch(Faction faction, string search)
        {
            var def = factionEnumToDef[faction];
            var name = def.Name.ToLower();
            var shortName = def.ShortName.ToLower();

            return name.StartsWith(search) || shortName.StartsWith(search) || name.StartsWith("the " + search) || shortName.StartsWith("the " + search);
        }

        public void ApplyFilter(SimGameState simGame, string search)
        {
            search = search.ToLower();
            foreach (var system in simGame.StarSystemDictionary.Keys)
            {
                var starSystem = simGame.StarSystemDictionary[system];
                var dim = DimLevel;

                // if the system starts with the search
                // or if system has tags that start with search
                if (string.IsNullOrEmpty(search) ||
                    starSystem.Name.ToLower().StartsWith(search) ||
                    starSystem.Def.ContractEmployers.Any((x) => DoesFactionMatchSearch(x, search)) ||
                    starSystem.Tags.Any((tagID) => tagIDToFriendlyName.ContainsKey(tagID) && tagIDToFriendlyName[tagID].StartsWith(search)))
                {
                    dim = 1;
                }

                Main.DimSystem(system, dim);
            }
        }

        public void Apply(SimGameState simGame)
        {
            if (factionEnumToDef == null)
            {
                factionEnumToDef = FactionDef.HACK_GetFactionDefsByEnum(simGame.DataManager);
            }

            Main.MapSearchGameObject.SetActive(true);
            Main.MapSearchInputField.onValueChanged.AddListener(new UnityAction<string>((x) => ApplyFilter(simGame, x)));
            Main.MapSearchInputField.ActivateInputField();
        }

        public void Unapply(SimGameState simGame)
        {
            Main.MapSearchInputField.onValueChanged.RemoveAllListeners();
            Main.MapSearchGameObject.SetActive(false);
        }
    }
}
