# NavigationComputer
BattleTech mod that adds search functionality to the navigation screen, as well as several map modes.

![Search Functionality](Screenshots/search.png?raw=true "Title")
![Search Functionality](Screenshots/systemDifficulty.png?raw=true "Title")
![Search Functionality](Screenshots/unvisitedSystems.png?raw=true "Title")

## Requirements
Requires [BTML](https://github.com/BattletechModders/BattleTechModLoader/releases) and [ModTek](https://github.com/BattletechModders/ModTek/releases). [Installation instructions for BTML/ModTek](https://github.com/BattleTechModders/ModTek/wiki/The-Drop-Dead-Simple-Guide-to-Installing-BTML-&-ModTek-&-ModTek-mods).

## Hotkeys

While in navigation screen:

* **F1**: Toggle unvisited systems map mode
* **F2**: Toggle system difficulty map mode
* **CTRL-F**: System search
* **ESC**: Exit map mode or search

## Searching

The search will look for:

* System name *(e.g. detroit)*
* System tags *(e.g. manufactoring)*
* Factions offering contracts there *(e.g. marik)*
  * excludes "the" in front *(e.g. free instead of the free)*

Additionally you can:

* Invert with '-' in front of query *(e.g. -pirates)*
* Chain together multiple queries *(e.g. marik -liao)*

## Limitations/known bugs

* Currently the search doesn't handle multi-word searches at all, treating each word as its own section.


## Future Plans

* Be able to make a custom route maybe
* Additional map modes if needed
* Options for configuring buttons
* Turn off fake-star-background or yellow overlay by default
* Default map mode

