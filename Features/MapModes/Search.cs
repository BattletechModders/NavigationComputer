using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;

// ReSharper disable StringLiteralTypo

namespace NavigationComputer.Features.MapModes
{
    public class Search : IMapMode
    {
        private static readonly Dictionary<string, string> TagIdToFriendlyName = new Dictionary<string, string>
        {
            {"planet_industry_agriculture", "agriculture"},
            {"planet_industry_aquaculture", "aquaculture"},
            {"planet_industry_manufacturing", "manufacturing"},
            {"planet_industry_mining", "mining"},
            {"planet_industry_poor", "poor"},
            {"planet_industry_recreation", "recreation"},
            {"planet_industry_research", "research"},
            {"planet_industry_rich", "rich"},
            {"planet_size_large", "high gravity planet"},
            {"planet_size_medium", "medium gravity planet"},
            {"planet_size_small", "low gravity planet"},
            {"planet_civ_innersphere", "inner sphere-level civilization"},
            {"planet_civ_periphery", "periphery-level civilization"},
            {"planet_civ_primitive", "primitive civilization"},
            {"planet_climate_arctic", "arctic world"},
            {"planet_climate_arid", "arid world"},
            {"planet_climate_desert", "desert world"},
            {"planet_climate_ice", "ice world"},
            {"planet_climate_mars", "martian world"},
            {"planet_climate_lunar", "lunar world"},
            {"planet_climate_moon", "barren world"},
            {"planet_climate_rocky", "rocky world"},
            {"planet_climate_terran", "terran world"},
            {"planet_climate_tropical", "tropical world"},
            {"planet_climate_water", "water world"},
            {"planet_other_alienvegetation", "alien vegetation"},
            {"planet_other_battlefield", "battlefield"},
            {"planet_other_blackmarket", "black market"},
            {"planet_other_boreholes", "geothermal boreholes"},
            {"planet_other_capital", "regional capital"},
            {"planet_other_comstar", "comstar presence"},
            {"planet_other_empty", "uninhabited"},
            {"planet_other_floatingworld", "dense cloud layer"},
            {"planet_other_fungus", "dominant fungus"},
            {"planet_other_gasgiant", "gas giant moon"},
            {"planet_other_hub", "travel hub"},
            {"planet_other_megacity", "megacity"},
            {"planet_other_megaforest", "planetwide forest"},
            {"planet_other_mudflats", "planetwide mudflats"},
            {"planet_other_newcolony", "recently colonized"},
            {"planet_other_pirate", "pirate presence"},
            {"planet_other_plague", "plague quarantine"},
            {"planet_other_prison", "prison planet"},
            {"planet_other_ruins", "ruins"},
            {"planet_other_starleague", "former star league presence"},
            {"planet_other_stonedcaribou", "hallucinatory vegetation"},
            {"planet_other_storms", "planetwide storms"},
            {"planet_other_taintedair", "tainted atmosphere"},
            {"planet_other_volcanic", "extensive vulcanism"},
            {"planet_other_moon", "moons"},
            {"planet_pop_large", "large population"},
            {"planet_pop_medium", "moderate population"},
            {"planet_pop_none", "token population"},
            {"planet_pop_small", "small population"}
        };

        private readonly float _dimLevel;

        public Search(float dimLevel = 10f)
        {
            _dimLevel = dimLevel;
        }


        public string Name { get; } = "System Search";

        public void Apply(SimGameState simGame)
        {
            MapModesUI.MapSearchGameObject.SetActive(true);
            MapModesUI.MapSearchInputField.onValueChanged.AddListener(x => ApplyFilter(simGame, x));
            MapModesUI.MapSearchInputField.ActivateInputField();
        }

        public void Unapply(SimGameState simGame)
        {
            MapModesUI.MapSearchInputField.onValueChanged.RemoveAllListeners();
            MapModesUI.MapSearchGameObject.SetActive(false);
        }


        private bool DoesFactionMatchSearch(string factionID, string search)
        {
            var def = FactionDef.GetFactionDefByEnum(UnityGameInstance.BattleTechGame.DataManager, factionID);
            var name = def.Name.ToLower();
            var shortName = def.ShortName.ToLower();

            // TODO: "the" thing is a hack, tbh
            return name.StartsWith(search) || shortName.StartsWith(search) || name.StartsWith("the " + search) ||
                   shortName.StartsWith("the " + search);
        }

        private bool DoesTagMatchSearch(string tagID, string search)
        {
            return TagIdToFriendlyName.ContainsKey(tagID) && TagIdToFriendlyName[tagID].StartsWith(search);
        }

        private bool DoesSystemMatchSearch(StarSystem system, SearchValue search)
        {
            if (string.IsNullOrEmpty(search.Value))
                return true;

            bool matches;
            switch (search.Type)
            {
                case "name":
                    matches = system.Name.ToLower().StartsWith(search.Value);
                    break;

                case "for":
                case "employer":
                    matches = system.Def.ContractEmployerIDList.Any(faction => DoesFactionMatchSearch(faction, search.Value));
                    break;

                case "against":
                case "target":
                    matches = system.Def.ContractTargetIDList.Any(faction => DoesFactionMatchSearch(faction, search.Value));
                    break;

                case "tag":
                    matches = system.Tags.Any(tagID => DoesTagMatchSearch(tagID, search.Value));
                    break;

                case "":
                    matches = system.Name.ToLower().StartsWith(search.Value) ||
                              system.Def.ContractEmployerIDList.Any(faction => DoesFactionMatchSearch(faction, search.Value)) ||
                              system.Tags.Any(tagID => DoesTagMatchSearch(tagID, search.Value));
                    break;

                default:
                    matches = false;
                    break;
            }

            return search.Inverted ? !matches : matches;
        }

        private void ApplyFilter(SimGameState simGame, string searchString)
        {
            searchString = searchString.ToLower();
            var andSplit = searchString.Split('&');
            var searchTree = andSplit.Select(andTerm => andTerm.Split('|').Select(orTerm => new SearchValue(orTerm)).ToArray()).ToArray();

            foreach (var systemID in simGame.StarSystemDictionary.Keys)
            {
                var system = simGame.StarSystemDictionary[systemID];
                var matches = searchTree.All(andTerm => andTerm.Any(searchValue => DoesSystemMatchSearch(system, searchValue)));

                // dim level of 1 means it should "stay" the reg system color
                MapModesUI.DimSystem(systemID, matches ? 1 : _dimLevel);
            }
        }


        private class SearchValue
        {
            private static readonly Regex ColonRegex = new Regex(@"^((?<type>\w+):)?\s?(?<search>.+)$\s?");

            public string Value;
            public string Type;
            public bool Inverted;

            public SearchValue(string searchString)
            {
                searchString = searchString.Trim();

                if (searchString.StartsWith("-"))
                {
                    searchString = searchString.Remove(0, 1);
                    Inverted = true;
                }

                var regexMatch = ColonRegex.Match(searchString);
                Type = regexMatch.Groups["type"].Value;
                Value = regexMatch.Groups["search"].Value;
            }
        }
    }
}
