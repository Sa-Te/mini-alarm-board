| Python              | C#                         | Notes                                                                                        |
| ------------------- | -------------------------- | -------------------------------------------------------------------------------------------- |
| `str`               | `string`                   | Alias for `System.String`. Non-nullable unless you write `string?`                           |
| `int`               | `int`                      | 32-bit, fixed size. `long` for 64-bit. Python ints are arbitrary precision, C# ints overflow |
| `float`             | `double`                   | `double` is the normal one. `decimal` for money, it avoids binary rounding errors            |
| `bool`              | `bool`                     |                                                                                              |
| `None`              | `null`                     | Nullability is declared in the type                                                          |
| `list`              | `List<T>`                  | Or `T[]` for a fixed array, `IEnumerable<T>` for a read-only view                            |
| `dict`              | `Dictionary<TKey, TValue>` |                                                                                              |
| `set`               | `HashSet<T>`               |                                                                                              |
| `tuple`             | `(string Name, int Count)` | ValueTuple. Named elements                                                                   |
| `datetime.datetime` | `DateTimeOffset`           |                                                                                              |
| `datetime.date`     | `DateOnly`                 |                                                                                              |
| `uuid.UUID`         | `Guid`                     |                                                                                              |
| `bytes`             | `byte[]`                   |                                                                                              |
| `object` / `Any`    | `object`                   | Works, but resist it                                                                         |
