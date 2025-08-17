using System;
using System.Collections;
using System.Collections.Generic;

namespace FrootLuips.Subnautica.Trees;
public partial class Tree<T>
{
	/// <summary>
	/// <inheritdoc cref="ITreeNode{T}"/>
	/// </summary>
	/// <param name="Value"></param>
	/// <param name="Handler"></param>
	public readonly record struct Node(T Value, ITreeHandler<T> Handler) : ITreeNode<T>, IEnumerable<Node>
	{
		/// <inheritdoc/>
		public readonly Node? Parent {
			get => Handler.GetParent(Value);
			set => Handler.SetParent(Value, value);
		}

		/// <inheritdoc/>
		public readonly string Name => Handler.GetName(Value);

		/// <inheritdoc/>
		public readonly int ChildCount => Handler.GetChildCount(Value);

		/// <inheritdoc cref="ITreeNode{T}.GetChild(int)"/>
		public readonly Node this[int childIndex] => GetChild(childIndex);

		/// <inheritdoc/>
		public Node GetChild(int childIndex)
		{
			if (childIndex < 0 || childIndex >= ChildCount)
				throw new IndexOutOfRangeException();

			return Handler.GetChild(Value, childIndex);
		}

		/// <summary>
		/// Returns the <paramref name="node"/>'s value.
		/// </summary>
		/// <param name="node"></param>
		public static implicit operator T(Node node) => node.Value;

		/// <summary>
		/// Enumerates over the direct children of this node.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Node> GetEnumerator()
		{
			for (int i = 0; i < ChildCount; i++)
			{
				yield return Handler.GetChild(Value, i);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
