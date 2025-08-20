using System;
using System.Collections;
using System.Collections.Generic;

namespace FrootLuips.Subnautica.Trees;
public partial class Tree<T>
{
	/// <summary>
	/// Represents a single node in a <see cref="Tree{T}"/> structure.
	/// </summary>
	public readonly record struct Node : IEnumerable<Node>
	{
		private readonly bool _isRoot;

		/// <summary>
		/// Is this node the root of a tree?
		/// </summary>
		public readonly bool IsRoot => _isRoot || !Parent.HasValue;

		/// <summary>
		/// Gets the parent of this node.
		/// </summary>
		public readonly Node? Parent => Handler.GetParent(Value);

		/// <summary>
		/// The name of this node.
		/// </summary>
		/// <remarks>
		/// Ideally a node's name should be unique across siblings.
		/// </remarks>
		public readonly string Name => Handler.GetName(Value);
		/// <summary>
		/// The node's underlying <typeparamref name="T"/> value.
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// The <see cref="ITreeHandler{T}"/> for this node.
		/// </summary>
		public ITreeHandler<T> Handler { get; }

		/// <summary>
		/// The number of children this node has.
		/// </summary>
		public readonly int ChildCount => Handler.GetChildCount(Value);

		/// <inheritdoc cref="GetChild(int)"/>
		public readonly Node this[int childIndex] => GetChild(childIndex);

		/// <inheritdoc cref="GetChild(string)"/>
		public readonly Node this[string name] => GetChild(name);

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

		/// <summary>
		/// Gets the child at the specified <paramref name="childIndex"/>.
		/// </summary>
		/// <param name="childIndex">Zero-based child index.</param>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public readonly Node GetChild(int childIndex)
		{
			if (childIndex < 0 || childIndex >= ChildCount)
				throw new IndexOutOfRangeException();

			return Handler.GetChild(Value, childIndex);
		}

		/// <summary>
		/// Gets the first child with the specified <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="NodeNotFoundException"></exception>
		public readonly Node GetChild(string name)
		{
			for (int i = 0; i < this.ChildCount; i++)
			{
				var child = this.GetChild(i);
				if (child.Name == name)
					return child;
			}
			throw NodeNotFoundException.WithName(name);
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

		/// <summary>
		/// Get the root node.
		/// </summary>
		/// <returns></returns>
		public readonly Node GetRoot()
		{
			var root = this;
			bool found = false;
			for (int i = 0; i < TreeHelpers.MaxDepth && !found; i++)
			{
				if (root.IsRoot)
					found = true;
				else
					root = root.Parent!.Value;
			}
			return root;
		}

		/// <summary>
		/// Counts the ancestors of this node.
		/// </summary>
		/// <returns></returns>
		public readonly int GetDepth()
		{
			var current = this;
			int depth;
			for (depth = 0; depth < TreeHelpers.MaxDepth; depth++)
			{
				if (current.IsRoot)
					break;
				else
					current = current.Parent!.Value;
			}
			return depth;
		}

		/// <summary>
		/// Gets this node's path.
		/// </summary>
		/// <returns></returns>
		public readonly string GetPath()
		{
			var current = this;
			Stack<string> path = new(capacity: 1);
			for (int i = 0; i < TreeHelpers.MaxDepth; i++)
			{
				path.Push(current.Name);

				if (current.IsRoot)
					break;
				else
					current = current.Parent!.Value;
			}
			return string.Join(TreeHelpers.PATH_SEPARATOR.ToString(), path);
		}
	}
}
