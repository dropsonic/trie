using System.Collections.Generic;
using System.Diagnostics;

// Using the existing system namespace is not always a good idea, as mentioned in
// https://stackoverflow.com/a/1320469/1373854 and https://softwareengineering.stackexchange.com/a/57371/319620
// But very suitable place in this particular case.

// ReSharper disable once CheckNamespace
namespace System.Collections.Specialized
{
	/// <summary>
    /// Double-array implementation of a trie (prefix tree).
    /// </summary>
    /// <remarks>
    /// <para>Uses ordinal case-sensitive comparison.</para>
    /// <para>
    /// Look-up is O(1) on average and O(k) in the worst case, 
    /// and insertion is about O(k^2) in the worst case, where k is the number of input symbols.
    /// </para>
	/// <para>
    /// References:
    /// <list type="number">
    /// <item><see href="https://linux.thai.net/~thep/datrie/">Karoonboonyanan, T. An Implementation of Double-Array Trie</see>;</item>
    /// <item><see href="https://ieeexplore.ieee.org/document/31365?arnumber=31365">Aoe, J. An Efficient Digital Search Algorithm by Using a Double-Array Structure</see>;</item>
    /// <item><see href="http://www.jsoftware.us/vol10/59-C017.pdf">Kanda, S., Morita K., Fuketa M., Aoe J. Experimental Observations of Construction Methods for Double Array Structures Using Linear Functions</see>.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class Trie
    {
        [DebuggerDisplay("Base = {Base} | Check = {Check}")]
        private struct Bucket
        {
            public int Base;
            public int Check;
        }

        private const short TerminatorValue = 1;
        private const int DefaultCapacity = 256;

        private Bucket[] _items;
        private int _size;
        private int _index = 1;
        private readonly Dictionary<char, short> _alphabet = new Dictionary<char, short>();
        private short _maxChar = TerminatorValue + 1;

        public Trie() : this(DefaultCapacity)
        {
        }

        public Trie(int capacity)
        {
	        if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));

	        _items = new Bucket[capacity];
            SetBase(1, 1);
        }

        public int Capacity
        {
	        get => _items.Length;
	        set
	        {
		        if (value < _items.Length)
			        throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");

                if (value != _items.Length)
                    EnsureCapacity(value);
	        }
        }

        /// <summary>
        /// Ensures that the capacity of internal array is at least the given minimum value.
        /// If the current capacity of the list is less than min, 
        /// the capacity is increased to twice the current capacity or to min,
        /// whichever is larger.
        /// </summary>
        /// <param name="min">Minimum capacity.</param>
        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int newCapacity = _items.Length == 0 ? DefaultCapacity : _items.Length * 2;

                if (newCapacity < min)
                    newCapacity = min;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse (keep newCapacity > 0 for readability)
                if (newCapacity != _items.Length && newCapacity > 0)
                {
                    Bucket[] newItems = new Bucket[newCapacity];

                    if (_size > 0)
                        Array.Copy(_items, 0, newItems, 0, _size);

                    _items = newItems;
                }
            }
        }

        private int Base(int s)
        {
            s -= 1;

            if (s < _items.Length)
                return _items[s].Base;

            return 0;
        }

        private int Check(int s)
        {
            s -= 1;

            if (s < _items.Length)
                return _items[s].Check;

            return 0;
        }

        private void SetBase(int s, int value)
        {
            s -= 1;

            if (s >= _items.Length)
                EnsureCapacity(s + 1);

            _items[s].Base = value;
            
            if (s + 1 > _size)
                _size = s + 1;
        }

        private void SetCheck(int s, int value)
        {
            s -= 1;

            if (s >= _items.Length)
                EnsureCapacity(s + 1);

            _items[s].Check = value;

            if (s + 1 > _size)
                _size = s + 1;
        }

        /// <summary>
        /// Moves the base for a state <paramref name="s"/> to a new place beginning at <paramref name="b"/>.
        /// </summary>
        private void Relocate(int s, int b)
        {
	        int size = _size;

	        for (int i = 1; i <= size; i++)
	        {
		        if (Check(i) == s)
		        {
			        int c = i - Base(s);                          // determine the input character
			        SetCheck(b + c, s);                   // mark owner
			        SetBase(b + c, Base(Base(s) + c));  // copy data

			        // The node base[s] + c is to be moved to b + c; 
			        // Hence, for any i for which check[i] = base[s] + c, update check[i] to b + c 
			        for (int j = 1; j <= _size; j++)
			        {
				        if (Check(j) == i)
				        {
					        int d = j - Base(i);
					        SetCheck(Base(Base(s) + c) + d, b + c);
				        }
			        }

			        SetCheck(Base(s) + c, 0);             // free the cell
		        }
	        }

	        SetBase(s, b);
        }

        public void Add(string value)
        {
	        if (value == null) throw new ArgumentNullException(nameof(value));

	        int s = 1;

            for (int i = 0; i <= value.Length; i++)
            {
                short c;

                // Add the terminator char to the end of the string
                if (i < value.Length)
                {
                    char ch = value[i];

                    if (!_alphabet.TryGetValue(ch, out c))
                        c = _alphabet[ch] = _maxChar++;
                }
                else
                {
                    c = TerminatorValue;
                }

                int t = Base(s) + c;

                if (Check(t) != s)
                {
                    int checkT = Check(t);
                    bool relocated = false;

                    if (Check(t) != 0) // if the state T is already occupied, we should relocate it
                    {
                        // Calculate how far we should relocate it
                        for (int j = 1; j <= _size; j++)
                        {
                            if (Check(j) == Check(t))
                            {
                                Relocate(checkT, _size + 1 - (j - Base(Check(t))));
                                relocated = true;
                                break;
                            }
                        }
                    }

                    // If some states have been relocated, we should recalculate the last state S because it might have been relocated too.
                    if (relocated)
                    {
                        s = 1;

                        for (int k = 0; k < i; k++)
                        {
                            char kc = value[k];
                            s = Base(s) + _alphabet[kc];
                        }
                    }

                    SetBase(t, ++_index);
                    SetCheck(Base(s) + c, s);
                }

                s = t;
            }
        }

        public bool Contains(string value) => ContainsInternal(value, true);

        public bool ContainsPrefix(string prefix) => ContainsInternal(prefix, false);

        private bool ContainsInternal(string str, bool appendTerminator)
        {
	        if (str == null) throw new ArgumentNullException(nameof(str));

	        int s = 1;

            for (int i = 0; i <= str.Length; i++)
            {
                short c;

                if (i < str.Length)
                {
                    char ch = str[i];

                    if (!_alphabet.TryGetValue(ch, out c))
                        return false;
                }
                else
                {
                    if (!appendTerminator)
                        break;

                    c = TerminatorValue;
                }

                int t = Base(s) + c;

                if (Check(t) == s)
                {
                    s = t;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
