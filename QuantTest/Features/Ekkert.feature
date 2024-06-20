Feature: Ekkert

A short summary of the feature

Scenario: Ekkert protocol key transmittion without intercept
Given Alice generates 3n size key where n is 1000
And 'Alice' chose basis for each bit in the key
And 'Bob' chose basis for each bit in the key
And Alice makes quantums stream
When Alice sends quantums stream to Bob
And Alice and Bob compares their basises in unencripted form
And Bob measure quantums stream with chosen basises for each quantum
And 'Alice' leave key bits that corresponds to coinciding basises
And 'Bob' leave key bits that corresponds to coinciding basises
And Alice and Bob compares one half of their key bits in unencripted form
Then Compared key bits are identical
And Alice and Bob keys are identical
