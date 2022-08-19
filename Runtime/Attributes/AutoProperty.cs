using System;
using UnityEngine;

namespace Utils.Attributes
{
	/// <summary>
	/// Automatically assign components to this Property.
	/// It searches for components from this GO or its children by default.
	/// Pass in an <c>AutoPropertyMode</c> to override this behaviour.
	/// <para></para>
	/// Advanced usage: Filter found objects with a method. To do that, create a 
	/// static method or member method of the current class with the same method
	/// signature as a Func&lt;UnityEngine.Object, bool&gt;. Your predicate method
	/// can be private.
	/// If your predicate method is a member method of the current class, pass in
	/// the nameof that method as the second argument.
	/// If your predicate method is a static method, pass in the typeof class that
	/// contains said method as the third argument.
	///
	/// source: https://github.com/Deadcows/MyBox
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoPropertyAttribute : PropertyAttribute
	{
		public readonly AutoPropertyMode Mode;
		public readonly string PredicateMethodName;
		public readonly Type PredicateMethodTarget;

		public AutoPropertyAttribute(AutoPropertyMode mode = AutoPropertyMode.Children,
			string predicateMethodName = null,
			Type predicateMethodTarget = null)
		{
			Mode = mode;
			PredicateMethodTarget = predicateMethodTarget;
			PredicateMethodName = predicateMethodName;
		}
	}

	public enum AutoPropertyMode
	{
		/// <summary>
		/// Search for Components from this GO or its children.
		/// </summary>
		Children = 0,
		/// <summary>
		/// Search for Components from this GO or its parents.
		/// </summary>
		Parent = 1,
		/// <summary>
		/// Search for Components from this GO's current scene.
		/// </summary>
		Scene = 2,
		/// <summary>
		/// Search for Objects from this project's asset folder.
		/// </summary>
		Asset = 3,
		/// <summary>
		/// Search for Objects from anywhere in the project.
		/// Combines the results of Scene and Asset modes.
		/// </summary>
		Any = 4
	}
}