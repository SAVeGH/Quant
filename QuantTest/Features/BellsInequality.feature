Feature: BellsInequality

A short summary of the feature

@tag1
Scenario: Bells Inequality test
	Given System has stream of entangled quantums size of 100000
	When 'Alice' has measured strem using arbitrary basises from three options basises
	And 'Bob' has measured strem using arbitrary basises from three options basises
	Then Comparision gives 1/2 of matched results for quantum case rather than 5/9 for the classical case with devation of 1 percents
