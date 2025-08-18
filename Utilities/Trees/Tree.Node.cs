using System;
using System.Collections;
using System.Collections.Generic;

namespace FrootLuips.Subnautica.Trees;
public partial class Tree<T>
{
	/// <summary>
	/// <inheritdoc cref="ITreeNode{T}"/>
	/// </summary>
	public readonly record struct Node : ITreeNode<T>, IEnumerable<Node>
	{
		private readonly bool _isRoot;

		/// <inheritdoc/>
		public T Value { get; }
		/// <summary>
		/// The <see cref="ITreeHandler{T}"/> for this node.
		/// </summary>
		public ITreeHandler<T> Handler { get; }
		/// <summary>
		/// Is this node the root of a tree?
		/// </summary>
		public readonly bool IsRoot => _isRoot || !Parent.HasValue;

		/// <summary/>
		/// <param name="value"></param>
		/// <param name="handler"></param>
		public Node(T value, ITreeHandler<T> handler) : this(value, handler, false) { }

		/// <summary/>
		/// <param name="value"></param>
		/// <param name="handler"></param>
		/// <param name="isRoot">Forces this node to be considered a root node, disregarding the value of <see cref="Parent"/>.</param>
		public Node(T value, ITreeHandler<T> handler, bool isRoot)
		{
			Value = value;
			Handler = handler;
			_isRoot = isRoot;
		}

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
		public readonly Node GetChild(int childIndex)
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
		public readonly IEnumerator<Node> GetEnumerator()
		{
			for (int i = 0; i < ChildCount; i++)
			{
				yield return Handler.GetChild(Value, i);
			}
		}

		readonly IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		/// <inheritdoc/>
		public override readonly string ToString() => $"{Name} ({nameof(Node)})";
	}
}
