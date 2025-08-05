using FrootLuips.ChaosMod.Utilities;
using Random = UnityEngine.Random;

namespace FrootLuips.ChaosMod.Objects;
public class RandomDistribution<T> where T : IDistributable
{
	private const string _NO_ITEMS_FOUND_MESSAGE = "No item was found. It is likely that all items have a weight of 0 and cannot be chosen.";
	private readonly List<T> _items;

	public IReadOnlyList<T> Items => _items;
	public int Count => _items.Count;

	public RandomDistribution()
	{
		_items = new();
	}

	public RandomDistribution(params T[] items)
	{
		_items = new(items);
	}

	public void AddItems(params T[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			_items.Add(items[i]);
		}
	}

	public void RemoveItem(params T[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			_items.Remove(_items[i]);
		}
	}

	public void Clear()
	{
		_items.Clear();
	}

	/// <summary>
	/// Gets a random item using Unity's Random Number Generator.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public T GetRandomItem()
	{
		if (Count <= 0)
			throw new InvalidOperationException($"{nameof(RandomDistribution<T>)} has no items.");

		int totalWeight = GetTotalWeight();

		int rng = Random.Range(0, totalWeight);
		int offset = 0;
		bool found = false;

		T? result = default;
		for (int i = 0; i < Count && !found; i++)
		{
			if (_items[i].Weight <= 0)
				continue;

			found = rng < offset + _items[i].Weight;
			if (found)
				result = _items[i];
			else
				offset += _items[i].Weight;
		}
		Assertions.Assert(found, _NO_ITEMS_FOUND_MESSAGE);
		return result!;
	}

	public IEnumerator<T> GetRandomItems(int count)
	{
		for (int i = 0; i < count; i++)
		{
			yield return GetRandomItem();
		}
	}

	private int GetTotalWeight()
	{
		int totalWeight = 0;
		for (int i = 0; i < Count; i++)
		{
			totalWeight += _items[i].Weight;
		}
		return totalWeight;
	}
}
