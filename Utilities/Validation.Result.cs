using System;
using System.Collections;
using System.Collections.Generic;

namespace FrootLuips.Subnautica;
public static partial class Validation
{
	public readonly record struct Result(bool Success, List<Exception> Errors) : IList<Exception>
	{
		public Exception this[int index] { get => ((IList<Exception>)this.Errors)[index]; set => ((IList<Exception>)this.Errors)[index] = value; }

		public int Count => ((ICollection<Exception>)this.Errors).Count;

		public bool IsReadOnly => ((ICollection<Exception>)this.Errors).IsReadOnly;

		public void Add(Exception item)
		{
			((ICollection<Exception>)this.Errors).Add(item);
		}

		public void Clear()
		{
			((ICollection<Exception>)this.Errors).Clear();
		}

		public bool Contains(Exception item)
		{
			return ((ICollection<Exception>)this.Errors).Contains(item);
		}

		public void CopyTo(Exception[] array, int arrayIndex)
		{
			((ICollection<Exception>)this.Errors).CopyTo(array, arrayIndex);
		}

		public IEnumerator<Exception> GetEnumerator()
		{
			return ((IEnumerable<Exception>)this.Errors).GetEnumerator();
		}

		public int IndexOf(Exception item)
		{
			return ((IList<Exception>)this.Errors).IndexOf(item);
		}

		public void Insert(int index, Exception item)
		{
			((IList<Exception>)this.Errors).Insert(index, item);
		}

		public bool Remove(Exception item)
		{
			return ((ICollection<Exception>)this.Errors).Remove(item);
		}

		public void RemoveAt(int index)
		{
			((IList<Exception>)this.Errors).RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.Errors).GetEnumerator();
		}

		public static implicit operator (bool result, List<Exception> errors)(Result value)
		{
			return (value.Success, value.Errors);
		}

		public static implicit operator Result((bool result, List<Exception> errors) value)
		{
			return new Result(value.result, value.errors);
		}

		public static implicit operator bool(Result obj) { return obj.Success; }
	}
}
