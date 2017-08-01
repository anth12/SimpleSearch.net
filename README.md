# SimpleSearch.net
Simple InMemory ranked search for .net applications.

![Logo](https://github.com/anth12/SimpleSearch.net/raw/master/logo.png)

## Instalation
    Install-Package SimpleSearch

View on [nuget](https://www.nuget.org/packages/SimpleSearch/).

## Usage
	SearchIndex = SearchIndexer.Build(Airports,
		new SearchIndexOptions<Airport>()
			.AddProperty(c => c.iata_code, 1, true)
			.AddProperty(c => c.name)
			.AddProperty(c => c.municipality, 0.6)
			.AddProperty(c => c.iso_country, 0.4)
	);
	
	var results = SearchIndex.Search("London");

- Logo made by [Freepik](http://www.freepik.com) from [www.flaticon.com](http://www.flaticon.com) is licensed by [CC 3.0](http://creativecommons.org/licenses/by/3.0/)
