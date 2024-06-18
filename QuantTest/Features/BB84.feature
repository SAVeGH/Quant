Feature: BB84

Проверка работы протокола передачи ключей BB84

Scenario: BB84 key transmittion without intercept
Given Alice generates 4n size key where n is 1000
And 'Alice' chose basis for each bit in the key
And 'Bob' chose basis for each bit in the key
And Alice makes transmittion quantums sequence
When StubWhen
Then StubThen