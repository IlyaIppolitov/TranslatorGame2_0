using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace TranslatorGame.Models
{
    public class WordProvider : IAsyncEnumerable<Word>, IEnumerable<Word>
    {
        private readonly (List<Word> list, double prob)[] _lists;

        public WordProvider(params (List<Word> list, double prob)[] lists)
        {
            _lists = lists;
        }

        public IAsyncEnumerator<Word> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new Enumerator(_lists);
        }

        public IEnumerator<Word> GetEnumerator()
        {
            return new Enumerator(_lists);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<Word>, IAsyncEnumerator<Word>
        {
            private readonly (List<Word> list, double prob)[] _lists;
            private int[] _listsIndexes;

            private int _index;
            private Word _currentWord;
            private readonly HashSet<Word> returned;
            private readonly Random random;
            private readonly int listsCount;

            internal Enumerator((List<Word> list, double prob)[] lists)
            {
                _lists = lists;
                _index = 0;
                _currentWord = new Word();
                returned = new HashSet<Word>();
                listsCount = _lists.Length;
                _listsIndexes = new int[listsCount];

                random = new Random();

            }

            //public Word Current => _currentWord;

            object IEnumerator.Current => Current;

            Word Current { get { return _currentWord; } }

            Word IAsyncEnumerator<Word>.Current => _currentWord;

            Word IEnumerator<Word>.Current => _currentWord;

            public void Dispose()
            {
                Reset();
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (allWordsDone())
                        Reset();

                    var listIndex = _index % listsCount; // корректировка номера листа согласно имеющимся

                    var (list, prob) = _lists[listIndex]; // получение текущих: листа и вероятности его использования
                    var itemIndex = _listsIndexes[listIndex]; // получение текущего слова

                    if (list.Count == itemIndex)
                    {
                        ++_index; // меняем лист
                        continue;
                    }

                    var word = list[itemIndex];
                    if (returned.Contains(word))
                    {
                        ++_listsIndexes[listIndex]; // меняем айтем
                        continue; // если слово уже было, пропускам его
                    }

                    if (!Lucky(prob))
                    {
                        ++_index; // меняем лист
                        continue;
                    }

                    _currentWord = word;
                    ++_listsIndexes[listIndex]; // меняем айтем
                    returned.Add(_currentWord);

                    return true;
                }
            }

            private bool allWordsDone()
            {
                var result = true;
                for (int i = 0; i < listsCount; i++)
                {
                    if (_listsIndexes[i] < _lists[i].list.Count)
                        result = false;
                }
                return result;
            }

            private bool Lucky(double prob)
            {
                if (prob == 1.0) return true;
                return random.NextDouble() < prob;
            }

            public void Reset()
            {
                _index = 0;
                _currentWord = new Word();
                _listsIndexes = new int[listsCount];
                returned.Clear();
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                while (true)
                {
                    if (allWordsDone())
                        Reset();

                    var listIndex = _index % listsCount; // корректировка номера листа согласно имеющимся

                    var (list, prob) = _lists[listIndex]; // получение текущих: листа и вероятности его использования
                    var itemIndex = _listsIndexes[listIndex]; // получение текущего слова

                    if (list.Count == itemIndex)
                    {
                        ++_index; // меняем лист
                        continue;
                    }

                    var word = list[itemIndex];
                    if (returned.Contains(word))
                    {
                        ++_listsIndexes[listIndex]; // меняем айтем
                        continue; // если слово уже было, пропускам его
                    }

                    if (!Lucky(prob))
                    {
                        ++_index; // меняем лист
                        continue;
                    }

                    _currentWord = word;
                    ++_listsIndexes[listIndex]; // меняем айтем
                    returned.Add(_currentWord);

                    return true;
                }
            }

            public async ValueTask DisposeAsync()
            {
                Reset();
            }
        }
    }
}
