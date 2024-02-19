﻿Feature: Ringify

A short summary of the feature

Scenario: Ringify three quants
	Given System has quantums
		| Name | Angle |
		| A    | 0     |		
		| B    | 0     |
		| C    | 0     |
	And Quantums 'A' and 'B' are entangled
	And Quantums 'B' and 'C' are entangled
	And Quantums 'C' and 'A' are entangled
	When Ringify 'A'
	Then Quantum 'A' has one way reference to quantum 'B'
	And Quantum 'B' has one way reference to quantum 'C'
	And Quantum 'C' has one way reference to quantum 'A'