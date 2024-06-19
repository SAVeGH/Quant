Feature: BB84

Проверка работы протокола передачи ключей BB84

Scenario: BB84 key transmittion without intercept
Given Alice generates 4n size key where n is 1000
And 'Alice' chose basis for each bit in the key
And 'Bob' chose basis for each bit in the key
And Alice makes quantums stream
When Alice sends quantums stream to Bob
And Bob measure quantums stream with chosen basises for each quantum
And Alice and Bob compares their basises in unencripted form
And 'Alice' leave key bits that corresponds to coinciding basises
And 'Bob' leave key bits that corresponds to coinciding basises
Then StubThen