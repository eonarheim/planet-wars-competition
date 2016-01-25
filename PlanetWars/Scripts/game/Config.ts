var Config = {

   // Map Size
   MapPadding: 50, // padding around edge of map, inside will be "drawable" space for planets
   MapSize: 400,

   // Starfield
   StarfieldSize: 1000,
   StarfieldMinFade: 0.2,
   StarfieldMaxFade: 0.7,
   StarfieldMinFadeRefreshAmount: 0.05,
   StarfieldMaxFadeRefreshAmount: 0.15,
   StarfieldRefreshRate: 300, // ms
   StarfieldMeteorFreqMin: 2000,
   StarfieldMeteorFreqMax: 7000,
   StarfieldMeteorSpeed: 320,

   //
   // fleets
   //
   FleetWidth: 6,
   FleetHeight: 7,   

   //
   // planets
   //

   // Size of planet * factor = world size
   PlanetMinSize: 15,
   PlanetMaxSize: 50,
   PlanetNeutralColor: ex.Color.Gray,

   //
   // players
   //

   PlayerAColor: ex.Color.fromHex("#c53e30"),
   PlayerBColor: ex.Color.fromHex("#3797bf")
}