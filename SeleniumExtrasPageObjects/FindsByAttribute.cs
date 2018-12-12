using System;
using OpenQA.Selenium;

namespace SeleniumExtras.PageObjects
{
    /// <summary>
    /// Marks program elements with methods by which to find a corresponding element on the page. Used
    /// in conjunction with the <see cref="PageObjectFactory"/>, it allows you to quickly create Page Objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can use this attribute by specifying the <see cref="How"/> and <see cref="Using"/> properties
    /// to indicate how to find the elements. This attribute can be used to decorate fields and properties
    /// in your Page Object classes. The <see cref="Type"/> of the field or property must be either
    /// <see cref="IWebElement"/> or IList{IWebElement}. Any other type will throw an
    /// <see cref="ArgumentException"/> when <see cref="PageObjectFactory.InitElements(object)"/> is called.
    /// </para>
    /// <para>
    /// <code>
    /// [FindsBy(How = How.Name, Using = "myElementName")]
    /// public IWebElement foundElement;
    ///
    /// [FindsBy(How = How.TagName, Using = "a")]
    /// public IList{IWebElement} allLinks;
    /// </code>
    /// </para>
    /// <para>
    /// You can also use multiple instances of this attribute to find an element that may meet
    /// one of multiple criteria.
    /// </para>
    /// <para>
    /// <code>
    /// // Will find the element with the name attribute matching the first of "anElementName"
    /// // or "differentElementName".
    /// [FindsBy(How = How.Name, Using = "anElementName")]
    /// [FindsBy(How = How.Name, Using = "differentElementName")]
    /// public IWebElement thisElement;
    /// </code>
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class FindsByAttribute : Attribute, IComparable, IFinder
    {
        private By _finder;

        /// <summary>
        /// Gets or sets the method used to look up the element
        /// </summary>
        internal How How { get; set; }

        /// <summary>
        /// Gets or sets the value to lookup by (i.e. for How.Name, the actual name to look up)
        /// </summary>
        internal string Using { get; set; }

        /// <summary>
        /// Creates a new instance of the FindsByAttribute, allowing to 
        /// </summary>
        /// <param name="how"></param>
        /// <param name="using"></param>
        public FindsByAttribute(How how, string @using)
        {
            How = how;
            Using = @using;
        }

        /// <inheritdoc cref="IFinder.Finder"/>
        public By Finder()
        {
            if (_finder == null)
            {
                _finder = ByFactory.From(this);
            }

            return _finder;
        }

        /// <summary>
        /// Determines if two <see cref="FindsByAttribute"/> instances are equal.
        /// </summary>
        /// <param name="one">One instance to compare.</param>
        /// <param name="two">The other instance to compare.</param>
        /// <returns><see langword="true"/> if the two instances are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(FindsByAttribute one, FindsByAttribute two)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(one, two))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)one == null) || ((object)two == null))
            {
                return false;
            }

            return one.Equals(two);
        }

        /// <summary>
        /// Determines if two <see cref="FindsByAttribute"/> instances are unequal.
        /// </summary>s
        /// <param name="one">One instance to compare.</param>
        /// <param name="two">The other instance to compare.</param>
        /// <returns><see langword="true"/> if the two instances are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(FindsByAttribute one, FindsByAttribute two)
        {
            return !(one == two);
        }

        /// <summary>
        /// Determines if one <see cref="FindsByAttribute"/> instance is greater than another.
        /// </summary>
        /// <param name="one">One instance to compare.</param>
        /// <param name="two">The other instance to compare.</param>
        /// <returns><see langword="true"/> if the first instance is greater than the second; otherwise, <see langword="false"/>.</returns>
        public static bool operator >(FindsByAttribute one, FindsByAttribute two)
        {
            if (one == null)
            {
                throw new ArgumentNullException(nameof(one), "Object to compare cannot be null");
            }

            return one.CompareTo(two) > 0;
        }

        /// <summary>
        /// Determines if one <see cref="FindsByAttribute"/> instance is less than another.
        /// </summary>
        /// <param name="one">One instance to compare.</param>
        /// <param name="two">The other instance to compare.</param>
        /// <returns><see langword="true"/> if the first instance is less than the second; otherwise, <see langword="false"/>.</returns>
        public static bool operator <(FindsByAttribute one, FindsByAttribute two)
        {
            if (one == null)
            {
                throw new ArgumentNullException(nameof(one), "Object to compare cannot be null");
            }

            return one.CompareTo(two) < 0;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an
        /// integer that indicates whether the current instance precedes, follows, or occurs
        /// in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// <list type="table">
        /// <listheader>Value</listheader><listheader>Meaning</listheader>
        /// <item><description>Less than zero</description><description>This instance precedes <paramref name="obj"/> in the sort order.</description></item>
        /// <item><description>Zero</description><description>This instance occurs in the same position in the sort order as <paramref name="obj"/>.</description></item>
        /// <item><description>Greater than zero</description><description>This instance follows <paramref name="obj"/> in the sort order. </description></item>
        /// </list>
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object to compare cannot be null");
            }

            var other = obj as FindsByAttribute;
            if (other == null)
            {
                throw new ArgumentException("Object to compare must be a FindsByAttribute", nameof(obj));
            }

            return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object">Object</see> is equal
        /// to the current <see cref="object">Object</see>.
        /// </summary>
        /// <param name="obj">The <see cref="object">Object</see> to compare with the
        /// current <see cref="object">Object</see>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="object">Object</see>
        /// is equal to the current <see cref="object">Object</see>; otherwise,
        /// <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as FindsByAttribute;
            if (other == null)
            {
                return false;
            }

            if (other.Finder() != Finder())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="object">Object</see>.</returns>
        public override int GetHashCode()
        {
            return Finder().GetHashCode();
        }
    }
}