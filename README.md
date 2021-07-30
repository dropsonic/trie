# trie
Double-array implementation of a trie (prefix tree) in C#.

Look-up is ![O(1)](https://render.githubusercontent.com/render/math?math=O(1)) on average and ![O(k)](https://render.githubusercontent.com/render/math?math=O(k)) in the worst case, and insertion is about ![O(k^2)](https://render.githubusercontent.com/render/math?math=O(k^2)) in the worst case, where ![k](https://render.githubusercontent.com/render/math?math=k) is the number of input symbols.

## Remarks
Ordinal case-sensitive comparison is used since I didn't have need for anything else.
The implementation can be improved to use case-insensitive and culture-aware comparison as well.
However, handling Unicode glyphs (e.g., diacritics) can be tricky since multiple characters may represent a single letter, and the implementation works with individual `char` values for memory optimization.

The collection could have implemented `IEnumerable<string>` and `IReadOnlyCollection<string>`. The easiest way to do that is to keep all added items in a separate `List<T>` but it is memory-consuming. Iterating over compacted double-array trie hardly makes any sense so this part is omitted for now.

## References
None of the references contains the actual implementation, despite the names. But they were used as the inspiration for the algorithm.

1. [Karoonboonyanan, T. An Implementation of Double-Array Trie](https://linux.thai.net/~thep/datrie/)
2. [Aoe, J. An Efficient Digital Search Algorithm by Using a Double-Array Structure](https://ieeexplore.ieee.org/document/31365?arnumber=31365)
3. [Kanda, S., Morita K., Fuketa M., Aoe J. Experimental Observations of Construction Methods for Double Array Structures Using Linear Functions](http://www.jsoftware.us/vol10/59-C017.pdf)
