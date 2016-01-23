${
    // Enable extension methods by adding using Typewriter.Extensions.*
    using Typewriter.Extensions.Types;

	Template(Settings settings) {
		settings.IncludeReferencedProjects();
	}
}

module Server {
    $Classes(PlanetWars.Shared.*)[
    export interface $Name$TypeParameters {
        $Properties[
        $name: $Type;]
    }]
}