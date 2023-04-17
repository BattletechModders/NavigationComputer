# NavigationComputer

**v2.1.0 and higher requires modtek v3 or higher**

BattleTech mod that adds several map modes, search functionality, and the ability to plan custom routes to the navigation screen.
## Download
Go to the [releases page](https://github.com/BattletechModders/NavigationComputer/releases) to download a release.

## Requirements
Requires [ModTek](https://github.com/BattletechModders/ModTek/releases).

## Hotkeys

While in navigation screen:

* **F1**: Toggle unvisited systems map mode
* **F2**: Toggle system difficulty map mode
* **CTRL-F**: System search
* **ESC**: Exit map mode or search
* **Shift-Click**: On system: route through this system 

## Custom Routes

Shift clicking a system will keep the previous route and path from the end of the previous route to the new destination. Use this for routing through systems that you want to check out (contracts, shops), but don't neccessarily want to stop at.

## Searching

The search will look for:

* System name *(e.g. detroit)*
* System tags *(e.g. manufactoring)*
* Factions offering contracts there *(e.g. marik)*
  * excludes "the" in front *(e.g. free instead of the free)*

Additionally you can:

* Invert with '-' in front of query *(e.g. -pirates)*
* Chain together multiple queries *(e.g. marik -liao)*

## Screenshots

![Search Functionality](Screenshots/search.png?raw=true "Title")
![Search Functionality](Screenshots/systemDifficulty.png?raw=true "Title")
![Search Functionality](Screenshots/unvisitedSystems.png?raw=true "Title")

## Limitations/known bugs

* Currently the search doesn't handle multi-word searches at all, treating each word as its own section.

## Future Plans

* Additional map modes if needed
* Options for configuring buttons
* Turn off fake-star-background or yellow overlay by default
* Default map mode
