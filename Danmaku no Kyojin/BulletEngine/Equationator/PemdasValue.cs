using System;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	/// <summary>
	/// Enum used to sort out the equation tree by order of operations.
	/// The lower the number, the leafier the node.
	/// </summary>
	public enum PemdasValue
	{
		Value, //numbers, equations, function, params, are always leaf nodes
		Exponent,
		Multiplication,
		Division,
		Addition,
		Subtraction,
		Invalid
	}
}

